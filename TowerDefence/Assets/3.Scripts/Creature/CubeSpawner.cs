using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    public GameObject cubePrefab; // ������ ť�� ������
    public Camera mainCamera;     // Ŭ�� ������ ���� ī�޶�

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // ���� ���콺 Ŭ�� ����
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); // ȭ�� ��ǥ -> ���� ����
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit)) // ���̰� �浹�� ��ġ
            {
                // �浹 ������ ť�� ����
                GameObject newCube = Instantiate(cubePrefab, hit.point, Quaternion.identity);

                // ������ ť���� y ���� 0.6���� ����
                Vector3 fixedPosition = newCube.transform.position;
                fixedPosition.y = 0.6f; // y�� ����
                newCube.transform.position = fixedPosition;
            }
        }
    }
}

/*
 * using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;  // NavMeshAgent ������Ʈ
    private Transform targetBase; // �÷��̾� ������ Transform

    [Header("Enemy Settings")]
    public float moveSpeed = 3.5f; // �� �̵� �ӵ�

    public void Initialize(Transform baseTransform)
    {
        // �÷��̾� ������ Transform�� ����
        targetBase = baseTransform;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // �̵� �ӵ� ����
        agent.speed = moveSpeed;
    }

    private void Update()
    {
        if (targetBase != null)
        {
            // ������ �̵�
            agent.SetDestination(targetBase.position);
        }
    }

    // Gizmos�� ����Ͽ� ��θ� �ð������� ǥ��
    private void OnDrawGizmos()
    {
        if (agent == null || agent.path == null)
            return;

        // ��θ� ��� ������ ǥ��
        Gizmos.color = Color.green;
        Vector3 previousCorner = transform.position; // ���� ����

        foreach (var corner in agent.path.corners)
        {
            Gizmos.DrawLine(previousCorner, corner); // ���� �������� ���� �������� �� �׸���
            previousCorner = corner;
        }
    }
}  ���⼭ �׸��� �߽����� �̵��ϴ� ���Ž���� ��������
 */