using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Barras de UI")]
    public Slider sliderJugador1;
    public Slider sliderJugador2;

    private bool barrasEnlazadas = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Si aún falta algún jugador por enlazar, sigue buscando en vivo
        if (!barrasEnlazadas)
        {
            SincronizarBarras();
        }
    }

    public void SincronizarBarras()
    {
        // Busca en la arena los cubos clonados con el Tag "Player"
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");

        // Enlazar el primer cubo a la barra 1
        if (jugadores.Length >= 1)
        {
            PlayerHealth health1 = jugadores[0].GetComponent<PlayerHealth>();
            if (health1 != null) health1.AsignarBarra(sliderJugador1);
        }

        // Enlazar el segundo cubo a la barra 2 y detener la búsqueda
        if (jugadores.Length >= 2)
        {
            PlayerHealth health2 = jugadores[1].GetComponent<PlayerHealth>();
            if (health2 != null) health2.AsignarBarra(sliderJugador2);

            barrasEnlazadas = true; // Enlace completado perfectamente
        }
    }
}