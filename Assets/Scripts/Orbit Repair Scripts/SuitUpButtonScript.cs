using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SuitUpButton : MonoBehaviour
{
    [Header("Button Interactivity")]
    public XRBaseInteractable buttonInteractable;
    public Vector3 pressedPosition = new Vector3(0f, -0.05f, 0f);
    public float pressDuration = 0.1f;

    [Header("Suiting Up")]
    public GameObject[] suitParts;
    public GameObject helmetHudOverlay;

    [Header("What should appear after suit-up")]
    [Tooltip("Table tool (e.g., screwdriver). Keep INACTIVE at scene start.")]
    public GameObject worldToolRoot;

    private Vector3 originalPosition;
    private bool isPressed = false;

    void Awake()
    {
        if (!buttonInteractable) buttonInteractable = GetComponent<XRBaseInteractable>();
        if (buttonInteractable) buttonInteractable.selectEntered.AddListener(OnButtonPressed);
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        // Mouse test in Editor/PC
        if (Input.GetMouseButtonDown(0) && !isPressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.collider && hit.collider.gameObject == gameObject)
                    OnButtonPressed(null);
            }
        }
    }

    private void OnButtonPressed(SelectEnterEventArgs _)
    {
        if (isPressed) return;
        isPressed = true;

        StartCoroutine(ButtonPressAnimation());
        SuitUpAll();
    }

    private System.Collections.IEnumerator ButtonPressAnimation()
    {
        Vector3 target = originalPosition + pressedPosition;
        float t = 0f;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(originalPosition, target, t / pressDuration);
            t += Time.deltaTime; yield return null;
        }
        transform.localPosition = target;
        yield return new WaitForSeconds(0.2f);

        t = 0f;
        while (t < pressDuration)
        {
            transform.localPosition = Vector3.Lerp(target, originalPosition, t / pressDuration);
            t += Time.deltaTime; yield return null;
        }
        transform.localPosition = originalPosition;
        isPressed = false;

        // Lock button forever after first use
        if (buttonInteractable) buttonInteractable.enabled = false;
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;
    }

    private void SuitUpAll()
    {
        // Show suit visuals
        foreach (var part in suitParts) if (part) part.SetActive(true);
        if (helmetHudOverlay) helmetHudOverlay.SetActive(true);

        // ✅ Show the table tool now (lever stays hidden)
        if (worldToolRoot) worldToolRoot.SetActive(true);

        // Allow tool pickup after suit-up
        ToolPickupEquipper.UnlockAllPickups();

        // Advance phase
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.GrabTool);
        OrbitRepairSequenceDirector.Instance?.NotifySuitUpPressed();

        Debug.Log("[SuitUpButton] Suit-up done → Tool enabled. Lever still locked.");
    }
}
