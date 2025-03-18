using UnityEngine;

/// <summary>
/// Manages audio playback in the game, using a singleton pattern.
/// Supports playing sound effects (SFX) by index, name, or randomly.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; // Singleton instance of AudioManager

    public AudioSource audioSource; // Reference to the AudioSource component (assign in Inspector)
    public AudioClip[] sfxList; // Array to store sound effects (assign in Inspector)

    private void Awake()
    {
        // Singleton pattern: Ensure only one instance of AudioManager exists
        if (Instance == null)
        {
            Instance = this; // Set the instance to this script
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }
    

    /// <summary>
    /// Plays a specific sound effect using an index.
    /// </summary>
    public void PlaySFX(int index)
    {
        // Ensure the provided index is within valid bounds
        if (index >= 0 && index < sfxList.Length)
        {
            audioSource.PlayOneShot(sfxList[index]); // Play the selected SFX
        }
        else
        {
            Debug.LogWarning("Invalid SFX index!"); // Log a warning if the index is out of range
        }
    }

    /// <summary>
    /// Plays a specific sound effect using its name.
    /// </summary>
    public void PlaySFX(string sfxName)
    {
        // Iterate through the SFX list to find a matching name
        foreach (var clip in sfxList)
        {
            if (clip.name == sfxName) // Check if the name matches
            {
                audioSource.PlayOneShot(clip); // Play the matching SFX
                return;
            }
        }
        Debug.LogWarning($"SFX '{sfxName}' not found!"); // Log a warning if no matching SFX is found
    }
}
