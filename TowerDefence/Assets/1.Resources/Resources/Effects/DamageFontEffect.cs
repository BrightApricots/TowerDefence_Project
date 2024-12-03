using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageFontEffect : MonoBehaviour
{
    public float moveSpeed = .1f;
    public float fadeSpeed = .1f;
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        transform.rotation = Camera.main.transform.rotation; // 카메라를 향하도록
        Destroy(gameObject, 1f);
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        // 위로 이동
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

        // 항상 카메라를 향하도록
        transform.rotation = Camera.main.transform.rotation;
    }

    IEnumerator FadeOut()
    {
        float alpha = 1f;
        while (alpha > 0)
        {
            alpha -= fadeSpeed * Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
            yield return null;
        }
    }
    //public float moveSpeed = 100f;
    //public float fadeSpeed = 1f;
    //private TextMeshProUGUI text;

    //void Start()
    //{
    //    text = GetComponent<TextMeshProUGUI>();
    //    Destroy(gameObject, 1f); // 1초 후 제거
    //    StartCoroutine(FadeOut());
    //}

    //void Update()
    //{
    //    // 위로 올라가는 효과
    //    transform.position += Vector3.up * moveSpeed * Time.deltaTime;
    //}

    //IEnumerator FadeOut()
    //{
    //    float alpha = 1f;
    //    while (alpha > 0)
    //    {
    //        alpha -= fadeSpeed * Time.deltaTime;
    //        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
    //        yield return null;
    //    }
    //}
}