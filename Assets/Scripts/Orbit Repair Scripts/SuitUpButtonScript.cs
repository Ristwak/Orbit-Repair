using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class SuitUpButton : MonoBehaviour
{
    [Header("Button Interactivity")]
    public XRBaseInteractable buttonInteractable;  // XR button component
    public Vector3 pressedPosition = new Vector3(0f, -0.05f, 0f);  // Downward movement
    public float pressDuration = 0.1f;  // Time to move down/up

    [Header("Suiting Up")]
    public GameObject[] suitParts;  // All suit parts to activate
    public GameObject helmetHudOverlay;  // Optional HUD overlay for helmet

    public LeverToSceneLoader leverToUnlock; // Reference to the lever script

    private Vector3 originalPosition;
    private bool isPressed = false;

    void Awake()
    {
        if (!buttonInteractable) buttonInteractable = GetComponent<XRBaseInteractable>();
        if (buttonInteractable)
        {
            buttonInteractable.selectEntered.AddListener(OnButtonPressed);
        }

        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // ✅ Mouse press simulation (for PC testing)
        if (Input.GetMouseButtonDown(0))  // Left click
        {
            // Raycast from camera center to simulate "pressing" button
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))  // Adjust distance as needed
            {
                if (hit.collider.gameObject == gameObject && !isPressed)
                {
                    OnButtonPressed(null);
                }
            }
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs interactor)
    {
        if (isPressed) return;

        isPressed = true;
        StartCoroutine(ButtonPressAnimation());
        SuitUpAll();
    }

    private System.Collections.IEnumerator ButtonPressAnimation()
    {
        Vector3 targetPosition = originalPosition + pressedPosition;
        float elapsedTime = 0f;

        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = targetPosition;
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;
        while (elapsedTime < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, elapsedTime / pressDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
        isPressed = false;
    }

    private void SuitUpAll()
    {
        foreach (var part in suitParts)
        {
            if (part != null)
                part.SetActive(true);
        }

        if (helmetHudOverlay != null)
            helmetHudOverlay.SetActive(true);

        // Unlock the lever after the suit-up process
        leverToUnlock.UnlockLever();  // Unlock lever after tool is picked
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.GrabTool);
        ToolPickupEquipper.UnlockAllPickups();  // ✅ allow tool pickup after suit-up
        Debug.Log("[SuitUpButton] Suit-up sequence complete!");
        OrbitRepairSequenceDirector.Instance?.NotifySuitUpPressed();
    }
}
