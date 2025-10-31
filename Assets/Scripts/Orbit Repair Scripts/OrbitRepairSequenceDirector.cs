// File: OrbitRepairSequenceDirector.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[DefaultExecutionOrder(-200)]
public class OrbitRepairSequenceDirector : MonoBehaviour
{
    public static OrbitRepairSequenceDirector Instance { get; private set; }

    public enum Step { SuitUpButton, PickTool, PullLever, Exterior, Done }
    [SerializeField] private Step step = Step.SuitUpButton;

    [Header("References (Interior)")]
    [Tooltip("Suit-up button root (parent object). Enabled ONLY in step: SuitUpButton.")]
    public GameObject suitUpButtonRoot;
    public XRBaseInteractable suitUpButtonInteractable; // XR ray/direct select
    public Collider suitUpButtonCollider;

    [Space(6)]
    [Tooltip("World tool on table. Enabled ONLY in step: PickTool.")]
    public GameObject worldToolRoot;
    public ToolPickupEquipper worldToolPickup;

    [Space(6)]
    [Tooltip("Lever root. Enabled ONLY in step: PullLever.")]
    public GameObject leverRoot;
    public LeverToSceneLoader leverLoader;
    public XRBaseInteractable leverInteractable;
    public Collider leverCollider;

    [Header("Options")]
    public bool autoWire = true;
    [Tooltip("Force-hide tool & lever immediately in Awake, regardless of scene defaults.")]
    public bool hardHideAtStart = true;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (autoWire)
        {
            if (suitUpButtonRoot)
            {
                if (!suitUpButtonInteractable) suitUpButtonInteractable = suitUpButtonRoot.GetComponent<XRBaseInteractable>();
                if (!suitUpButtonCollider)     suitUpButtonCollider     = suitUpButtonRoot.GetComponent<Collider>();
            }
            if (worldToolRoot && !worldToolPickup) worldToolPickup = worldToolRoot.GetComponent<ToolPickupEquipper>();
            if (leverRoot)
            {
                if (!leverLoader)       leverLoader       = leverRoot.GetComponent<LeverToSceneLoader>();
                if (!leverInteractable) leverInteractable = leverRoot.GetComponent<XRBaseInteractable>();
                if (!leverCollider)     leverCollider     = leverRoot.GetComponent<Collider>();
            }
        }

        if (hardHideAtStart)
        {
            SafeSetActive(worldToolRoot, false);
            SafeSetActive(leverRoot, false);
            if (leverLoader)       leverLoader.enabled = false;
            if (leverInteractable) leverInteractable.enabled = false;
            if (leverCollider)     leverCollider.enabled = false;
        }

        ApplyStepGates(); // do it NOW
        Debug.Log($"[SeqDirector] Awake -> step={step}. Tool active? {IsActive(worldToolRoot)}. Lever active? {IsActive(leverRoot)}.");
    }

    void OnEnable() => ApplyStepGates();

    // --------- Notifications ---------
    public void NotifySuitUpPressed()
    {
        if (step != Step.SuitUpButton) return;
        step = Step.PickTool;

        EnableButton(false);
        SafeSetActive(worldToolRoot, true);
        ToolPickupEquipper.UnlockAllPickups();

        ApplyStepGates();
        Debug.Log("[SeqDirector] Suit-up complete -> PickTool enabled.");
    }

    public void NotifyToolPicked()
    {
        if (step != Step.PickTool) return;
        step = Step.PullLever;

        SafeSetActive(worldToolRoot, false);
        EnableLever(true);

        ApplyStepGates();
        Debug.Log("[SeqDirector] Tool picked -> Lever enabled.");
    }

    public void NotifyLeverPulled()
    {
        if (step != Step.PullLever) return;
        step = Step.Exterior;

        EnableLever(false); // prevent re-use during load
        Debug.Log("[SeqDirector] Lever pulled -> Loading exterior (Lever disabled).");
    }

    // --------- Gating core ---------
    private void ApplyStepGates()
    {
        switch (step)
        {
            case Step.SuitUpButton:
                EnableButton(true);
                SafeSetActive(worldToolRoot, false);
                EnableLever(false);
                ToolPickupEquipper.LockAllPickups();
                break;

            case Step.PickTool:
                EnableButton(false);
                SafeSetActive(worldToolRoot, true);
                EnableLever(false);
                ToolPickupEquipper.UnlockAllPickups();
                break;

            case Step.PullLever:
                EnableButton(false);
                SafeSetActive(worldToolRoot, false);
                EnableLever(true);
                break;

            default:
                EnableButton(false);
                SafeSetActive(worldToolRoot, false);
                EnableLever(false);
                break;
        }
    }

    private void EnableButton(bool on)
    {
        SafeSetActive(suitUpButtonRoot, on);
        if (suitUpButtonInteractable) suitUpButtonInteractable.enabled = on;
        if (suitUpButtonCollider)     suitUpButtonCollider.enabled     = on;
        Debug.Log($"[SeqDirector] Button {(on ? "ENABLED" : "DISABLED")}");
    }

    private void EnableLever(bool on)
    {
        SafeSetActive(leverRoot, on);
        if (leverLoader)       leverLoader.enabled       = on;
        if (leverInteractable) leverInteractable.enabled = on;
        if (leverCollider)     leverCollider.enabled     = on;
        Debug.Log($"[SeqDirector] Lever {(on ? "ENABLED" : "DISABLED")}");
    }

    private static void SafeSetActive(GameObject go, bool on)
    {
        if (go && go.activeSelf != on) go.SetActive(on);
    }
    private static bool IsActive(GameObject go) => go && go.activeInHierarchy;
}
