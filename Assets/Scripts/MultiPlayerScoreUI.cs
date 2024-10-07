using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class MultiPlayerScoreUI : MonoBehaviour
{
    public Text player1ScoreText;
    public Text player2ScoreText;
    private MultiPlayerScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<MultiPlayerScoreManager>();

        if (scoreManager != null)
        {
            // Abonniere Änderungen an beiden Spieler-Scores
            scoreManager.player1Score.OnValueChanged += UpdatePlayer1ScoreUI;
            scoreManager.player2Score.OnValueChanged += UpdatePlayer2ScoreUI;
        }

        // Initialisiere die UI
        UpdatePlayer1ScoreUI(0, scoreManager.player1Score.Value);
        UpdatePlayer2ScoreUI(0, scoreManager.player2Score.Value);
    }

    void UpdatePlayer1ScoreUI(int previousValue, int newValue)
    {
        player1ScoreText.text = $"Player 1 Score: {newValue}";
    }

    void UpdatePlayer2ScoreUI(int previousValue, int newValue)
    {
        player2ScoreText.text = $"Player 2 Score: {newValue}";
    }
}
