using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LookAtCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
