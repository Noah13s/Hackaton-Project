using Unity.Netcode;
using UnityEngine;

public class MultiPlayerScoreManager : NetworkBehaviour
{
    // NetworkVariables für beide Spieler-Scores
    public NetworkVariable<int> player1Score = new NetworkVariable<int>(0);
    public NetworkVariable<int> player2Score = new NetworkVariable<int>(0);

    void Start()
    {
        if (IsServer)
        {
            // Initialisiere Scores für beide Spieler auf 0
            player1Score.Value = 0;
            player2Score.Value = 0;
        }

        // Abonniere die Score-Änderungen, um zu reagieren (optional)
        player1Score.OnValueChanged += OnPlayer1ScoreChanged;
        player2Score.OnValueChanged += OnPlayer2ScoreChanged;
    }

    void OnPlayer1ScoreChanged(int previousValue, int newValue)
    {
        Debug.Log($"Player 1 Score updated from {previousValue} to {newValue}");
    }

    void OnPlayer2ScoreChanged(int previousValue, int newValue)
    {
        Debug.Log($"Player 2 Score updated from {previousValue} to {newValue}");
    }

    // Methoden, um die Scores der Spieler zu erhöhen
    [ServerRpc]
    public void AddScoreServerRpc(ulong playerId, int points)
    {
        if (playerId == 1)
        {
            player1Score.Value += points;
        }
        else if (playerId == 2)
        {
            player2Score.Value += points;
        }
    }
}
