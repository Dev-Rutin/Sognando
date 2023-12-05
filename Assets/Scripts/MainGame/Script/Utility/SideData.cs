using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public struct SideData
{
    public Vector2 position { get; private set; }
    public Vector2 transform { get; private set; }
    public GameObject noise;
    public GameObject lineAttack;
    public GameObject linkLineAttack;
    public GameObject crossAttack;
    public bool areaAttackSafe;
    public GameObject pathNoise;
    public SideData(Vector2 _position, Vector2 _transform)
    {
        position = _position;
        transform = _transform;
        noise = null;
        lineAttack = null;
        linkLineAttack = null;
        crossAttack = null;
        areaAttackSafe = false;
        pathNoise = null;
    }
    public bool isCanMakeCheck(bool isStack, string dataName)
    {
        if (this.GetType().GetField(dataName).GetValue(this) != null)
        {
            return false;
        }
        if (isStack)
        {

        }
        else
        {
            if (noise != null)
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
            if(crossAttack!=null)
            {
                return false;
            }
            if(areaAttackSafe==true)
            {
                return false;
            }
            if(pathNoise!=null)
            {
                return false;
            }
        }
        return true;
    }

    public void Clear()
    {
        noise = null;
        lineAttack = null;
        linkLineAttack = null;
        crossAttack = null;
        areaAttackSafe = false;
        pathNoise = null;
    }
}