using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance; // 싱글톤 인스턴스
    public Grid grid; // Unity Grid 컴포넌트 참조

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

    // 월드 좌표 -> 그리드 좌표로 변환
    public Vector3Int WorldToGrid(Vector3 worldPosition)
    {
        return grid.WorldToCell(worldPosition);
    }

    // 그리드 좌표 -> 월드 좌표로 변환
    public Vector3 GridToWorld(Vector3Int gridPosition)
    {
        return grid.CellToWorld(gridPosition) + new Vector3(grid.cellSize.x / 2f, 0, grid.cellSize.z / 2f);
    }
}
