// File: OrbitRepairSequenceDirector.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// Controls the linear flow:
/// 1) Press Suit-Up Button  -> 2) Pick Tool  -> 3) Pull Lever -> (loads exterior)
public class OrbitRepairSequenceDirector : MonoBehaviour
{
    public static OrbitRepairSequenceDirector Instance { get; private set; }

    public enum Step { SuitUpButton, PickTool, PullLever, Exterior, Done }
    [SerializeField] private Step step = Step.SuitUpButton;

    [Header("References (Interior Scene)")]
    [Tooltip("The physical button the player presses to suit up.")]
    public GameObject suitUpButtonRoot;                    // parent of the button
    public XRBaseInteractable suitUpButtonInteractable;    // optional, if using XRI
    public Collider suitUpButtonCollider;                  // if you drive it via collider

    [Space(4)]
    [Tooltip("World tool lying on table that triggers equip on pickup.")]
    public GameObject worldToolRoot;                       // the table screwdriver (active only in PickTool step)
    public ToolPickupEquipper worldToolPickup;             // the pickup script on the world tool

    [Space(4)]
    [Tooltip("Lever to open the hatch (loads next scene).")]
    public GameObject leverRoot;
    public LeverToSceneLoader leverLoader;                 // your LeverToSceneLoader script
    public XRBaseInteractable leverInteractable;           // optional, if you use XRI events instead of angle check
    public Collider leverCollider;                         // if you gate it via collider

    [Header("Optional")]
    [Tooltip("Set true to try auto-wiring components from the assigned roots on Awake.")]
    public bool autoWire = true;

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
            if (worldToolRoot)
            {
                if (!worldToolPickup) worldToolPickup = worldToolRoot.GetComponent<ToolPickupEquipper>();
            }
            if (leverRoot)
            {
                if (!leverLoader)        leverLoader        = leverRoot.GetComponent<LeverToSceneLoader>();
                if (!leverInteractable)  leverInteractable  = leverRoot.GetComponent<XRBaseInteractable>();
                if (!leverCollider)      leverCollider      = leverRoot.GetComponent<Collider>();
            }
        }
    }

    void Start()
    {
        ApplyStepGates();
    }

    // ---- Public notifications called by your existing scripts ----

    public void NotifySuitUpPressed()
    {
        if (step != Step.SuitUpButton) return;
        step = Step.PickTool;

        // Permanently disable the button
        EnableButton(false);

        // Allow tool pickup and show the world tool
        if (worldToolRoot) worldToolRoot.SetActive(true);
        ToolPickupEquipper.UnlockAllPickups();

        ApplyStepGates();
        Debug.Log("[SeqDirector] Suit-up complete -> PickTool enabled.");
    }

    public void NotifyToolPicked()
    {
        if (step != Step.PickTool) return;
        step = Step.PullLever;

        // Hide world tool if still present (safety)
        if (worldToolRoot) worldToolRoot.SetActive(false);

        // Now enable lever
        EnableLever(true);

        ApplyStepGates();
        Debug.Log("[SeqDirector] Tool picked -> Lever enabled.");
    }

    public void NotifyLeverPulled()
    {
        if (step != Step.PullLever) return;
        step = Step.Exterior;

        // Optional: disable lever so it can't be re-used while loading
        EnableLever(false);

        Debug.Log("[SeqDirector] Lever pulled -> Loading exterior.");
        // LeverToSceneLoader handles the scene load; nothing else here
    }

    // ---- Gating helpers ----
    private void ApplyStepGates()
    {
        switch (step)
        {
            case Step.SuitUpButton:
                // Only button active
                EnableButton(true);
                if (worldToolRoot) worldToolRoot.SetActive(false);
                EnableLever(false);
                ToolPickupEquipper.LockAllPickups();
                break;

            case Step.PickTool:
                EnableButton(false);
                if (worldToolRoot) worldToolRoot.SetActive(true);
                EnableLever(false);
                ToolPickupEquipper.UnlockAllPickups();
                break;

            case Step.PullLever:
                EnableButton(false);
                if (worldToolRoot) worldToolRoot.SetActive(false);
                EnableLever(true);
                break;

            case Step.Exterior:
            case Step.Done:
                // nothing in interior
                EnableButton(false);
                if (worldToolRoot) worldToolRoot.SetActive(false);
                EnableLever(false);
                break;
        }
    }

    private void EnableButton(bool on)
    {
        if (suitUpButtonRoot) suitUpButtonRoot.SetActive(on);
        if (suitUpButtonInteractable) suitUpButtonInteractable.enabled = on;
        if (suitUpButtonCollider) suitUpButtonCollider.enabled = on;
    }

    private void EnableLever(bool on)
    {
        if (leverRoot) leverRoot.SetActive(on);
        if (leverInteractable) leverInteractable.enabled = on;
        if (leverCollider) leverCollider.enabled = on;

        // If your LeverToSceneLoader should be inert when off:
        if (leverLoader) leverLoader.enabled = on;
    }
}
