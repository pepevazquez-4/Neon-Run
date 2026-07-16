using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransicionPantalla : MonoBehaviour
{
    public static TransicionPantalla Instance;

    public Image panelNegro;
    public float duracionApagado = 0.15f;

    void Awake()
    {
        Instance = this;
        SetAlpha(0f);
    }

    public void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(FlashCoroutine());
    }

    IEnumerator FlashCoroutine()
    {
        SetAlpha(1f);
        yield return new WaitForSeconds(duracionApagado);
        SetAlpha(0f);
    }

    void SetAlpha(float a)
    {
        if (panelNegro == null) return;
        Color c = panelNegro.color;
        c.a = a;
        panelNegro.color = c;
    }
}