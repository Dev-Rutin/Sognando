using UnityEngine;

public class StageDataController : MonoBehaviour
{
    public int stage;
    public int score;
    public int maxCombo;
    public int perfectCount;
    public int goodCount;
    public int missCount;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
