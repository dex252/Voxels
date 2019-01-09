using UnityEngine;
using UnityEngine.Networking;

namespace ObjectStatus
{
    public class ObjectStatusScript : NetworkBehaviour
    {
        [SyncVar]
        public int currentHealth = maxHealth;
        [SyncVar]
        public int score;

        [SerializeField] private Transform parent;
        [SerializeField] private int teamIndex;

        [SerializeField] private const int maxHealth = 100;

        public int TeamIndex
        {
            get
            {
                return teamIndex;
            }

            set
            {
                teamIndex = value;
            }
        }

        public Transform Parent
        {
            get
            {
                return parent;
            }

            set
            {
                parent = value;
            }
        }

        public void TakeDamage(int amount , Transform damager)
        {
            if (!isServer)
            {
                return;
            }

            currentHealth -= amount;
            if (currentHealth <= 0)
            {
                DestroySelf();
            }
        }

        public void DestroySelf()
        {
            if (Parent != null && Parent.GetComponent<ObjectStatusScript>() != null)
            {

                switch (gameObject.tag)
                {
                    case "Bullet":
                        break;
                    case "Tanks":
                        break;
                    case "Player":
                        break;
                }
            }
            Destroy(gameObject);
        }
    }
}

