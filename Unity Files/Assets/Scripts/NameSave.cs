using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class OpeningSceneManager : MonoBehaviour
{
    public TMP_InputField nameInput; // Reference to the input field where the player enters their name
    public TMP_Dropdown gridDropdown; // Dropdown for selecting grid size

    /// <summary>
    /// Called when the "Start Game" button is pressed.
    /// Stores the player's name and selected grid size, then loads the corresponding game scene.
    /// </summary>
    public void StartGame()
    {
        // Retrieve player name from the input field
        string playerName = nameInput.text;

        // Get the selected grid size from the dropdown menu
        string selectedGrid = gridDropdown.options[gridDropdown.value].text;

        // Construct the scene name dynamically based on grid size
        string sceneName = selectedGrid + " Single Player";

        // If the player entered a name, store it along with the selected grid size
        if (!string.IsNullOrEmpty(playerName))
        {
            PlayerPrefs.SetString("PlayerName", playerName); // Save player name
            PlayerPrefs.SetString("GridSize", selectedGrid); // Save selected grid size
            PlayerPrefs.Save(); // Persist data across scenes
        }

        // Load the corresponding game scene
        SceneManager.LoadScene(sceneName);
    }
}
