using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Collections;

public class LeverToSceneLoader : MonoBehaviour
{
    [Header("XR / Handle")]
    public XRGrabInteractable xrGrab;          // Optional (for VR). If missing, mouse mode still works.
    public Transform leverPivot;               // Rotating part
    public Vector3 rotateAxis = Vector3.right; // Which local axis rotates
    [Tooltip("How many degrees from the start counts as FULLY DOWN (e.g., 70–90).")]
    public float fullDownAngle = 80f;          // Required pull from start to count as 'fully down'
    [Tooltip("Allowed error in degrees for 'fully down' detection.")]
    public float downTolerance = 5f;           // Slack for end position
    public Animator leverAnimator;             // Optional: trigger 'Pulled' when it’s fully down

    [Header("Scene Loading")]
    public string sceneToLoad = "ExteriorSpace";
    public LoadingScreen loadingScreen;        // Your loading UI script (Canvas with CanvasGroup)

    [Header("Mouse Testing")]
    public bool enableMouseTest = true;        // Click to simulate pull in Editor/PC
    public float mouseSimSpeed = 360f;         // deg/sec for the quick sim rotate

    private float startAngle;                  // cached initial lever angle
    private float targetDownAngle;             // startAngle + fullDownAngle (normalized)
    private bool fired = false;

    void Awake()
    {
        if (!leverPivot) leverPivot = transform;
        if (!xrGrab) xrGrab = GetComponent<XRGrabInteractable>();

        startAngle = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);
        targetDownAngle = Mathf.Repeat(startAngle + fullDownAngle, 360f);
    }

    void OnEnable()
    {
        if (xrGrab)
        {
            // Fire when player releases the lever AND it is fully down.
            xrGrab.selectExited.AddListener(_ => TryFireIfFullyDown());
        }
    }

    void OnDisable()
    {
        if (xrGrab)
        {
            xrGrab.selectExited.RemoveAllListeners();
        }
    }

    void Update()
    {
        if (fired) return;

        // If player holds it at bottom without releasing, still allow firing
        TryFireIfFullyDown();

        // Mouse test path (click lever to simulate a full pull)
        if (enableMouseTest && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main ? Camera.main.ScreenPointToRay(Input.mousePosition) : new Ray(Vector3.zero, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider && (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform)))
                {
                    StartCoroutine(MouseSimPullThenLoad());
                }
            }
        }
    }

    void TryFireIfFullyDown()
    {
        if (fired || leverPivot == null) return;

        float current = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);
        float diff = Mathf.Abs(Mathf.DeltaAngle(current, targetDownAngle));

        if (diff <= downTolerance)  // ✅ Only when truly at the bottom
        {
            Fire();
        }
    }

    void Fire()
    {
        if (fired) return;
        fired = true;

        if (leverAnimator) leverAnimator.SetTrigger("Pulled");
        Debug.Log("[LeverToSceneLoader] Lever fully down. Showing loading and opening scene: " + sceneToLoad);

        if (loadingScreen)
        {
            // ✅ Ensure the LoadingScreen GameObject is active first
            if (!loadingScreen.gameObject.activeSelf)
                loadingScreen.gameObject.SetActive(true);

            // Show loading UI first, then async-load
            loadingScreen.BeginLoad(sceneToLoad);
        }
        else
        {
            // Fallback (no UI)
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator MouseSimPullThenLoad()
    {
        if (leverPivot)
        {
            // Smoothly rotate to the exact target down angle
            float current = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);

            while (Mathf.Abs(Mathf.DeltaAngle(current, targetDownAngle)) > 0.5f)
            {
                float next = Mathf.MoveTowardsAngle(current, targetDownAngle, mouseSimSpeed * Time.deltaTime);

                Vector3 e = leverPivot.localEulerAngles;
                ApplyAxisAngle(ref e, rotateAxis, next);
                leverPivot.localEulerAngles = e;

                current = next;
                yield return null;
            }
        }

        // Once visually at the bottom, fire
        Fire();
    }

    // --- helpers to read/set one axis of localEulerAngles ---
    float GetAxisAngle(Vector3 euler, Vector3 axis)
    {
        axis = axis.normalized;
        if (axis == Vector3.right) return euler.x;
        if (axis == Vector3.up) return euler.y;
        /* axis == Vector3.forward */
        return euler.z;
    }

    void ApplyAxisAngle(ref Vector3 euler, Vector3 axis, float angle)
    {
        axis = axis.normalized;
        if (axis == Vector3.right) euler.x = angle;
        else if (axis == Vector3.up) euler.y = angle;
        else euler.z = angle;
    }
}
