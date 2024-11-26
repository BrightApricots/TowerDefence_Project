using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // 생성할 큐브 프리팹
    public Camera mainCamera;     // 클릭 감지를 위한 카메라
    public float fixedY = 0.6f;   // 생성된 큐브의 y값 (Inspector에서 설정 가능)

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 왼쪽 마우스 클릭 감지
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // 화면 좌표 -> 월드 레이
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // 레이가 충돌한 위치
            {
                // 충돌 지점에 큐브 생성
                GameObject newCube = Instantiate(cubePrefab, hit.point, Quaternion.identity);

                // 생성된 큐브의 y 값을 Inspector에서 설정된 값으로 고정
                Vector3 fixedPosition = newCube.transform.position;
                fixedPosition.y = fixedY; // y값 설정
                newCube.transform.position = fixedPosition;
            }
        }
    }
}
