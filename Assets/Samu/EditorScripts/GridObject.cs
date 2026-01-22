using UnityEngine;


public class GridObject : MonoBehaviour
{
    [Header("Dimensions on the grid")]
    public int gridSizeX = 1;
    public int gridSizeY = 1;
    public int gridSizeZ = 1;

    [Header("Grid Snapping")]
    public bool snapToGrid = true;

    [Header("Auto-Configurationn")]
    public bool autoCalculateSize = true;
    public ObjectType objectType;

    public Color gizmoColor = new Color(0, 1, 0, 0.3f);
    public bool showGizmos = true;

    public enum ObjectType
    {
        Floor,
        Wall,
        Counter,
        Obstacle,
        Stair,
        Platform,
        Decoration,
        Cachorro
    }

    private void OnValidate()
    {
        if (autoCalculateSize)
        {
            CalculateGridSizeFromBounds();
        }
    }
    public void CalculateGridSizeFromBounds()
    {
        Bounds bounds = GetTotalBounds();

        if (bounds.size == Vector3.zero)
        {
            gridSizeX = gridSizeY = gridSizeZ = 1;
            return;
        }

        gridSizeX = Mathf.Max(1, Mathf.CeilToInt(bounds.size.x));
        gridSizeY = Mathf.Max(1, Mathf.CeilToInt(bounds.size.y));
        gridSizeZ = Mathf.Max(1, Mathf.CeilToInt(bounds.size.z));
    }
    private Bounds GetTotalBounds()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        Collider[] colliders = GetComponentsInChildren<Collider>();

        Bounds bounds = new Bounds(transform.position, Vector3.zero);
        bool boundsInitialized = false;

        foreach (Renderer renderer in renderers)
        {
            if (!boundsInitialized)
            {
                bounds = renderer.bounds;
                boundsInitialized = true;
            }
            else
            {
                bounds.Encapsulate(renderer.bounds);
            }
        }

        if (!boundsInitialized)
        {
            foreach (Collider col in colliders)
            {
                if (!boundsInitialized)
                {
                    bounds = col.bounds;
                    boundsInitialized = true;
                }
                else
                {
                    bounds.Encapsulate(col.bounds);
                }
            }
        }

        return bounds;
    }

    public Vector3 GetGridBasePosition()
    {
        Vector3 offset = new Vector3(
            gridSizeX * 0.5f,
            gridSizeY * 0.5f,
            gridSizeZ * 0.5f
        );

        return transform.position - offset;
    }
    public Vector3 GetSnappedPosition()
    {
        if (!snapToGrid)
            return transform.position;

        Vector3 basePos = GetGridBasePosition();
        Vector3 snappedBase = new Vector3(
            Mathf.Round(basePos.x),
            Mathf.Round(basePos.y),
            Mathf.Round(basePos.z)
        );

        Vector3 offset = new Vector3(
            gridSizeX * 0.5f,
            gridSizeY * 0.5f,
            gridSizeZ * 0.5f
        );

        return snappedBase + offset;
    }
    public void SnapToGridNow()
    {
        if (snapToGrid)
        {
            transform.position = GetSnappedPosition();
        }
    }

    public Vector3Int[] GetOccupiedGridCells()
    {
        Vector3 basePos = GetGridBasePosition();
        Vector3Int baseInt = new Vector3Int(
            Mathf.RoundToInt(basePos.x),
            Mathf.RoundToInt(basePos.y),
            Mathf.RoundToInt(basePos.z)
        );

        Vector3Int[] cells = new Vector3Int[gridSizeX * gridSizeY * gridSizeZ];

        int index = 0;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    cells[index] = baseInt + new Vector3Int(x, y, z);
                    index++;
                }
            }
        }

        return cells;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Vector3 basePos = GetGridBasePosition();
        if (snapToGrid)
        {
            basePos = new Vector3(
                Mathf.Round(basePos.x),
                Mathf.Round(basePos.y),
                Mathf.Round(basePos.z)
            );
        }

        Vector3 size = new Vector3(gridSizeX, gridSizeY, gridSizeZ);
        Vector3 center = basePos + size * 0.5f;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.15f);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                for (int z = 0; z < gridSizeZ; z++)
                {
                    Vector3 cellCenter = basePos + new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);
                    Gizmos.DrawWireCube(cellCenter, Vector3.one * 0.95f);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 basePos = GetGridBasePosition();
        if (snapToGrid)
        {
            basePos = new Vector3(
                Mathf.Round(basePos.x),
                Mathf.Round(basePos.y),
                Mathf.Round(basePos.z)
            );
        }

        Vector3 size = new Vector3(gridSizeX, gridSizeY, gridSizeZ);
        Vector3 center = basePos + size * 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, basePos);
        Gizmos.DrawSphere(basePos, 0.1f);
    }
}