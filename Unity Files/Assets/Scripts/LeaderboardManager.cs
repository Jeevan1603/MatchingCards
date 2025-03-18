using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LeaderboardManager : MonoBehaviour
{
    public Transform leaderboardContainer; // Parent transform where leaderboard entries will be instantiated
    public GameObject entryPrefab; // Prefab for a single leaderboard entry
    public Button button3x4, button4x4, button5x4; // Buttons for filtering leaderboard based on grid size

    private List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>(); // Stores leaderboard data
    private const int MaxEntries = 10; // Maximum entries allowed per grid size

    void Start()
    {
        LoadLeaderboard();

        // Assign button click listeners to filter leaderboard
        button3x4.onClick.AddListener(() => DisplayLeaderboard("3x4"));
        button4x4.onClick.AddListener(() => DisplayLeaderboard("4x4"));
        button5x4.onClick.AddListener(() => DisplayLeaderboard("5x4"));

        // Check if a new score was stored from the previous game
        if (PlayerPrefs.HasKey("Last_Player_Name") && PlayerPrefs.HasKey("Last_Player_Score") && PlayerPrefs.HasKey("Last_Grid_Size"))
        {
            string newName = PlayerPrefs.GetString("Last_Player_Name");
            int newScore = PlayerPrefs.GetInt("Last_Player_Score");
            string newGridSize = PlayerPrefs.GetString("Last_Grid_Size");

            Debug.Log($"New Score Detected: {newName} - {newScore} (Grid: {newGridSize})");

            // Add the new score to the leaderboard
            AddScore(newName, newScore, newGridSize);

            // Remove temporary stored values after processing
            PlayerPrefs.DeleteKey("Last_Player_Name");
            PlayerPrefs.DeleteKey("Last_Player_Score");
            PlayerPrefs.DeleteKey("Last_Grid_Size");
            PlayerPrefs.Save();
        }

        // Default leaderboard display (e.g., show 3x4 grid leaderboard on start)
        DisplayLeaderboard("3x4");
    }

    /// <summary>
    /// Loads the saved leaderboard entries from PlayerPrefs.
    /// </summary>
    void LoadLeaderboard()
    {
        leaderboardEntries.Clear();
        Debug.Log("Loading leaderboard...");

        for (int i = 0; i < 15; i++) // Load scores for all grid sizes
        {
            string nameKey = "Leaderboard_Name_" + i;
            string scoreKey = "Leaderboard_Score_" + i;
            string gridKey = "Leaderboard_Grid_" + i;

            if (PlayerPrefs.HasKey(nameKey) && PlayerPrefs.HasKey(scoreKey) && PlayerPrefs.HasKey(gridKey))
            {
                string name = PlayerPrefs.GetString(nameKey);
                int score = PlayerPrefs.GetInt(scoreKey);
                string grid = PlayerPrefs.GetString(gridKey);
                leaderboardEntries.Add(new LeaderboardEntry(name, score, grid));

                Debug.Log($"Loaded Entry {i}: {name} - {score} (Grid: {grid})");
            }
        }
    }

    /// <summary>
    /// Saves the leaderboard entries to PlayerPrefs.
    /// </summary>
    public void SaveLeaderboard()
    {
        for (int i = 0; i < leaderboardEntries.Count; i++)
        {
            PlayerPrefs.SetString("Leaderboard_Name_" + i, leaderboardEntries[i].username);
            PlayerPrefs.SetInt("Leaderboard_Score_" + i, leaderboardEntries[i].score);
            PlayerPrefs.SetString("Leaderboard_Grid_" + i, leaderboardEntries[i].gridSize);
        }
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Adds a new score to the leaderboard, updates existing scores if necessary, and maintains sorting.
    /// </summary>
    public void AddScore(string playerName, int score, string gridSize)
    {
        // Check if the player already has a score recorded for the same grid size
        LeaderboardEntry existingEntry = leaderboardEntries.Find(entry => entry.username == playerName && entry.gridSize == gridSize);

        if (existingEntry != null)
        {
            // Update the score only if the new score is better (lower)
            if (score < existingEntry.score)
            {
                existingEntry.score = score;
            }
        }
        else
        {
            // Add a new leaderboard entry if the player is not found
            leaderboardEntries.Add(new LeaderboardEntry(playerName, score, gridSize));
        }

        // Sort leaderboard entries in ascending order (since lower score is better)
        leaderboardEntries.Sort((a, b) => a.score.CompareTo(b.score));

        // Keep only the top 10 entries per grid size
        List<LeaderboardEntry> filteredEntries = leaderboardEntries.FindAll(entry => entry.gridSize == gridSize);
        if (filteredEntries.Count > MaxEntries)
        {
            leaderboardEntries.Remove(filteredEntries[filteredEntries.Count - 1]); // Remove the lowest-ranked score
        }

        SaveLeaderboard(); // Save updated leaderboard
        DisplayLeaderboard(gridSize); // Update UI
    }

    /// <summary>
    /// Clears the entire leaderboard from PlayerPrefs and resets the UI.
    /// </summary>
    public void ClearLeaderboard()
    {
        Debug.Log("Clearing leaderboard...");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        leaderboardEntries.Clear();
        DisplayLeaderboard("3x4"); // Reset to default leaderboard display
    }

    /// <summary>
    /// Displays the leaderboard entries for a specific grid size.
    /// </summary>
    public void DisplayLeaderboard(string gridSize)
    {
        // Remove existing leaderboard UI entries before updating
        foreach (Transform child in leaderboardContainer)
        {
            Destroy(child.gameObject);
        }

        // Filter leaderboard entries based on the selected grid size
        List<LeaderboardEntry> filteredEntries = leaderboardEntries.FindAll(entry => entry.gridSize == gridSize);

        // Sort in ascending order (lowest to highest score)
        filteredEntries.Sort((a, b) => a.score.CompareTo(b.score));

        // Populate the leaderboard UI with filtered entries
        foreach (var entry in filteredEntries)
        {
            GameObject newEntry = Instantiate(entryPrefab, leaderboardContainer);

            Transform nameTransform = newEntry.transform.Find("Name");
            Transform scoreTransform = newEntry.transform.Find("Score");

            if (nameTransform != null && scoreTransform != null)
            {
                TextMeshProUGUI nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI scoreText = scoreTransform.GetComponent<TextMeshProUGUI>();

                if (nameText != null && scoreText != null)
                {
                    nameText.text = entry.username;
                    scoreText.text = entry.score.ToString();
                }
            }
        }
    }
}

/// <summary>
/// Represents a single leaderboard entry containing a player's name, score, and grid size.
/// </summary>
[System.Serializable]
public class LeaderboardEntry
{
    public string username; // Player's name
    public int score; // Player's score
    public string gridSize; // The grid size the score belongs to

    public LeaderboardEntry(string name, int score, string gridSize)
    {
        this.username = name;
        this.score = score;
        this.gridSize = gridSize;
    }
}
