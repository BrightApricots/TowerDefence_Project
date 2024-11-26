using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Enemy : MonoBehaviour
{
    private List<Node> path; // A* �˰�������� ���� ���
    private int pathIndex; // ���� ��ο��� �̵� ���� ����� �ε���
    private Transform targetBase; // ���� ��ǥ�� ��� ������ Transform
    private Pathfinding_AstarMove pathfinding; // A* �˰���� ��� Ž���� ����ϴ� Pathfinding ��ũ��Ʈ ����
    public float moveSpeed = 3.5f; // ���� �̵� �ӵ�
    public int Hp;
    public int Speed;
    public int Damage;
    private NavMeshAgent agent;
    private Transform targetBase;
    private LineRenderer lineRenderer;

    private LineRenderer lineRenderer; // ��θ� �ð�ȭ�� LineRenderer

    private void Awake()
    {
        // LineRenderer �ʱ�ȭ
        lineRenderer = GetComponent<LineRenderer>(); // ���� GameObject���� LineRenderer ��ũ��Ʈ�� ������

        // LineRenderer�� �� �β� ����
        lineRenderer.startWidth = 0.1f; // ���� ���� �κ� �β� ����
        lineRenderer.endWidth = 0.1f;   // ���� �� �κ� �β� ����

        // LineRenderer�� ��Ƽ���� ����
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // LineRenderer�� ����� �⺻ ��Ƽ���� �Ҵ�

        // LineRenderer�� ���� ����
        lineRenderer.startColor = Color.green; // ���� ���� �κ� ������ ������� ����
        lineRenderer.endColor = Color.red;     // ���� �� �κ� ������ ���������� ����
    }

    // Enemy�� �ʱ�ȭ �޼���. ��ǥ ������ Pathfinding ��ũ��Ʈ�� �����մϴ�.
    public void Initialize(Transform baseTransform, Pathfinding_AstarMove pathfindingComponent)
    {
        targetBase = baseTransform; // ��ǥ ���� ����
        pathfinding = pathfindingComponent; // Pathfinding ��ũ��Ʈ ����

        // Pathfinding�� TargetBase�� ���������� �ʱ�ȭ�Ǿ����� Ȯ��
        if (pathfinding != null && targetBase != null)
        {
            // A* �˰������ ����Ͽ� ��θ� ���
            path = pathfinding.FindPath(transform.position, targetBase.position);
            pathIndex = 0; // ����� ���� �ε����� 0���� ����

            // ��θ� LineRenderer�� �ð�ȭ
            DrawPath();
        }
        else
        {
            // �ʱ�ȭ ���� �� ���� �޽��� ���
            Debug.LogError("Pathfinding ��ũ��Ʈ �Ǵ� ��ǥ ������ Enemy���� �ʱ�ȭ���� �ʾҽ��ϴ�.");
        }
    }

    // �� �����Ӹ��� ȣ��Ǿ� ���� �̵���Ű�� �޼���
    private void Update()
    {
        // ��ΰ� �����ϰ�, ���� ��θ� ���󰡴� ������ Ȯ��
        if (path != null && pathIndex < path.Count)
        {
            // ���� �̵��ؾ� �� ����� ��ġ
            Vector3 targetPos = path[pathIndex].worldPosition;

            // MoveTowards�� ����Ͽ� ���� ��ġ���� ��ǥ ��ġ�� �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

            // ��ǥ ��ġ�� �����ϸ� ���� ���� �̵�
            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                pathIndex++; // ���� ���� �̵�
                //UpdatePathVisualization(); // �̵� ��� ������Ʈ
            }
        }
        else if (pathIndex >= path.Count) // ����� ������ ��忡 ������ ���
        {
            // ����� �޽��� ���
            Debug.Log("��ǥ ������ ����, Enemy ������Ʈ ����");
            Destroy(gameObject); // Enemy ������Ʈ ����
        }
    }

    // ��θ� LineRenderer�� �ð�ȭ
    private void DrawPath()
    {
        if (path != null && path.Count > 0)
        {
            lineRenderer.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
            {
                lineRenderer.SetPosition(i, path[i].worldPosition);
            }
        }
    }

    // �̵� ���� ��θ� �ǽð����� ������Ʈ
    private void UpdatePathVisualization()
    {
        if (lineRenderer.positionCount > pathIndex)
        {
            // �̹� ������ ��θ� �����ϰ� ó��
            lineRenderer.startColor = Color.clear;
            lineRenderer.endColor = Color.red;
        }
    }
}
