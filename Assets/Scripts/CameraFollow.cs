using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;

    private Camera cam;

    [Header("Configuración de Cámara")]
    public float sizeMinimo = 3f;
    public float sizeMaximo = 7f;
    public float margenEspacio = 1.5f;
    public float velocidadSuave = 5f;

    [Header("Screen Shake")]
    public float duracionShake = 0.2f;
    public float intensidadShake = 0.3f;

    private Vector3 offsetShake = Vector3.zero;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void LateUpdate()
    {
        GameObject[] jugadoresRaw = GameObject.FindGameObjectsWithTag("Player");
        if (jugadoresRaw.Length == 0) return;

        List<Transform> vivos = new List<Transform>();
        foreach (GameObject jugador in jugadoresRaw)
        {
            PlayerHealth salud = jugador.GetComponent<PlayerHealth>();
            if (salud == null || !salud.EstaMuerto)
            {
                vivos.Add(jugador.transform);
            }
        }

        if (vivos.Count == 0) return;

        Vector3 posicionDestino;
        float sizeDestino = sizeMinimo;

        if (vivos.Count == 1)
        {
            Transform unico = vivos[0];
            posicionDestino = new Vector3(unico.position.x, unico.position.y + 1f, -10f);
            sizeDestino = sizeMinimo;
        }
        else
        {
            Transform j1 = vivos[0];
            Transform j2 = vivos[1];

            float puntoMedioX = (j1.position.x + j2.position.x) / 2f;
            float puntoMedioY = (j1.position.y + j2.position.y) / 2f;
            posicionDestino = new Vector3(puntoMedioX, puntoMedioY + 1f, -10f);

            float distanciaX = Mathf.Abs(j1.position.x - j2.position.x);
            sizeDestino = (distanciaX / 2f) + margenEspacio;
            sizeDestino = Mathf.Clamp(sizeDestino, sizeMinimo, sizeMaximo);
        }

        transform.position = Vector3.Lerp(transform.position, posicionDestino + offsetShake, velocidadSuave * Time.deltaTime);
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, sizeDestino, velocidadSuave * Time.deltaTime);
    }

    public void Sacudir()
    {
        StopCoroutine("ShakeCoroutine");
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionShake)
        {
            float x = Random.Range(-1f, 1f) * intensidadShake;
            float y = Random.Range(-1f, 1f) * intensidadShake;
            offsetShake = new Vector3(x, y, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }
        offsetShake = Vector3.zero;
    }
}