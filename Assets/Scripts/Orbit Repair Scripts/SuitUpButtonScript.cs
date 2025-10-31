using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class SuitUpButton : MonoBehaviour
{
    [Header("Button Interactivity")]
    public XRBaseInteractable buttonInteractable;          // XR (ray/direct)
    public Vector3 pressedPosition = new Vector3(0f, -0.05f, 0f);
    public float pressDuration = 0.1f;

    [Header("Hand Touch Press (optional)")]
    [Tooltip("If true, a collider tagged 'Hand' touching this button will press it once.")]
    public bool allowHandTouchPress = true;
    [Tooltip("Tags that can press by touch (add your hand/controller tags here).")]
    public string[] handPressTags = new[] { "Hand", "LeftHand", "RightHand" };

    [Header("Suiting Up")]
    public GameObject[] suitParts;
    public GameObject helmetHudOverlay;

    [Header("What should appear after suit-up")]
    [Tooltip("Table tool (e.g., screwdriver). Keep INACTIVE at scene start.")]
    public GameObject worldToolRoot;

    private Vector3 originalPosition;
    private bool isPressedAnimating = false;
    private bool usedOnce = false;

    void Awake()
    {
        // Collider for hand-touch detection
        var col = GetComponent<Collider>();
        col.isTrigger = true; // so hand touch doesn’t push it physically

        if (!buttonInteractable) buttonInteractable = GetComponent<XRBaseInteractable>();
        if (buttonInteractable) buttonInteractable.selectEntered.AddListener(OnXRPressed);

        originalPosition = transform.localPosition;
    }

    void OnDestroy()
    {
        if (buttonInteractable) buttonInteractable.selectEntered.RemoveListener(OnXRPressed);
    }

    // XR press via ray/direct
    private void OnXRPressed(SelectEnterEventArgs _)
    {
        TryPressOnce();
    }

    // Hand touch press
    void OnTriggerEnter(Collider other)
    {
        if (!allowHandTouchPress || usedOnce) return;
        if (!other) return;

        // Accept any of the allowed tags
        for (int i = 0; i < handPressTags.Length; i++)
        {
            if (other.CompareTag(handPressTags[i]))
            {
                TryPressOnce();
                return;
            }
        }
    }

    // Mouse test in Editor/PC
    void Update()
    {
        if (usedOnce || isPressedAnimating) return;

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main ? Camera.main.ScreenPointToRay(Input.mousePosition) : new Ray(Vector3.zero, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                if (hit.collider && (hit.collider.gameObject == gameObject))
                    TryPressOnce();
            }
        }
    }

    private void TryPressOnce()
    {
        if (usedOnce || isPressedAnimating) return;
        usedOnce = true;

        StartCoroutine(ButtonPressAnimation());
        DoSuitUpEffects();
    }

    private System.Collections.IEnumerator ButtonPressAnimation()
    {
        isPressedAnimating = true;

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

        // Hard-lock interactivity after first use
        if (buttonInteractable) buttonInteractable.enabled = false;
        var col = GetComponent<Collider>();
        if (col) col.enabled = false;

        isPressedAnimating = false;
    }

    private void DoSuitUpEffects()
    {
        // Show suit visuals
        if (suitParts != null)
            foreach (var part in suitParts) if (part) part.SetActive(true);

        if (helmetHudOverlay) helmetHudOverlay.SetActive(true);

        // Enable world tool now (lever stays hidden here)
        if (worldToolRoot) worldToolRoot.SetActive(true);

        // Allow tool pickup after suit-up
        ToolPickupEquipper.UnlockAllPickups();

        // Advance phase + notify
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.GrabTool);
        OrbitRepairSequenceDirector.Instance?.NotifySuitUpPressed();

        Debug.Log("[SuitUpButton] Suit-up done → Tool enabled. Lever still locked.");
    }
}
