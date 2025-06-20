using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoeDeerMovement : MonoBehaviour
{
    [SerializeField] NavMeshAgent navMeshAgent;
    [SerializeField] Animator animator;

    public bool IsMoving { get; private set; }

    private Vector3 moveDirection;

    [Header("Animation")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private string movingBoolParam = "isMoving";

    void Start()
    {
        navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        if (navMeshAgent == null || !navMeshAgent.isOnNavMesh || !navMeshAgent.isActiveAndEnabled)
        {
            Debug.LogError("NavMeshAgent " + navMeshAgent.isOnNavMesh + " " + navMeshAgent.isActiveAndEnabled);
            return;
        }
        UpdateMovementStatus();
        UpdateRotation();
    }

    private void UpdateMovementStatus()
    {
        IsMoving = !HasReachedDestination();

        if (navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            moveDirection = navMeshAgent.velocity.normalized;
            animator.SetBool(movingBoolParam, true);
        }
        else if (HasReachedDestination())
        {
            animator.SetBool(movingBoolParam, false);
        }
    }

    private void UpdateRotation()
    {
        if (navMeshAgent.velocity.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(navMeshAgent.velocity.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void StayIdle()
    {
        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        animator.SetBool(movingBoolParam, false);
        IsMoving = false;
    }

    public void RunTo(Vector3 targetPosition)
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(targetPosition);
        IsMoving = true;
    }

    public bool HasReachedDestination()
    {
        return !navMeshAgent.pathPending &&
               navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance &&
               (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }
}