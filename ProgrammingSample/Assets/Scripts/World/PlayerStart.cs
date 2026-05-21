using AnyoxGames.Service;
using UnityEditor;
using UnityEngine;

public class PlayerStart : MonoBehaviourService
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.up, transform.rotation, new Vector3(0.75f, 2, 0.75f));
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
        Handles.ArrowHandleCap(-1,  transform.position + Vector3.up, transform.rotation, 1f, EventType.Repaint);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.TRS(transform.position + Vector3.up, transform.rotation, new Vector3(0.75f, 2, 0.75f));
        Gizmos.color = new Color(1.0f, 1.0f, 1.0f, 0.1f);
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        Gizmos.matrix = Matrix4x4.identity;
        Handles.ArrowHandleCap(-1,  transform.position + Vector3.up, transform.rotation, 1f, EventType.Ignore);
    }
}