using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InGameSideData_s:MonoBehaviour,IScript // side data
{
    [Header("scripts")]
    private ScriptManager_s _scripts;
    [Header("main system reference data")]
    [SerializeField]private RectTransform divideRectSizeTsf;

    [SerializeField] private Vector2Int m_divideSize;
    public Vector2Int divideSize { get => m_divideSize; }
    public SideData[,] sideDatas {get; set;}  //x,y

    public void ScriptBind(ScriptManager_s script)
    {
        _scripts = script;
    }
    private void Start()
    {
        sideDatas = new SideData[divideSize.x, divideSize.y];
        float xChange = divideRectSizeTsf.rect.width / divideSize.x;
        float yChange = divideRectSizeTsf.rect.height / divideSize.y;
        Vector2 CubePivot = new Vector2(divideRectSizeTsf.rect.width / -2, divideRectSizeTsf.rect.height / 2);
        for (int i = 0; i < divideSize.x; i++)
        {
            for (int j = 0; j < divideSize.y; j++)
            {
                sideDatas[i, j] = new SideData(new Vector2(i, j), new Vector2((CubePivot.x + xChange / 2) + i * xChange, (CubePivot.y - yChange / 2) - j * yChange));
            }
        }
    }
    public void InGameBind()
    {
        _scripts._inGamefunBind_s.EgameEnd+=GameEnd;
    }
    public void GameEnd()
    {
        for (int i = 0; i < divideSize.x; i++)
        {
            for (int j = 0; j < divideSize.y; j++)
            {
                //sideDatas[i, j].Clear();
            }
        }
    }
}

