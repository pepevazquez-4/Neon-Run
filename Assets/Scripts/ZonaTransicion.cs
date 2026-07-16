using UnityEngine;
using Photon.Pun;

public class ZonaTransicion : MonoBehaviour
{
    public Collider2D colliderSolido;
    public Collider2D colliderTrigger;

    private float ultimoFlash = -999f;
    public float cooldownFlash = 1f;

    void Update()
    {
        if (GameManager.Instance == null) return;

        bool debeBloquear = GameManager.Instance.estadoActual == EstadoAvance.Neutro;
        colliderSolido.enabled = debeBloquear;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (GameManager.Instance != null && GameManager.Instance.estadoActual == EstadoAvance.Neutro) return;

        PhotonView view = other.GetComponentInParent<PhotonView>();
        if (view == null || !view.IsMine) return;

        if (Time.time >= ultimoFlash + cooldownFlash)
        {
            ultimoFlash = Time.time;
            if (TransicionPantalla.Instance != null)
            {
                TransicionPantalla.Instance.Flash();
            }
        }
    }
}