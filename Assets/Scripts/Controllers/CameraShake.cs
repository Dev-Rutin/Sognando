using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float shakeAmount = 3.0f;
    public float shakeTime = 1.0f;
    private Vector3 _shakePos;

    public void ShakeStart()
    {
        StartCoroutine(Shake());
    }
    IEnumerator Shake()
    {
        float timer = 0;
        Vector3 originPos = transform.position;
        while (timer < shakeTime)
        {
            _shakePos = originPos + Random.insideUnitSphere * shakeAmount;

            transform.position = Vector3.Lerp(transform.position, _shakePos, Time.deltaTime * shakeTime);

            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = originPos;
    }
}
