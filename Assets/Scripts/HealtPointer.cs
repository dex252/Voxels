using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HealtPointer : MonoBehaviour
{
    [SerializeField] GameObject enemyTank;
    [SerializeField] Text text;
    [SerializeField] Image lose;

    private int health;
    private bool endChecker;
    private HealtPointer enemyHealth;

    private void Start()
    {
        lose.gameObject.SetActive(false);
        health = 100;
        text.text = "Health: " + health;
        endChecker = false;
        enemyHealth = enemyTank.gameObject.GetComponent<HealtPointer>();
    }

    public void TakeDamage()
    {
        System.Random rand = new System.Random();
        health = health - rand.Next (15,25);

        if (health <= 0 && enemyHealth.endChecker == false)
        {
            health = 9999;
            GameOver();
        }

        text.text = "Health: " + health;
    }

    public void TakeSmallDamage()
    {
        System.Random rand = new System.Random();
        health = health - rand.Next(3, 10);

        if (health <= 0 && enemyHealth.endChecker == false)
        {
            health = 9999;
            GameOver();
        }

        text.text = "Health: " + health;
    }


    private void FixedUpdate()
    {
        if (Input.anyKey && endChecker == true)
        {
            SceneManager.LoadScene("Menu");
        }
    }
    private void GameOver()
    {
        lose.gameObject.SetActive(true);
        StartCoroutine(WaitLoadWin());
    }

    IEnumerator WaitLoadWin()
    {
        for (int i = 0; i < 201; i++)
        {
            if (i == 200)
            {
                endChecker = true;
            }
           
            yield return null;
        }
       
    }
}
