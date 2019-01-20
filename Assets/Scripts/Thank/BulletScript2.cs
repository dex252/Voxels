using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript2 : MonoBehaviour
{
    public float speed;
    public int damage;
    public float timeLife;
    public HealtPointer healthPointer;
    private botHealth botHealth;

    private string whoseBullet;


    private double timeOfLiveBullet = 0;

    private void Awake()
    {
        whoseBullet = "";
    }

    private void FixedUpdate()
    {

        Vector3 newVelocity = this.transform.forward;

        newVelocity = newVelocity.normalized;
        newVelocity = newVelocity * speed * Time.deltaTime;
        this.transform.position += newVelocity;

        this.timeOfLiveBullet += Time.deltaTime;
        if (this.timeOfLiveBullet >= this.timeLife)
        {
            Destroy(this.gameObject);
            Debug.Log("Destroy bullet time");
        }

    }

    public void FireProjectile(Vector3 rotation, string whoseBullet)
    {
        this.transform.rotation = Quaternion.Euler(rotation);
        this.whoseBullet = whoseBullet;
    }

    private void OnCollisionEnter(Collision col)
    {
        GameObject enemy = col.gameObject;

        if (whoseBullet == "Player1" || whoseBullet == "Player2")
        {
            if (enemy.tag == "Player1" || enemy.tag == "Player2")
            {
                healthPointer = enemy.GetComponent<HealtPointer>();
                Debug.Log("Touch player-player");
                Debug.Log("Destroy bullet");
                healthPointer.TakeDamage();
                Destroy(this.gameObject);
            }

        }

        if (whoseBullet == "Bot")
        {
            if (enemy.tag == "Player1" || enemy.tag == "Player2")
            {
                healthPointer = enemy.GetComponent<HealtPointer>();
                Debug.Log("Touch bot-player");
                healthPointer.TakeSmallDamage();
                Destroy(this.gameObject);
            }
        }


        Debug.Log("Destroy bullet");
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject enemy = other.gameObject;

        if (whoseBullet == "Player1" || whoseBullet == "Player2")
        {

            if (enemy.tag == "Bot")
            {
                botHealth = enemy.GetComponent<botHealth>();
                Debug.Log("Touch player-bot");
                botHealth.TakeBotDamage();
                Destroy(this.gameObject);
            }
        }


        if (whoseBullet == "Bot")
        {

            if (enemy.tag == "Bot")
            {
                Debug.Log("Touch bot-bot");
                Destroy(this.gameObject);
            }
        }
    }

}
