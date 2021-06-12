using UnityEngine;
using UnityEngine.UI;

public class UI_CONTROLS : MonoBehaviour
{
    public Text tittle;
    public Text movement;
    public Text chest;
    public Text change;
    public Text sprint;
    public Text attack; 

    public Text back_button;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameManager.GetInstance();
        tittle.text = "GAME CONTROLS";
        movement.text = "USE WASD OR THE ARROWKEYS TO MOVE";
        change.text = "USE 1, 2, 3 or 4 TO CHANGE CHARACTERS";
        chest.text = "MOUSE LEFT CLICK TO TAKE CHESTS";
        attack.text = "MOUSE LEFT CLICK TO ATTACK ENEMIES";
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
