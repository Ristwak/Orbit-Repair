using UnityEngine;

public class OrbitRepairGameManager : MonoBehaviour
{
    public static OrbitRepairGameManager Instance { get; private set; }
    public enum Phase { PullLever, SuitUp, GrabTool, ExitToSpace, Repair, PowerOn, Complete }

    [Header("UI")]
    public GameObject successPanel;
    private Phase phase = Phase.PullLever;
    private bool ended = false;

    void Awake()
    {
        Instance = this;
        // No timer needed anymore
    }

    public void SetPhase(Phase p)
    {
        if (ended) return;
        phase = p;

        // Optional: Start music when game begins
        if (p == Phase.SuitUp)
        {
            if (AudioManager.instance) 
                AudioManager.instance.PlayMusic(AudioManager.instance.gameMusic);
        }
    }

    public void MissionSuccess()
    {
        if (ended) return;
        ended = true;
        SetPhase(Phase.Complete);

        // Optional audio play (check if audio exists)
        if (AudioManager.instance)
        {
            AudioManager.instance.PlayNarration(AudioManager.instance.missionCompleteClip);
            AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);
        }

        if (successPanel) successPanel.SetActive(true);
    }
}
