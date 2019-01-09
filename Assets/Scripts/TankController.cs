using NetWork;
using ShootingScript;
using UnityEngine;
using UnityEngine.Networking;

namespace TankController
{
    [RequireComponent(typeof(NetworkScript))]
    [RequireComponent(typeof(TankShootingScript))]
    public class TankController : NetworkBehaviour
    {
        private NetworkScript networkScript;
        private TankShootingScript shootingScript;

        // Update is called once per frame
        private void Start()
        {
            networkScript = gameObject.GetComponent<NetworkScript>();
            shootingScript = gameObject.GetComponent<TankShootingScript>();
        }

        private void FixedUpdate()
        {
            if (hasAuthority)
            {
                if (!Input.GetMouseButton(1))
                {
                    if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                    {
                        networkScript.Move(Input.GetAxis("Horizontal") , Input.GetAxis("Vertical"));
                    }

                    if (Input.GetKey(KeyCode.Space))
                    {
                        shootingScript.Shoot();
                    }
                }
            }

        }
    }
}
