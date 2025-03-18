using UnityEngine;
using System.Collections;

public class Card : MonoBehaviour
{
    private bool isFlipped = false; // Tracks whether the card is flipped
    private bool isAnimating = false; // Prevents multiple flips at the same time
    private Quaternion defaultRotation; // Stores the initial rotation of the card
    private Quaternion flippedRotation; // Stores the target rotation when flipped
    private MeshRenderer meshRenderer; // Reference to the card's mesh renderer
    private GameManager gameManager; // Reference to the single-player game manager
    private MultiplayerGameManager multiplayergameManager; // Reference to the multiplayer game manager
    private Material material; // Stores the card's material
    private Color startColor; // Stores the initial color of the card

    void Start()
    {
        // Get the material and store its original color
        material = GetComponent<MeshRenderer>().material;
        startColor = material.color;

        // Store the default rotation of the card
        defaultRotation = transform.rotation;

        // Set the flipped rotation (180 degrees on the Y-axis)
        flippedRotation = Quaternion.Euler(0, 180, 0) * defaultRotation;

        // Get the MeshRenderer component
        meshRenderer = GetComponent<MeshRenderer>();

        // Find the GameManager instance in the scene (for single-player mode)
        gameManager = FindObjectOfType<GameManager>();

        // Find the MultiplayerGameManager instance in the scene (for multiplayer mode)
        multiplayergameManager = FindObjectOfType<MultiplayerGameManager>();
    }

    void OnMouseDown()
    {
        // Flip the card only if it's not already flipped or animating
        if (!isAnimating && !isFlipped)
        {
            StartCoroutine(FlipCard());

            // Notify the appropriate game manager about the flip
            if (gameManager != null)
            {
                gameManager.CardFlipped(this);
            }
            else if (multiplayergameManager != null)
            {
                multiplayergameManager.CardFlipped(this);
            }
        }
    }

    IEnumerator FlipCard()
    {
        // Play a random flip sound effect
        AudioManager.Instance.PlaySFX(Random.Range(0, 3));

        isAnimating = true; // Prevent further interactions during the animation
        isFlipped = !isFlipped; // Toggle the flipped state

        Quaternion startRotation = transform.rotation; // Store the initial rotation
        Quaternion targetRotation = isFlipped ? flippedRotation : defaultRotation; // Set the target rotation

        float duration = 0.3f; // Duration of the flip animation
        float time = 0;

        // Smoothly rotate the card
        while (time < duration)
        {
            float t = Mathf.Clamp01(time / duration); // Ensure smooth interpolation
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
            time += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // Ensure the final rotation is set correctly
        isAnimating = false; // Allow further interactions
    }

    // Returns the name of the first material applied to the card
    public string GetMaterial0Name()
    {
        if (meshRenderer != null && meshRenderer.materials.Length > 0)
        {
            return meshRenderer.materials[0].name.Replace(" (Instance)", ""); // Removes instance suffix
        }
        return ""; // Return an empty string if no material is found
    }

    // Hides the card after a delay when a match is found
    public void HideCard()
    {
        StartCoroutine(FadeOutAndDestroy());
    }

    IEnumerator FadeOutAndDestroy()
    {
        float fadeDuration = 1f; // Time in seconds
        float elapsedTime = 0f;

        // Gradually fade out the card's material color
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            material.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        // Ensure the material is fully transparent before hiding the card
        material.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        gameObject.SetActive(false); // Hides the card after fading
    }

    // Resets the card to its default state if it's flipped
    public void ResetCard()
    {
        if (isFlipped)
        {
            StartCoroutine(FlipCard());
        }
    }
}
