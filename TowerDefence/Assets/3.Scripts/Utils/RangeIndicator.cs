using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeIndicator : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public void Initialize(float range)
    {
        lineRenderer = GetComponent<LineRenderer>();
        DrawCircle(100, range);
    }

    private void DrawCircle(int steps, float radius)
    {
        lineRenderer.positionCount = steps + 1;

        for (int currentStep = 0; currentStep <= steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep / steps;
            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            Vector3 currentPosition = new Vector3(
                Mathf.Cos(currentRadian) * radius,
                0.1f,  // 바닥보다 약간 위에
                Mathf.Sin(currentRadian) * radius
            );

            lineRenderer.SetPosition(currentStep, currentPosition);
        }
    }
}
