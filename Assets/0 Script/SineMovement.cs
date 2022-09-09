using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineMovement : MonoBehaviour
{
    public Vector3 distance = new Vector3(0f, 0.5f, 0f);
    public Vector3 frequency = new Vector3(1f, 1f, 1f);

    [HideInInspector]
    public Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = initialPosition + new Vector3(
            Mathf.Sin(Time.timeSinceLevelLoad * frequency.x) * distance.x,
            Mathf.Sin(Time.timeSinceLevelLoad * frequency.y) * distance.y,
            Mathf.Sin(Time.timeSinceLevelLoad * frequency.z) * distance.z);
    }
}
