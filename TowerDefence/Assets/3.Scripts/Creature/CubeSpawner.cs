using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CubeSettings
{
    public GameObject cubePrefab; // 큐브 프리팹
    public float positionY;  // Position Y 값
    public float scaleY;     // Scale Y 값
}

public class CubeSpawner : MonoBehaviour
{
    public List<CubeSettings> cubeSettingsList; // 큐브 설정 리스트
    public Camera mainCamera;                   // 클릭 감지를 위한 카메라

    private GameObject previewCube;             // 프리뷰 큐브 오브젝트
    private CustomGrid customGrid;              // CustomGrid 스크립트 참조
    private Pathfinding_AstarMove pathfinding;  // Pathfinding 스크립트 참조
    private Quaternion currentRotation = Quaternion.identity; // 현재 회전 상태
    private Node currentPreviewNode;            // 현재 프리뷰가 고정된 노드
    private int selectedCubeIndex = 0;          // 선택된 큐브 인덱스

    void Start()
    {
        // CustomGrid와 Pathfinding 스크립트 찾기
        customGrid = FindObjectOfType<CustomGrid>();
        pathfinding = FindObjectOfType<Pathfinding_AstarMove>();

        if (customGrid == null)
        {
            Debug.LogError("씬에 CustomGrid 스크립트를 찾을 수 없습니다.");
        }

        if (pathfinding == null)
        {
            Debug.LogError("씬에 Pathfinding_AstarMove 스크립트를 찾을 수 없습니다.");
        }

        // 큐브 설정 리스트 확인
        if (cubeSettingsList != null && cubeSettingsList.Count > 0)
        {
            previewCube = Instantiate(cubeSettingsList[selectedCubeIndex].cubePrefab); // 첫 번째 큐브를 프리뷰로 설정
            previewCube.SetActive(false); // 초기 비활성화
        }
        else
        {
            Debug.LogError("큐브 설정 리스트가 비어 있습니다. 프리팹을 추가하세요.");
        }
    }

    void Update()
    {
        UpdatePreviewCube();

        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭 감지
        {
            TryPlaceCube();
        }

        if (Input.GetMouseButtonDown(1)) // 우클릭으로 좌우 회전
        {
            RotatePreviewCube();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // 마우스 휠 위로 스크롤
        {
            CycleCubePrefabs(1);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // 마우스 휠 아래로 스크롤
        {
            CycleCubePrefabs(-1);
        }
    }

    // 프리뷰 큐브 업데이트
    void UpdatePreviewCube()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (customGrid != null)
            {
                Node node = customGrid.NodeFromWorldPoint(hit.point);

                // 현재 노드와 이전 노드가 같다면 업데이트하지 않음
                if (node != null && node != currentPreviewNode)
                {
                    currentPreviewNode = node; // 현재 노드 저장
                    previewCube.SetActive(true); // 프리뷰 큐브 활성화
                    SetCubeProperties(previewCube, node.worldPosition, currentRotation, selectedCubeIndex); // 위치와 회전 설정

                    // 설치 가능 여부에 따라 색상 변경
                    UpdatePreviewColors(previewCube.transform);
                }
            }
        }
        else
        {
            previewCube.SetActive(false); // 레이가 맞는 곳이 없으면 비활성화
        }
    }

    // 큐브 설치 시도
    void TryPlaceCube()
    {
        if (currentPreviewNode != null && IsPlacementValid(previewCube.transform)) // 현재 노드와 전체 계층 구조 검사
        {
            GameObject newCube = Instantiate(cubeSettingsList[selectedCubeIndex].cubePrefab);
            SetCubeProperties(newCube, currentPreviewNode.worldPosition, currentRotation, selectedCubeIndex); // 위치와 회전 설정

            // CustomGrid 업데이트: 계층 구조 전체를 처리
            UpdateGridForHierarchy(newCube.transform);

            // 모든 Enemy의 경로를 재탐색하도록 요청
            Enemy.RequestPathUpdate();
            Debug.Log("모든 Enemy의 경로를 갱신했습니다.");
        }
        else
        {
            Debug.LogWarning("현재 노드는 설치가 불가능한 영역입니다.");
        }
    }

    // 프리뷰 큐브 좌우 회전
    void RotatePreviewCube()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0); // 90도 회전
    }

    // 큐브 프리팹 순환 변경
    void CycleCubePrefabs(int direction)
    {
        if (cubeSettingsList == null || cubeSettingsList.Count == 0)
        {
            Debug.LogWarning("큐브 설정 리스트가 비어 있습니다.");
            return;
        }

        selectedCubeIndex = (selectedCubeIndex + direction + cubeSettingsList.Count) % cubeSettingsList.Count; // 순환 변경
        Destroy(previewCube); // 기존 프리뷰 삭제
        previewCube = Instantiate(cubeSettingsList[selectedCubeIndex].cubePrefab); // 새 프리뷰 생성
        previewCube.SetActive(false); // 초기 비활성화
        Debug.Log($"현재 선택된 큐브 프리팹: {selectedCubeIndex + 1}/{cubeSettingsList.Count}");
    }

    // 위치와 크기, 회전을 설정
    private void SetCubeProperties(GameObject cube, Vector3 position, Quaternion rotation, int index)
    {
        if (cube != null && index >= 0 && index < cubeSettingsList.Count)
        {
            CubeSettings settings = cubeSettingsList[index];

            position.y = settings.positionY; // Position Y 설정
            cube.transform.position = position;

            Vector3 scale = cube.transform.localScale;
            scale.y = settings.scaleY; // Scale Y 설정
            cube.transform.localScale = scale;

            cube.transform.rotation = rotation; // 회전 설정
        }
    }

    // 계층 구조의 모든 자식 오브젝트를 처리하여 CustomGrid 업데이트 및 설치 가능 여부 확인
    private bool IsPlacementValid(Transform parent)
    {
        // 부모 오브젝트의 위치 확인
        if (!IsNodeWalkable(parent.position))
        {
            return false;
        }

        // 자식 오브젝트 순회
        foreach (Transform child in parent)
        {
            if (!IsPlacementValid(child))
            {
                return false;
            }
        }

        return true; // 모든 노드가 설치 가능하면 true 반환
    }

    // 특정 위치의 노드가 walkable인지 확인
    private bool IsNodeWalkable(Vector3 position)
    {
        Node node = customGrid.NodeFromWorldPoint(position);
        return node != null && node.walkable;
    }

    // 리스트에 포함된 모든 오브젝트의 설치 가능 여부를 확인하여 색상 변경
    private void UpdatePreviewColors(Transform parent)
    {
        Color color = IsPlacementValid(parent) ? Color.blue : Color.red;
        SetPreviewColor(color);
    }

    // 프리뷰 큐브의 색상을 설정
    private void SetPreviewColor(Color color)
    {
        Renderer renderer = previewCube.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = color;
        }
    }

    // 계층 구조의 모든 자식 오브젝트를 처리하여 CustomGrid 업데이트
    private void UpdateGridForHierarchy(Transform parent)
    {
        customGrid.UpdateGrid(parent.position);

        foreach (Transform child in parent)
        {
            UpdateGridForHierarchy(child);
        }
    }
}
