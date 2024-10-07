using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System.Net;
using Unity.Netcode.Transports.UTP;

public class NetworkManagerUI : MonoBehaviour
{
    public Button hostButton;  // Button für den Host
    public Button clientButton;  // Button für den Client
    public Button backButton;  // Button zum Zurückkehren zur Auswahl
    public Button enterButton;  // Button zum Verbinden nach Eingabe der IP
    public InputField ipInputField;  // Eingabefeld für die IP-Adresse
    public Text hostIpText;  // Text für die Host-IP-Anzeige

    private UnityTransport transport;

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
            // Transportverbindung setzen
            transport.ConnectionData.Address = ipInputField.text;

            // Starte den Client
            NetworkManager.Singleton.StartClient();

            // IP-Eingabefeld und Enter-Button verstecken
            ipInputField.gameObject.SetActive(false);
            enterButton.gameObject.SetActive(false);
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
    }

    // Methode zur Ermittlung der lokalen IP-Adresse
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
