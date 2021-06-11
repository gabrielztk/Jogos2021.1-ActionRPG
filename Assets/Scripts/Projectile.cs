using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    GameManager gm;
    public int damage;

    private void Start()
    {
        Debug.Log("hi");
        gm = GameManager.GetInstance();
    }

    private void Update()
    {
        
        if (gm.gameState == GameManager.GameState.ENDGAME){
            Destroy(gameObject);
        }
        if (gm.gameState != GameManager.GameState.GAME) return;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("colide");
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyAi>().TakeDamage(damage);
        }
        Destroy(gameObject);  
    }
}
