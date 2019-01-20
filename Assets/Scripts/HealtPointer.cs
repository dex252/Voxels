using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealtPointer : MonoBehaviour
{
    [SerializeField] GameObject enemyTank;
    [SerializeField] Text text;
    [SerializeField] Image win;
    [SerializeField] GameObject deathEffect;

    private int health;
    private bool endChecker;
    private bool finalChecker;
    private HealtPointer enemyHealth;

    private void Start()
    {
        finalChecker = false;
        win.gameObject.SetActive(false);
        health = 100;
        text.text = "Health: " + health;
        endChecker = false;
        enemyHealth = enemyTank.gameObject.GetComponent<HealtPointer>();
    }

    public void TakeDamage()
    {
        System.Random rand = new System.Random();
        health = health - rand.Next (15,25);

        if (health <= 0 && enemyHealth.health > 0 && endChecker == false && enemyHealth.endChecker == false)
        {
            endChecker = true;
            enemyHealth.WinGame();
            GameOver();
        }
        else
        {
            if (health <= 0 && enemyHealth.health <= 0 && enemyHealth.endChecker == true)
            {
                health = 9999;
            }
        }

        text.text = "Health: " + health;
    }

    public void TakeSmallDamage()
    {
        System.Random rand = new System.Random();
        health = health - rand.Next(3, 10);

        if (health <= 0 && enemyHealth.health > 0 && endChecker == false && enemyHealth.endChecker == false)
        {
            endChecker = true;
            enemyHealth.WinGame();
            GameOver();
        }
        else
        {
            if (health <= 0 && enemyHealth.health <= 0 && enemyHealth.endChecker == true)
            {
                health = 9999;
            }
        }

        text.text = "Health: " + health;
    }


    private void FixedUpdate()
    {
        if (Input.anyKey && finalChecker == true)
        {
            SceneManager.LoadScene("Menu");
        }
    }
    
    private void WinGame()
    {
        win.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        DestroyAnimation();
        StartCoroutine(WaitLoadWin());
    }

    private void DestroyAnimation()
    {
        Destroy(Instantiate(deathEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, transform.position)) as GameObject, 5f);
    }

    IEnumerator WaitLoadWin()
    {
        for (int i = 0; i < 251; i++)
        {
            if (i == 250)
            {
                finalChecker = true;
                enemyHealth.finalChecker = true;
            }
            yield return null;
        }
       
    }
}
