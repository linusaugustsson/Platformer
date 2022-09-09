using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizeyeCircling : MonoBehaviour
{
    public GlobalData globalData;

    public CircleMovementAxis axis = CircleMovementAxis.Y;
    public float circleRadius;
    public float circleSpeed;

    [Space(2)]
    public float sineRadius;
    public float sineFrequency;

    [HideInInspector]
    public float time;

    [HideInInspector]
    public Vector3 initialPosition;

    void Update()
    {
        float sineValue = Mathf.Sin(Time.time * sineFrequency) * sineRadius;

        Vector3 offset = new Vector3();

        if(circleSpeed != 0f)
        {
            time += Time.deltaTime;

            float a = time * circleSpeed;

            if(a > Mathf.PI)
            {
                time = -(Mathf.PI / circleSpeed);
                a = time * circleSpeed;
            }

            float angle = a;

            float s = Mathf.Sin(angle) * circleSpeed;
            float c = Mathf.Cos(angle) * circleSpeed;

            float x = (circleRadius * c);
            float y = (circleRadius * s);

            if(axis == CircleMovementAxis.X)
            {
                offset = new Vector3(sineValue, x, y);
            }
            else if(axis == CircleMovementAxis.Y)
            {
                offset = new Vector3(x, sineValue, y);
            }
            else if(axis == CircleMovementAxis.Z)
            {
                offset = new Vector3(x, y, sineValue);
            }
        }

        transform.SetPositionAndRotation(globalData.player.characterController.transform.position + offset, transform.rotation);
    }
}
