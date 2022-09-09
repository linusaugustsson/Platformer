using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelTransition : MonoBehaviour
{


    public GlobalData globalData;

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI sentenceText;

    public CanvasGroup fadeBlackGroup;
    public CanvasGroup titleTextGroup;
    public CanvasGroup sentenceTextGroup;

    public float maxStartTime = 3.0f;

    public float maxSentenceInTime = 3.0f;
    public float maxSentenceStayTime = 3.0f;
    public float maxSentenceOutTime = 3.0f;

    public float maxTitleInTime = 3.0f;
    public float maxTitleStayTime = 3.0f;
    public float maxTitleOutTime = 3.0f;

    public float maxEndTime = 3.0f;

    private float timer = 0.0f;

    public int levelToLoad = 0;

    public GameObject loadedLevel;

    public GameManager gameManager;

    private float normalTime = 0.0f;

    public enum FadeState {
        off = 0,
        start = 1,
        sentenceIn = 2,
        sentenceStay = 3,
        sentenceOut = 4,
        titleIn = 5,
        titleStay = 6,
        titleOut = 7,
        end = 8
    }

    private void Start() {
        normalTime = maxSentenceStayTime;
    }

    public FadeState currentFadeState = FadeState.off;


    private void Update() {

        if(Input.GetKeyDown(KeyCode.F4)) {
            //StartTransition();
            GoToNextLevel();
        }
        
        if(currentFadeState == FadeState.start) {
            timer += Time.deltaTime;

            fadeBlackGroup.alpha = Mathf.Lerp(0, 1, timer / maxStartTime);

            if(timer >= maxStartTime) {
                
                if (sentenceText.text != "") {
                    currentFadeState = FadeState.sentenceIn;

                } else {
                    currentFadeState = FadeState.titleIn;
                }
                timer = 0.0f;

                if(loadedLevel != null) {
                    Destroy(loadedLevel);
                }
                

            }
        }

        if (currentFadeState == FadeState.sentenceIn) {
            timer += Time.deltaTime;

            sentenceTextGroup.alpha = Mathf.Lerp(0, 1, timer / maxSentenceInTime);

            if (timer >= maxSentenceInTime) {
                timer = 0.0f;
                currentFadeState = FadeState.sentenceStay;

            }
        }

        if (currentFadeState == FadeState.sentenceStay) {
            timer += Time.deltaTime;

            if (timer >= maxSentenceStayTime) {
                timer = 0.0f;
                currentFadeState = FadeState.sentenceOut;

            }
        }

        if (currentFadeState == FadeState.sentenceOut) {
            timer += Time.deltaTime;

            sentenceTextGroup.alpha = Mathf.Lerp(1, 0, timer / maxSentenceOutTime);

            if (timer >= maxSentenceOutTime) {
                timer = 0.0f;
                currentFadeState = FadeState.titleIn;

            }
        }

        if (currentFadeState == FadeState.titleIn) {
            timer += Time.deltaTime;

            titleTextGroup.alpha = Mathf.Lerp(0, 1, timer / maxTitleInTime);

            if (timer >= maxTitleInTime) {
                timer = 0.0f;
                currentFadeState = FadeState.titleStay;
                
            }
        }

        if (currentFadeState == FadeState.titleStay) {
            timer += Time.deltaTime;


            if (timer >= maxTitleStayTime) {
                timer = 0.0f;
                currentFadeState = FadeState.titleOut;
                ChangeLevel();
            }
        }

        if (currentFadeState == FadeState.titleOut) {
            timer += Time.deltaTime;

            titleTextGroup.alpha = Mathf.Lerp(1, 0, timer / maxTitleInTime);

            if (timer >= maxTitleOutTime) {
                timer = 0.0f;
                currentFadeState = FadeState.end;

                gameManager.playerInput.SwitchCurrentActionMap("Gameplay");
                
                //EndTransition();
            }
        }




        if (currentFadeState == FadeState.end) {
            timer += Time.deltaTime;

            fadeBlackGroup.alpha = Mathf.Lerp(1, 0, timer / maxEndTime);

            if (timer >= maxEndTime) {

                timer = 0.0f;
                currentFadeState = FadeState.off;
                
            }
        }


    }

    public void SetTitleText(string _text) {
        titleText.text = _text;
    }


    public void SetSentenceText(string _text) {
        sentenceText.text = _text;
    }


    public void StartTransition(int _level) {
        levelToLoad = _level;
        SetTitleText(globalData.levelList.levels[_level].displayName);
        SetSentenceText(globalData.levelList.levels[_level].sentence);

        currentFadeState = FadeState.start;
        timer = 0.0f;

        if(_level == 0) {
            RenderSettings.fog = true;
        }

        if(_level == 1) {
            RenderSettings.fog = false;
            globalData.player.jumpUnlocked = true;
        }

        if(_level == 2) {
            RenderSettings.fog = false;
            globalData.player.glassesUnlocked = true;
        }

        if(_level == 3) {
            RenderSettings.fog = false;
            globalData.player.dashUnlocked = true;
        }

        maxSentenceStayTime = normalTime;
    }

    public void GoToNextLevel() {
        gameManager.playerInput.SwitchCurrentActionMap("Menu");
        levelToLoad++;
        StartTransition(levelToLoad);
    }


    public void EndTransition() {

    }

    private void ChangeLevel() {

        globalData.player.inputMessages.HideMessages();

        loadedLevel = Instantiate(globalData.levelList.levels[levelToLoad].levelPrefab);

        globalData.player.musicSourceA.Stop();
        globalData.player.musicSourceB.Stop();
        globalData.levelList.LoadLevel(globalData.levelList.levels[levelToLoad].name);
        globalData.player.characterController.motor.SetPosition(globalData.levelList.levels[levelToLoad].playerPosition);
    }




}
