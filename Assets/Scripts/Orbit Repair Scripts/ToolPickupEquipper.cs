using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ToolPickupEquipper : MonoBehaviour
{
    // ---- Global lock/unlock (default: locked) ----
    private static bool pickupsUnlocked = false;
    public static void UnlockAllPickups()  { pickupsUnlocked = true; }
    public static void LockAllPickups()    { pickupsUnlocked = false; }

    [Header("Who can trigger pickup")]
    public string requiredTag = "Player";

    [Header("Equip on pickup")]
    [Tooltip("In-hand tool (disabled at start) parented to player's hand.")]
    public GameObject equippedTool;
    public bool destroyWorldTool = true;

    [Header("What should appear after pickup")]
    [Tooltip("Lever root to enable only after tool pickup. Keep INACTIVE at start.")]
    public GameObject leverRootToEnable;

    [Header("Optional SFX")]
    public AudioSource sfxSource;
    public AudioClip pickupSfx;

    [Header("XR (optional)")]
    public XRSimpleInteractable xrInteractable;

    private bool done = false;

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void Awake()
    {
        if (!xrInteractable) xrInteractable = GetComponent<XRSimpleInteractable>();
        if (equippedTool && equippedTool.activeSelf)
            equippedTool.SetActive(false); // hidden until suit-up allows pickup
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
        if (!pickupsUnlocked) return; // 🔒 Only after suit-up

        done = true;

        // Activate in-hand tool
        if (equippedTool) equippedTool.SetActive(true);

        // ✅ Now reveal/enable the lever
        if (leverRootToEnable) 
        {
            leverRootToEnable.SetActive(true);

            // If lever script is on the same root, enable it too
            var leverLoader = leverRootToEnable.GetComponent<LeverToSceneLoader>();
            if (leverLoader) leverLoader.enabled = true;
        }

        // Phase & narration
        OrbitRepairSequenceDirector.Instance?.NotifyToolPicked();
        OrbitRepairGameManager.Instance?.SetPhase(OrbitRepairGameManager.Phase.ExitToSpace);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayNarrationCue(AudioManager.NarrationCue.AfterToolPickup);

        if (sfxSource && pickupSfx) sfxSource.PlayOneShot(pickupSfx);

        // Remove / hide world tool
        if (destroyWorldTool) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}
