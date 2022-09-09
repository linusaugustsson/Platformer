using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum CircleMovementAxis {
    X = 0,
    Y = 1,
    Z = 2,
};

public class CircleMovement : MonoBehaviour
{
    public CircleMovementAxis axis = CircleMovementAxis.Y;

    public float radius = 1f;
    public float speed = 2f;

    [HideInInspector]
    public float time;

    [HideInInspector]
    public Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if(speed != 0f)
        {
            time += Time.deltaTime;

            float a = time * speed;

            if(a > Mathf.PI)
            {
                time = -(Mathf.PI / speed);
                a = time * speed;
            }

            float angle = a;

            float s = Mathf.Sin(angle) * radius;
            float c = Mathf.Cos(angle) * radius;

            float x = (radius * c);
            float y = (radius * s);

            if(axis == CircleMovementAxis.X)
            {
                transform.localPosition = new Vector3(initialPosition.x, initialPosition.y + x, initialPosition.z + y);
            }
            else if(axis == CircleMovementAxis.Y)
            {
                transform.localPosition = new Vector3(initialPosition.x + x, initialPosition.y, initialPosition.z + y);
            }
            else if(axis == CircleMovementAxis.Z)
            {
                transform.localPosition = new Vector3(initialPosition.x + x, initialPosition.y + y, initialPosition.z);
            }
        }
    }
}
