using UnityEngine;
using Photon.Pun;

public class ZonaMeta : MonoBehaviour
{
    [Tooltip("Marca esta casilla SOLO en el trigger del lado derecho del mapa")]
    public bool esMetaDerecha;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PhotonView viewJugador = other.GetComponentInParent<PhotonView>();
        if (viewJugador == null || !viewJugador.IsMine) return;

        int idLocal = PhotonNetwork.IsMasterClient ? 1 : 2;

        bool esMiTurnoDeGanar =
            (esMetaDerecha && GameManager.Instance.estadoActual == EstadoAvance.AvanzaP1 && idLocal == 1) ||
            (!esMetaDerecha && GameManager.Instance.estadoActual == EstadoAvance.AvanzaP2 && idLocal == 2);

        if (esMiTurnoDeGanar)
        {
            GameManager.Instance.DeclararVictoria(idLocal);
        }
    }
}