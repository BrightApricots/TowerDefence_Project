using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance; // �̱��� �ν��Ͻ�
    public Grid grid; // Unity Grid ������Ʈ ����

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ���� ��ǥ -> �׸��� ��ǥ�� ��ȯ
    public Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    // �׸��� ��ǥ -> ���� ��ǥ�� ��ȯ
    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        return grid.CellToWorld(gridPosition) + new Vector3(grid.cellSize.x / 2f, 0, grid.cellSize.z / 2f);
    }
}
