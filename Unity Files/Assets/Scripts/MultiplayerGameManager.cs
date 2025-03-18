using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

using UnityEngine.UI;
/// <summary>
/// Manages the multiplayer matching card game, including player turns, score tracking,
/// UI updates, and scene transitions upon game completion.
/// </summary>
public class MultiplayerGameManager : MonoBehaviour
{
    public TextMeshProUGUI playerTurnText; // UI text to show the current player's turn
    public TextMeshProUGUI player1ScoreText; // UI text for Player 1 score
    public TextMeshProUGUI player2ScoreText; // UI text for Player 2 score

    private List<Card> flippedCards = new List<Card>(); // Tracks the currently flipped cards
    private bool canClick = true; // Controls whether players can click cards (prevents extra clicks during match checking)
    private List<Card> allCards; // Stores all cards for reset functionality

    public Image myPanel; // UI panel to visually indicate the active player (assign in Inspector)

    private int currentPlayer = 1; // Tracks the current player's turn (1 or 2)
    private int player1Score = 0; // Stores Player 1's score
    private int player2Score = 0; // Stores Player 2's score

    public CanvasGroup player1Panel; // UI panel for Player 1's victory screen (assign in Inspector)
    public CanvasGroup player2Panel; // UI panel for Player 2's victory screen (assign in Inspector)
    public CanvasGroup tiePanel; // UI panel for a tie game (assign in Inspector)

    public float fadeDuration = 1f; // Duration for fade-in effect during scene transitions
    public GameObject grid; // Reference to the card grid (assign in Inspector)

    void Start()
    {
        myPanel.color = Color.red;
        // Find all cards at the start and store them
        allCards = new List<Card>(FindObjectsOfType<Card>());
        UpdateUI();
    }
    void Update()
    {
        // Check if all cards have been matched (i.e., all children in the grid are inactive)
        if (AllChildrenHidden(grid.transform))
        {
            if (player1Score > player2Score)
            {

                LoadWinningScene(player1Panel);// Player 1 wins

            }
            else if (player1Score < player2Score)
            {
                LoadWinningScene(player1Panel);// Player 2 wins

            }
            else
            {
                LoadWinningScene(tiePanel); // It's a tie
            }
        }
    }
    /// <summary>
    /// Checks if all cards in the grid have been matched (hidden).
    /// </summary>
    bool AllChildrenHidden(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                return false; // At least one child is active
            }
        }
        return true; // All children are inactive
    }
    /// <summary>
    /// Called when a card is flipped. Manages flipping logic and checks for matches.
    /// </summary>
    public void CardFlipped(Card card)
    {
        if (!canClick || flippedCards.Contains(card)) return; // Prevent third click

        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            canClick = false; // Disable clicks until match check is done
            SetCardsColliders(false); // Temporarily disable colliders
            StartCoroutine(CheckMatch());
        }
    }
    /// <summary>
    /// Loads the appropriate winning scene based on the game outcome.
    /// </summary>
    public void LoadWinningScene(CanvasGroup fadePanel)
    {
        StartCoroutine(FadeInAndLoad(fadePanel));
    }
    /// <summary>
    /// Handles the fade-in animation before showing the winning/tie panel.
    /// </summary>
    private IEnumerator FadeInAndLoad(CanvasGroup fadePanel)
    {
        fadePanel.alpha = 0; // Start fully invisible
        AudioManager.Instance.PlaySFX(3);
        fadePanel.gameObject.SetActive(true); // Ensure it’s active

        float fadeDuration = 1.5f; // Define fade duration
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fadePanel.interactable = true; 
        fadePanel.blocksRaycasts = true;
        fadePanel.alpha = 1; // Ensure it fully fades in
    }
    /// <summary>
    /// Checks if the two flipped cards match and updates scores accordingly.
    /// </summary>
    IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1.0f); // Wait for animations to finish

        string mat1 = flippedCards[0].GetMaterial0Name();
        string mat2 = flippedCards[1].GetMaterial0Name();

        if (mat1 == mat2)
        {
            // Cards match, hide them and award points
            flippedCards[0].HideCard();
            flippedCards[1].HideCard();
            AudioManager.Instance.PlaySFX(5);

            if (currentPlayer == 1)
                player1Score++;
            else
                player2Score++;
        }
        else
        {
            // Cards don't match, flip them back over and switch turns
            flippedCards[0].ResetCard();
            flippedCards[1].ResetCard();
            AudioManager.Instance.PlaySFX(4);
            AudioManager.Instance.PlaySFX(2);
            SwitchTurn();

        }

        flippedCards.Clear(); // Clear list for the next turn
        yield return new WaitForSeconds(0.2f); // Allow animations to finish
        canClick = true; // Enable clicks again
        SetCardsColliders(true); // Re-enable colliders (So that a 3rd card is not turned)
        UpdateUI();
    }

    /// <summary>
    /// Switches the turn to the other player.
    /// </summary>
    private void SwitchTurn()
    {
        currentPlayer = (currentPlayer == 1) ? 2 : 1;
        myPanel.color = (currentPlayer == 1) ? Color.red : Color.green;
    }

    /// <summary>
    /// Updates the UI to display the current player's turn and scores.
    /// </summary>
    private void UpdateUI()
    {
        playerTurnText.text = "Player " + currentPlayer + "'s Turn";
        player1ScoreText.text = "Player 1: " + player1Score;
        player2ScoreText.text = "Player 2: " + player2Score;
    }

    /// <summary>
    /// Enables or disables all card colliders in the scene.
    /// </summary>
    private void SetCardsColliders(bool enabled)
    {
        foreach (Card card in FindObjectsOfType<Card>())
        {
            if (card != null && card.gameObject.activeSelf)
            {
                card.GetComponent<Collider>().enabled = enabled;
            }
        }
    }
    /// <summary>
    /// Resets the game by reloading the current scene.
    /// </summary>
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
