using UnityEngine;
using UnityEngine.Networking;
using Bullet;
using ObjectStatus;

namespace ShootingScript
{
    public class TankShootingScript : NetworkBehaviour
    {

        [SerializeField] private float barrelCooldownStatus = 0;
        [SerializeField] private GameObject bulletPrefab;
        [SerializeField] private Transform[] bulletSpawns;
        [SerializeField] private float coolDownTime;

        [SerializeField] private float clipsReloadTime;
        [SerializeField] private float clipsReloadStatus;

        [SerializeField] private int bulletsInClips;
        [SerializeField] private int maxBulletsInClips;

        [Command]
        void CmdShoot()
        {
            if (bulletsInClips > 0)
            {
                if (barrelCooldownStatus == 0)
                {
                    foreach (var spawnPoint in bulletSpawns)
                    {
                        barrelCooldownStatus = coolDownTime;
                        var bullet = (GameObject) Instantiate(
                            bulletPrefab ,
                            spawnPoint.position ,
                            spawnPoint.rotation);
                        var objectStatus = gameObject.GetComponent<ObjectStatusScript>();

                        bullet.GetComponent<BulletScript>().FriendlyFire = false;
                        bullet.GetComponent<ObjectStatusScript>().TeamIndex = objectStatus.TeamIndex;
                        bullet.GetComponent<ObjectStatusScript>().Parent = transform;

                        NetworkServer.Spawn(bullet);
                        Destroy(bullet , 5.0f);
                    }

                    bulletsInClips--;
                    if (bulletsInClips == 0)
                    {
                        clipsReloadStatus = clipsReloadTime;
                    }
                }
            }
        }

        public void Shoot()
        {
            CmdShoot();
        }

        private void FixedUpdate()
        {
            barrelCooldownStatus = Mathf.Max(0, barrelCooldownStatus - Time.deltaTime);
            clipsReloadStatus = Mathf.Max(0, clipsReloadStatus - Time.deltaTime);

            if (clipsReloadStatus <= 0 && bulletsInClips==0)
            {
                bulletsInClips = maxBulletsInClips;
            }
        }
    }
}
