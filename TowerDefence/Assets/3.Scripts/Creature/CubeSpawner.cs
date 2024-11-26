using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // ������ ť�� ������
    public Camera mainCamera;     // Ŭ�� ������ ���� ī�޶�
    public float fixedY = 0.6f;   // ������ ť���� y�� (Inspector���� ���� ����)

    private GameObject previewCube; // ������ ť�� ������Ʈ
    private CustomGrid customGrid; // CustomGrid ��ũ��Ʈ ����
    private Pathfinding_AstarMove pathfinding; // Pathfinding ��ũ��Ʈ ����
    private Quaternion currentRotation = Quaternion.identity; // ���� ȸ�� ����

    void Start()
    {
        // CustomGrid�� Pathfinding ��ũ��Ʈ ã��
        customGrid = FindObjectOfType<CustomGrid>();
        pathfinding = FindObjectOfType<Pathfinding_AstarMove>();

        if (customGrid == null)
        {
            Debug.LogError("���� CustomGrid ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }

        if (pathfinding == null)
        {
            Debug.LogError("���� Pathfinding_AstarMove ��ũ��Ʈ�� ã�� �� �����ϴ�.");
        }

        // ������ ť�� ����
        previewCube = Instantiate(cubePrefab);
        previewCube.SetActive(false); // �ʱ� ��Ȱ��ȭ
    }

    void Update()
    {
        UpdatePreviewCube();

        if (Input.GetMouseButtonDown(0)) // ���� ���콺 Ŭ�� ����
        {
            TryPlaceCube();
        }

        if (Input.GetMouseButtonDown(1)) // ��Ŭ������ �¿� ȸ��
        {
            RotatePreviewCube();
        }
    }

    // ������ ť�� ������Ʈ
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
                    previewCube.SetActive(true); // ������ ť�� Ȱ��ȭ
                    Vector3 previewPosition = node.worldPosition;
                    previewPosition.y = fixedY; // y�� ����
                    previewCube.transform.position = previewPosition;
                    previewCube.transform.rotation = currentRotation;

                    // ��ġ ���� ���ο� ���� ���� ����
                    Renderer renderer = previewCube.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.material.color = node.walkable ? Color.blue : Color.red;
                    }
                }
                else
                {
                    previewCube.SetActive(false); // ��ȿ���� ���� ����� ��� ��Ȱ��ȭ
                }
            }
        }
        else
        {
            previewCube.SetActive(false); // ���̰� �´� ���� ������ ��Ȱ��ȭ
        }
    }

    // ť�� ��ġ �õ�
    void TryPlaceCube()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (customGrid != null)
            {
                Node node = customGrid.NodeFromWorldPoint(hit.point);
                if (node != null && node.walkable) // ��ġ ���� Ȯ��
                {
                    Vector3 spawnPosition = node.worldPosition;
                    spawnPosition.y = fixedY; // y�� ����

                    GameObject newCube = Instantiate(cubePrefab, spawnPosition, currentRotation);

                    // CustomGrid ������Ʈ: ���ο� ��ֹ� ��ġ �ݿ�
                    customGrid.UpdateGrid(newCube.transform.position);

                    // Pathfinding ��� ��Ž��
                    if (pathfinding != null)
                    {
                        pathfinding.UpdatePath(Vector3.zero, new Vector3(10, 0, 10)); // ���� ���
                        Debug.Log("��� ��Ž�� �Ϸ�");
                    }
                }
                else
                {
                    Debug.LogWarning("Ŭ���� ��ġ�� ��ġ�� �Ұ����� �����Դϴ�.");
                }
            }
        }
    }

    // ������ ť�� �¿� ȸ��
    void RotatePreviewCube()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0); // 90�� ȸ��
    }
}
