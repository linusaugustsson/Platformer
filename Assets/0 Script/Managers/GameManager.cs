using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    public int maxFrameRate = 144;

    public GameObject mainMenuObject;
    public CanvasGroup mainMenuGroup;

    public GameObject pauseMenuObject;
    public CanvasGroup pauseMenuGroup;

    public float maxMenuTime = 0.3f;
    private float timer = 0.0f;

    public bool hasStarted = false;

    public PlayerInput playerInput;

    public EventSystem eventSystem;

    public GameObject startGameButton;
    public GameObject continueButton;

    public LevelTransition levelTransition;

    public GameObject buttons;

    public CanvasGroup victoryGroup;
    private bool hasWon = false;

    public float maxWonTime = 0.5f;
    private float wonTimer = 0.0f;

    private bool waitForMenu = false;

    public float maxWaitTime = 5.0f;
    private float waitTimer = 0.0f;

    public SoundManager soundManager;

    public GlobalData globalData;


    private void Awake() {
        globalData.gameManager = this;
    }

    private void Start() {
        Application.targetFrameRate = maxFrameRate;
        playerInput.SwitchCurrentActionMap("Menu");
        soundManager = GetComponent<SoundManager>();
        
    }

    private void Update() {
        if(hasStarted == true) {
            timer += Time.deltaTime;

            mainMenuGroup.alpha = Mathf.Lerp(1.0f, 0.0f, timer / maxMenuTime);

            if(timer >= maxMenuTime) {
                hasStarted = false;
                timer = 0.0f;
                mainMenuObject.SetActive(false);

                
            }

        }


        if(hasWon == true) {
            wonTimer += Time.deltaTime;

            victoryGroup.alpha = Mathf.Lerp(0, 1, wonTimer / maxWonTime);

            if(wonTimer >= maxWonTime) {
                hasWon = false;
                waitForMenu = true;
                wonTimer = 0.0f;
            }
        }

        if (waitForMenu == true) {
            waitTimer += Time.deltaTime;

            if(waitTimer >= maxWaitTime) {
                waitTimer = 0.0f;
                waitForMenu = false;
                victoryGroup.alpha = 0.0f;

                globalData.player.musicSourceA.Stop();
                globalData.player.musicSourceB.Stop();

                ShowMainMenu();
            }
        }

    }

    public void Pause() {
        eventSystem.SetSelectedGameObject(continueButton);
        playerInput.SwitchCurrentActionMap("Pause");
        pauseMenuObject.SetActive(true);
    }

    public void Continue() {
        
        playerInput.SwitchCurrentActionMap("Gameplay");
        pauseMenuObject.SetActive(false);
    }

    public void Retry() {

        if (globalData.player.currentCheckPoint != null) {
            globalData.player.currentCheckPoint.TriggerCheckPoint();
        }

        playerInput.SwitchCurrentActionMap("Gameplay");
        pauseMenuObject.SetActive(false);
    }

    public void StartGame() {
        mainMenuGroup.blocksRaycasts = false;
        mainMenuGroup.interactable = false;
        hasStarted = true;

        buttons.SetActive(false);

        levelTransition.StartTransition(0);

        levelTransition.maxSentenceStayTime = 5.0f;

    }

    public void ExitGame() {
        Application.Quit();
    }

    public void ShowMainMenu() {
        buttons.SetActive(true);
        playerInput.SwitchCurrentActionMap("Menu");
        mainMenuObject.SetActive(true);
        eventSystem.SetSelectedGameObject(startGameButton);
        mainMenuGroup.alpha = 1.0f;
        mainMenuGroup.blocksRaycasts = true;
        mainMenuGroup.interactable = true;
        pauseMenuObject.SetActive(false);
        hasWon = false;
        victoryGroup.alpha = 0.0f;

        if(levelTransition.loadedLevel != null) {
            Destroy(levelTransition.loadedLevel);
        }

    }

    public void ShowVictoryScreen() {
        hasWon = true;
        playerInput.SwitchCurrentActionMap("Menu");
    }



}
