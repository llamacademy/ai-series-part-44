using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private Player Player;
    [SerializeField]
    private Enemy EnemyPrefab;
    [SerializeField]
    [Range(1, 100)]
    private int EnemiesToSpawn = 10;

    private NavMeshTriangulation Triangulation;

    private void Awake()
    {
        NavMesh.pathfindingIterationsPerFrame = 400;
        Triangulation = NavMesh.CalculateTriangulation();
    }

    private void Start()
    {
        for (int i = 0; i < EnemiesToSpawn; i++)
        {
            int index = Random.Range(1, Triangulation.vertices.Length);
            Enemy enemy = Instantiate(EnemyPrefab,
               Vector3.Lerp(Triangulation.vertices[index-1], Triangulation.vertices[index], Random.value),
               Quaternion.identity
            );
            enemy.Movement.Triangulation = Triangulation;
            enemy.Movement.GoTowardsPlayer(Player);
        }
    }

}