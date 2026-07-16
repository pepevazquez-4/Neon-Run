using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    public int maxVida = 100;
    private int vidaActual;
    private PhotonView view;
    private bool estaMuerto = false;
    public bool EstaMuerto => estaMuerto;

    [Header("Respawn")]
    public float distanciaRespawn = 15f;
    public float tiempoInvulnerabilidad = 1f;
    public float duracionAnimMuerte = 0.8f;

    [Header("Límites del Mapa (Clamp de Respawn)")]
    public float limiteMapaIzquierdo = -35f;
    public float limiteMapaDerecho = 35f;

    [Header("Caída al Vacío")]
    public float limiteCaidaY = -10f;

    [Header("Feedback de Impacto")]
    public Color colorFlash = Color.red;
    public float duracionFlash = 0.1f;
    public AudioClip sonidoGolpe;

    [Header("UI")]
    private UnityEngine.UI.Slider sliderVida;

    private bool esInvulnerable = false;
    private SpriteRenderer sr;
    private Color colorOriginal;
    private AudioSource audioSource;
    private Animator anim;
    private Rigidbody2D rb;
    private BoxCollider2D col;
    private PlayerController controlador;

    void Start()
    {
        view = GetComponent<PhotonView>();
        vidaActual = maxVida;
        sr = GetComponent<SpriteRenderer>();
        colorOriginal = sr.color;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<BoxCollider2D>();
        controlador = GetComponent<PlayerController>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!view.IsMine) return;
        if (estaMuerto) return;

        // Detección de caída al vacío
        if (transform.position.y < limiteCaidaY)
        {
            view.RPC("MorirPorCaidaRPC", RpcTarget.All);
        }
    }

    [PunRPC]
    void MorirPorCaidaRPC()
    {
        if (estaMuerto) return;
        Debug.Log(gameObject.name + " cayó al vacío.");
        Morir();
    }

    [PunRPC]
    public void RecibirDanioRPC(int cantidad)
    {
        if (estaMuerto || esInvulnerable) return;

        vidaActual = 0;
        if (sliderVida != null) sliderVida.value = 0;

        StartCoroutine(FlashImpacto());
        if (CameraFollow.Instance != null) CameraFollow.Instance.Sacudir();
        if (sonidoGolpe != null && audioSource != null) audioSource.PlayOneShot(sonidoGolpe);

        Morir();
    }

    IEnumerator FlashImpacto()
    {
        sr.color = colorFlash;
        yield return new WaitForSeconds(duracionFlash);
        sr.color = colorOriginal;
    }

    void Morir()
    {
        estaMuerto = true;

        if (controlador != null) controlador.bloquearMovimiento = true;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }

        if (anim != null) anim.SetTrigger("Morir");

        if (PhotonNetwork.IsMasterClient)
        {
            int idGanador = (PhotonNetwork.LocalPlayer.ActorNumber == view.Owner.ActorNumber) ? 2 : 1;
            GameManager.Instance.GetComponent<PhotonView>().RPC("ActualizarPrioridadRPC", RpcTarget.All, idGanador);
        }

        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(duracionAnimMuerte);

        sr.enabled = false;
        col.enabled = false;

        yield return new WaitForSeconds(2f - duracionAnimMuerte);

        float spawnX;
        if (GameManager.Instance.estadoActual == EstadoAvance.AvanzaP1)
        {
            spawnX = transform.position.x + distanciaRespawn;
        }
        else if (GameManager.Instance.estadoActual == EstadoAvance.AvanzaP2)
        {
            spawnX = transform.position.x - distanciaRespawn;
        }
        else
        {
            spawnX = 0f;
        }

        // Aseguramos que nunca aparezca fuera de los límites reales del mapa
        spawnX = Mathf.Clamp(spawnX, limiteMapaIzquierdo, limiteMapaDerecho);

        transform.position = new Vector3(spawnX, 1f, 0f);
        vidaActual = maxVida;
        estaMuerto = false;

        if (sliderVida != null) sliderVida.value = vidaActual;

        sr.color = colorOriginal;
        sr.enabled = true;
        col.enabled = true;
        rb.isKinematic = false;

        if (controlador != null) controlador.bloquearMovimiento = false;

        StartCoroutine(InvulnerabilidadTemporal());
    }

    IEnumerator InvulnerabilidadTemporal()
    {
        esInvulnerable = true;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < tiempoInvulnerabilidad)
        {
            sr.enabled = !sr.enabled;
            yield return new WaitForSeconds(0.1f);
            tiempoTranscurrido += 0.1f;
        }
        sr.enabled = true;

        esInvulnerable = false;
    }

    public void AsignarBarra(UnityEngine.UI.Slider slider)
    {
        sliderVida = slider;
        sliderVida.maxValue = maxVida;
        sliderVida.value = vidaActual;
    }
}