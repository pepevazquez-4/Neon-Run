using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using System.Collections;

public class UIManagerPartida : MonoBehaviourPunCallbacks
{
    public static UIManagerPartida Instance;

    [Header("Panel de Resultado")]
    public GameObject panelVictoria;
    public TextMeshProUGUI textoResultado;
    public Button botonSalir;

    [Header("Efecto de Shake en Texto")]
    public float duracionShakeTexto = 0.4f;
    public float intensidadShakeTexto = 8f;

    void Awake()
    {
        Instance = this;
        panelVictoria.SetActive(false);
    }

    void Start()
    {
        botonSalir.onClick.AddListener(ClickSalir);
    }

    public void MostrarPanelVictoria(int idGanador)
    {
        int idLocal = PhotonNetwork.IsMasterClient ? 1 : 2;

        panelVictoria.SetActive(true);
        textoResultado.text = (idGanador == idLocal) ? "¡VENCEDOR!" : "DERROTA";

        StartCoroutine(ShakeTexto());
    }

    IEnumerator ShakeTexto()
    {
        RectTransform rt = textoResultado.rectTransform;
        Vector3 posOriginal = rt.anchoredPosition;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionShakeTexto)
        {
            float x = Random.Range(-1f, 1f) * intensidadShakeTexto;
            float y = Random.Range(-1f, 1f) * intensidadShakeTexto;
            rt.anchoredPosition = posOriginal + new Vector3(x, y, 0f);

            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        rt.anchoredPosition = posOriginal;
    }

    void ClickSalir()
    {
        botonSalir.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MenuPrincipal");
    }
}