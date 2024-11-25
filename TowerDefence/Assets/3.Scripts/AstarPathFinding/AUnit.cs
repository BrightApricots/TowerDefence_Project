using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AUnit : MonoBehaviour
{
    public Transform target;
    private Vector3[] path;
    private Vector3[] previewPath;
    private LineRenderer actualPathRenderer;
    private LineRenderer previewPathRenderer;
    public Material lineMaterial;
    [SerializeField]
    private Color actualPathColor = Color.black;
    [SerializeField]
    private Color previewPathColor = new Color(1f, 0.5f, 0f, 0.5f);

    void Awake()
    {
        // 실제 경로용 LineRenderer는 별도의 게임오브젝트에 생성
        GameObject actualPathObj = new GameObject("ActualPathRenderer");
        actualPathObj.transform.SetParent(transform);
        actualPathRenderer = actualPathObj.AddComponent<LineRenderer>();
        SetupLineRenderer(actualPathRenderer, actualPathColor);

        // 프리뷰 경로용 LineRenderer도 별도의 게임오브젝트에 생성
        GameObject previewObj = new GameObject("PreviewPathRenderer");
        previewObj.transform.SetParent(transform);
        previewPathRenderer = previewObj.AddComponent<LineRenderer>();
        SetupLineRenderer(previewPathRenderer, previewPathColor);
        
        // 초기 상태 설정
        ShowPreviewPath(false);
        ShowActualPath(true);
    }

    private void SetupLineRenderer(LineRenderer renderer, Color color)
    {
        renderer.startWidth = 0.15f;
        renderer.endWidth = 0.15f;
        renderer.material = new Material(lineMaterial);
        renderer.startColor = color;
        renderer.endColor = color;
        renderer.positionCount = 0;
        renderer.enabled = true;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
            UpdatePath();
    }

    public void UpdatePath()
    {
        if (target == null) return;
        ShowPreviewPath(false);
        ShowActualPath(true);
        PathRequestManager.RequestPath(transform.position, target.position, 
            (path, success) => OnPathFound(path, success, false));
    }

    public void UpdatePreviewPath()
    {
        if (target == null) return;
        ShowPreviewPath(true);
        ShowActualPath(true);
        PathRequestManager.RequestPath(transform.position, target.position, 
            (path, success) => OnPathFound(path, success, true));
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful, bool isPreview = false)
    {
        if (pathSuccessful)
        {
            if (isPreview)
            {
                previewPath = newPath;
                DrawPath(previewPath, previewPathRenderer);
            }
            else
            {
                path = newPath;
                DrawPath(path, actualPathRenderer);
                previewPathRenderer.positionCount = 0;
            }

            if (PathManager.Instance != null)
            {
                PathManager.Instance.OnPathCalculated(newPath, true, isPreview);
            }
        }
        else
        {
            if (isPreview)
            {
                previewPath = null;
                previewPathRenderer.positionCount = 0;
            }
            else
            {
                path = null;
                actualPathRenderer.positionCount = 0;
            }

            if (PathManager.Instance != null)
            {
                PathManager.Instance.OnPathCalculated(null, false, isPreview);
            }
        }
    }

    void DrawPath(Vector3[] pathToDraw, LineRenderer renderer)
    {
        if (pathToDraw == null || pathToDraw.Length == 0) 
        {
            renderer.positionCount = 0;
            return;
        }

        Vector3[] points = new Vector3[pathToDraw.Length + 2];
        points[0] = transform.position;
        for (int i = 0; i < pathToDraw.Length; i++)
        {
            points[i + 1] = pathToDraw[i];
        }
        points[points.Length - 1] = target.position;

        float pathHeight = transform.position.y;
        for (int i = 0; i < points.Length; i++)
        {
            points[i].y = pathHeight;
        }

        renderer.positionCount = points.Length;
        renderer.SetPositions(points);
    }

    public void ShowPreviewPath(bool show)
    {
        previewPathRenderer.enabled = show;
        if (!show)
        {
            previewPathRenderer.positionCount = 0;
        }
    }

    public void ShowActualPath(bool show)
    {
        actualPathRenderer.enabled = show;
        if (!show)
        {
            actualPathRenderer.positionCount = 0;
        }
    }

    public void ClearAllPaths()
    {
        actualPathRenderer.positionCount = 0;
        previewPathRenderer.positionCount = 0;
        path = null;
        previewPath = null;
    }
}
