using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PushableCrate : MonoBehaviour
{
    public GlobalData globalData;
    public AudioSource audioSource;
    public Rigidbody body;

    [HideInInspector]
    public Vector3[] previousPositions;
    public float timeSinceLastMovement;
    public bool isMoving;

    public float GetDistance(float ax, float ay, float bx, float by)
    {
        return ((bx - ax) * (bx - ax)) + ((by - ay) * (by - ay));
    }

    public float GetDistance(Vector2 a, Vector2 b)
    {
        return GetDistance(a.x, a.y, b.x, b.y);
    }

    public float GetDistanceXZ(Vector3 a, Vector3 b)
    {
        return GetDistance(a.x, a.z, b.x, b.z);
    }

    public float GetDistanceOverTime()
    {
        float result = GetDistanceXZ(transform.position, previousPositions[0]);

        for(int it_index = 1; it_index < previousPositions.Length; it_index += 1)
        {
            result += GetDistanceXZ(previousPositions[it_index], previousPositions[it_index - 1]);
        }

        return result;
    }

    public void UpdatePositions()
    {
        for(int it_index = previousPositions.Length - 1; it_index > 0; it_index -= 1)
        {
            previousPositions[it_index] = previousPositions[it_index - 1];
        }
        previousPositions[0] = transform.position;
    }

    void Awake()
    {
        if(body == null)
        {
            if(gameObject.TryGetComponent(out Rigidbody rigidbody))
            {
                body = rigidbody;
            }
        }

        timeSinceLastMovement = 0f;

        previousPositions = new Vector3[9];
        for(int it_index = 0; it_index < previousPositions.Length; it_index += 1)
        {
            previousPositions[it_index] = transform.position;
        }
    }

    void FixedUpdate()
    {
        // body.AddRelativeForce(Physics.gravity * (body.drag * 0.25f));
        // body.AddForce(Physics.gravity * body.drag);

        if(GetDistanceOverTime() >= 0.001f)
        {
            timeSinceLastMovement = 0f;

            if(!isMoving)
            {
                audioSource.Stop();
                globalData.audioList.PlaySoundEffect(audioSource, "push_loop");
                isMoving = true;
            }
        }
        else if(isMoving && timeSinceLastMovement > 0.05f)
        {
            timeSinceLastMovement = 0f;
            audioSource.Stop();
            globalData.audioList.PlaySoundEffect(audioSource, "push_end");
            isMoving = false;
        }

        UpdatePositions();
        timeSinceLastMovement += Time.fixedDeltaTime;
    }
}
