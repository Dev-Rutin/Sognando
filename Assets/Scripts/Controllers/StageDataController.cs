using UnityEngine;

public class StageDataController : Singleton<StageDataController>
{
    public bool isClear;
    public int stage;
    public int score;
    public int maxCombo;
    public int perfectCount;
    public int goodCount;
    public int missCount;
    public bool isHardModeOn;
    //score
    public int totalValue;
    public int judgementValue;
    public float loseValuel;
    public int totalScroe;
    public float rankScore;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
