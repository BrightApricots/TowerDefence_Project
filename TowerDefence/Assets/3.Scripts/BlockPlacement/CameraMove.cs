using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    public GameObject parent;
    public Camera MainCam;
    Vector3 defPosition;
    Quaternion defRotation;
    float defZoom;
    public float moveSpeed = 5f;  // 이동 속도 조절 변수 추가

    Vector3 forward;
    Vector3 right;
    void Start()
    {
        // 기본 위치 저장
        defPosition = transform.position;
        defRotation = parent.transform.rotation;
        defZoom = Camera.main.fieldOfView;
       

    }
    //    float mouseX = -Input.GetAxis("Mouse X");
    //    float mouseY = -Input.GetAxis("Mouse Y");

    //    // 카메라의 forward와 right 벡터 가져오기
    //    Camera cam = GetComponentInChildren<Camera>();
    //    Vector3 forward = cam.transform.forward;
    //    Vector3 right = cam.transform.right;

    //    // y 성분을 0으로 만들어서 수평 평면상의 이동만 처리
    //    forward.y = 0;
    //   right.y = 0;
    //   forward.Normalize();
    //   right.Normalize();

    //   // 이동 방향 계산
    //   Vector3 moveDirection = (forward * mouseY + right * mouseX).normalized;

    //    // 이동 적용
    //parent.transform.Translate(moveDirection* moveSpeed * Time.deltaTime, Space.World);

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            forward = MainCam.transform.forward;
            right = MainCam.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();
            float mouseX = -Input.GetAxis("Mouse X");
            float mouseY = -Input.GetAxis("Mouse Y");

            // 이동 방향 계산
            Vector3 moveDirection = (forward * mouseY + right * mouseX).normalized;

            // 이동 적용
            parent.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
            //Vector3 moveDirection = new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y")).normalized;
            //parent.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.fieldOfView -= (20 * Input.GetAxis("Mouse ScrollWheel"));
        }

        if (Camera.main.fieldOfView < 10)
            Camera.main.fieldOfView = 10;
        else if (Camera.main.fieldOfView > 100)
            Camera.main.fieldOfView = 100;
    }
   
}
