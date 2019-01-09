using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DisableObjects : NetworkBehaviour
{
    public GameObject[] targets;

	void Start ()
    { 
		if (!isLocalPlayer)
        {
            foreach(var obj in targets)
            {
                obj.SetActive(false);
            }
        }
	}
}
