using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ENDGAME : MonoBehaviour
{
    public Text message;
    public Text button;
    public PlayerController playerController;
    GameManager gm;

    private void OnEnable()
    {
        gm = GameManager.GetInstance();

        button.text = "MAIN MENU";

        if (gm.points == 4)
        {
            message.text = "VOCÊ GANHOU!!!";
        }  
        else
        {
            message.text = "VOCÊ PERDEU!!!";
        }
    }
    
    public void Voltar()
    {
        gm.ChangeState(GameManager.GameState.MENU);
        // playerController.Reset();
    }
}

