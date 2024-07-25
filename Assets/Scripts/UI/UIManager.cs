using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements.Experimental;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;
    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO unloadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;
    public VoidEventSO unPauseEvent;
    public VoidEventSO newGameEvent;

    public VoidEventSO getDoubleJumpEvent;
    public VoidEventSO getDashEvent;
    public VoidEventSO getWallSlideEvent;
    public VoidEventSO getInvulnerableDashEvent;


    [Header("广播")]
    public VoidEventSO pauseEvent;


    [Header("组件")]
    public GameObject player;
    public GameObject instructionObj;
    private Instruction instruction;
    public GameObject instructionMove;
    public GameObject instructionJump;
    public GameObject instructionAttack;
    public GameObject instructionAim;

    public GameObject gameOverPanel;
    public GameObject continueButon;
    public GameObject mobileTouch;
    public Button settingButton;
    public GameObject pausePanel;
    public Slider volumeSlider;
    public SceneLoader sceneLoader;

    public GameObject doubleJumpInstr;
    public GameObject dashInstr;
    public GameObject wallSlideInstr;
    public GameObject invulnerableDashInstr;

    public PlayerinputControl inputControl;

    [Header("参数")]
    public bool alreadyDead;
    public bool instrAvailable;



    private void Awake()
    {
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif
        instruction = instructionObj.GetComponent<Instruction>();

        settingButton.onClick.AddListener(TogglePausePannel);

        inputControl = new PlayerinputControl();

    }


    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent += OnUnloadSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
        unPauseEvent.OnEventRaised += TogglePausePannel;

        loadDataEvent.OnEventRaised += openInstr;
        backToMenuEvent.OnEventRaised += closeInstr;
        newGameEvent.OnEventRaised += openInstr;

        getDoubleJumpEvent.OnEventRaised += GetDoubleJump;
        getDashEvent.OnEventRaised += GetDash;
        getWallSlideEvent.OnEventRaised += GetWallSlide;
        getInvulnerableDashEvent.OnEventRaised += GetInvulnerableDash;
        
    }

    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent -= OnUnloadSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;     
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
        unPauseEvent.OnEventRaised -= TogglePausePannel;

        loadDataEvent.OnEventRaised -= openInstr;
        newGameEvent.OnEventRaised -= openInstr;
        backToMenuEvent.OnEventRaised -= closeInstr;

        getDoubleJumpEvent.OnEventRaised -= GetDoubleJump;
        getDashEvent.OnEventRaised -= GetDash;
        getWallSlideEvent.OnEventRaised -= GetWallSlide;
        getInvulnerableDashEvent.OnEventRaised -= GetInvulnerableDash;
    }

    

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !sceneLoader.isMenu)
        {
            if (pausePanel.activeInHierarchy)
            {
                pausePanel.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                pauseEvent.RaiseEvent();
                pausePanel.SetActive(true);
                Time.timeScale = 0;
            }
        }

        if (instrAvailable)
        {
            instructionMove.SetActive(instruction.instructionMove);
            instructionJump.SetActive(instruction.instructionJump);
            instructionAttack.SetActive(instruction.instructionAttack);
            instructionAim.SetActive(instruction.instructionAim);
        }

    }

    private void closeInstr()
    {
        instrAvailable = false;
        instruction.instructionMove = false;
        instruction.instructionJump = false;
        instruction.instructionAttack = false;
        instruction.instructionAim = false;
        instructionMove.SetActive(false);
        instructionJump.SetActive(false);
        instructionAttack.SetActive(false);
        instructionAim.SetActive(false);
    }
    private void openInstr()
    {
        instrAvailable = true;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }


    private void TogglePausePannel()
    {
        if (pausePanel.activeInHierarchy)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
    }


    private void OnGameOverEvent()
    {

            gameOverPanel.SetActive(true);
            EventSystem.current.SetSelectedGameObject(continueButon);

        
    }

    private void OnLoadDataEvent()
    {
        player.GetComponent<Character>().invulnerableCounter = 1;
        gameOverPanel.SetActive(false);
    }

    private void OnUnloadSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        if(sceneToLoad.sceneType == SceneType.Menu)
        {
            playerStateBar.gameObject.SetActive(false);
        }
        else
        {
            playerStateBar.gameObject.SetActive(true);
        }
    }

    private void OnHealthEvent(Character character)
    {
        var percentage =  (float)character.currentHeath / (float)character.maxHeath;
        
        playerStateBar.OnHealthChange(percentage);

        playerStateBar.OnPowerChange(character);
    }


    private void GetDoubleJump()
    {
        doubleJumpInstr.SetActive(true);
        Time.timeScale = 0.2f;
        doubleJumpInstr.GetComponent<AbilityInstr>().TriggerprintInstrTimer();
    }

    private void GetDash()
    {
        dashInstr.SetActive(true);
        Time.timeScale = 0.2f;
        dashInstr.GetComponent<AbilityInstr>().TriggerprintInstrTimer();
    }

    private void GetWallSlide()
    {
        wallSlideInstr.SetActive(true);
        Time.timeScale = 0.2f;
        wallSlideInstr.GetComponent<AbilityInstr>().TriggerprintInstrTimer();
    }

    private void GetInvulnerableDash()
    {
        invulnerableDashInstr.SetActive(true);
        Time.timeScale = 0.2f;
        invulnerableDashInstr.GetComponent<AbilityInstr>().TriggerprintInstrTimer();
    }
}
