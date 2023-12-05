using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ObjectButtonController : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    private void OnMouseOver()
    {
        GetComponent<SpriteRenderer>().sprite = _sprites[1];
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().sprite = _sprites[0];
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            switch (gameObject.name)
            {
                case ObjectButtonName.GAMESTARTBUTTON:
                    LobbyManager.Instance.OpenStageSelect();
                    break;
                case ObjectButtonName.CREDITBUTTON:
                    LobbyManager.Instance.LoadScene("CreditScene");
                    break;
                case ObjectButtonName.ARCHIVEBUTTON:
                    LobbyManager.Instance.LoadScene("ArchiveScene");
                    break;
                case ObjectButtonName.SETTINGBUTTON:
                    SettingUtility.Instance.OpenSetting();
                    break;

            }
        }
    }
}
