using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New GlobalData", menuName = "Data/GlobalData"), System.Serializable]
public class GlobalData : ScriptableObject
{
    public AudioList audioList;
    public LevelList levelList;

    [HideInInspector]
    public Player player;

    [HideInInspector]
    public TaxiManager taxiManager;

    [HideInInspector]
    public SoundManager soundManager;

    [HideInInspector]
    public GameManager gameManager;

}
