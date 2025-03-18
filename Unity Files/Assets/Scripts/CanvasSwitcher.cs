using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CanvasSwitcher : MonoBehaviour
{
    public CanvasGroup currentCanvas;
    public CanvasGroup newCanvas;
    /// <summary>
    /// Switches a Menu with another menu
    /// </summary>
    public void SwitchCanvas()
    {
        StartCoroutine(InstantSwitch(currentCanvas, newCanvas));
    }

    private IEnumerator InstantSwitch(CanvasGroup from, CanvasGroup to)
    {
        // Disable the current canvas
        from.alpha = 0;
        from.interactable = false;
        from.blocksRaycasts = false;
        from.gameObject.SetActive(false);

        

        // Enable the new canvas
        to.gameObject.SetActive(true);
        to.alpha = 1;
        to.interactable = true;
        to.blocksRaycasts = true;
        // Wait for one frame to ensure UI updates properly
        yield return null;
    }
}
