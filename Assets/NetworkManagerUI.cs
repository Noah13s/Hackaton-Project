using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Collections;
using System.Net;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;       // Button für den Host
    public Button clientButton;     // Button für den Client
    public Button backButton;       // Button zum Zurückkehren zur Auswahl
    public Button enterButton;      // Button zum Verbinden nach Eingabe der IP
    public Button startButton;      // Button zum Starten des Timers (nur für den Host sichtbar)
    public InputField ipInputField; // Eingabefeld für die IP-Adresse
    public Text hostIpText;         // Text für die Host-IP-Anzeige
    public Text statusText;         // Text für den Verbindungsstatus (z.B. "Connected")

    private UnityTransport transport;
    private MultiplayerTimer multiplayerTimer; // Referenz auf das MultiplayerTimer-Skript
    private bool isHostConnected = false;
    private bool isClientConnected = false;

    void Start()
    {
        // Setze die Transport-Komponente
        transport = FindObjectOfType<UnityTransport>();

        // Timer-Objekt finden
        multiplayerTimer = FindObjectOfType<MultiplayerTimer>();

        // Buttons setzen
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(ShowClientInput);
        backButton.onClick.AddListener(BackToMenu);
        enterButton.onClick.AddListener(StartClient);
        startButton.onClick.AddListener(StartTimer);

        // Initialer Zustand: Host- und Client-Buttons sichtbar, andere Elemente versteckt
        ipInputField.gameObject.SetActive(false);  // IP-Eingabefeld versteckt
        enterButton.gameObject.SetActive(false);   // Enter-Button versteckt
        hostIpText.gameObject.SetActive(false);    // Host-IP-Anzeige versteckt
        backButton.gameObject.SetActive(false);    // Back-Button versteckt
        startButton.gameObject.SetActive(false);   // Start-Button versteckt (Host-only)
        statusText.gameObject.SetActive(false);    // Status-Text versteckt
    }

    public void StartHost()
    {
        // Starte den Host
        NetworkManager.Singleton.StartHost();

        // Eigene IP-Adresse ermitteln und anzeigen
        hostIpText.text = "Host IP: " + GetLocalIPAddress();
        hostIpText.gameObject.SetActive(true);

        // Host- und Client-Buttons verstecken
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);

        // Back-Button anzeigen
        backButton.gameObject.SetActive(true);

        // Starte Überprüfung für "connected"
        StartCoroutine(CheckForConnection());
    }

    public void ShowClientInput()
    {
        // Zeige das IP-Eingabefeld und den Enter-Button, nachdem der Client-Button gedrückt wurde
        ipInputField.gameObject.SetActive(true);
        enterButton.gameObject.SetActive(true);

        // Host- und Client-Buttons verstecken
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);

        // Back-Button anzeigen
        backButton.gameObject.SetActive(true);
    }

    public void StartClient()
    {
        // Stelle sicher, dass die IP-Adresse eingegeben wurde
        if (!string.IsNullOrEmpty(ipInputField.text))
        {
            // Setze die eingegebene IP-Adresse in den UnityTransport
            transport.ConnectionData.Address = ipInputField.text;

            // Starte den Client
            NetworkManager.Singleton.StartClient();

            // IP-Eingabefeld und Enter-Button verstecken
            ipInputField.gameObject.SetActive(false);
            enterButton.gameObject.SetActive(false);

            // Starte Überprüfung für "connected"
            StartCoroutine(CheckForConnection());
        }
        else
        {
            Debug.LogError("Bitte eine gültige IP-Adresse eingeben.");
        }
    }

    public void BackToMenu()
    {
        // Beende den Host oder den Client, falls sie laufen
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Zurück zur Host/Client-Auswahl
        hostButton.gameObject.SetActive(true);
        clientButton.gameObject.SetActive(true);

        // Andere UI-Elemente verstecken
        ipInputField.gameObject.SetActive(false);
        hostIpText.gameObject.SetActive(false);
        enterButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);  // Back-Button ausblenden
        startButton.gameObject.SetActive(false); // Start-Button ausblenden
        statusText.gameObject.SetActive(false);  // Status-Text ausblenden

        // Stoppe die "Connection working"-Überprüfung
        StopCoroutine(CheckForConnection());
    }

    // Verbindung beenden, wenn das Spiel geschlossen wird
    private void OnApplicationQuit()
    {
        if (NetworkManager.Singleton != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient))
        {
            NetworkManager.Singleton.Shutdown(); // Beendet die Verbindung bei Spielende
        }
    }

    // Coroutine zur Überprüfung, ob die Verbindung hergestellt ist
    private IEnumerator CheckForConnection()
    {
        while (true)
        {
            if (NetworkManager.Singleton.IsHost)
            {
                isHostConnected = true;  // Host ist verbunden
            }

            if (NetworkManager.Singleton.IsConnectedClient)
            {
                isClientConnected = true;  // Client ist verbunden
            }

            // Nur der Host soll den Start-Button sehen, wenn beide verbunden sind
            if (isHostConnected && isClientConnected && NetworkManager.Singleton.IsHost)
            {
                startButton.gameObject.SetActive(true); // Zeige den Start-Button nur für den Host
                yield break;
            }

            yield return null;
        }
    }

    // Startet den Timer, wenn der Start-Button gedrückt wird (nur für den Host)
    public void StartTimer()
    {
        if (isHostConnected && isClientConnected)
        {
            multiplayerTimer.StartTimer(); // Timer wird gestartet
            startButton.gameObject.SetActive(false); // Start-Button ausblenden nach Klick
        }
        else
        {
            Debug.LogError("Host und Client müssen verbunden sein, bevor das Spiel gestartet werden kann.");
        }
    }

    // Methode zur Ermittlung der lokalen IP-Adresse des Hosts
    private string GetLocalIPAddress()
    {
        string localIP = "";
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
