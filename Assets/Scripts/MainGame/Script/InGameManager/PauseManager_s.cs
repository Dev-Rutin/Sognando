using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager_s : MonoBehaviour
{
    public GameObject cube;
    public void ReTry()
    {
        InGameManager_s.Instance.GameUnPuase();
    }

    public void ReStart()
    {
        cube.SetActive(false);
        SceneManager.LoadScene("GameScene");
    }
    public void Exit()
    {
        StartCoroutine(InGameManager_s.Instance.FadeStart("LobbyScene"));
    }
}
