using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*public struct CubeData
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
    public CubeData(Vector2 _position, Vector2 _transform)
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
}*/
public class DIvideCubeData_s 
{

}
