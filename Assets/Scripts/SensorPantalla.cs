using UnityEngine;
using Photon.Pun;

public class SensorPantalla : MonoBehaviour
{
    [Header("Configuración")]
    public int direccionAvance = 1;
    private float tiempoEsperaCambio = 0f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Evitamos doble activación en el mismo segundo
        if (Time.time < tiempoEsperaCambio) return;

        if (collision.CompareTag("Player"))
        {
            PhotonView pv = collision.GetComponent<PhotonView>();

            // Solo el jugador que controlas localmente puede activar SU sensor de avance
            if (pv != null && pv.IsMine)
            {
                tiempoEsperaCambio = Time.time + 1f; // Bloqueo de 1 segundo
                GameManager.Instance.CambiarDePantalla(direccionAvance);
                MoverContenedorSensores();
            }
        }
    }

    private void MoverContenedorSensores()
    {
        Transform padreContenedor = transform.parent;
        if (padreContenedor != null)
        {
            float desplazamiento = direccionAvance * CamaraNidhogg.Instance.anchoPantalla;
            padreContenedor.position = new Vector3(padreContenedor.position.x + desplazamiento, padreContenedor.position.y, padreContenedor.position.z);
        }
    }
}