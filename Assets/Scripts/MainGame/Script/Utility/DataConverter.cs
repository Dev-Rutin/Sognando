using System.Collections.Generic;
using UnityEngine;
public static class DataConverter
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
    public static ERotatePosition GetVec2ToERotatePosition(Vector2Int pos, Vector2Int divideSize)
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