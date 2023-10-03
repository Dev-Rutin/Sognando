using UnityEngine;

public class StageDataController : MonoBehaviour
{
    public int data;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
