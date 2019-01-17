using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRig : MonoBehaviour {

    public float moveSpeed;
    public float xIndented= 0f;
    public float yIndented = 0.8f;
    public float zIndented = -1.0f;
    public float xRotation = 24.482f;

    public GameObject target;
    public Transform rotationObject;

    private Transform rigTransform;
    private Transform characterTransform;

    void Start () {

        rigTransform = this.transform;//.parent;
        characterTransform = rotationObject.GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            return;
        }

        Transform Target = rigTransform;
        Target.position = Vector3.Lerp(Target.position, target.transform.position, Time.deltaTime * moveSpeed);
        Target.position += new Vector3(xIndented, yIndented, zIndented);
        rigTransform.position = Target.position;
        Vector3 rotationValue = new Vector3(xRotation, characterTransform.rotation.eulerAngles.y, 0);
        this.transform.rotation = Quaternion.Euler(rotationValue);

    }

}
