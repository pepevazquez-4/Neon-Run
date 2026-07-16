using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [Header("Configuración de Movimiento")]
    public float velocidad = 8f;
    public float fuerzaSalto = 12f;

    [Header("Audio")]
    public AudioClip sonidoSalto;
    private AudioSource audioSource;

    private Rigidbody2D rb;
    private float movimientoH;
    private bool estaEnElSuelo;
    private PhotonView view;
    private Animator anim;

    [HideInInspector] public bool bloquearMovimiento = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        view = GetComponent<PhotonView>();
        anim = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (rb != null) rb.freezeRotation = true;
    }

    void Update()
    {
        if (!view.IsMine) return;

        if (bloquearMovimiento)
        {
            movimientoH = 0f;
            if (anim != null) anim.SetFloat("Velocidad", 0f);
            return;
        }

        movimientoH = Input.GetAxisRaw("Horizontal");
        if (anim != null) anim.SetFloat("Velocidad", Mathf.Abs(movimientoH));

        // Le avisamos al Animator si estamos en el aire, sin importar si es por salto o por caída natural
        if (anim != null) anim.SetBool("EnElAire", !estaEnElSuelo);

        if (Input.GetButtonDown("Jump") && estaEnElSuelo)
        {
            rb.velocity = new Vector2(rb.velocity.x, fuerzaSalto);
            estaEnElSuelo = false;

            if (sonidoSalto != null && audioSource != null)
            {
                audioSource.PlayOneShot(sonidoSalto);
            }
        }

        if (movimientoH > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (movimientoH < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    void FixedUpdate()
    {
        if (!view.IsMine) return;
        if (bloquearMovimiento) return;

        rb.velocity = new Vector2(movimientoH * velocidad, rb.velocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Suelo")) estaEnElSuelo = true;
    }
}