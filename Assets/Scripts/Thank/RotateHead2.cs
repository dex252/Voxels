using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHead2 : MonoBehaviour
{
    // Start is called before the first frame update
    public LayerMask mask;
    public Transform transformOfBody;
    public Camera cam;
    public Vector3 lastRotate;
    public Vector3 lastDirection;
    public float speedOfRotate;

    private Transform transformOfHead;

    void Start()
    {
        transformOfHead = this.GetComponent<Transform>();
        lastRotate = new Vector3(0, 0, 0);
        lastDirection = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {

            Vector3 rotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y + Time.deltaTime * speedOfRotate), rotation.z);
            this.transform.rotation = Quaternion.Euler(rotation);
        }

        if (Input.GetKey(KeyCode.Z))
        {

            Vector3 rotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y - Time.deltaTime * speedOfRotate), rotation.z);
            this.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public void Remath()
    {
        lastRotate = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
    }

}