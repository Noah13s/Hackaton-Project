using System.Collections;
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
    private bool isClientConnected = false;

    void Start()
    {
        // Timer wird nicht direkt gestartet, sondern wartet auf die Verbindung
        if (IsServer)
        {
            currentTime.Value = countDown ? timerLimit : 0f; // Initialer Timerwert
        }
    }

    void Update()
    {
        // Starte den Timer erst, wenn der Client verbunden ist
        if (IsServer && isClientConnected && !isGameOver)
        {
            // Timer logik, herunterzählen oder hochzählen
            currentTime.Value = countDown ? currentTime.Value -= Time.deltaTime : currentTime.Value += Time.deltaTime;

            // �berpr�fe, ob der Timer das Limit erreicht hat
            if (hasLimit && ((countDown && currentTime.Value <= 0f) || (!countDown && currentTime.Value >= timerLimit)))
            {
                // Timer beenden
                currentTime.Value = countDown ? 0f : timerLimit;
                GameOverClientRpc();
                isGameOver = true;
            }
        }

        // Timer-Text aktualisieren
        SetTimerText();
    }

    // Methode zum Starten des Timers nach Verbindungsherstellung
    public void StartTimer()
    {
        isClientConnected = true; // Client ist verbunden, Timer kann gestartet werden
    }

    // Textanzeige des Timers aktualisieren
    void SetTimerText()
    {
        timerText.text = currentTime.Value.ToString("0.0");
    }

    // Game Over RPC für alle Clients
    [ClientRpc]
    void GameOverClientRpc()
    {
        timerText.text = "Game Over";
        timerText.color = Color.red;
        Debug.Log("Game Over");
    }
}
