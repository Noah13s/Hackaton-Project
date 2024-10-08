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
    public InputField ipInputField; // Eingabefeld für die IP-Adresse
    public Text hostIpText;         // Text für die Host-IP-Anzeige
    public Text statusText;         // Text für den Verbindungsstatus (z.B. "Connected")

    private UnityTransport transport;

    public bool isConnected = false;

    void Start()
    {
        // Setze die Transport-Komponente
        transport = FindObjectOfType<UnityTransport>();

        // Buttons setzen
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(ShowClientInput);
        backButton.onClick.AddListener(BackToMenu);
        enterButton.onClick.AddListener(StartClient);  // Enter-Button Listener

        // Initialer Zustand: Host- und Client-Buttons sichtbar, andere Elemente versteckt
        ipInputField.gameObject.SetActive(false);  // IP-Eingabefeld versteckt
        enterButton.gameObject.SetActive(false);   // Enter-Button versteckt
        hostIpText.gameObject.SetActive(false);    // Host-IP-Anzeige versteckt
        backButton.gameObject.SetActive(false);    // Back-Button versteckt
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
        if (NetworkManager.Singleton.IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (NetworkManager.Singleton.IsClient)
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
        statusText.gameObject.SetActive(false);  // Status-Text ausblenden

        // Stoppe die "Connection working"-Überprüfung
        StopCoroutine(CheckForConnection());
    }

    // Coroutine zur Überprüfung, ob die Verbindung hergestellt ist
    private IEnumerator CheckForConnection()
    {
        while (true)
        {
            // Warte auf den Verbindungsaufbau
            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost)
            {
                // Setze den Verbindungsstatus
                isConnected = true;

                // Zeige "Connected" für 2 Sekunden
                statusText.text = "Connected";
                statusText.gameObject.SetActive(true);

                yield return new WaitForSeconds(2f);
                statusText.gameObject.SetActive(false);

                // Starte die "Connection working"-Überprüfung
                StartCoroutine(ConnectionWorkingLog());
                yield break; // Beende die Schleife, da die Verbindung hergestellt wurde
            }

            yield return null;
        }
    }

    // Coroutine zum Protokollieren von "Connection working" alle 10 Sekunden
    private IEnumerator ConnectionWorkingLog()
    {
        while (isConnected)
        {
            Debug.Log("Connection working");
            yield return new WaitForSeconds(10f);
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
