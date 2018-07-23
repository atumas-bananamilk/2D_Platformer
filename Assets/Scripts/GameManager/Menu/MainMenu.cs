using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.OnGameStateChange.AddListener(HandleGameStateChanged);
    }

    public void OnFadeOutComplete()
    {
        Debug.Log("FadeOut Complete");
    }

    public void OnFadeInComplete()
    {
        Debug.LogWarning("FadeIn Complete");
        //UIManager.Instance.SetDummyCameraActive(true);
    }

    public void FadeIn()
    {

    }

    public void FadeOut()
    {
        //UIManager.Instance.SetDummyCameraActive(false);
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (previousState == GameManager.GameState.PREGAME && currentState == GameManager.GameState.RUNNING)
        {
            FadeOut();
        }
    }


}
