using System.Collections.Generic;
using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public List<GameObject> cubePrefabs; // ������ ť�� ������ ����Ʈ
    public Camera mainCamera;           // Ŭ�� ������ ���� ī�޶�
    public float fixedY = 0.6f;         // ������ ť���� y�� (Inspector���� ���� ����)

    private GameObject previewCube;     // ������ ť�� ������Ʈ
    private CustomGrid customGrid;      // CustomGrid ��ũ��Ʈ ����
    private Pathfinding_AstarMove pathfinding; // Pathfinding ��ũ��Ʈ ����
    private Quaternion currentRotation = Quaternion.identity; // ���� ȸ�� ����
    private Node currentPreviewNode;    // ���� �����䰡 ������ ���
    private int selectedCubeIndex = 0;  // ���õ� ť�� ������ �ε���

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
        if (cubePrefabs != null && cubePrefabs.Count > 0)
        {
            previewCube = Instantiate(cubePrefabs[selectedCubeIndex]); // ù ��° ť�긦 ������� ����
            previewCube.SetActive(false); // �ʱ� ��Ȱ��ȭ
        }
        else
        {
            Debug.LogError("ť�� ������ ����Ʈ�� ��� �ֽ��ϴ�. �������� �߰��ϼ���.");
        }
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

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // ���콺 �� ���� ��ũ��
        {
            CycleCubePrefabs(1);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) // ���콺 �� �Ʒ��� ��ũ��
        {
            CycleCubePrefabs(-1);
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

                // ���� ���� ���� ��尡 ���ٸ� ������Ʈ���� ����
                if (node != null && node != currentPreviewNode)
                {
                    currentPreviewNode = node; // ���� ��� ����
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
        if (currentPreviewNode != null && currentPreviewNode.walkable) // ���� ����� ��ġ ���� ���� Ȯ��
        {
            Vector3 spawnPosition = currentPreviewNode.worldPosition;
            spawnPosition.y = fixedY; // y�� ����

            GameObject newCube = Instantiate(cubePrefabs[selectedCubeIndex], spawnPosition, currentRotation);

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
            Debug.LogWarning("���� ���� ��ġ�� �Ұ����� �����Դϴ�.");
        }
    }

    // ������ ť�� �¿� ȸ��
    void RotatePreviewCube()
    {
        currentRotation *= Quaternion.Euler(0, 90, 0); // 90�� ȸ��
    }

    // ť�� ������ ��ȯ ����
    void CycleCubePrefabs(int direction)
    {
        if (cubePrefabs == null || cubePrefabs.Count == 0)
        {
            Debug.LogWarning("ť�� ������ ����Ʈ�� ��� �ֽ��ϴ�.");
            return;
        }

        selectedCubeIndex = (selectedCubeIndex + direction + cubePrefabs.Count) % cubePrefabs.Count; // ��ȯ ����
        Destroy(previewCube); // ���� ������ ����
        previewCube = Instantiate(cubePrefabs[selectedCubeIndex]); // �� ������ ����
        previewCube.SetActive(false); // �ʱ� ��Ȱ��ȭ
        Debug.Log($"���� ���õ� ť�� ������: {selectedCubeIndex + 1}/{cubePrefabs.Count}");
    }
}
