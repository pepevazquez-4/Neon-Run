using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class SalaEsperaManager : MonoBehaviourPunCallbacks
{
    [Header("Componentes UI")]
    public TextMeshProUGUI textoJugadores;
    public Button botonIniciarPartida;
    public TextMeshProUGUI textoCuentaRegresiva;

    private const string CLAVE_INICIANDO = "iniciando";

    public void InicializarSalaEspera()
    {
        textoCuentaRegresiva.text = "";
        ActualizarUI();
    }

    public override void OnPlayerEnteredRoom(Player nuevoJugador) => ActualizarUI();
    public override void OnPlayerLeftRoom(Player jugadorQueSalio) => ActualizarUI();
    public override void OnMasterClientSwitched(Player nuevoMaster) => ActualizarUI();

    void ActualizarUI()
    {
        int actuales = PhotonNetwork.CurrentRoom.PlayerCount;
        int maximos = PhotonNetwork.CurrentRoom.MaxPlayers;
        textoJugadores.text = $"Jugadores: {actuales}/{maximos}";

        botonIniciarPartida.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        botonIniciarPartida.interactable = PhotonNetwork.IsMasterClient && actuales == maximos;
    }

    public void ClickIniciarPartida()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        ExitGames.Client.Photon.Hashtable propiedades = new ExitGames.Client.Photon.Hashtable { { CLAVE_INICIANDO, true } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(propiedades);
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propiedadesCambiadas)
    {
        if (propiedadesCambiadas.ContainsKey(CLAVE_INICIANDO) && (bool)propiedadesCambiadas[CLAVE_INICIANDO])
        {
            StartCoroutine(CuentaRegresivaYCargarEscena());
        }
    }

    IEnumerator CuentaRegresivaYCargarEscena()
    {
        botonIniciarPartida.interactable = false;

        int segundos = 3;
        while (segundos > 0)
        {
            textoCuentaRegresiva.text = segundos.ToString();
            yield return new WaitForSeconds(1f);
            segundos--;
        }

        textoCuentaRegresiva.text = "¡Ya!";

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Escena_Juego");
        }
    }
}