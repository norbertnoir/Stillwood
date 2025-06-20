using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoeDeerManager : MonoBehaviour
{
    public RoeDeerMovement roeDeerFemalePrefab;
    public List<RoeDeerMovement> roeDeers = new List<RoeDeerMovement>();
    public GameObject player;

    public int roeDeerFemaleCount = 10;

    [Header("Deer Spawning")]
    public float minSpawnDistance = 15f;
    public float maxSpawnDistance = 30f;

    [Header("Distance Constraints")]
    public float minDistanceFromPlayer = 15f;  // Minimalna odległość od gracza
    public float maxDistanceFromPlayer = 40f;  // Maksymalna odległość od gracza

    [Header("Deer Behavior")]
    public float updateFrequency = 2f;         // Jak często sarny aktualizują swoje zachowanie
    public float fleeDistance = 20f;           // Dystans, przy którym sarny uciekają od gracza
    public float minDistanceBetweenDeer = 3f;  // Minimalna odległość między sarnami
    public float surroundRadius = 25f;         // Promień okręgu wokół gracza
    public float idleProbability = 0.7f;       // Zwiększone prawdopodobieństwo bezczynności
    public float minIdleTime = 3f;             // Minimalny czas stania w miejscu
    public float maxIdleTime = 8f;             // Zwiększony maksymalny czas stania w miejscu

    [Header("Flock Settings")]
    public float cohesionWeight = 1.0f;
    public float separationWeight = 1.5f;
    public float alignmentWeight = 1.0f;

    private float nextUpdateTime;
    private Dictionary<RoeDeerMovement, float> idleTimers = new Dictionary<RoeDeerMovement, float>();

    void Start()
    {
        SpawnDeer();
        nextUpdateTime = Time.time + Random.Range(0, updateFrequency);
    }

    void Update()
    {
        // Regularnie sprawdzaj pozycje saren
        if (Time.time >= nextUpdateTime)
        {
            UpdateDeerBehavior();
            nextUpdateTime = Time.time + updateFrequency;
        }

        // Sprawdź odległość od gracza w każdej klatce (dla natychmiastowej ucieczki)
        CheckFleeingConditions();

        // Aktualizacja timerów bezczynności
        UpdateIdleTimers();
    }

    void UpdateIdleTimers()
    {
        List<RoeDeerMovement> deersToMove = new List<RoeDeerMovement>();

        foreach (var deer in roeDeers)
        {
            if (idleTimers.ContainsKey(deer) && idleTimers[deer] > 0)
            {
                idleTimers[deer] -= Time.deltaTime;
                if (idleTimers[deer] <= 0)
                {
                    // Koniec czasu bezczynności, wznów ruch
                    deersToMove.Add(deer);
                }
            }
        }

        // Przydziel nowe cele dla saren, które zakończyły bezczynność
        foreach (var deer in deersToMove)
        {
            AssignNewTargetForDeer(deer);
        }
    }

    void CheckFleeingConditions()
    {
        foreach (var deer in roeDeers)
        {
            float distanceToPlayer = Vector3.Distance(deer.transform.position, player.transform.position);

            // Natychmiastowa ucieczka gdy gracz za blisko
            if (distanceToPlayer < fleeDistance)
            {
                Vector3 fleeDirection = (deer.transform.position - player.transform.position).normalized;
                Vector3 fleeTarget = CalculateFleeTarget(deer, fleeDirection);
                deer.RunTo(fleeTarget);
                idleTimers[deer] = 0f;
            }
            // Sprawdź czy sarna nie jest za daleko od gracza
            else if (distanceToPlayer > maxDistanceFromPlayer && !deer.IsMoving)
            {
                // Jeśli sarna jest bezczynna i za daleko, przymuś ją do ruchu
                AssignNewTargetForDeer(deer);
            }
        }
    }

    Vector3 CalculateFleeTarget(RoeDeerMovement deer, Vector3 fleeDirection)
    {
        // Oblicz punkt ucieczki na optymalnej odległości
        Vector3 fleeTarget = player.transform.position + fleeDirection * surroundRadius * 1.2f;

        // Upewnij się, że punkt ucieczki jest na NavMesh
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(fleeTarget, out hit, 20f, UnityEngine.AI.NavMesh.AllAreas))
        {
            fleeTarget = hit.position;
        }

        return fleeTarget;
    }

    void SpawnDeer()
    {
        for (int i = 0; i < roeDeerFemaleCount; i++)
        {
            // Losowy punkt wokół gracza z zachowaniem minimalnej odległości
            Vector2 randomCircle = Random.insideUnitCircle.normalized *
                                  Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);
            Vector3 spawnPosition = player.transform.position + new Vector3(randomCircle.x, 0, randomCircle.y);

            // Upewnij się, że punkt jest na NavMesh
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPosition, out hit, 10f, UnityEngine.AI.NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
            }

            RoeDeerMovement newDeer = Instantiate(roeDeerFemalePrefab, spawnPosition, Quaternion.identity);
            roeDeers.Add(newDeer);

            // Na początku każda sarna stoi przez losowy czas
            idleTimers.Add(newDeer, Random.Range(minIdleTime, maxIdleTime));
            newDeer.StayIdle();
        }
    }

    void UpdateDeerBehavior()
    {
        foreach (var deer in roeDeers)
        {
            float distanceToPlayer = Vector3.Distance(deer.transform.position, player.transform.position);

            // Jeśli sarna jest bezczynna i ma aktywny timer, kontynuuj bezczynność
            if (idleTimers.ContainsKey(deer) && idleTimers[deer] > 0)
            {
                continue;
            }

            // Jeśli sarna nie ma aktywnego timera i zakończyła przemieszczanie
            if (deer.HasReachedDestination())
            {
                // Wysokie prawdopodobieństwo, że zostanie bezczynna
                if (Random.value < idleProbability)
                {
                    deer.StayIdle();
                    idleTimers[deer] = Random.Range(minIdleTime, maxIdleTime);
                    continue;
                }
                else
                {
                    // W przeciwnym razie - przesuń się na nowe miejsce wokół gracza
                    AssignNewTargetForDeer(deer);
                }
            }
        }
    }

    void AssignNewTargetForDeer(RoeDeerMovement deer)
    {
        Vector3 cohesion = CalculateCohesion(deer);
        Vector3 separation = CalculateSeparation(deer);
        Vector3 alignment = CalculateAlignment(deer);

        // Znajdź pozycję na okręgu wokół gracza
        Vector3 targetPosition = FindValidPositionAroundPlayer(deer, cohesion, separation, alignment);

        // Upewnij się, że docelowy punkt jest na NavMesh
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(targetPosition, out hit, 20f, UnityEngine.AI.NavMesh.AllAreas))
        {
            targetPosition = hit.position;
        }

        deer.RunTo(targetPosition);
    }

    Vector3 FindValidPositionAroundPlayer(RoeDeerMovement deer, Vector3 cohesion, Vector3 separation, Vector3 alignment)
    {
        // Próbuj znaleźć pozycję spełniającą wymagania dotyczące odległości
        for (int attempt = 0; attempt < 10; attempt++)
        {
            // Losowy kąt na okręgu
            float angle = Random.Range(0f, 2f * Mathf.PI);
            // Losowa odległość między min a max
            float distance = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer);

            Vector3 circlePosition = player.transform.position + new Vector3(
                Mathf.Cos(angle) * distance,
                0,
                Mathf.Sin(angle) * distance
            );

            // Połącz siły zachowania stadnego z pozycją na okręgu
            Vector3 targetPosition = circlePosition +
                                    cohesion * cohesionWeight +
                                    separation * separationWeight +
                                    alignment * alignmentWeight;

            float distToPlayer = Vector3.Distance(targetPosition, player.transform.position);

            // Sprawdź, czy pozycja spełnia wymagania dotyczące odległości
            if (distToPlayer >= minDistanceFromPlayer && distToPlayer <= maxDistanceFromPlayer)
            {
                return targetPosition;
            }
        }

        // Jeśli nie udało się znaleźć idealnej pozycji, zwróć bezpieczną pozycję na okręgu
        float safeAngle = Random.Range(0f, 2f * Mathf.PI);
        Vector3 safePosition = player.transform.position + new Vector3(
            Mathf.Cos(safeAngle) * ((minDistanceFromPlayer + maxDistanceFromPlayer) / 2),
            0,
            Mathf.Sin(safeAngle) * ((minDistanceFromPlayer + maxDistanceFromPlayer) / 2)
        );

        return safePosition;
    }

    // Reguły zachowań stadnych (flocking)
    Vector3 CalculateCohesion(RoeDeerMovement currentDeer)
    {
        Vector3 centerPosition = Vector3.zero;
        int count = 0;

        foreach (var deer in roeDeers)
        {
            if (deer != currentDeer)
            {
                float distance = Vector3.Distance(currentDeer.transform.position, deer.transform.position);
                if (distance < surroundRadius)
                {
                    centerPosition += deer.transform.position;
                    count++;
                }
            }
        }

        if (count > 0)
        {
            centerPosition /= count;
            return (centerPosition - currentDeer.transform.position).normalized;
        }

        return Vector3.zero;
    }

    Vector3 CalculateSeparation(RoeDeerMovement currentDeer)
    {
        Vector3 separationForce = Vector3.zero;

        foreach (var deer in roeDeers)
        {
            if (deer != currentDeer)
            {
                float distance = Vector3.Distance(currentDeer.transform.position, deer.transform.position);
                if (distance < minDistanceBetweenDeer)
                {
                    Vector3 moveAway = (currentDeer.transform.position - deer.transform.position).normalized;
                    moveAway /= Mathf.Max(0.1f, distance); // Im bliżej, tym silniejsza siła
                    separationForce += moveAway;
                }
            }
        }

        return separationForce.normalized;
    }

    Vector3 CalculateAlignment(RoeDeerMovement currentDeer)
    {
        Vector3 averageDirection = Vector3.zero;
        int count = 0;

        foreach (var deer in roeDeers)
        {
            if (deer != currentDeer)
            {
                float distance = Vector3.Distance(currentDeer.transform.position, deer.transform.position);
                if (distance < surroundRadius / 2)
                {
                    if (deer.IsMoving)
                    {
                        averageDirection += deer.GetMoveDirection();
                        count++;
                    }
                }
            }
        }

        if (count > 0)
        {
            averageDirection /= count;
            return averageDirection.normalized;
        }

        return Vector3.zero;
    }
}