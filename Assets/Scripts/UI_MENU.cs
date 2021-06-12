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

   public GameObject player;
   public PlayerController playerController;

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
        player.transform.position = new Vector3(-21.8f, 30.15f, 407.2f);
        playerController.Reset();
        player.transform.position = new Vector3(-21.8f, 30.15f, 407.2f);
        Debug.Log(player.transform.position);
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
