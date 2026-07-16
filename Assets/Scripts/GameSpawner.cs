using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameSpawner : MonoBehaviour
{
    [Header("Desfase de Esquinas (Local)")]
    public float distanciaIzquierda = -6f;
    public float distanciaDerecha = 6f;
    public float alturaSuelo = 1f;

    void Start()
    {
        StartCoroutine(EsperarConexionYSpawnear());
    }

    IEnumerator EsperarConexionYSpawnear()
    {
        Debug.Log("<color=yellow>[SPAWNER-INFO]</color> Esperando confirmación de conexión con Photon...");

        float timeoutTimer = 0f;
        const float TIEMPO_MAXIMO_ESPERA = 10f;

        while (!PhotonNetwork.IsConnectedAndReady && timeoutTimer < TIEMPO_MAXIMO_ESPERA)
        {
            timeoutTimer += Time.deltaTime;
            yield return null;
        }

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogError("<color=red>[SPAWNER FATAL]</color> Se agotó el tiempo de espera.");
            yield break;
        }

        InstanciarJugador();
    }

    void InstanciarJugador()
    {
        GameManager.Instance.ReiniciarEstado();

        bool esInvitado = !PhotonNetwork.IsMasterClient;
        Vector3 puntoSpawn;

        if (PhotonNetwork.IsMasterClient)
        {
            puntoSpawn = new Vector3(transform.position.x + distanciaIzquierda, transform.position.y + alturaSuelo, 0f);
            Debug.Log($"<color=green>[SPAWNER-ROLES]</color> Cliente Host. Spawn P1 en: {puntoSpawn}");
        }
        else
        {
            puntoSpawn = new Vector3(transform.position.x + distanciaDerecha, transform.position.y + alturaSuelo, 0f);
            Debug.Log($"<color=green>[SPAWNER-ROLES]</color> Cliente Invitado. Spawn P2 en: {puntoSpawn}");
        }

        string prefabElegido = SelectorPersonaje.ObtenerPrefabElegido();
        Debug.Log($"<color=cyan>[SPAWNER-PERSONAJE]</color> Instanciando: {prefabElegido}");

        GameObject jugadorInstanciado = PhotonNetwork.Instantiate(prefabElegido, puntoSpawn, Quaternion.identity);

        if (jugadorInstanciado != null)
        {
            Rigidbody2D rb = jugadorInstanciado.GetComponent<Rigidbody2D>();
            if (rb != null) rb.velocity = Vector2.zero;

            if (esInvitado)
            {
                jugadorInstanciado.transform.localScale = new Vector3(-1f, 1f, 1f);
            }

            Debug.Log($"<color=white>[SPAWNER-OK]</color> '{prefabElegido}' fijado con éxito en: {jugadorInstanciado.transform.position}");
        }
        else
        {
            Debug.LogError("<color=red>[SPAWNER-ERROR]</color> El prefab no pudo nacer. Revisa que esté en Assets/Resources/.");
        }
    }
}