using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WheelsScript;

public class MoveAndRotateBody : MonoBehaviour
{

    public float speedOfRotate = 3;
    public float moveAcceleration = 30; //ускорение
    public float maxSpeedOfMove = 20;
    public GameObject wheelsLeftObject;
    public GameObject wheelsRightObject;
    public bool IgnoreOfColision = false;

   
    private Rigidbody rigidBody;
    private Transform characterObject;
    private WheelsColisionDetected wheelsRight;
    private WheelsColisionDetected wheelsLeft;
    private float localSpeed = 0;
    private int traction = 0;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        characterObject = GetComponent<Transform>();
        wheelsRight = wheelsRightObject.GetComponent<WheelsColisionDetected>();
        wheelsLeft = wheelsLeftObject.GetComponent<WheelsColisionDetected>();
    }

    private void Update()
    {
            
        if (Input.GetKey(KeyCode.D) && !wheelsRight.rotateLocker || Input.GetKey(KeyCode.D) && IgnoreOfColision) // turn right
        {
            Vector3 rotation = new Vector3(characterObject.transform.eulerAngles.x, characterObject.transform.eulerAngles.y, characterObject.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y + Time.deltaTime * speedOfRotate), rotation.z);
            characterObject.rotation = Quaternion.Euler(rotation);
        }

        if (Input.GetKey(KeyCode.A) && !wheelsLeft.rotateLocker || Input.GetKey(KeyCode.A) && IgnoreOfColision) // turn Left
        {
            Vector3 rotation = new Vector3(characterObject.transform.eulerAngles.x, characterObject.transform.eulerAngles.y, characterObject.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y - Time.deltaTime * speedOfRotate), rotation.z);
            characterObject.rotation = Quaternion.Euler(rotation);
        }

        if (Input.GetKey(KeyCode.W))
        {


            if (traction == -1)
            {
                localSpeed = 0;
            }

            traction = 1;
            localSpeed += moveAcceleration;

            if (localSpeed > maxSpeedOfMove)
            {
                localSpeed = maxSpeedOfMove;
            }

            Vector3 newVelocity = characterObject.forward; //* (baseSpeed + currectVelocity.magnitude);
            newVelocity = newVelocity.normalized;
            newVelocity = newVelocity * localSpeed * Time.deltaTime;
            characterObject.position += newVelocity;
        }

        if (Input.GetKey(KeyCode.S))
        {

            if (traction == 1)
            {
                localSpeed = 0;
            }

            traction = -1;
            localSpeed += moveAcceleration;

            if (localSpeed > maxSpeedOfMove)
            {
                localSpeed = maxSpeedOfMove;
            }

            Vector3 newVelocity = characterObject.forward; //* (baseSpeed + currectVelocity.magnitude);
            newVelocity = newVelocity.normalized;
            newVelocity.Set(newVelocity.x * (-1), newVelocity.y, newVelocity.z * (-1));
            newVelocity = newVelocity * localSpeed * Time.deltaTime;
            characterObject.position += newVelocity;
        }

        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            localSpeed = 0;
        }

    }

}
