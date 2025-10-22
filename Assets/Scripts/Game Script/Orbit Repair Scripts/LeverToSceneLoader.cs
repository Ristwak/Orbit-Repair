using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;
using System.Collections;

public class LeverToSceneLoader : MonoBehaviour
{
    [Header("XR / Handle")]
    public XRGrabInteractable xrGrab;      // Optional (for VR). If missing, mouse mode still works.
    public Transform leverPivot;           // The pivot whose localEulerAngles change when pulled
    public Vector3 rotateAxis = Vector3.right; // Which local axis the lever rotates around
    public float pullAngleThreshold = 35f; // Degrees from start considered "pulled"
    public Animator leverAnimator;         // Optional: has "Pulled" trigger to play an anim

    [Header("Scene Loading")]
    public string sceneToLoad = "ExteriorSpace";  // Target scene name
    public LoadingScreen loadingScreen;            // Reference to loading UI controller

    [Header("Mouse Testing")]
    public bool enableMouseTest = true;     // Allow left-click test in Editor/PC
    public float mouseSimPullAngle = 40f;   // How far to rotate when simulating a pull
    public float mouseSimSpeed = 360f;      // deg/sec for the quick sim rotate

    private float startAngle;               // initial local angle about rotateAxis
    private bool fired = false;

    void Awake()
    {
        if (!leverPivot) leverPivot = transform;
        // Cache starting angle around chosen axis
        startAngle = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);

        if (!xrGrab) xrGrab = GetComponent<XRGrabInteractable>();
    }

    void OnEnable()
    {
        if (xrGrab)
        {
            // If you only want to evaluate when released, you could use selectExited
            xrGrab.selectExited.AddListener(_ => TryFireByAngle());
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

        // Live check (useful if player keeps holding the lever in VR)
        TryFireByAngle();

        // Mouse test path
        if (enableMouseTest && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main ? Camera.main.ScreenPointToRay(Input.mousePosition) : new Ray(Vector3.zero, Vector3.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                if (hit.collider && hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform))
                {
                    StartCoroutine(MouseSimPullThenLoad());
                }
            }
        }
    }

    void TryFireByAngle()
    {
        if (fired || leverPivot == null) return;

        float current = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);
        float delta = Mathf.Abs(Mathf.DeltaAngle(startAngle, current));

        if (delta >= pullAngleThreshold)
        {
            Fire();
        }
    }

    void Fire()
    {
        if (fired) return;
        fired = true;

        if (leverAnimator) leverAnimator.SetTrigger("Pulled");

        Debug.Log("[LeverToSceneLoader] Lever pulled. Loading scene: " + sceneToLoad);

        if (loadingScreen)
        {
            loadingScreen.BeginLoad(sceneToLoad);
        }
        else
        {
            // Fallback direct load (no UI)
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    IEnumerator MouseSimPullThenLoad()
    {
        if (leverPivot)
        {
            // Rotate the lever quickly for visual feedback
            float target = startAngle + mouseSimPullAngle;
            float current = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);

            while (Mathf.Abs(Mathf.DeltaAngle(current, target)) > 0.5f)
            {
                float step = mouseSimSpeed * Time.deltaTime;
                float next = Mathf.MoveTowardsAngle(current, target, step);

                // Apply on the chosen axis while preserving the others
                Vector3 e = leverPivot.localEulerAngles;
                ApplyAxisAngle(ref e, rotateAxis, next);
                leverPivot.localEulerAngles = e;

                current = next;
                yield return null;
            }
        }

        Fire();
    }

    // --- helpers to isolate one axis of localEulerAngles ---
    float GetAxisAngle(Vector3 euler, Vector3 axis)
    {
        axis = axis.normalized;
        if (axis == Vector3.right)   return euler.x;
        if (axis == Vector3.up)      return euler.y;
        /* axis == Vector3.forward */ return euler.z;
    }

    void ApplyAxisAngle(ref Vector3 euler, Vector3 axis, float angle)
    {
        axis = axis.normalized;
        if (axis == Vector3.right)   euler.x = angle;
        else if (axis == Vector3.up) euler.y = angle;
        else                         euler.z = angle; // forward
    }
}
