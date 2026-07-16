using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class IndicadorAvance : MonoBehaviour
{
    public Image[] cuadros;
    public Color colorApagado = Color.gray;
    public Color colorEncendidoP1 = Color.green;
    public Color colorEncendidoP2 = Color.red;

    [Header("Referencia de mapa")]
    public float distanciaTotalMapa = 74f;

    void Update()
    {
        if (GameManager.Instance == null) return;

        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("Player");
        if (jugadores.Length == 0) return;

        EstadoAvance estado = GameManager.Instance.estadoActual;
        if (estado == EstadoAvance.Neutro)
        {
            ApagarTodos();
            return;
        }

        Transform jugadorConPrioridad = null;
        foreach (GameObject j in jugadores)
        {
            PhotonView pv = j.GetComponent<PhotonView>();
            if (pv == null || pv.Owner == null) continue;

            bool esP1 = pv.Owner.IsMasterClient;
            if ((estado == EstadoAvance.AvanzaP1 && esP1) || (estado == EstadoAvance.AvanzaP2 && !esP1))
            {
                PlayerHealth salud = j.GetComponent<PlayerHealth>();
                if (salud != null && !salud.EstaMuerto)
                {
                    jugadorConPrioridad = j.transform;
                }
            }
        }

        if (jugadorConPrioridad == null)
        {
            ApagarTodos();
            return;
        }

        float progreso = Mathf.Abs(jugadorConPrioridad.position.x) / (distanciaTotalMapa / 2f);
        progreso = Mathf.Clamp01(progreso);

        int cuadrosEncendidos = Mathf.RoundToInt(progreso * cuadros.Length);
        Color colorActivo = (estado == EstadoAvance.AvanzaP1) ? colorEncendidoP1 : colorEncendidoP2;

        for (int i = 0; i < cuadros.Length; i++)
        {
            cuadros[i].color = (i < cuadrosEncendidos) ? colorActivo : colorApagado;
        }
    }

    void ApagarTodos()
    {
        foreach (Image cuadro in cuadros)
        {
            cuadro.color = colorApagado;
        }
    }
}