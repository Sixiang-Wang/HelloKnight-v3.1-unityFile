using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour,ISaveable
{
    public bool isMenu;
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    [Header("组件")] 
    public GameObject settingButtonObject;
    public GameObject pausePanel;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEvent;
    public SceneLoadEventSO unloadedSceneEvent;
    public VoidEventSO loadIsDoneDataEvent;

    public float fadeDuration;

    [Header("Scene")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuScene;
    public GameSceneSO greenPassScene;
    public GameSceneSO caveScene;
    public GameSceneSO crossRoadScene;

    public GameSceneSO currentLoadedScene;
    private GameSceneSO sceneToLoad;

    private Vector3 positionToGo;
    private bool fadeScr;
    private bool isLoading;

    [Header("isDone")]
    public GameObject chestFly;

    private void Awake()
    {
        //Addressables.LoadSceneAsync(firstLoadScene.sceneReference,LoadSceneMode.Additive);
        //currentLoadedScene = firstLoadScene;
        //currentLoadedScene.sceneReference.LoadSceneAsync(LoadSceneMode.Additive);
    }

    private void Start()
    {
        isMenu = true;
        loadEventSO.RaiseLoadRequestEvent(menuScene, menuPosition, true);
        //NewGame();
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;
        
        ISaveable saveable = this;
        saveable.RegisterSaveData();

        
    }
    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void OnBackToMenuEvent()
    {
        Time.timeScale = 1;
        sceneToLoad = menuScene;
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad, menuPosition, true);
    }

    public void NewGame()
    {
        chestFly.GetComponent<Chest>().isDone = false;
        sceneToLoad = firstLoadScene;
        //OnLoadRequestEvent(sceneToLoad, firstPosition, true);
        loadEventSO.RaiseLoadRequestEvent(sceneToLoad,firstPosition,true);
    }

    private void OnLoadRequestEvent(GameSceneSO locationToLoad, Vector3 posToGo, bool fadeScreen)
    {
        if (isLoading)
            return;

        isLoading = true;
        sceneToLoad = locationToLoad;
        positionToGo = posToGo;
        fadeScr = fadeScreen;
        if (currentLoadedScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        }
        else
        {
            LoadNewScene();
        }
        loadIsDoneDataEvent.RaiseEvent();
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (fadeScr)
        {
            //变黑
            fadeEvent.FadeIn(fadeDuration);
        }
        yield return new WaitForSeconds(fadeDuration);

        //显示血条
        unloadedSceneEvent.RaiseLoadRequestEvent(sceneToLoad, positionToGo, true);
        
        yield return currentLoadedScene.sceneReference.UnLoadScene();

        //关闭player
        playerTrans.gameObject.SetActive(false);

        //加载
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        var loadingOption = sceneToLoad.sceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOption.Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<SceneInstance> obj)
    {
        currentLoadedScene = sceneToLoad;
        playerTrans.position = positionToGo;

        playerTrans.gameObject.SetActive(true);
        if (fadeScr)
        {
            //变透明
            fadeEvent.FadeOut(fadeDuration);
        }
        isLoading = false;

        if(currentLoadedScene.sceneType != SceneType.Menu)
        {
            isMenu = false;
            afterSceneLoadedEvent.RaiseEvent();
            pausePanel.SetActive(false);
            settingButtonObject.SetActive(true);
        }
        else
        {
            isMenu = true;
            pausePanel.SetActive(false);
            settingButtonObject.SetActive(false);
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadedScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            positionToGo = data.characterPosDict[playerID].ToVector3();
            sceneToLoad = data.GetSavedScene();

            OnLoadRequestEvent(sceneToLoad, positionToGo, true);

            
        }
    }

}
