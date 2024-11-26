using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // 생성할 큐브 프리팹
    public Camera mainCamera;     // 클릭 감지를 위한 카메라
    public float fixedY = 0.6f;   // 생성된 큐브의 y값 (Inspector에서 설정 가능)

    private GameObject previewCube; // 프리뷰 큐브 오브젝트
    private CustomGrid customGrid; // CustomGrid 스크립트 참조
    private Pathfinding_AstarMove pathfinding; // Pathfinding 스크립트 참조
    private Quaternion currentRotation = Quaternion.identity; // 현재 회전 상태

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
        previewCube = Instantiate(cubePrefab);
        previewCube.SetActive(false); // 초기 비활성화
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
                if (node != null)
                {
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
                else
                {
                    previewCube.SetActive(false); // 유효하지 않은 노드일 경우 비활성화
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (customGrid != null)
            {
                Node node = customGrid.NodeFromWorldPoint(hit.point);
                if (node != null && node.walkable) // 설치 가능 확인
                {
                    Vector3 spawnPosition = node.worldPosition;
                    spawnPosition.y = fixedY; // y값 설정

                    GameObject newCube = Instantiate(cubePrefab, spawnPosition, currentRotation);

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
                    Debug.LogWarning("클릭한 위치는 설치가 불가능한 영역입니다.");
                }
            }
        }
    }

    // 프리뷰 큐브 좌우 회전
    void RotatePreviewCube()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0); // 90도 회전
    }
}
