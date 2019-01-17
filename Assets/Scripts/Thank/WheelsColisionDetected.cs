using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WheelsScript {
    public class WheelsColisionDetected : MonoBehaviour
    {
        public bool rotateLocker = false;

        private void OnTriggerEnter(Collider other)
        {
            rotateLocker = true;
        }

        private void OnTriggerExit(Collider other)
        {
            rotateLocker = false;
        }
    }
}
