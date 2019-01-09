using ObjectStatus;
using UnityEngine;
using UnityEngine.Networking;

namespace Bullet
{
    public class BulletScript : NetworkBehaviour
    {
        [SerializeField] private float bulletSpeed;
        [SerializeField] private int damagePoint;
        private bool friendlyFire;

        public bool FriendlyFire
        {
            get
            {
                return friendlyFire;
            }

            set
            {
                friendlyFire = value;
            }
        }

        private void Start()
        {
            gameObject.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * bulletSpeed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (isServer)
            {
                ObjectStatusScript target = collision.gameObject.GetComponent<ObjectStatusScript>();
                ObjectStatusScript selfStatus = gameObject.GetComponent<ObjectStatusScript>();
                if (target != null && (selfStatus.TeamIndex != target.TeamIndex || FriendlyFire))
                {
                    target.TakeDamage(damagePoint , selfStatus.Parent);
                }

                Destroy(gameObject);
            }
        }
    }
}
