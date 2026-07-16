using UnityEngine;
using Photon.Pun;

public enum EstadoAvance { Neutro, AvanzaP1, AvanzaP2 }

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    [Header("Control de Pantallas (Nidhogg)")]
    public int pantallaActual = 0;
    public EstadoAvance estadoActual = EstadoAvance.Neutro;

    private bool partidaTerminada = false;
    public bool PartidaTerminada => partidaTerminada;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Application.runInBackground = true;
    }

    public void ReiniciarEstado()
    {
        partidaTerminada = false;
        estadoActual = EstadoAvance.Neutro;
        pantallaActual = 0;
    }

    public void CambiarDePantalla(int direccion)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_ActualizarPantalla", RpcTarget.All, direccion);
        }
    }

    [PunRPC]
    private void RPC_ActualizarPantalla(int direccion)
    {
        pantallaActual += direccion;
    }

    [PunRPC]
    public void ActualizarPrioridadRPC(int idGanador)
    {
        if (idGanador == 1) estadoActual = EstadoAvance.AvanzaP1;
        else if (idGanador == 2) estadoActual = EstadoAvance.AvanzaP2;
    }

    public void DeclararVictoria(int idGanador)
    {
        if (partidaTerminada) return;
        partidaTerminada = true;

        photonView.RPC("RPC_MostrarVictoria", RpcTarget.All, idGanador);
    }

    [PunRPC]
    private void RPC_MostrarVictoria(int idGanador)
    {
        PlayerController[] jugadores = FindObjectsOfType<PlayerController>();
        foreach (PlayerController jugador in jugadores)
        {
            jugador.bloquearMovimiento = true;
        }

        if (UIManagerPartida.Instance != null)
        {
            UIManagerPartida.Instance.MostrarPanelVictoria(idGanador);
        }
    }
}