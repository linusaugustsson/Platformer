using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EyeBeam : MonoBehaviour
{
    public Transform visualBeam;
    
    public float minDistance;
    public bool isColliding;
    public bool wasColliding;
    public float initialBeamRange;

    void OnTriggerStay(Collider other)
    {
        if(!other.isTrigger)
        {
            EyeTimer timer = other.GetComponentInParent<EyeTimer>();

            if(other.transform.parent.TryGetComponent(out Player player))
            {
            }
            else if(timer == null)
            {
                isColliding = true;

                if(Physics.Raycast(transform.position, other.bounds.center - transform.position, out RaycastHit hit, initialBeamRange))
                {
                    Vector3 forward = transform.TransformDirection(Vector3.forward);

                    Vector3 a = Vector3.Scale((other.bounds.max - other.bounds.min) * 0.45f, forward) + other.bounds.center;

                    a.x = Mathf.Lerp(transform.position.x, a.x, Mathf.Abs(forward.x));
                    a.y = Mathf.Lerp(transform.position.y, a.y, Mathf.Abs(forward.y));
                    a.z = Mathf.Lerp(transform.position.z, a.z, Mathf.Abs(forward.z));

                    float distance = Vector3.Distance(transform.position, a);

                    minDistance = Mathf.Max(Mathf.Min(distance, minDistance), 0.001f);
                }
            }
        }
    }

    void Start()
    {
        EyeTimer timer = gameObject.GetComponentInParent<EyeTimer>();
        if(timer == null)
        {
            visualBeam.gameObject.SetActive(true);
        }

        initialBeamRange = visualBeam.transform.localScale.z;
        transform.localScale = visualBeam.transform.localScale;

        minDistance = initialBeamRange;
    }

    void FixedUpdate()
    {
        minDistance = initialBeamRange;
        isColliding = false;
    }

    void Update()
    {
        if(isColliding)
        {
            visualBeam.transform.localScale = new Vector3(visualBeam.transform.localScale.x, visualBeam.transform.localScale.y, minDistance);
        }
        else if(wasColliding)
        {
            visualBeam.transform.localScale = new Vector3(visualBeam.transform.localScale.x, visualBeam.transform.localScale.y, initialBeamRange);
        }

        wasColliding = isColliding;
    }
}
