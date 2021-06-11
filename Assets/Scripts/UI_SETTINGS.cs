using UnityEngine;
using UnityEngine.UI;

public class UI_SETTINGS : MonoBehaviour
{
    public Text tittle;
    public Text master_slider;
    public Text music_slider;
    public Text sfx_slider;
    public Text back_button;

    GameManager gm;
    private void OnEnable()
    {
        gm = GameManager.GetInstance();
        tittle.text = "SETTINGS MENU";
        master_slider.text = "MASTER";
        music_slider.text = "MUSIC";
        sfx_slider.text = "SFX";
        back_button.text = "BACK";
    }

    public void Back()
    {
        // Debug.Log($"LastState: {gm.last_state}");

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
