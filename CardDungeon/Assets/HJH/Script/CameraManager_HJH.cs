using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager_HJH : MonoBehaviour
{
    public bool isShaking = false;
    public IEnumerator Shake(float ShakeAmount, float ShakeTime)
    {
        if (!isShaking)
        {
            isShaking = true;
            Vector3 startPos = transform.position;
            float timer = 0;
            while (timer <= ShakeTime)
            {
                Camera.main.transform.position = startPos + (Vector3)UnityEngine.Random.insideUnitCircle * ShakeAmount;
                timer += Time.deltaTime;
                yield return null;
            }
            Camera.main.transform.position = startPos;
            isShaking = false;
        }
    }
}