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

    // Referenz auf das NetworkManagerUI-Skript
    private NetworkManagerUI networkManagerUI;

    void Start()
    {
        // Finde das NetworkManagerUI-Skript in der Szene
        networkManagerUI = FindObjectOfType<NetworkManagerUI>();

        // Wenn es ein Host ist, setze den Timer auf den Startwert
        if (IsServer)
        {
            if (countDown)
                currentTime.Value = timerLimit; // Wenn der Timer herunterzählen soll
            else
                currentTime.Value = 0f; // Wenn der Timer hochzählen soll
        }
    }

    void Update()
    {
        // Prüfe, ob die Verbindung besteht und der Timer auf dem Server läuft
        if (networkManagerUI != null && networkManagerUI.isConnected && IsServer && !isGameOver)
        {
            // Zähle den Timer herunter oder hoch
            currentTime.Value = countDown ? currentTime.Value -= Time.deltaTime : currentTime.Value += Time.deltaTime;

            // Überprüfe, ob der Timer das Limit erreicht hat
            if (hasLimit && ((countDown && currentTime.Value <= 0f) || (!countDown && currentTime.Value >= timerLimit)))
            {
                // Wenn das Limit erreicht ist, stelle sicher, dass der Timer nicht weiterläuft
                currentTime.Value = countDown ? 0f : timerLimit;

                // Zeige Game Over auf beiden Geräten
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
        // Zeige "Game Over" auf beiden Geräten an
        timerText.text = "Game Over";
        timerText.color = Color.red;

        Debug.Log("Game Over");
    }
}
