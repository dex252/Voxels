using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botHealth : MonoBehaviour
{
    [SerializeField]GameObject bot;

    public void TakeBotDamage()
    {
        Destroy(bot.gameObject);
    }
}
