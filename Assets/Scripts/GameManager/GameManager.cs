using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

[Serializable] public class EventGameState : UnityEvent<GameManager.GameState, GameManager.GameState> { }

public class GameManager : Singleton<GameManager>
{
	public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED
    }

    // Array of prefabs that can be populated in the Editor
    public GameObject[] SystemPrefabs;
    public EventGameState OnGameStateChange;

    // List of prefabs already created for GameManager to keep track off
    List<GameObject> _instanceSystemPrefabs;
    List<AsyncOperation> _loadOperations;
    GameState _currentGameState = GameState.PREGAME;

    private string _currentLevelName = string.Empty;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        _instanceSystemPrefabs = new List<GameObject>();
        _loadOperations = new List<AsyncOperation>();

        InstantiateSystemPrefabs();
    }

    public void StartGame()
    {
        LoadLevel("HomePage");
    }

    #region Load and Unload Operations

    void OnLoadOperationComplete(AsyncOperation ao)
    {
        if (_loadOperations.Contains(ao))
        {
            _loadOperations.Remove(ao);

            if(_loadOperations.Count == 0)
            {
                UpdateState(GameState.RUNNING);
            }

            // dispatch messages
            // transition between scenes
        }
        Debug.Log("Load Complete");
    }

    public void LoadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to load level " + levelName);
        }

        ao.completed += OnLoadOperationComplete;
        _loadOperations.Add(ao);

        _currentLevelName = levelName;
    }

    public void UnloadLevel(string levelName)
    {
        AsyncOperation ao = SceneManager.UnloadSceneAsync(levelName);

        if (ao == null)
        {
            Debug.LogError("[GameManager] Unable to unload level " + levelName);
            return;
        }

        ao.completed += OnUnloadOperationComplete;
    }

    void OnUnloadOperationComplete(AsyncOperation ao)
    {
        Debug.Log("Unload Complete");
    }
    #endregion

    #region States
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    private void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;

        switch(_currentGameState)
        {
            case GameState.PREGAME:
                break;
            case GameState.RUNNING:
                break;
            case GameState.PAUSED:
                break;
            default:
                break;
        }

        // dispatch messages
        // transition between scenes
        OnGameStateChange.Invoke(_currentGameState, previousGameState);
    }
    #endregion


    #region Initializations
    void InstantiateSystemPrefabs()
    {
        GameObject prefabInstance;

        for (int i = 0; i < SystemPrefabs.Length; i++)
        {
            prefabInstance = Instantiate(SystemPrefabs[i]);
            _instanceSystemPrefabs.Add(prefabInstance);
        }
    }
    #endregion

    #region Destructors
    protected override void OnDestroy()
    {
        base.OnDestroy();

        for (int i = 0; i < _instanceSystemPrefabs.Count; i++)
        {
            Destroy(_instanceSystemPrefabs[i]);
        }

        _instanceSystemPrefabs.Clear();
    }
    #endregion
}
