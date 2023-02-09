using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public NavMeshTriangulation Triangulation;
    public Player Player;  // for custom editor drawing stuff
    public NavMeshAgent Agent;
    [SerializeField]
    [Range(0f, 3f)]
    private float WaitDelay = 1f;
    [SerializeField]
    public bool UseMovementPrediction;
    [SerializeField]
    [Range(-1, 1)]
    public float MovementPredictionThreshold = 0;
    [SerializeField]
    [Range(0.25f, 2f)]
    public float MovementPredictionTime = 1f;
    public bool UseDistanceBasedPrediction;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    public void GoToRandomPoint()
    {
        StartCoroutine(DoMoveToRandomPoint());
    }

    public void GoTowardsPlayer(Player Player)
    {
        StartCoroutine(DoMoveToPlayer(Player));
    }

    public void StopMoving()
    {
        Agent.isStopped = true;
        StopAllCoroutines();
    }

    private IEnumerator DoMoveToRandomPoint()
    {
        Agent.enabled = true;
        Agent.isStopped = false;
        WaitForSeconds Wait = new WaitForSeconds(WaitDelay);
        while (true)
        {
            int index = Random.Range(1, Triangulation.vertices.Length - 1);
            Agent.SetDestination(Vector3.Lerp(
                Triangulation.vertices[index],
                Triangulation.vertices[index + (Random.value > 0.5f ? -1 : 1)],
                Random.value)
            );

            yield return null;
            yield return new WaitUntil(() => Agent.remainingDistance <= Agent.stoppingDistance);
            yield return Wait;
        }
    }

    private IEnumerator DoMoveToPlayer(Player Player)
    {
        WaitForSeconds repathingDelay = new WaitForSeconds(0.15f);
        Agent.enabled = true;
        this.Player = Player; // for custom editor drawing stuff
        Agent.isStopped = false;
        while (true)
        {
            if (!UseMovementPrediction)
            {
                Agent.SetDestination(Player.transform.position);
            }
            else
            {
                float timeToPlayer = Vector3.Distance(Player.transform.position, transform.position) / Agent.speed;
                if (timeToPlayer > MovementPredictionTime)
                {
                    timeToPlayer = MovementPredictionTime;
                }
                
                Vector3 targetPosition = Player.transform.position + Player.Movement.AverageVelocity * timeToPlayer;
                Vector3 directionToTarget = (targetPosition - transform.position).normalized;
                Vector3 directionToPlayer = (Player.transform.position - transform.position).normalized;

                float dot = Vector3.Dot(directionToPlayer, directionToTarget);

                if (dot < MovementPredictionThreshold)
                {
                    targetPosition = Player.transform.position;
                }


                Agent.SetDestination(targetPosition);
            }

            yield return repathingDelay;
        }
    }

}