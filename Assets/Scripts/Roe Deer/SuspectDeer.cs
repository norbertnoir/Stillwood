using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum SuspectDeerState
{
    Idle,
    Patrol,
    Observe,
    Stalk,
    Attack,
    Flee,
    StandUpright,
    Hunt  // Nowy stan: aktywne polowanie na gracza
}

public class SuspectDeer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject attackHitbox;

    [Header("Omniscience Settings")]
    [SerializeField] private bool alwaysKnowsPlayerPosition = true;  // Zawsze wie, gdzie jest gracz
    [SerializeField] private float huntingCooldown = 15f;            // Czas pomiędzy polowaniami
    [SerializeField] private float huntingDuration = 7f;             // Czas trwania polowania
    [SerializeField] private float attackFrequency = 0.3f;           // Prawdopodobieństwo ataku podczas polowania
    private float nextHuntTime;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 50f;            // Zwiększony zasięg detekcji
    [SerializeField] private float immediateDetectionRadius = 30f;   // Zwiększony zasięg natychmiastowej detekcji
    [SerializeField] private float viewAngle = 120f;
    [SerializeField] private float eyeHeight = 1f;
    [SerializeField] private LayerMask obstacleMask;
    private bool playerDetected = false;

    [Header("Movement")]
    [SerializeField] private float patrolSpeed = 3f;
    [SerializeField] private float chaseSpeed = 7f;                 // Zwiększona prędkość pościgu
    [SerializeField] private float stalkSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float patrolRadius = 20f;
    [SerializeField] private float minDistanceFromPlayer = 3f;
    [SerializeField] private Transform[] patrolPoints;
    [SerializeField] private bool useRandomPatrolPoints = true;
    private int currentPatrolPoint = 0;

    [Header("Combat")]
    [SerializeField] private float attackRange = 3f;                // Zwiększony zasięg ataku
    [SerializeField] private float attackCooldown = 2f;             // Zmniejszony cooldown ataku
    [SerializeField] private int attackDamage = 15;
    [SerializeField] private float attackHitboxDuration = 0.5f;
    [SerializeField] private float attackLungeDistance = 2.5f;
    private float lastAttackTime;

    [Header("Behavior")]
    [SerializeField] private float observationTime = 2f;           // Krótszy czas obserwacji
    [SerializeField] private float stalkTime = 5f;
    [SerializeField] private float standUprightTime = 4f;
    [SerializeField] private float standUprightCooldown = 15f;
    [SerializeField] private float fleeHealthThreshold = 20f;      // Niższy próg ucieczki
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;

    [Header("Stalking")]
    [SerializeField] private float stalkMaxDistance = 15f;
    [SerializeField] private float stalkMinDistance = 8f;
    [SerializeField] private float stalkCircleSpeed = 2f;

    [Header("Animation Parameters")]
    [SerializeField] private string isMovingParam = "isMoving";
    [SerializeField] private string speedParam = "speed";
    [SerializeField] private string attackTriggerParam = "attack";
    [SerializeField] private string isUprightParam = "isUpright";
    [SerializeField] private string hitTriggerParam = "hit";
    [SerializeField] private string dieParam = "die";

    // State machine
    private SuspectDeerState currentState;
    private float stateTimer = 0f;
    private Vector3 targetPosition;
    private Vector3 startingPosition;
    private bool isDead = false;

    private float nextStandUprightTime;
    private float stalkAngle = 0f;

    private void Start()
    {
        startingPosition = transform.position;
        navMeshAgent.updateRotation = false;

        // Inicjalizacja stanów
        SetState(SuspectDeerState.Patrol);
        nextStandUprightTime = Time.time + Random.Range(10f, 20f);
        nextHuntTime = Time.time + Random.Range(5f, 10f);  // Pierwsze polowanie po krótkim czasie

        // Inicjalizacja hitboxa ataku
        if (attackHitbox != null)
            attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (isDead) return;

        // Aktualizacja timera stanu
        stateTimer -= Time.deltaTime;

        // Jeśli ma zawsze wiedzieć, gdzie jest gracz
        if (alwaysKnowsPlayerPosition)
        {
            playerDetected = true;

            // Sprawdź, czy czas na polowanie
            if (Time.time >= nextHuntTime && currentState != SuspectDeerState.Hunt &&
                currentState != SuspectDeerState.Attack && currentHealth > fleeHealthThreshold)
            {
                SetState(SuspectDeerState.Hunt);
                nextHuntTime = Time.time + huntingDuration + huntingCooldown;
            }
        }
        else
        {
            // Standardowe wykrywanie gracza
            DetectPlayer();
        }

        // Aktualizacja bieżącego stanu
        UpdateCurrentState();

        // Aktualizacja animacji
        UpdateAnimation();

        // Obrót
        UpdateRotation();
    }

    private void UpdateCurrentState()
    {
        switch (currentState)
        {
            case SuspectDeerState.Idle:
                HandleIdleState();
                break;

            case SuspectDeerState.Patrol:
                HandlePatrolState();
                break;

            case SuspectDeerState.Observe:
                HandleObserveState();
                break;

            case SuspectDeerState.Stalk:
                HandleStalkState();
                break;

            case SuspectDeerState.Attack:
                HandleAttackState();
                break;

            case SuspectDeerState.Flee:
                HandleFleeState();
                break;

            case SuspectDeerState.StandUpright:
                HandleStandUprightState();
                break;

            case SuspectDeerState.Hunt:
                HandleHuntState();
                break;
        }

        // Random szansa na stanie na dwóch nogach, jeśli jesteśmy w patrolowaniu lub idle i cooldown minął
        if ((currentState == SuspectDeerState.Patrol || currentState == SuspectDeerState.Idle) &&
            Time.time > nextStandUprightTime && Random.value < 0.005f)
        {
            SetState(SuspectDeerState.StandUpright);
            nextStandUprightTime = Time.time + standUprightCooldown;
        }
    }

    #region State Handlers

    private void HandleIdleState()
    {
        // Po prostu stój i obserwuj przez określony czas
        if (stateTimer <= 0)
        {
            // Przejdź do patrolowania lub skradania, jeśli gracz jest wykryty
            if (playerDetected)
            {
                SetState(SuspectDeerState.Stalk);
            }
            else
            {
                SetState(SuspectDeerState.Patrol);
            }
        }
    }

    private void HandlePatrolState()
    {
        // Poruszaj się do punktu patrolowego
        if (patrolPoints.Length > 0 && !navMeshAgent.pathPending &&
            (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || !navMeshAgent.hasPath))
        {
            // Osiągnięto punkt patrolowy
            if (useRandomPatrolPoints)
            {
                currentPatrolPoint = Random.Range(0, patrolPoints.Length);
            }
            else
            {
                currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
            }

            GoToPoint(patrolPoints[currentPatrolPoint].position);
        }
        else if (patrolPoints.Length == 0 && !navMeshAgent.pathPending &&
                (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || !navMeshAgent.hasPath))
        {
            // Brak punktów patrolowych - wygeneruj losowy punkt w pobliżu pozycji startowej
            Vector3 randomPoint = startingPosition + Random.insideUnitSphere * patrolRadius;
            randomPoint.y = startingPosition.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
            {
                GoToPoint(hit.position);
            }
        }

        // Czasami przystawaj
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && Random.value < 0.01f)
        {
            SetState(SuspectDeerState.Idle);
            stateTimer = Random.Range(1f, 3f);
        }

        // Jeśli wykryto gracza, zacznij skradanie
        if (playerDetected)
        {
            SetState(SuspectDeerState.Stalk);
        }
    }

    private void HandleObserveState()
    {
        // Zatrzymaj się i obserwuj gracza
        navMeshAgent.isStopped = true;

        // Zawsze obracaj się w stronę gracza
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed * 1.5f);

        if (stateTimer <= 0)
        {
            // Po obserwacji, zdecyduj: stalk, attack lub flee
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (currentHealth < fleeHealthThreshold)
            {
                SetState(SuspectDeerState.Flee);
            }
            else if (distanceToPlayer <= attackRange)
            {
                SetState(SuspectDeerState.Attack);
            }
            else
            {
                // 50% szansa na skradanie się lub polowanie
                if (Random.value < 0.5f)
                {
                    SetState(SuspectDeerState.Stalk);
                }
                else
                {
                    SetState(SuspectDeerState.Hunt);
                }
            }
        }

        // Jeśli gracz jest w zasięgu ataku, atakuj natychmiast
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        if (distToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            SetState(SuspectDeerState.Attack);
        }
    }

    private void HandleStalkState()
    {
        // Skradanie się wokół gracza, utrzymując określoną odległość
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Jeśli straciliśmy gracza z oczu, wróć do patrolowania
        if (!playerDetected)
        {
            SetState(SuspectDeerState.Patrol);
            return;
        }

        // Jeśli jesteśmy za blisko, cofnij się
        if (distanceToPlayer < stalkMinDistance)
        {
            Vector3 directionFromPlayer = (transform.position - player.position).normalized;
            Vector3 targetPos = player.position + directionFromPlayer * stalkMinDistance;
            GoToPoint(targetPos, stalkSpeed);
        }
        // Jeśli jesteśmy za daleko, zbliż się
        else if (distanceToPlayer > stalkMaxDistance)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            Vector3 targetPos = player.position - directionToPlayer * stalkMaxDistance;
            GoToPoint(targetPos, stalkSpeed);
        }
        // W przeciwnym razie krąż wokół
        else
        {
            // Krążenie wokół gracza
            stalkAngle += stalkCircleSpeed * Time.deltaTime;
            float radius = Mathf.Lerp(stalkMinDistance, stalkMaxDistance, 0.5f);

            Vector3 offset = new Vector3(
                Mathf.Cos(stalkAngle) * radius,
                0,
                Mathf.Sin(stalkAngle) * radius
            );

            Vector3 targetPos = player.position + offset;
            GoToPoint(targetPos, stalkSpeed);
        }

        // Po upływie czasu skradania, zdecyduj co dalej
        if (stateTimer <= 0)
        {
            // Większa szansa na atak jeśli jesteśmy bliżej
            float attackChance = Mathf.Lerp(0.3f, 0.9f, 1 - (distanceToPlayer / stalkMaxDistance));

            if (Random.value < attackChance && distanceToPlayer <= stalkMaxDistance)
            {
                SetState(SuspectDeerState.Attack);
            }
            else
            {
                // Przejdź do polowania lub obserwacji
                if (Random.value < 0.7f)
                {
                    SetState(SuspectDeerState.Hunt);
                }
                else
                {
                    SetState(SuspectDeerState.Observe);
                }
            }
        }

        // Jeśli jesteśmy w zasięgu ataku, rozważ atak z większym prawdopodobieństwem
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown && Random.value < 0.2f)
        {
            SetState(SuspectDeerState.Attack);
        }
    }

    private void HandleHuntState()
    {
        // Aktywne polowanie - bezpośrednie ściganie gracza z zamiarem ataku
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Bezpośrednio podążaj za graczem
        GoToPoint(player.position, chaseSpeed);

        // Jeśli osiągnęliśmy zasięg ataku, zaatakuj
        if (distanceToPlayer <= attackRange && Time.time > lastAttackTime + attackCooldown)
        {
            SetState(SuspectDeerState.Attack);
            return;
        }

        // Losowo rozważ atak, nawet gdy jesteśmy trochę dalej
        if (distanceToPlayer <= attackRange * 1.5f && Time.time > lastAttackTime + attackCooldown &&
            Random.value < attackFrequency * Time.deltaTime * 10)
        {
            SetState(SuspectDeerState.Attack);
            return;
        }

        // Zakończ polowanie po upływie czasu
        if (stateTimer <= 0)
        {
            if (Random.value < 0.3f)
            {
                SetState(SuspectDeerState.Stalk);
            }
            else
            {
                SetState(SuspectDeerState.Observe);
            }
        }
    }

    private void HandleAttackState()
    {
        // Zatrzymaj nawigację podczas ataku
        navMeshAgent.isStopped = true;

        // Atak trwa przez animację
        if (stateTimer <= 0)
        {
            // Zakończ atak i wróć do polowania lub obserwacji
            attackHitbox.SetActive(false);

            // Większa szansa na kontynuowanie polowania
            if (Random.value < 0.7f)
            {
                SetState(SuspectDeerState.Hunt);
            }
            else
            {
                SetState(SuspectDeerState.Observe);
            }
        }
    }

    private void HandleFleeState()
    {
        // Uciekaj od gracza
        if (!navMeshAgent.pathPending && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance || !navMeshAgent.hasPath))
        {
            // Znajdź kierunek ucieczki
            Vector3 fleeDirection = (transform.position - player.position).normalized;
            Vector3 targetPosition = transform.position + fleeDirection * 15f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 15f, NavMesh.AllAreas))
            {
                GoToPoint(hit.position, chaseSpeed);
            }
        }

        // Jeśli gracz jest wystarczająco daleko, przejdź do obserwacji
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius)
        {
            if (currentHealth > fleeHealthThreshold)
            {
                SetState(SuspectDeerState.Observe);
            }
            else
            {
                // Kontynuuj ucieczkę, jeśli zdrowie nadal niskie
                stateTimer = Random.Range(2f, 5f);
            }
        }

        // Jeśli czas ucieczki minął, zastanów się czy dalej uciekać
        if (stateTimer <= 0)
        {
            if (currentHealth < fleeHealthThreshold || Random.value < 0.5f)
            {
                // Kontynuuj ucieczkę
                stateTimer = Random.Range(2f, 5f);
            }
            else if (distanceToPlayer > attackRange * 2)
            {
                SetState(SuspectDeerState.Observe);
            }
        }
    }

    private void HandleStandUprightState()
    {
        // Stanie na dwóch nogach
        navMeshAgent.isStopped = true;

        if (stateTimer <= 0)
        {
            // Po staniu na dwóch nogach, często przejdź do polowania
            if (Random.value < 0.7f && currentHealth > fleeHealthThreshold)
            {
                SetState(SuspectDeerState.Hunt);
            }
            else if (playerDetected)
            {
                SetState(SuspectDeerState.Observe);
            }
            else
            {
                SetState(SuspectDeerState.Patrol);
            }
        }
    }

    #endregion

    #region Helper Functions

    private void SetState(SuspectDeerState newState)
    {
        // Zakończ obecny stan
        switch (currentState)
        {
            case SuspectDeerState.Idle:
                break;

            case SuspectDeerState.StandUpright:
                animator.SetBool(isUprightParam, false);
                break;

            case SuspectDeerState.Attack:
                attackHitbox.SetActive(false);
                break;
        }

        // Ustaw nowy stan
        currentState = newState;

        // Inicjalizuj nowy stan
        switch (newState)
        {
            case SuspectDeerState.Idle:
                navMeshAgent.isStopped = true;
                stateTimer = Random.Range(1f, 3f);
                navMeshAgent.speed = patrolSpeed;
                break;

            case SuspectDeerState.Patrol:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = patrolSpeed;
                // Jeśli nie mamy punktu docelowego, znajdź jeden
                if (patrolPoints.Length > 0)
                {
                    if (useRandomPatrolPoints)
                    {
                        currentPatrolPoint = Random.Range(0, patrolPoints.Length);
                    }
                    GoToPoint(patrolPoints[currentPatrolPoint].position);
                }
                else
                {
                    Vector3 randomPoint = startingPosition + Random.insideUnitSphere * patrolRadius;
                    randomPoint.y = startingPosition.y;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
                    {
                        GoToPoint(hit.position);
                    }
                }
                break;

            case SuspectDeerState.Observe:
                navMeshAgent.isStopped = true;
                stateTimer = observationTime;
                break;

            case SuspectDeerState.Stalk:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = stalkSpeed;
                stateTimer = stalkTime;
                break;

            case SuspectDeerState.Hunt:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = chaseSpeed;
                stateTimer = huntingDuration;
                break;

            case SuspectDeerState.Attack:
                navMeshAgent.isStopped = true;
                stateTimer = attackHitboxDuration;
                lastAttackTime = Time.time;

                // Uruchom animację ataku
                animator.SetTrigger(attackTriggerParam);

                // Wykonaj skok w kierunku gracza
                StartCoroutine(PerformAttackLunge());

                break;

            case SuspectDeerState.Flee:
                navMeshAgent.isStopped = false;
                navMeshAgent.speed = chaseSpeed;
                stateTimer = Random.Range(2f, 5f);
                break;

            case SuspectDeerState.StandUpright:
                navMeshAgent.isStopped = true;
                stateTimer = standUprightTime;
                // Włącz animację stania na dwóch nogach
                animator.SetBool(isUprightParam, true);
                break;
        }
    }

    private IEnumerator PerformAttackLunge()
    {
        // Czekaj na odpowiedni moment w animacji
        yield return new WaitForSeconds(0.1f);

        // Skok w kierunku gracza
        Vector3 lungeDirection = (player.position - transform.position).normalized;
        lungeDirection.y = 0;

        float originalStoppingDistance = navMeshAgent.stoppingDistance;
        navMeshAgent.stoppingDistance = 0;
        navMeshAgent.isStopped = false;
        navMeshAgent.velocity = lungeDirection * attackLungeDistance * 5f;

        // Aktywuj hitbox ataku z opóźnieniem
        yield return new WaitForSeconds(0.2f);
        attackHitbox.SetActive(true);

        // Wyłącz hitbox po określonym czasie
        yield return new WaitForSeconds(attackHitboxDuration);
        attackHitbox.SetActive(false);

        // Przywróć ustawienia
        navMeshAgent.stoppingDistance = originalStoppingDistance;
    }

    private void DetectPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Sprawdź czy gracz jest w zasięgu detekcji
        if (distanceToPlayer <= detectionRadius)
        {
            // Sprawdź czy gracz jest w polu widzenia (tylko jeśli nie jest zbyt blisko)
            if (distanceToPlayer <= immediateDetectionRadius)
            {
                playerDetected = true;
            }
            else
            {
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToPlayer);

                if (angle <= viewAngle * 0.5f)
                {
                    // Sprawdź czy coś nie blokuje widoku (ale z mniejszą szansą na utratę śledzenia)
                    RaycastHit hit;
                    Vector3 eyePosition = transform.position + Vector3.up * eyeHeight;
                    Vector3 targetEyePosition = player.position + Vector3.up * 1.7f; // Przybliżona wysokość oczu gracza

                    if (!Physics.Linecast(eyePosition, targetEyePosition, obstacleMask))
                    {
                        playerDetected = true;
                    }
                    else
                    {
                        // Trudniej stracić zainteresowanie graczem
                        if (playerDetected && currentState != SuspectDeerState.Flee && Random.value < 0.002f)
                        {
                            playerDetected = false;
                        }
                    }
                }
            }
        }
        else if (distanceToPlayer > detectionRadius * 1.5f)
        {
            // Gracz jest za daleko, trudno go stracić z widoku
            if (Random.value < 0.005f)
            {
                playerDetected = false;
            }
        }
    }

    private void GoToPoint(Vector3 point, float speed = -1)
    {
        if (speed > 0)
            navMeshAgent.speed = speed;

        navMeshAgent.SetDestination(point);
        navMeshAgent.isStopped = false;
        targetPosition = point;
    }

    private void UpdateAnimation()
    {
        // Aktualizuj parametr prędkości
        float normalizedSpeed = navMeshAgent.velocity.magnitude / navMeshAgent.speed;
        animator.SetFloat(speedParam, normalizedSpeed);

        // Aktualizuj parametr ruchu
        animator.SetBool(isMovingParam, navMeshAgent.velocity.magnitude > 0.1f);
    }

    private void UpdateRotation()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.1f)
        {
            // Obracaj się w kierunku ruchu
            Quaternion lookRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
        else if (currentState == SuspectDeerState.Observe || currentState == SuspectDeerState.Attack ||
                currentState == SuspectDeerState.StandUpright)
        {
            // Podczas obserwacji, ataku i stania na dwóch nogach zawsze patrz na gracza
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;

            if (directionToPlayer != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    // Funkcja wywoływana przez animację w momencie zadania obrażeń
    public void DealDamage()
    {
        // Domyślnie hitbox już jest aktywny, ale możemy dodać dodatkową logikę tutaj
        if (Vector3.Distance(transform.position, player.position) <= attackRange * 1.5f)
        {
            // Zadaj obrażenia graczowi
            // player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            Debug.Log("Gracz otrzymał: " + attackDamage + " obrażeń");
        }
    }

    // Funkcja do otrzymywania obrażeń
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        // Uruchom animację otrzymania obrażeń
        animator.SetTrigger(hitTriggerParam);

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (currentHealth < fleeHealthThreshold)
        {
            // Jeśli zdrowie jest niskie, zawsze uciekaj
            SetState(SuspectDeerState.Flee);
        }
    }

    private void Die()
    {
        isDead = true;
        navMeshAgent.isStopped = true;

        // Wyłącz kolizje
        Collider coll = GetComponent<Collider>();
        if (coll != null)
            coll.enabled = false;

        // Uruchom animację śmierci
        animator.SetBool(dieParam, true);

        // Wyłącz wszystkie skrypty
        enabled = false;
    }

    #endregion

    // Funkcje debugowania - pomocne przy rozwoju
    private void OnDrawGizmosSelected()
    {
        // Zasięg detekcji
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Zasięg natychmiastowej detekcji
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, immediateDetectionRadius);

        // Kąt widzenia
        Gizmos.color = Color.blue;
        Vector3 viewAngleA = Quaternion.AngleAxis(-viewAngle * 0.5f, Vector3.up) * transform.forward;
        Vector3 viewAngleB = Quaternion.AngleAxis(viewAngle * 0.5f, Vector3.up) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + viewAngleA * detectionRadius);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleB * detectionRadius);

        // Zasięg ataku
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Promień patrolowania
        if (patrolPoints.Length == 0)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(startingPosition, patrolRadius);
        }
    }
}