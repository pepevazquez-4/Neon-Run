using UnityEngine;
using Photon.Pun;
using System.Collections;

public class PlayerCombat : MonoBehaviourPunCallbacks
{
    [Header("Configuración de Ataque")]
    public Transform hitboxAtaque;
    public float radioHitbox = 0.5f;
    public LayerMask capaRival;
    public float cooldownAtaque = 0.4f;

    [Header("Estocada (Lunge)")]
    public float fuerzaEstocada = 6f;
    public float duracionEstocada = 0.15f;

    private PhotonView view;
    private Animator anim;
    private Rigidbody2D rb;
    private PlayerController controlador;
    private float ultimoAtaque = -999f;

    void Awake()
    {
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        controlador = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (!view.IsMine) return;
        if (GameManager.Instance != null && GameManager.Instance.PartidaTerminada) return;

        bool botonAtaque = Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.J);

        if (botonAtaque && Time.time >= ultimoAtaque + cooldownAtaque)
        {
            ultimoAtaque = Time.time;
            StopAllCoroutines();
            EjecutarAtaque();
        }
    }

    void EjecutarAtaque()
    {
        anim.SetTrigger("Golpe");

        Collider2D[] impactos = Physics2D.OverlapCircleAll(hitboxAtaque.position, radioHitbox, capaRival);
        foreach (Collider2D col in impactos)
        {
            PhotonView viewRival = col.GetComponentInParent<PhotonView>();
            if (viewRival != null && viewRival.ViewID != view.ViewID)
            {
                viewRival.RPC("RecibirDanioRPC", RpcTarget.All, 100);
                break;
            }
        }

        float direccion = Mathf.Sign(transform.localScale.x);
        StartCoroutine(EstocadaHaciaAdelante(direccion));
    }

    IEnumerator EstocadaHaciaAdelante(float direccion)
    {
        if (controlador != null) controlador.bloquearMovimiento = true;

        float tiempoTranscurrido = 0f;
        while (tiempoTranscurrido < duracionEstocada)
        {
            rb.velocity = new Vector2(direccion * fuerzaEstocada, rb.velocity.y);
            tiempoTranscurrido += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        // Solo desbloquea si la partida sigue activa (evita reactivar movimiento tras Victoria/Derrota)
        bool partidaSigueActiva = GameManager.Instance == null || !GameManager.Instance.PartidaTerminada;
        if (controlador != null && partidaSigueActiva)
        {
            controlador.bloquearMovimiento = false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (hitboxAtaque == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(hitboxAtaque.position, radioHitbox);
    }
}