using UnityEngine;
using TMPro;
using Unity.Netcode;

public class MultiplayerScore : NetworkBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI myScoreText;       // Textfeld f�r eigene Punkte
    public TextMeshProUGUI opponentScoreText; // Textfeld f�r die Punkte des Gegners

    [Header("Score Settings")]
    public NetworkVariable<int> myScore = new NetworkVariable<int>(0);           // Eigene Punkte
    public NetworkVariable<int> opponentScore = new NetworkVariable<int>(0);     // Punkte des Gegners

    void Start()
    {
        // Setze die Startpunkte f�r den Host
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

    // Funktion, um die Punkte zu erh�hen (wird vom Host oder dem Spieler aufgerufen)
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

    // Methode, um die Punktest�nde in der UI anzuzeigen
    void UpdateScoreText()
    {
        // Zeige die aktuellen Punktest�nde an (Synchronisation erfolgt automatisch durch NetworkVariables)
        myScoreText.text = "My Score: " + myScore.Value.ToString();
        opponentScoreText.text = "Opponent Score: " + opponentScore.Value.ToString();
    }
}
