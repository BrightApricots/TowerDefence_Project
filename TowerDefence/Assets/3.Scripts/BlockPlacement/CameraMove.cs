using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMove : MonoBehaviour
{
    public GameObject parent;
    public Camera MainCam;
    public CinemachineVirtualCamera virtualCamera;

    Vector3 defPosition;
    Quaternion defRotation;
    float defZoom;
    public float moveSpeed = 20f;
    public float zoomSpeed = 10f;

    Vector3 forward;
    Vector3 right;
    void Start()
    {
        defPosition = transform.position;
        defRotation = parent.transform.rotation;
        defZoom = Camera.main.fieldOfView;
    }

    public bool IsPointerOverUI() => EventSystem.current.IsPointerOverGameObject();

    private void Update()
    {
        //if (!IsPointerOverUI())
        //{
        //    if (Input.GetMouseButton(0))
        //    {
        //        forward = MainCam.transform.forward;
        //        right = MainCam.transform.right;

        //        forward.y = 0;
        //        right.y = 0;
        //        forward.Normalize();
        //        right.Normalize();
        //        float mouseX = -Input.GetAxis("Mouse X");
        //        float mouseY = -Input.GetAxis("Mouse Y");

        //        // 이동 방향 계산
        //        Vector3 moveDirection = (forward * mouseY + right * mouseX).normalized;

        //        // 이동 적용
        //        parent.transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        //    }

        //    if (Input.GetAxis("Mouse ScrollWheel") != 0)
        //    {
        //        Camera.main.fieldOfView -= (20 * Input.GetAxis("Mouse ScrollWheel"));
        //    }

        //    if (Camera.main.fieldOfView < 10)
        //        Camera.main.fieldOfView = 10;
        //    else if (Camera.main.fieldOfView > 100)
        //        Camera.main.fieldOfView = 100;
        //}

        if (!IsPointerOverUI())
        {
            if (Input.GetMouseButton(0))
            {
                forward = virtualCamera.transform.forward;
                right = virtualCamera.transform.right;

                forward.y = 0;
                right.y = 0;
                forward.Normalize();
                right.Normalize();
                float mouseX = -Input.GetAxis("Mouse X");
                float mouseY = -Input.GetAxis("Mouse Y");

                // 이동 방향 계산
                Vector3 moveDirection = (forward * mouseY + right * mouseX).normalized;

                // 이동 적용
                parent.transform.Translate(moveDirection * moveSpeed * Time.unscaledDeltaTime, Space.World);
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                virtualCamera.m_Lens.OrthographicSize -= (zoomSpeed * Input.GetAxis("Mouse ScrollWheel"));
            }
            // orthographic size = 4 ~ 9
            if(virtualCamera.m_Lens.OrthographicSize >= 9)
            {
                virtualCamera.m_Lens.OrthographicSize = 9;
            }
            else if(virtualCamera.m_Lens.OrthographicSize <= 4)
            {
                virtualCamera.m_Lens.OrthographicSize = 4;
            }


            if (Camera.main.fieldOfView < 10)
                Camera.main.fieldOfView = 10;
            else if (Camera.main.fieldOfView > 100)
                Camera.main.fieldOfView = 100;
        }
    }
   
}
