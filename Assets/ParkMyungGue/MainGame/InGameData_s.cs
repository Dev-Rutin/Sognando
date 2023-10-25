using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public struct SideData
{
    public Vector2 position;
    public Vector2 transform;
    public GameObject coin;
    public int coincount;
    public GameObject fire;
    public GameObject path;
    public GameObject wall;
    public GameObject lineAttack;
    public GameObject linkLineAttack;
    public SideData(Vector2 _position, Vector2 _transform)
    {
        position = _position;
        transform = _transform;
        coin = null;
        fire = null;
        path = null;
        wall = null;
        lineAttack = null;
        linkLineAttack = null;
        coincount = 0;
    }
    public bool isCanMakeCheck(bool isStack, string dataName)
    {
        if (this.GetType().GetField(dataName).GetValue(this) != null)
        {
            return false;
        }
        if (isStack)
        {
            if (fire != null)
            {
                return false;
            }
            if (wall != null)
            {
                return false;
            }
        }
        else
        {
            if (coin != null)
            {
                return false;
            }
            if (fire != null)
            {
                return false;
            }
            if (path != null)
            {
                return false;
            }
            if (wall != null)
            {
                return false;
            }
            if (lineAttack != null)
            {
                return false;
            }
            if (linkLineAttack != null)
            {
                return false;
            }
        }
        return true;
    }
}
public partial class InGameData_s:MonoBehaviour,IDataSetting // data는 게임이 실행되기 전에 초기화 및 할당되는 값을 보관함
{
    [Header("main system reference data")]
    public Vector2Int divideSize;
    public Transform divideRectSizeTsf;
    public SideData[,] sideDatas;  //x,y
    public Transform buttonsTsf;
    public Transform bgmSlider;
    public Transform ShowTextTsf;
    public Transform scoreTsf;
    public Transform comboTsf;
    public EStage curStage;
    public Transform beatTsf;
    public float beatEndScaleX;
    public float beatEndScaleY;
    public float beatJudgeMax;
    public float beatJudgeMin;
    public float beatJudgeExtra;
    public int bpm;
    public float movingTime;
    public float animationTime;
    public GameObject beatObj;
    [Header("player reference data")]
    public GameObject inGamePlayerObj;
    public GameObject playerImageObj;
    public Transform playerHPOffTsf;
    public GameObject playerAttackEffectObj;
    public int playerMaxHP;
    [Header("cube reference data")]
    public GameObject gameCubeObj;
    public Material cubeMaterial;
    public Material cubeSliceMaterial;
    public GameObject cubeEffectObj;
    public Transform rotateImageTsf;
    [Header("enemy reference data")]
    public Transform enemyHPTsf;
    public Transform enemyATKOnTsf;
    public GameObject enemyImageObj;
    public GameObject enemyHitEffectObj;
    public int enemyMaxHP;
    public int enemyAttackGaugeMax;
    [Header("enemy reference data(enemy attack)")]
    public Transform movePathTsf;
    public GameObject pathSampleObj;
    public Transform coinsTsf;
    public GameObject coinSampleObj;
    public Transform fireTsf;
    public GameObject fireSampleObj;
    public Transform blockTsf;
    public GameObject blockSampleObj;
    public Transform lineAttackTsf;
    public GameObject lineAttackSampleObj;
    public Transform linkLineAttackTsf;
    public GameObject linkLineAttackSampleObj;
    public GameObject needleAttackSampleObj;
    public Transform needleAttackTsf;
    public GameObject ghostSampleObj;
    public Transform ghostTsf;
    [Header("music reference data")]
    public AudioClip musicClip;
    public void DefaultDataSetting()
    {
        divideSize = divideSize == Vector2Int.zero ? new Vector2Int(4, 4) : divideSize;
        sideDatas = new SideData[divideSize.x, divideSize.y];
        float xChange = divideRectSizeTsf.GetComponent<RectTransform>().rect.width / divideSize.x;
        float yChange = divideRectSizeTsf.GetComponent<RectTransform>().rect.height / divideSize.y;
        for (int i = 0; i < divideSize.x; i++)
        {
            for (int j = 0; j < divideSize.y; j++)
            {
                sideDatas[i, j] = new SideData(new Vector2(i, j), new Vector2(-1 * (xChange + inGamePlayerObj.GetComponent<RectTransform>().rect.width) - 6 + xChange * i, yChange + inGamePlayerObj.GetComponent<RectTransform>().rect.height + 6 - yChange * j));
            }
        }
        beatEndScaleX = beatEndScaleX==0?0.95f:beatEndScaleX;
        beatEndScaleY = beatEndScaleY ==0?0.95f:beatEndScaleY;
        beatJudgeMax = beatJudgeMax == 0 ? 0.3f : beatJudgeMax;
        beatJudgeMin = beatJudgeMin == 0 ? 0.8f : beatJudgeMin;
        beatJudgeExtra = beatJudgeExtra == 0 ? 0.3f : beatJudgeExtra;
        bpm = bpm == 0 ? 100 : bpm;
        movingTime = movingTime == 0 ? 0.1f : movingTime;
        animationTime = animationTime == 0 ? 3 : animationTime;
        playerMaxHP = playerMaxHP == 0 ? 6 : playerMaxHP;
        enemyMaxHP = enemyMaxHP == 0 ? 6 : enemyMaxHP;
        enemyAttackGaugeMax = enemyAttackGaugeMax == 0 ? 3 : enemyAttackGaugeMax;
        StopAllParticle();
        StopAllAnimation();
    }
    public void GameStart()
    {
        StopAllParticle();
        StartAllAnimation();
    }
    public void GameEnd()
    {
        StopAllParticle();
        StopAllAnimation();
    }
    public void StopAllAnimation()
    {
        playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.TimeScale = 0;
        enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.TimeScale = 0;
    }
    public void StartAllAnimation()
    {
        playerImageObj.GetComponent<SkeletonAnimation>().AnimationState.TimeScale = 1;
        enemyImageObj.GetComponent<SkeletonAnimation>().AnimationState.TimeScale = 1;
    }
    public void StopAllParticle()
    {
        cubeEffectObj.GetComponent<ParticleSystem>().Stop();
        inGamePlayerObj.transform.Find("Effect").GetComponent<ParticleSystem>().Stop();
        enemyHitEffectObj.GetComponent<ParticleSystem>().Stop();
        playerAttackEffectObj.GetComponent<ParticleSystem>().Stop();
    }
}
public partial class InGameData_s // data converter
{
    public static ERotatePosition GetVec3ToERotatePosition(Vector3 position)
    {
        if (position == new Vector3(0, -90, 0))
        {
            return ERotatePosition.LEFT;
        }
        else if (position == new Vector3(0, 90, 0))
        {
            return ERotatePosition.RIGHT;
        }
        else if (position == new Vector3(-90, 0, 0))
        {
            return ERotatePosition.UP;
        }
        else if (position == new Vector3(90, 0, 0))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    public static Vector3 GetERotatePositionToVec3(ERotatePosition position)
    {
        switch (position)
        {
            case ERotatePosition.LEFT:
                return new Vector3(0, -90, 0);
            case ERotatePosition.RIGHT:
                return new Vector3(0, 90, 0);
            case ERotatePosition.UP:
                return new Vector3(-90, 0, 0);
            case ERotatePosition.DOWN:
                return new Vector3(90, 0, 0);
            default:
                break;
        }
        return new Vector3(0, 0, 0);
    }
    public static ERotatePosition GetVec2ToERotatePosition(Vector2Int pos,Vector2Int divideSize)
    {
        if (pos == new Vector2(-1, -1 * (divideSize.y - 1)))
        {
            return ERotatePosition.LEFT;
        }
        else if (pos == new Vector2(1, -1 * (divideSize.y - 1)))
        {
            return ERotatePosition.RIGHT;
        }
        else if (pos == new Vector2(-1 * (divideSize.x - 1), -1))
        {
            return ERotatePosition.UP;
        }
        else if (pos == new Vector2(-1 * (divideSize.x - 1), 1))
        {
            return ERotatePosition.DOWN;
        }
        return ERotatePosition.NONE;
    }
    public static Vector2 GetERotatePositionToVec2(ERotatePosition position, Vector2Int divideSize)
    {
        switch (position)
        {
            case ERotatePosition.LEFT:
                return new Vector2(-1, -1 * (divideSize.y - 1));
            case ERotatePosition.RIGHT:
                return new Vector2(1, -1 * (divideSize.y - 1));
            case ERotatePosition.UP:
                return new Vector2(-1 * (divideSize.x - 1), -1);
            case ERotatePosition.DOWN:
                return new Vector2(-1 * (divideSize.x - 1), 1);
            default:
                break;
        }
        return new Vector2(0, 0);
    }
    public static Queue<T> ReverseQueue<T>(Queue<T> queue)
    {
        List<T> prim = new List<T>();
        Queue<T> temp = new Queue<T>();
        foreach (var data in queue)
        {
            prim.Add(data);
        }
        for (int i = prim.Count - 1; i >= 0; i--)
        {
            temp.Enqueue(prim[i]);
        }
        return temp;
    }
}
