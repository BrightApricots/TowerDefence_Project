using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid grid; // Unity Grid ������Ʈ ����

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
