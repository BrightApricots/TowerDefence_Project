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

        // 이동 속도 설정
        agent.speed = moveSpeed;

        SetupLineRenderer();
    }

    private void Update()
    {
        if (targetBase != null)
        {
            // 기지로 이동
            agent.SetDestination(targetBase.position);
            UpdateLineRenderer();

            // 목표 지점에 도달하면 삭제
            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                Debug.Log("Enemy 삭제");
                Destroy(gameObject);
            }
        }
    }

    private void SetupLineRenderer()
    {
        // LineRenderer 기본 설정
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.textureMode = LineTextureMode.Tile;

        // 점선 효과를 위한 텍스처 설정
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

        // 현재 위치와 다음 목표 위치만 LineRenderer에 설정
        Vector3[] pathSegment = new Vector3[2];
        pathSegment[0] = transform.position; // 유닛 현재 위치
        pathSegment[1] = agent.path.corners.Length > 1 ? agent.path.corners[1] : agent.path.corners[0]; // 다음 코너

        lineRenderer.positionCount = pathSegment.Length;
        lineRenderer.SetPositions(pathSegment);
    }

    private Texture2D GenerateDashedTexture()
    {
        // 점선 텍스처 생성
        int width = 8;
        int height = 1;
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Repeat;

        for (int x = 0; x < width; x++)
        {
            Color color = (x % 2 == 0) ? Color.white : Color.clear; // 흰색과 투명 교차
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

        // 현재 경로와 동일한 스타일로 Scene 뷰에 시각화
        Gizmos.color = Color.yellow;

        Vector3 currentPos = transform.position; //생성된 유닛의 현재 위치
        Vector3 nextCorner = agent.path.corners.Length > 1 ? agent.path.corners[1] : agent.path.corners[0]; //유닛의 다음 경유지 

        // 유닛 위치에서 다음 경로 코너까지 선 그리기
        Gizmos.DrawLine(currentPos, nextCorner);
    }
}
