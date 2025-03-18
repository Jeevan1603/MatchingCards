using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the matching card game, tracking moves, checking matches, and handling UI updates.
/// </summary>
public class GameManager : MonoBehaviour
{
    public TextMeshProUGUI matchText; // UI text to display the number of moves
    private List<Card> flippedCards = new List<Card>(); // Stores flipped cards
    private int number_of_moves = 0; // Counter for moves made
    private bool canClick = true; // Controls card clicking to prevent errors
    private List<Card> allCards; // Stores all game cards for reset purposes
    public GameObject grid; // Reference to the card grid
    private string playerName; // Stores the player's name
    private string gridSize; // Stores the selected grid size

    public CanvasGroup winningPanel; // UI panel to display when the game is won
    private int playerScore; // Stores the player's score

    void Start()
    {
        // Retrieve stored player data
        allCards = new List<Card>(FindObjectsOfType<Card>());
        playerName = PlayerPrefs.GetString("PlayerName", "Guest");
        gridSize = PlayerPrefs.GetString("GridSize", "3x4");
        playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
        Debug.Log(playerName);
    }

    void Update()
    {
        // Check if all cards have been matched
        if (AllChildrenHidden(grid.transform))
        {
            LoadWinningScene(winningPanel);
        }
    }

    /// <summary>
    /// Checks if all cards are hidden (matched)
    /// </summary>
    private bool AllChildrenHidden(Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.activeSelf)
            {
                return false; // At least one card is still visible
            }
        }
        return true; // All cards are hidden
    }

    /// <summary>
    /// Handles card flipping logic, ensuring only two cards can be flipped at a time.
    /// </summary>
    public void CardFlipped(Card card)
    {
        if (!canClick || flippedCards.Contains(card)) return; // Prevent invalid flips

        flippedCards.Add(card);

        if (flippedCards.Count == 2)
        {
            canClick = false; // Disable further clicks while checking the match
            SetCardsColliders(false);
            StartCoroutine(CheckMatch());
        }
    }

    /// <summary>
    /// Checks if the flipped cards match and updates scores accordingly.
    /// </summary>
    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1.0f);
        number_of_moves++;
        PlayerPrefs.SetInt("PlayerScore", number_of_moves);
        PlayerPrefs.Save();
        matchText.text = number_of_moves.ToString();

        string mat1 = flippedCards[0].GetMaterial0Name();
        string mat2 = flippedCards[1].GetMaterial0Name();

        if (mat1 == mat2)
        {
            flippedCards[0].HideCard();
            flippedCards[1].HideCard();
            AudioManager.Instance.PlaySFX(5);
        }
        else
        {
            flippedCards[0].ResetCard();
            flippedCards[1].ResetCard();
            AudioManager.Instance.PlaySFX(4);
            AudioManager.Instance.PlaySFX(2);
        }

        flippedCards.Clear();
        yield return new WaitForSeconds(0.2f);
        canClick = true;
        SetCardsColliders(true);
    }

    /// <summary>
    /// Enables or disables all card colliders.
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
    /// Removes a card from the active card list when it is hidden.
    /// </summary>
    public void RemoveCardReference(Card card)
    {
        allCards.Remove(card);
    }

    /// <summary>
    /// Ends the game and saves the player's score.
    /// </summary>
    public void EndGame()
    {
        int finalScore = number_of_moves;
        Debug.Log($"Saving Score: {playerName} - {finalScore}");

        PlayerPrefs.SetString("Last_Player_Name", playerName);
        PlayerPrefs.SetString("Last_Grid_Size", gridSize);
        PlayerPrefs.SetInt("Last_Player_Score", finalScore);
        PlayerPrefs.Save();

    }

    /// <summary>
    /// Resets the game by reloading the current scene.
    /// </summary>
    public void ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Displays the winning panel with a fade-in effect.
    /// </summary>
    public void LoadWinningScene(CanvasGroup fadePanel)
    {
        StartCoroutine(FadeInAndLoad(fadePanel));
    }

    /// <summary>
    /// Fades in the winning panel before showing it.
    /// </summary>
    private IEnumerator FadeInAndLoad(CanvasGroup fadePanel)
    {
        fadePanel.alpha = 0;
        fadePanel.gameObject.SetActive(true);
        float fadeDuration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            fadePanel.alpha = Mathf.Lerp(0, 1, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        fadePanel.interactable = true;
        fadePanel.blocksRaycasts = true;
        fadePanel.alpha = 1;
    }
}
