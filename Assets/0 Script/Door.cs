using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Door : MonoBehaviour
{
    public bool isOpen;
    public bool isLocked;
    public bool isFrozen;

    [Space(8)]
    public bool isOpening;

    [Space(8)]
    public float openAngle = 160f;
    public float openTime;
    public float maxOpenTime = 0.125f;

    public bool tmpToggleLock;
    public bool tmpToggleFreeze;

    [Space(8)]
    public GlobalData globalData;
    public GameObject padlock;
    public Transform hinge;
    public Collider doorCollider;
    public Collider boundEventCollider;
    public AudioSource audioSource;

    [HideInInspector]
    public Quaternion initialRotation;

    public void SetDoorState(bool open, bool locked, bool frozen)
    {
        isOpen = open;
        isLocked = locked;
        isFrozen = frozen;

        isOpening = false;
        openTime = 0f;

        if(isOpen)
        {
            Vector3 angle = initialRotation.eulerAngles;
            hinge.transform.localRotation = Quaternion.Euler(angle.x, angle.y + openAngle, angle.z);
        }
        else
        {
            hinge.transform.localRotation = initialRotation;
        }
        
        padlock.SetActive(isLocked);

        if(isLocked)
        {
            isOpen = false;
            hinge.transform.localRotation = initialRotation;
        }

        if(gameObject.TryGetComponent(out Collider collider))
        {
            if(isFrozen || isLocked)
            {
                collider.enabled = false;
            }
            else
            {
                collider.enabled = true;
            }
        }
    }

    public void ToggleDoorOpen()
    {
        if(!isLocked)
        {
            openTime = 0f;
            isOpening = true;

            if(isOpen)
            {
                globalData.audioList.PlaySoundEffect(audioSource, "door_close");
            }
            else
            {
                globalData.audioList.PlaySoundEffect(audioSource, "door_open");
            }
        }
    }

    public void ToggleDoorOpenInstantly()
    {
        if(!isLocked)
        {
            isOpening = true;
            openTime = maxOpenTime;
        }
    }

    public void SetDoorOpen(bool enabled)
    {
        if(isOpen != enabled)
        {
            ToggleDoorOpen();
        }
    }

    public void SetLocked(bool enabled)
    {
        isLocked = enabled;
        padlock.SetActive(isLocked);

        if(gameObject.TryGetComponent(out Collider collider))
        {
            collider.enabled = !isLocked;
        }

        if(isLocked)
        {
            isOpen = false;
            hinge.transform.localRotation = initialRotation;
        }
        else
        {
            globalData.audioList.PlaySoundEffect(audioSource, "door_unlock");
        }
    }

    public void ToggleFreeze() {
        isFrozen = !isFrozen;

        if(isFrozen)
        {
            if(isOpening)
            {
                openTime = maxOpenTime;
            }

            if(gameObject.TryGetComponent(out Collider collider))
            {
                collider.enabled = false;
            }
        }
        else
        {
            if(gameObject.TryGetComponent(out Collider collider))
            {
                collider.enabled = !isLocked;
            }
        }
    }

    public void SetFreeze(bool enabled) {
        if(isFrozen != enabled)
        {
            ToggleFreeze();
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        initialRotation = hinge.transform.localRotation;
        padlock.SetActive(isLocked);

        if(gameObject.TryGetComponent(out Collider collider))
        {
            collider.enabled = !isLocked;
        }

        SetDoorState(isOpen, isLocked, isFrozen);
    }

    // Update is called once per frame
    void Update()
    {
        if(tmpToggleLock)
        {
            SetLocked(!isLocked);
            tmpToggleLock = false;
        }

        if(tmpToggleFreeze)
        {
            ToggleFreeze();
            tmpToggleFreeze = false;
        }

        if(isOpening)
        {
            if(openTime < maxOpenTime)
            {
                Vector3 angle = initialRotation.eulerAngles;

                float t = openTime / maxOpenTime;

                if(isOpen)
                {
                    t = 1f - t;
                }

                hinge.transform.localRotation = Quaternion.Euler(angle.x, Mathf.Lerp(angle.y, angle.y + openAngle, t), angle.z);

                doorCollider.enabled = false;
                openTime += Time.deltaTime;
            }
            else
            {
                isOpen = !isOpen;
                isOpening = false;
                doorCollider.enabled = true;
                openTime = 0f;

                if(isOpen)
                {
                    Vector3 angle = initialRotation.eulerAngles;
                    hinge.transform.localRotation = Quaternion.Euler(angle.x, angle.y + openAngle, angle.z);
                }
                else
                {
                    hinge.transform.localRotation = initialRotation;
                }
            }
        }
    }
}
