using UnityEngine;

[DisallowMultipleComponent]
public class Enemy : MonoBehaviour
{
    public EnemyMovement Movement;

    private void Awake()
    {
        Movement = GetComponent<EnemyMovement>();
    }
}
