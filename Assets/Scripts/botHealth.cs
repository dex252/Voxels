using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botHealth : MonoBehaviour
{
    [SerializeField]GameObject bot;
    [SerializeField] GameObject deathEffect;

    public void TakeBotDamage(Vector3 hitDirection)   
    {
        bot.SetActive(false);
        DestroyAnimation(hitDirection);
    }

    
    private void DestroyAnimation(Vector3 hitDirection)
    {
        Destroy(Instantiate(deathEffect, bot.transform.position, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, 5f);

        Destroy(bot.gameObject);
    }
   
}
