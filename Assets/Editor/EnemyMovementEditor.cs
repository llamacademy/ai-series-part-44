using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovement))]
[CanEditMultipleObjects]
public class EnemyMovementEditor : Editor
{
    private GUIStyle PlayerFontStyle;
    private GUIStyle ProjectionFontStyle;
    private void OnEnable()
    {
        PlayerFontStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.black
            },
            fontSize = 24
        };
        ProjectionFontStyle = new GUIStyle()
        {
            normal = new GUIStyleState()
            {
                textColor = Color.yellow
            },
            fontSize = 24
        };
    }

    private void OnSceneGUI()
    {
        EnemyMovement movement = (EnemyMovement)target;

        Player player = movement.Player;

        if (player != null)
        {
            if (movement.Agent.hasPath)
            {
                Handles.color = Color.green;
                Handles.DrawSolidDisc(movement.Agent.destination, Vector3.up, 0.25f);
                Handles.Label(movement.Agent.destination, "Destination");
            }

            Handles.color = Color.black;
            Handles.DrawLine(movement.transform.position, player.transform.position);
            Handles.Label(Vector3.Lerp(player.transform.position, movement.transform.position, 0.5f), "Player Position", PlayerFontStyle);
            Handles.DrawSolidDisc(player.transform.position, Vector3.up, 0.25f);
            Vector3 targetPosition = player.transform.position + player.Movement.AverageVelocity * movement.MovementPredictionTime;

            Handles.color = Color.yellow;
            Handles.DrawLine(movement.transform.position, targetPosition);
            Handles.DrawSolidDisc(targetPosition, Vector3.up, 0.25f);

            Handles.Label(Vector3.Lerp(targetPosition, movement.transform.position, 0.5f), "Projected Position", ProjectionFontStyle);

            Vector3 directionToTarget = (targetPosition - movement.transform.position).normalized;
            Vector3 directionToPlayer = (player.transform.position - movement.transform.position).normalized;

            Handles.ArrowHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), movement.transform.position, Quaternion.LookRotation(directionToTarget), 2f, EventType.Repaint);
            Handles.color = Color.black;
            Handles.ArrowHandleCap(EditorGUIUtility.GetControlID(FocusType.Passive), movement.transform.position, Quaternion.LookRotation(directionToPlayer), 2f, EventType.Repaint);

            float dot = Vector3.Dot(directionToPlayer, directionToTarget);
            Handles.Label(movement.transform.position - (movement.Agent.velocity.normalized), $"Dot: {dot:N2}", PlayerFontStyle);

            Vector3[] corners = movement.Agent.path.corners;
            for (int i = 1; i < corners.Length; i++)
            {
                Handles.color = Color.cyan;
                Handles.DrawLine(corners[i - 1], corners[i], 3);
            }
        }
    }
}
