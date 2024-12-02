using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    //일단은 shader graph material 쓰기 전까진 이걸로 처리함
    private LineRenderer lineRenderer;
    public int segments = 10;
    public float amplitude = 0.5f;
    public float duration = 0.2f;
    public Color lightningColor = new Color(0.8f, 0.8f, 1f, 1f);
    public float startWidth = 0.2f;
    public float endWidth = 0.1f;
    
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startColor = lightningColor;
        lineRenderer.endColor = lightningColor;
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
    }

    public void CreateLightning(Vector3 start, Vector3 end)
    {
        lineRenderer.positionCount = segments;//번개 가닥수
        Vector3[] positions = new Vector3[segments];
        
        for (int i = 0; i < segments; i++)
        {
            float progress = (float)i / (segments - 1);
            Vector3 position = Vector3.Lerp(start, end, progress);

            //대충 지그재그 효과주려고 하는거 amplitude가 커질수록 복잡하고 많아짐
            if (i != 0 && i != segments - 1)
            {
                position += Random.insideUnitSphere * amplitude;
            }
            
            positions[i] = position;
        }
        
        lineRenderer.SetPositions(positions);
        Destroy(gameObject, duration);
    }

    private void Update()
    {
        if (lineRenderer != null)
        {
            Vector3[] positions = new Vector3[lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);
            
            for (int i = 1; i < positions.Length - 1; i++)
            {
                positions[i] += Random.insideUnitSphere * (amplitude * 0.2f);
            }
            
            lineRenderer.SetPositions(positions);
        }
    }
} 