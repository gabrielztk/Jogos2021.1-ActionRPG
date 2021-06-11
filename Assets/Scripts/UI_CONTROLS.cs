using UnityEngine;
using UnityEngine.UI;

public class UI_CONTROLS : MonoBehaviour
{
    public Text tittle;
    public Text movement;
    public Text chest;
    public Text sprint;

    public Text back_button;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameManager.GetInstance();
        tittle.text = "GAME CONTROLS";
        movement.text = "USE WASD OR THE ARROWKEYS TO MOVE";
        chest.text = "USE THE SPACE BAR TO TAKE CHESTS";
        sprint.text = "USE THE LEFT SHIFT KEY TO SPRINT";
        back_button.text = "BACK";
    }

    public void Back()
    {   
        if (gm.last_state == "MENU")
        {
            gm.ChangeState(GameManager.GameState.MENU);
        }

        if (gm.last_state == "PAUSE")
        {
            gm.ChangeState(GameManager.GameState.PAUSE);
        }
    }
}
