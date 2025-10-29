// File: ToolPickupEquipper.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolPickupEquipper : MonoBehaviour
{
    // ---- Global lock/unlock (default: locked) ----
    private static bool pickupsUnlocked = false;
    public static void UnlockAllPickups()  { pickupsUnlocked = true; }
    public static void LockAllPickups()    { pickupsUnlocked = false; }

    [Header("Who can trigger pickup")]
    [Tooltip("Leave empty to accept any collider, or set to 'Player' / 'Hand' etc.")]
    public string requiredTag = "Player";

    [Header("Equip on pickup")]
    [Tooltip("In-hand tool (disabled at start) parented to player's hand.")]
    public GameObject equippedTool;
    public bool destroyWorldTool = true;

    [Header("Optional SFX")]
    public AudioSource sfxSource;
    public AudioClip pickupSfx;

    [Header("XR (optional)")]
    [Tooltip("If the world tool has XRBaseInteractable, pickup will also trigger on grab.")]
    public XRBaseInteractable xrInteractable;

    private bool done = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!xrInteractable) xrInteractable = GetComponent<XRBaseInteractable>();
        if (equippedTool && equippedTool.activeSelf)
            equippedTool.SetActive(false); // keep hidden until suit-up
    }

    void OnEnable()
    {
        if (xrInteractable)
            xrInteractable.selectEntered.AddListener(OnGrabbed);
    }

    void OnDisable()
    {
        if (xrInteractable)
            xrInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs _) => TryEquip();

    private void OnTriggerEnter(Collider other)
    {
        if (done) return;

        if (string.IsNullOrEmpty(requiredTag) || other.CompareTag(requiredTag) || other.CompareTag("Hand"))
            TryEquip();
    }

    private void TryEquip()
    {
        if (done) return;

        // 🔒 Block pickup until Suit Up has unlocked it
        if (!pickupsUnlocked) return;

        done = true;

        if (equippedTool) equippedTool.SetActive(true);
        OrbitRepairSequenceDirector.Instance?.NotifyToolPicked();

        // Advance phase: GrabTool -> ExitToSpace
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.ExitToSpace);

        // Narration
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.AfterToolPickup);
            // Or: AudioManager.instance.PlayNarration(AudioManager.instance.afterToolPickupClip);
        }

        if (sfxSource && pickupSfx) sfxSource.PlayOneShot(pickupSfx);

        if (destroyWorldTool) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
