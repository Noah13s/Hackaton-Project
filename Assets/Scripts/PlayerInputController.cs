using UnityEngine;
using Unity.Netcode;

public class PlayerInputController : NetworkBehaviour
{
    private MultiPlayerScoreManager scoreManager;

    void Start()
    {
        // Finde den ScoreManager, der die Punkte verwaltet
        scoreManager = FindObjectOfType<MultiPlayerScoreManager>();
    }

    void Update()
    {
        // Nur der Besitzer dieses Objekts darf die Eingaben steuern (IsOwner stellt sicher, dass nur der lokale Spieler dies tut)
        if (IsOwner)
        {
            HandlePlayerInput();
        }
    }

    private void HandlePlayerInput()
    {
        // Beispiel: Spieler drückt die Leertaste, um seinen Score um 10 Punkte zu erhöhen
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Hol dir die Client-ID und sende die Anfrage an den Server, um den Score zu erhöhen
            ulong playerId = NetworkManager.Singleton.LocalClientId;
            scoreManager.AddScoreServerRpc(playerId, 10);
        }
    }
}
