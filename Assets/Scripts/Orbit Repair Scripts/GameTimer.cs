// File: GameTimer.cs
using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    public event Action<float> OnTick;   // remaining seconds
    public event Action OnTimeUp;

    private float remaining;
    private bool running;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Optional: DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (!running) return;

        remaining -= Time.deltaTime;
        if (remaining > 0f)
        {
            OnTick?.Invoke(remaining);
        }
        else
        {
            remaining = 0f;
            running = false;
            OnTick?.Invoke(remaining);
            OnTimeUp?.Invoke();
        }
    }

    public void StartTimer(float seconds)
    {
        remaining = Mathf.Max(0f, seconds);
        running = remaining > 0f;
        OnTick?.Invoke(remaining);
        if (!running) OnTimeUp?.Invoke();
    }

    public void StopTimer()
    {
        running = false;
    }

    public void ResetTimer(float seconds)
    {
        running = false;
        remaining = Mathf.Max(0f, seconds);
        OnTick?.Invoke(remaining);
    }

    public float GetRemaining() => remaining;
    public bool IsRunning() => running;
}
