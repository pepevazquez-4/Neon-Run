using UnityEngine;

public class SacudidaMenu : MonoBehaviour
{
    [Header("Configuración de la Sacudida")]
    [Tooltip("Qué tan rápido vibra el fondo")]
    public float velocidad = 15f;

    [Tooltip("Qué tan fuerte es la sacudida (valores pequeños son mejores para UI)")]
    public float intensidad = 5f;

    private Vector3 posicionOriginal;

    void Start()
    {
        // Guardamos la posición inicial en pantalla del fondo de la UI
        posicionOriginal = transform.localPosition;
    }

    void Update()
    {
        // Usamos un pequeño desfase aleatorio usando el tiempo del juego
        float offsetX = (Mathf.PerlinNoise(Time.time * velocidad, 0f) - 0.5f) * intensidad;
        float offsetY = (Mathf.PerlinNoise(0f, Time.time * velocidad) - 0.5f) * intensidad;

        // Aplicamos el movimiento manteniendo la posición original como base
        transform.localPosition = new Vector3(
            posicionOriginal.x + offsetX,
            posicionOriginal.y + offsetY,
            posicionOriginal.z
        );
    }
}