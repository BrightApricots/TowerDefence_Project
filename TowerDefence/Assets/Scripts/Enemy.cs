using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class Enemy : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform targetBase;
    private LineRenderer lineRenderer;

    [Header("Enemy Settings")]
    public float moveSpeed = 3.5f;

    public void Initialize(Transform baseTransform)
    {
        targetBase = baseTransform;
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = GetComponent<LineRenderer>();

        // �̵� �ӵ� ����
        agent.speed = moveSpeed;

        SetupLineRenderer();
    }

    private void Update()
    {
        if (targetBase != null)
        {
            // ������ �̵�
            agent.SetDestination(targetBase.position);
            UpdateLineRenderer();

            // ��ǥ ������ �����ϸ� ����
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Debug.Log("Enemy ����");
                Destroy(gameObject);
            }
        }
    }

    private void SetupLineRenderer()
    {
        // LineRenderer �⺻ ����
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.textureMode = LineTextureMode.Tile;

        // ���� ȿ���� ���� �ؽ�ó ����
        lineRenderer.material.mainTexture = GenerateDashedTexture();
        lineRenderer.material.mainTextureScale = new Vector2(1f, 1f);
    }

    private void UpdateLineRenderer()
    {
        if (agent.path == null || agent.path.corners.Length == 0)
        {
            lineRenderer.positionCount = 0;
            return;
        }

        // ���� ��ġ�� ���� ��ǥ ��ġ�� LineRenderer�� ����
        Vector3[] pathSegment = new Vector3[2];
        pathSegment[0] = transform.position; // ���� ���� ��ġ
        pathSegment[1] = agent.path.corners.Length > 1 ? agent.path.corners[1] : agent.path.corners[0]; // ���� �ڳ�

        lineRenderer.positionCount = pathSegment.Length;
        lineRenderer.SetPositions(pathSegment);
    }

    private Texture2D GenerateDashedTexture()
    {
        // ���� �ؽ�ó ����
        int width = 8;
        int height = 1;
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Repeat;

        for (int x = 0; x < width; x++)
        {
            Color color = (x % 2 == 0) ? Color.white : Color.clear; // ����� ���� ����
            for (int y = 0; y < height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private void OnDrawGizmos()
    {
        if (agent == null || agent.path == null || agent.path.corners.Length == 0)
            return;

        // ���� ��ο� ������ ��Ÿ�Ϸ� Scene �信 �ð�ȭ
        Gizmos.color = Color.yellow;

        Vector3 currentPos = transform.position; //������ ������ ���� ��ġ
        Vector3 nextCorner = agent.path.corners.Length > 1 ? agent.path.corners[1] : agent.path.corners[0]; //������ ���� ������ 

        // ���� ��ġ���� ���� ��� �ڳʱ��� �� �׸���
        Gizmos.DrawLine(currentPos, nextCorner);
    }
}
