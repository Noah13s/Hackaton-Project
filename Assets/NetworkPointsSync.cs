using UnityEngine;
using TMPro;
using Unity.Netcode;

public class MultiplayerScore : NetworkBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI myScoreText;       // Textfeld für eigene Punkte
    public TextMeshProUGUI opponentScoreText; // Textfeld für die Punkte des Gegners

    [Header("Score Settings")]
    public NetworkVariable<int> myScore = new NetworkVariable<int>(0);           // Eigene Punkte
    public NetworkVariable<int> opponentScore = new NetworkVariable<int>(0);     // Punkte des Gegners

    void Start()
    {
        // Setze die Startpunkte für den Host
        if (IsServer)
        {
            myScore.Value = 0;
            opponentScore.Value = 0;
        }
    }

    void Update()
    {
        // Aktualisiere die Anzeige der Punkte auf allen Clients
        UpdateScoreText();
    }

    // Funktion, um die Punkte zu erhöhen (wird vom Host oder dem Spieler aufgerufen)
    public void AddPoints(bool isMyScore, int points)
    {
        if (IsServer)
        {
            if (isMyScore)
            {
                myScore.Value += points;
            }
            else
            {
                opponentScore.Value += points;
            }
        }
    }

    // Methode, um die Punktestände in der UI anzuzeigen
    void UpdateScoreText()
    {
        // Zeige die aktuellen Punktestände an (Synchronisation erfolgt automatisch durch NetworkVariables)
        myScoreText.text = "My Score: " + myScore.Value.ToString();
        opponentScoreText.text = "Opponent Score: " + opponentScore.Value.ToString();
    }
}
