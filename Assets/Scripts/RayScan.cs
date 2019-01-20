using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayScan : MonoBehaviour
{

    private GameObject target1;
    private GameObject target2;
    public bool result;

    public int rays;
    public int distance;
    public float angle;

    public RayScan ray;

    // Start is called before the first frame update
    void Start()
    {
        ray = this.gameObject.GetComponent<RayScan>();
        ray.result = false;
        ray.target1 = GameObject.FindGameObjectWithTag("Player1");
        ray.target2 = GameObject.FindGameObjectWithTag("Player2");
    }

    bool GetRaycast(Vector3 dir)
    {
        bool result = false;
        RaycastHit hit = new RaycastHit();
        Vector3 pos = ray.transform.position;
        if (Physics.Raycast(pos, dir, out hit, distance))
        {
            if (hit.transform == target1.transform || hit.transform == target2.transform)
            {
                result = true;
                Debug.DrawLine(pos, hit.point, Color.green);
            }
            else
            {
                Debug.DrawLine(pos, hit.point, Color.blue);
            }
        }
        else
        {
            Debug.DrawRay(pos, dir * distance, Color.red);
        }
        return result;
    }

    bool RayToScan()
    {
        bool result = false;
        bool a = false;
        bool b = false;
        float j = 0;
        for (int i = 0; i < rays; i++)
        {
            var x = Mathf.Sin(j);
            var y = Mathf.Cos(j);

            j += angle * Mathf.Deg2Rad / rays;

            Vector3 dir = ray.transform.TransformDirection(new Vector3(x, 0, y));
            if (GetRaycast(dir)) a = true;

            if (x != 0)
            {
                dir = ray.transform.TransformDirection(new Vector3(-x, 0, y));
                if (GetRaycast(dir)) b = true;
            }
        }

        if (a || b) result = true;
        return result;
    }

    void Update()
    {
        if (Vector3.Distance(new Vector3(ray.transform.position.x, 0, ray.transform.position.z), target1.transform.position) < distance || Vector3.Distance(new Vector3(ray.transform.position.x, 0, ray.transform.position.z), target2.transform.position) < distance)
        {
            if (RayToScan())
            {
                ray.result = true;
                // Контакт с целью
            }
            else
            {
                ray.result = false;
                //контакт закончен
            }
        }
    }
}
