using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*[System.Serializable]
public struct TaxiChunk {
    public GameObject gameObject;
    public 
};*/

public class TaxiManager : MonoBehaviour
{
    public Transform chunkParent;

    public float secondsPerChunk = 4f;
    public int currentChunk;

    public GameObject[] chunks;

    public float hurtTime;
    public int playerHealth;
    public Transform playerHealthParent;

    public GlobalData globalData;

    [HideInInspector]
    public float chunkPopUpTime;


    public void DamagePlayer()
    {
        if(hurtTime <= 0f)
        {
            playerHealth -= 1;
            hurtTime = 1f;

            globalData.audioList.PlaySoundEffect(globalData.player.generalSoundEffectSource, "player_hurt");

            if(playerHealth < 0)
            {
                currentChunk = 0;
                ResetChunks();

                playerHealth = playerHealthParent.childCount;
            }

            for(int it_index = 0; it_index < playerHealthParent.childCount; it_index += 1)
            {
                playerHealthParent.GetChild(it_index).gameObject.SetActive(it_index < playerHealth);
            }
        }
    }

    void ResetChunks()
    {
        for(int it_index = 0; it_index < chunkParent.childCount; it_index += 1)
        {
            chunks[it_index] = chunkParent.GetChild(it_index).gameObject;
            chunkParent.GetChild(it_index).gameObject.SetActive(false);
        }

        for(int it_index = 0; it_index < 4; it_index += 1)
        {
            int chunk_index = it_index + currentChunk;
            if(chunk_index >= chunkParent.childCount)
            {
                chunk_index -= chunkParent.childCount;
            }

            chunkParent.GetChild(chunk_index).gameObject.SetActive(true);
        }

        chunkPopUpTime = secondsPerChunk;
    }

    // Start is called before the first frame update
    void Start()
    {
        globalData.taxiManager = this;

        playerHealth = playerHealthParent.childCount;

        chunks = new GameObject[chunkParent.childCount];

        ResetChunks();
    }

    // Update is called once per frame
    void Update()
    {
        if(hurtTime > 0f)
        {
            hurtTime -= Time.deltaTime;
        }

        if(secondsPerChunk > 0f)
        {
            if(chunkPopUpTime > 0f)
            {
                float t = 1f - (chunkPopUpTime / secondsPerChunk);

                for(int it_index = 0; it_index < 4; it_index += 1)
                {
                    int chunk_index = it_index + currentChunk;
                    if(chunk_index >= chunkParent.childCount)
                    {
                        chunk_index -= chunkParent.childCount;
                    }

                    Transform chunk = chunkParent.GetChild(chunk_index);
                    float z = (float)it_index * 32f;

                    if(it_index < 3)
                    {
                        chunk.localPosition = new Vector3(0f, 0f, Mathf.Lerp(z, z - 32f, t));
                    }
                    else
                    {
                        chunk.localPosition = new Vector3(0f, Mathf.Lerp(-13f, 0f, t), Mathf.Lerp(z, z - 32f, t));
                    }
                }

                chunkPopUpTime -= Time.deltaTime;
            }
            else
            {
                for(int it_index = 0; it_index < chunkParent.childCount; it_index += 1)
                {
                    chunkParent.GetChild(it_index).gameObject.SetActive(false);
                }
                currentChunk += 1;

                for(int it_index = 0; it_index < 4; it_index += 1)
                {
                    int chunk_index = it_index + currentChunk;

                    if(chunk_index >= chunkParent.childCount)
                    {
                        chunk_index -= chunkParent.childCount;
                    }

                    Transform chunk = chunkParent.GetChild(chunk_index);
                    chunk.gameObject.SetActive(true);

                    if(it_index < 3)
                    {
                        chunk.localPosition = new Vector3(0f, 0f, (float)it_index * 32f);
                    }
                    else
                    {
                        chunk.localPosition = new Vector3(0f, -13f, (float)it_index * 32f);
                    }
                }

                chunkPopUpTime = secondsPerChunk;

                if(currentChunk >= chunkParent.childCount)
                {
                    globalData.player.crazyTaxiMovement = false;
                    globalData.player.gameManager.levelTransition.GoToNextLevel();
                }
            }
        }
    }
}
