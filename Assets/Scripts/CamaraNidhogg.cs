using UnityEngine;

public class CamaraNidhogg : MonoBehaviour
{
    public static CamaraNidhogg Instance;

    [Header("Configuración de Dimensiones")]
    public float anchoPantalla = 18f;
    public float velocidadMover = 15f; // Aumentamos velocidad para transiciones limpias

    private Vector3 posicionDestino;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        posicionDestino = transform.position;
    }

    void LateUpdate() // Cambiado a LateUpdate para que corra después del movimiento del jugador
    {
        if (GameManager.Instance == null) return;

        // Calculamos la posición exacta X del bloque
        float objetivoX = GameManager.Instance.pantallaActual * anchoPantalla;
        posicionDestino = new Vector3(objetivoX, transform.position.y, transform.position.z);

        // MoveTowards elimina los temblores de cálculo frame a frame
        transform.position = Vector3.MoveTowards(transform.position, posicionDestino, velocidadMover * Time.deltaTime);
    }
}