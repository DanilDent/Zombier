using UnityEngine;

[ExecuteInEditMode]
public class ObstacleComp : MonoBehaviour
{
    public bool DrawGizmos = false;
    public Bounds Bounds;
    public float BoundsScale = 1f;

    private Color _boundsColor;
    private void OnEnable()
    {
        _boundsColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 0.5f);
        RecalBounds();
    }

    private void Update()
    {
        if (Application.isEditor)
        {
            if (transform.hasChanged)
            {
                RecalBounds();
                transform.hasChanged = false;
            }
        }
    }

    public void RecalBounds()
    {
        Bounds = new Bounds(transform.position, Vector3.one);
        var collidres = GetComponentsInChildren<Collider>();

        foreach (var collider in collidres)
        {
            Bounds.Encapsulate(collider.bounds);
        }

        Bounds.Expand(BoundsScale);
    }

    private void OnDrawGizmos()
    {
        if (!DrawGizmos)
        {
            return;
        }
        Gizmos.color = _boundsColor;
        Gizmos.DrawCube(Bounds.center, Bounds.size);

        Gizmos.color = Color.red;
        DrawRay(Bounds.center + transform.right * Bounds.extents.x + Vector3.up * 30f);
        DrawRay(Bounds.center - transform.right * Bounds.extents.x + Vector3.up * 30f);
        DrawRay(Bounds.center + transform.forward * Bounds.extents.z + Vector3.up * 30f);
        DrawRay(Bounds.center - transform.forward * Bounds.extents.z + Vector3.up * 30f);
    }

    private bool DrawRay(Vector3 origin)
    {
        Gizmos.DrawRay(origin, Vector3.down * 100f);

        return false;
    }
}
