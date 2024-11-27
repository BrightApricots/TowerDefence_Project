using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public List<GameObject> cubePrefabs; // 생성할 큐브 프리팹 리스트
    public Camera mainCamera;           // 클릭 감지를 위한 카메라
    public float fixedY = 0.6f;         // 생성된 큐브의 y값 (Inspector에서 설정 가능)

    private GameObject previewCube;     // 프리뷰 큐브 오브젝트
    private CustomGrid customGrid;      // CustomGrid 스크립트 참조
    private Pathfinding_AstarMove pathfinding; // Pathfinding 스크립트 참조
    private Quaternion currentRotation = Quaternion.identity; // 현재 회전 상태
    private Node currentPreviewNode;    // 현재 프리뷰가 고정된 노드
    private int selectedCubeIndex = 0;  // 선택된 큐브 프리팹 인덱스

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

        // 프리뷰 큐브 생성
        if (cubePrefabs != null && cubePrefabs.Count > 0)
        {
            previewCube = Instantiate(cubePrefabs[selectedCubeIndex]); // 첫 번째 큐브를 프리뷰로 설정
            previewCube.SetActive(false); // 초기 비활성화
        }
        else
        {
            Debug.LogError("큐브 프리팹 리스트가 비어 있습니다. 프리팹을 추가하세요.");
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
                    Vector3 previewPosition = node.worldPosition;
                    previewPosition.y = fixedY; // y값 설정
                    previewCube.transform.position = previewPosition;
                    previewCube.transform.rotation = currentRotation;

                    // 설치 가능 여부에 따라 색상 변경
                    Renderer renderer = previewCube.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = node.walkable ? Color.blue : Color.red;
                    }
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
        if (currentPreviewNode != null && currentPreviewNode.walkable) // 현재 노드의 설치 가능 여부 확인
        {
            Vector3 spawnPosition = currentPreviewNode.worldPosition;
            spawnPosition.y = fixedY; // y값 설정

            GameObject newCube = Instantiate(cubePrefabs[selectedCubeIndex], spawnPosition, currentRotation);

            // CustomGrid 업데이트: 새로운 장애물 위치 반영
            customGrid.UpdateGrid(newCube.transform.position);

            // Pathfinding 경로 재탐색
            if (pathfinding != null)
            {
                pathfinding.UpdatePath(Vector3.zero, new Vector3(10, 0, 10)); // 예시 경로
                Debug.Log("경로 재탐색 완료");
            }
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
        if (cubePrefabs == null || cubePrefabs.Count == 0)
        {
            Debug.LogWarning("큐브 프리팹 리스트가 비어 있습니다.");
            return;
        }

        selectedCubeIndex = (selectedCubeIndex + direction + cubePrefabs.Count) % cubePrefabs.Count; // 순환 변경
        Destroy(previewCube); // 기존 프리뷰 삭제
        previewCube = Instantiate(cubePrefabs[selectedCubeIndex]); // 새 프리뷰 생성
        previewCube.SetActive(false); // 초기 비활성화
        Debug.Log($"현재 선택된 큐브 프리팹: {selectedCubeIndex + 1}/{cubePrefabs.Count}");
    }
}
