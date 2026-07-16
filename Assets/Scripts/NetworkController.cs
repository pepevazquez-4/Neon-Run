using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    [Header("Paneles")]
    public GameObject panelInicio;
    public GameObject panelListaSalas;
    public GameObject panelSalaEspera;

    [Header("Componentes Lista de Salas")]
    public TMP_InputField inputNombreSala;
    public Button botonCrear;
    public TextMeshProUGUI textoEstado;
    public Transform contenedorListaSalas;
    public GameObject prefabItemSala;

    [Header("Nickname")]
    public TMP_InputField inputNickname;

    [Header("Preview de Personaje")]
    public GameObject personajePreview;

    [Header("Referencia")]
    public SalaEsperaManager salaEsperaManager;

    private Dictionary<string, GameObject> itemsDeSalasVisibles = new Dictionary<string, GameObject>();

    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        MostrarPanel(panelInicio);
        textoEstado.text = "Conectando al servidor maestro de Photon...";

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ClickJugar()
    {
        string nickname = inputNickname.text;
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Jugador" + Random.Range(1000, 9999);
        }
        PhotonNetwork.NickName = nickname;

        MostrarPanel(panelListaSalas);
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=green>[NETWORK]</color> Conectado al servidor maestro.");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("<color=green>[NETWORK]</color> Dentro del Lobby.");
        textoEstado.text = "Conectado. Crea una sala o únete a una existente.";
        botonCrear.interactable = true;
    }

    public override void OnRoomListUpdate(List<RoomInfo> listaSalas)
    {
        foreach (RoomInfo sala in listaSalas)
        {
            if (sala.RemovedFromList || !sala.IsOpen || !sala.IsVisible)
            {
                if (itemsDeSalasVisibles.ContainsKey(sala.Name))
                {
                    Destroy(itemsDeSalasVisibles[sala.Name]);
                    itemsDeSalasVisibles.Remove(sala.Name);
                }
                continue;
            }

            if (itemsDeSalasVisibles.ContainsKey(sala.Name)) continue;

            GameObject nuevoItem = Instantiate(prefabItemSala, contenedorListaSalas);
            nuevoItem.GetComponent<RoomListItem>().Configurar(sala.Name, this);
            itemsDeSalasVisibles.Add(sala.Name, nuevoItem);
        }
    }

    public void ClickCrearSala()
    {
        string nombreSala = inputNombreSala.text;
        if (string.IsNullOrEmpty(nombreSala))
        {
            textoEstado.text = "<color=red>¡Escribe un nombre para la sala!</color>";
            return;
        }

        RoomOptions opciones = new RoomOptions()
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true
        };

        PhotonNetwork.CreateRoom(nombreSala, opciones);
    }

    public void UnirseASalaEspecifica(string nombreSala)
    {
        PhotonNetwork.JoinRoom(nombreSala);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("<color=green>[NETWORK]</color> Dentro de la sala: " + PhotonNetwork.CurrentRoom.Name);
        MostrarPanel(panelSalaEspera);
        salaEsperaManager.InicializarSalaEspera();

        SelectorPersonaje selector = panelSalaEspera.GetComponent<SelectorPersonaje>();
        if (selector != null) selector.InicializarSelector();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        textoEstado.text = "<color=red>Fallo al unirse: " + message + "</color>";
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        textoEstado.text = "<color=red>No se pudo crear la sala: " + message + "</color>";
    }

    void MostrarPanel(GameObject panelActivo)
    {
        panelInicio.SetActive(panelActivo == panelInicio);
        panelListaSalas.SetActive(panelActivo == panelListaSalas);
        panelSalaEspera.SetActive(panelActivo == panelSalaEspera);

        if (personajePreview != null)
        {
            personajePreview.SetActive(panelActivo == panelSalaEspera);
        }
    }
}