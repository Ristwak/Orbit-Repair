// File: LeverToSceneLoader.cs
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.SceneManagement;

public class LeverToSceneLoader : MonoBehaviour
{
    [Header("XR / Handle (no rigidbody needed)")]
    public XRSimpleInteractable xrInteractable;    // Put on the clickable handle
    public Transform leverPivot;                   // Rotates visually
    public Vector3 rotateAxis = Vector3.right;     // Local axis to rotate around
    [Range(0, 180f)] public float fullDownAngle = 80f;
    public float rotateSpeedDegPerSec = 180f;      // Pull speed while held
    public float downTolerance = 5f;               // How close counts as "down"

    [Header("Scene Loading")]
    public string sceneToLoad = "ExteriorSpace";
    public LoadingScreen loadingScreen;

    [Header("Mouse Testing")]
    public bool enableMouseTest = true;

    // --- internal state ---
    float startAngle;
    float targetDownAngle;
    bool fired = false;
    bool leverUnlocked = false;
    bool isSelected = false;

    void Awake()
    {
        if (!leverPivot) leverPivot = transform;
        if (!xrInteractable) xrInteractable = GetComponent<XRSimpleInteractable>();

        startAngle      = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);
        targetDownAngle = Mathf.Repeat(startAngle + fullDownAngle, 360f);

        // start locked
        if (xrInteractable) xrInteractable.enabled = false;
    }

    void OnEnable()
    {
        if (xrInteractable)
        {
            xrInteractable.selectEntered.AddListener(OnSelectEntered);
            xrInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDisable()
    {
        if (xrInteractable)
        {
            xrInteractable.selectEntered.RemoveListener(OnSelectEntered);
            xrInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void Update()
    {
        if (fired || !leverUnlocked) return;

        // While selected: go to DOWN; otherwise: relax to START
        float current = GetAxisAngle(leverPivot.localEulerAngles, rotateAxis);
        float target  = isSelected ? targetDownAngle : startAngle;

        if (Mathf.Abs(Mathf.DeltaAngle(current, target)) > 0.1f)
        {
            float next = Mathf.MoveTowardsAngle(current, target, rotateSpeedDegPerSec * Time.deltaTime);
            Vector3 e = leverPivot.localEulerAngles;
            ApplyAxisAngle(ref e, rotateAxis, next);
            leverPivot.localEulerAngles = e;
            current = next;
        }

        // Fire once lever reaches bottom
        if (Mathf.Abs(Mathf.DeltaAngle(current, targetDownAngle)) <= downTolerance)
        {
            Fire();
        }

        // Mouse quick test (click handle)
        if (enableMouseTest && Input.GetMouseButtonDown(0))
        {
            var cam = Camera.main;
            if (cam && Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out var hit, 100f, ~0, QueryTriggerInteraction.Collide))
            {
                if (hit.collider && (hit.collider.transform == transform || hit.collider.transform.IsChildOf(transform)))
                    isSelected = true;
            }
        }
        if (enableMouseTest && Input.GetMouseButtonUp(0)) isSelected = false;
    }

    void OnSelectEntered(SelectEnterEventArgs _)
    {
        if (!leverUnlocked || fired) return;
        isSelected = true; // works for ray and direct interactor
    }

    void OnSelectExited(SelectExitEventArgs _)
    {
        isSelected = false;
    }

    public void UnlockLever()
    {
        leverUnlocked = true;
        if (xrInteractable) xrInteractable.enabled = true;
        Debug.Log("[LeverToSceneLoader] Lever unlocked.");
    }

    void Fire()
    {
        if (fired) return;
        fired = true;

        OrbitRepairSequenceDirector.Instance?.NotifyLeverPulled();
        Debug.Log("[LeverToSceneLoader] Down. Loading: " + sceneToLoad);

        if (loadingScreen)
        {
            if (!loadingScreen.gameObject.activeSelf) loadingScreen.gameObject.SetActive(true);
            loadingScreen.BeginLoad(sceneToLoad);
        }
        else
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // --- angle helpers ---
    float GetAxisAngle(Vector3 euler, Vector3 axis)
    {
        axis = axis.normalized;
        if (axis == Vector3.right) return euler.x;
        if (axis == Vector3.up)    return euler.y;
        return euler.z;
    }
    void ApplyAxisAngle(ref Vector3 euler, Vector3 axis, float angle)
    {
        axis = axis.normalized;
        if (axis == Vector3.right)      euler.x = angle;
        else if (axis == Vector3.up)    euler.y = angle;
        else                            euler.z = angle;
    }
}
