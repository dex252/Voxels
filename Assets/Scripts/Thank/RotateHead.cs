using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHead : MonoBehaviour
{
    const float speedOfRotate = 70f;

    // Start is called before the first frame update
    public LayerMask mask;
    public Transform transformOfBody;
    public Camera cam;
    public Vector3 lastRotate;
    public Vector3 lastDirection;

    private Transform transformOfHead;

    void Start()
    {
        transformOfHead = this.GetComponent<Transform>();
        lastRotate = new Vector3(0,0,0);
        lastDirection = new Vector3(0, 0, 0);
    }

    void Update()
    {
        //raycastOnMouseEntered();
        towerRotation();
    }

    void raycastOnMouseEntered() //создание лучей и объектов при нажатии кнопки мыши
    {
        RaycastHit hit;
        Ray rayToFloor = cam.ScreenPointToRay(Input.mousePosition); //Узнаем точку соприкосновения от амеры до объекта 
        if (Physics.Raycast(rayToFloor, out hit, 100.0f, mask, QueryTriggerInteraction.Collide)) // стрельба засчитывается, если луч пересекает пол с заданным расстоянием в 100f
        {                                                           Debug.DrawRay(rayToFloor.origin, rayToFloor.direction * 100.1f, Color.red, 2);
            float curAngle = Mathf.Atan2(hit.point.x- this.transform.position.x, hit.point.z - this.transform.position.z) / Mathf.PI * 180;
            curAngle = (curAngle < 0) ? curAngle + 360 : curAngle;
            Vector3 rotation= Vector3.Lerp(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z), 
                                                                        new Vector3(this.transform.rotation.eulerAngles.x, curAngle, this.transform.rotation.eulerAngles.z), 0.1f);
            this.transform.rotation = Quaternion.Euler(rotation);
            this.lastDirection = new Vector3(hit.point.x - this.transform.position.x, this.transform.position.y, hit.point.z - this.transform.position.z);

            lastRotate = rotation;
        }
    }

    private void towerRotation()
    {
        if (Input.GetKey(KeyCode.PageDown))
        {

            Vector3 rotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y + Time.deltaTime * speedOfRotate), rotation.z);
            this.transform.rotation = Quaternion.Euler(rotation);
            //lastRotate = rotation;
        }

        if (Input.GetKey(KeyCode.Delete))
        {

            Vector3 rotation = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
            rotation.Set(rotation.x, (rotation.y - Time.deltaTime * speedOfRotate), rotation.z);
            this.transform.rotation = Quaternion.Euler(rotation);
            //lastRotate = rotation;
        }
    }

    public void Remath()
    {
        lastRotate = new Vector3(this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z); 
    }
}
