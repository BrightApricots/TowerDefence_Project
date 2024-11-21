using UnityEngine;
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
}
