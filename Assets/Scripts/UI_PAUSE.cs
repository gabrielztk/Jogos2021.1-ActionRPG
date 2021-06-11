using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PAUSE : MonoBehaviour
{
    public PlayerController playerController;
    public Text tittle;
    public Text resume_button;
    public Text settings_button;
    public Text controls_button;
    public Text menu_button;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameManager.GetInstance();
        tittle.text = "GAME PAUSED";
        resume_button.text = "RESUME";
        settings_button.text = "SETTINGS";
        controls_button.text = "CONTROLS";
        menu_button.text = "MAIN MENU";
    }

    public void Resume()
    {   
        gm.last_state = "PAUSE";
        gm.ChangeState(GameManager.GameState.GAME);
    }

    public void Settings()
    {
        gm.last_state = "PAUSE";
        gm.ChangeState(GameManager.GameState.SETTINGS);
    }

    public void Controls()
    {
        gm.last_state = "PAUSE";
        gm.ChangeState(GameManager.GameState.CONTROLS);
    }

    public void Menu()
    {
        gm.last_state = "PAUSE";
        gm.ChangeState(GameManager.GameState.MENU);
        // playerController.Reset();
    }
}