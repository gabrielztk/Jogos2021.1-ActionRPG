using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    private static GameManager _instance;
    public string last_state;

    public bool win;

    public static GameManager GetInstance()
    {
        if(_instance == null)
        {
            _instance = new GameManager();
        }

        return _instance;
    }

    public delegate void ChangeStateDelegate();
    public static ChangeStateDelegate changeStateDelegate;

    public bool launched;

    public void ChangeState(GameState nextState)
    {
        if (gameState == GameState.MENU)
        {
            last_state = "MENU";
        }

        if (gameState == GameState.PAUSE)
        {
            last_state = "PAUSE";
        } 

        if (nextState == GameState.GAME) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Reset();
        }
        else 
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
            
        if (nextState == GameState.MENU)
            currentTime = maxTime;
        gameState = nextState;
        changeStateDelegate();
    }

    private void Reset()
    {
        win = false;
        points = 0;
    }

    public enum GameState { MENU, GAME, PAUSE, ENDGAME, CONTROLS, SETTINGS };

    public GameState gameState { get; private set; }
    // public int lifes;
    public int points;
    public float currentTime = 0f;
    public float maxTime = 45f;

    private GameManager()
    {
        // lifes = 3;
        points = 0;
        currentTime = maxTime;
        gameState = GameState.MENU;
        
        last_state = "MENU";
    }
}
