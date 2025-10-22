using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class SuitUpButton : MonoBehaviour
{
    [Header("Button Interactivity")]
    public XRBaseInteractable buttonInteractable;  // The XR interactable component for the button
    public Vector3 pressedPosition = new Vector3(0f, -0.05f, 0f);  // Where the button moves when pressed (downwards)
    public float pressDuration = 0.1f;  // Time to take for the button to move down and then back up

    [Header("Suiting Up")]
    public GameObject[] suitParts;  // List of suit parts (meshes) to activate during suit-up
    public GameObject helmetHudOverlay;  // Optional: HUD overlay when the helmet is on

    private Vector3 originalPosition;  // The original position of the button (before press)
    private bool isPressed = false;  // To track the press state

    void Awake()
    {
        if (!buttonInteractable) buttonInteractable = GetComponent<XRBaseInteractable>();
        if (!buttonInteractable) 
        {
            Debug.LogError("No XRBaseInteractable component found on the button!");
            return;
        }
        
        originalPosition = transform.localPosition;  // Store the original button position

        // Correct way to add the listener
        buttonInteractable.selectEntered.AddListener(OnButtonPressed);
    }

    void OnButtonPressed(SelectEnterEventArgs interactor)
    {
        if (isPressed) return;  // Ensure we don't trigger multiple presses

        isPressed = true;

        // Start the button press animation (move down)
        StartCoroutine(ButtonPressAnimation());

        // Trigger the suit-up
        SuitUpAll();
    }

    // Coroutine to animate the button press
    private System.Collections.IEnumerator ButtonPressAnimation()
    {
        // Move the button down
        Vector3 targetPosition = originalPosition + pressedPosition;
        float elapsedTime = 0f;

        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;  // Ensure it reaches the target position

        // Wait for a small moment before resetting the button position
        yield return new WaitForSeconds(0.2f);

        // Reset the button position to its original state
        elapsedTime = 0f;
        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;  // Ensure it reaches the original position

        // Reset isPressed flag after animation
        isPressed = false;
    }

    // Method to trigger the suit-up process
    private void SuitUpAll()
    {
        // Loop through all the suit parts and activate them
        foreach (var part in suitParts)
        {
            if (part != null)
            {
                part.SetActive(true);  // Activate the suit part
            }
        }

        // Optional: Show helmet HUD overlay if you have one
        if (helmetHudOverlay != null)
        {
            helmetHudOverlay.SetActive(true);  // Show the HUD overlay
        }
    }
}
