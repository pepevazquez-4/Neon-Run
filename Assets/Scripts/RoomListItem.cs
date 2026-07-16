using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoomListItem : MonoBehaviour
{
    public TextMeshProUGUI textoNombreSala;
    public Button botonUnirse;

    private string nombreSala;
    private NetworkController controladorRed;

    void Awake()
    {
        // Forzamos el tamaño del ítem para que el texto nunca se corte
        RectTransform rt = GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(300f, 50f);
        }

        // Confirmamos que el texto quede centrado tanto horizontal como verticalmente
        if (textoNombreSala != null)
        {
            textoNombreSala.alignment = TextAlignmentOptions.Center;
        }
    }

    public void Configurar(string nombre, NetworkController controlador)
    {
        nombreSala = nombre;
        controladorRed = controlador;
        textoNombreSala.text = nombre;

        botonUnirse.onClick.RemoveAllListeners();
        botonUnirse.onClick.AddListener(() => controladorRed.UnirseASalaEspecifica(nombreSala));
    }
}