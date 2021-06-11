using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_MENU : MonoBehaviour
{
    public Text tittle;
   public Text play_button;
   public Text settings_button;
   public Text controls_button;

    GameManager gm;
   private void OnEnable()
   {
        gm = GameManager.GetInstance();
        tittle.text = "DRAGON QUEST";
        play_button.text = "PLAY";
        settings_button.text = "SETTINGS";
        controls_button.text = "CONTROLS";
        gm.last_state = "MENU";
   }

    public void Play()
    {
        gm.last_state = "MENU";
        gm.ChangeState(GameManager.GameState.GAME);
    }

    public void Settings()
    {
        gm.last_state = "MENU";
        gm.ChangeState(GameManager.GameState.SETTINGS);
    }

    public void Controls()
    {
        gm.last_state = "MENU";
        gm.ChangeState(GameManager.GameState.CONTROLS);
    }
}
