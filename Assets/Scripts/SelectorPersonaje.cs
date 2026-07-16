using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using ExitGames.Client.Photon;

public class SelectorPersonaje : MonoBehaviourPunCallbacks
{
    public static readonly string[] NOMBRES_PERSONAJES = { "PUNK", "BIKER", "CYBORG" };
    public static readonly string[] PREFABS_PERSONAJES = { "Jugador_Network", "Jugador_Network_Biker", "Jugador_Network_Cyborg" };
    private const string CLAVE_PERSONAJE = "personaje";

    [Header("UI")]
    public TextMeshProUGUI textoNickname;
    public TextMeshProUGUI textoNombrePersonaje;
    public Button botonIzquierda;
    public Button botonDerecha;

    [Header("Preview en mundo")]
    public Animator previewAnimator;
    public RuntimeAnimatorController[] controllersPersonajes;

    private int indiceActual = 0;
    private bool botonesConectados = false;

    void Start()
    {
        InicializarSelector();
    }

    void OnBecameVisible()
    {
        InicializarSelector();
    }

    // Este método público lo puede llamar SalaEsperaManager cuando active el panel, para refrescar todo
    public void InicializarSelector()
    {
        if (!botonesConectados)
        {
            botonIzquierda.onClick.AddListener(() => CambiarSeleccion(-1));
            botonDerecha.onClick.AddListener(() => CambiarSeleccion(1));
            botonesConectados = true;
        }

        if (textoNickname != null) textoNickname.text = PhotonNetwork.NickName;

        ActualizarPreview();
        GuardarSeleccion();
    }

    void CambiarSeleccion(int direccion)
    {
        indiceActual += direccion;
        if (indiceActual < 0) indiceActual = NOMBRES_PERSONAJES.Length - 1;
        if (indiceActual >= NOMBRES_PERSONAJES.Length) indiceActual = 0;

        ActualizarPreview();
        GuardarSeleccion();
    }

    void ActualizarPreview()
    {
        if (textoNombrePersonaje != null)
        {
            textoNombrePersonaje.text = NOMBRES_PERSONAJES[indiceActual];
        }

        if (previewAnimator != null && controllersPersonajes != null && controllersPersonajes.Length > indiceActual)
        {
            previewAnimator.runtimeAnimatorController = controllersPersonajes[indiceActual];
        }
    }

    void GuardarSeleccion()
    {
        Hashtable propiedades = new Hashtable { { CLAVE_PERSONAJE, indiceActual } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(propiedades);
    }

    public static string ObtenerPrefabElegido()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("personaje", out object valor))
        {
            int indice = (int)valor;
            return PREFABS_PERSONAJES[indice];
        }
        return PREFABS_PERSONAJES[0];
    }
}