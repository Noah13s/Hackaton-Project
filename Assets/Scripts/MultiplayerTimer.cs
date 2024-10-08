using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;

public class MultiplayerTimer : NetworkBehaviour
{
    [Header("Component")]
    public TextMeshProUGUI timerText;

    [Header("Timer Settings")]
    public NetworkVariable<float> currentTime = new NetworkVariable<float>(0f); // Timer wird synchronisiert
    public bool countDown = true;

    [Header("Limit Settings")]
    public bool hasLimit = true;
    public float timerLimit = 0f;

    private bool isGameOver = false;

    void Start()
    {
        // Wenn es ein Host ist, setze den Timer auf den Startwert
        if (IsServer)
        {
            if (countDown)
                currentTime.Value = timerLimit; // Wenn der Timer herunterz�hlen soll
            else
                currentTime.Value = 0f; // Wenn der Timer hochz�hlen soll
        }
    }

    void Update()
    {
        // Nur der Host steuert den Timer
        if (IsServer && !isGameOver)
        {
            // Z�hle den Timer herunter oder hoch
            currentTime.Value = countDown ? currentTime.Value -= Time.deltaTime : currentTime.Value += Time.deltaTime;

            // �berpr�fe, ob der Timer das Limit erreicht hat
            if (hasLimit && ((countDown && currentTime.Value <= 0f) || (!countDown && currentTime.Value >= timerLimit)))
            {
                // Wenn das Limit erreicht ist, stelle sicher, dass der Timer nicht weiterl�uft
                currentTime.Value = countDown ? 0f : timerLimit;

                // Zeige Game Over auf beiden Ger�ten
                GameOverClientRpc();

                // Timer deaktivieren
                isGameOver = true;
            }
        }

        // Aktualisiere die Anzeige des Timers auf allen Clients
        SetTimerText();
    }

    // Aktualisiere die Textanzeige des Timers
    void SetTimerText()
    {
        timerText.text = currentTime.Value.ToString("0.0");
    }

    // Diese RPC sendet die "Game Over"-Nachricht an alle Clients
    [ClientRpc]
    void GameOverClientRpc()
    {
        // Zeige "Game Over" auf beiden Ger�ten an
        timerText.text = "Game Over";
        timerText.color = Color.red;

        Debug.Log("Game Over");
    }
}
