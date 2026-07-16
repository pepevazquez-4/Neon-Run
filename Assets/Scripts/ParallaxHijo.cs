using UnityEngine;

// [ExecuteInEditMode] // Descomenta esta línea para previsualizar en el editor
public class ParallaxHijo : MonoBehaviour
{
    [Header("Configuración de Parallax")]
    [Tooltip("Intensidad del efecto (0.1 = muy sutil lejanía, 0.9 = casi igual que la cámara)")]
    [Range(0f, 1f)]
    public float factorEfecto = 0.5f;

    private Transform mainCameraTransform;
    private Vector3 initialLocalPosition;

    void Start()
    {
        // Encontrar la cámara principal en la escena
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No se encontró una Main Camera en la escena con el Tag 'MainCamera'.");
            enabled = false; // Desactivar el script si no hay cámara
            return;
        }
        mainCameraTransform = mainCam.transform;

        // Guardar la posición inicial relativa (local) del objeto hijo
        initialLocalPosition = transform.localPosition;
    }

    void LateUpdate()
    {
        // LateUpdate se ejecuta DESPUÉS de que la cámara se ha movido
        // para que nuestro cálculo use la posición final de la cámara.

        // Calcular la posición local ajustada en base al factor
        // Movimiento local = (Factor * (1/factorEfecto)) -> Esto no es muy intuitivo
        // Mejor enfoque: La posición local cambia en relación inversa al factor
        // Si factor es 1, la posición local no cambia. Si factor es 0.1, la posición local cambia mucho en dirección opuesta.

        // Un factor más grande (ej. 0.9) debe mover el objeto casi igual que la cámara -> posición local cambia poco.
        // Un factor más pequeño (ej. 0.1) debe mover el objeto mucho menos que la cámara -> posición local cambia mucho en contra.

        // Corrección del enfoque de movimiento relativo para objetos hijos:
        // Necesitamos que el objeto hijo se desplace LOCALMENTE para "quedarse atrás" visualmente.
        // Desplazamiento Local = -(Vector de movimiento de la cámara * (1 - factor))

        // La posición del objeto hijo en relación a la cámara será:
        // transform.position = mainCameraTransform.position + transform.localPosition

        // Lo que realmente queremos es que transform.position se mueva a factor * mainCameraTransform.position (más o menos)
        // Entonces: transLocalPosition + mainCamTransformPos = factor * mainCamTransformPos
        // transLocalPosition = (factor - 1) * mainCamTransformPos

        // Este enfoque funciona mejor si queremos control absoluto.
        // Vamos a usar una variación más simple para 'Parallax Relativo' como hijos:
        // Simplemente movemos el objeto hijo LOCALMENTE en la dirección OPUESTA al movimiento de la cámara, escalado.

        // Obtenemos el vector de la cámara y le aplicamos el factor de retroceso (1 - factor)
        // Un factorEfecto alto (ej. 0.9) significa poco retroceso (1 - 0.9 = 0.1), objeto casi 'pegado'
        // Un factorEfecto bajo (ej. 0.1) significa mucho retroceso (1 - 0.1 = 0.9), objeto se queda atrás

        Vector3 retrocesoLocal = new Vector3(mainCameraTransform.position.x * (1f - factorEfecto), 0f, 0f);

        // Aplicamos el retroceso a la posición local inicial, solo en el eje X
        transform.localPosition = new Vector3(initialLocalPosition.x - retrocesoLocal.x, initialLocalPosition.y, initialLocalPosition.z);
    }
}