using PathFinderNameSpace;
using System.Collections.Generic;
using UnityEngine;

public class SkyNet2019 : MonoBehaviour
{
    public float xOffset = 0, yOffset = 0;
    public int speed = 1;
    public int speedRotate;

    private PathFinder pathFinder;
    private List<Vector2> localListWay = new List<Vector2>();
    private Transform enemyTransform;
    private System.Random rnd = new System.Random();

    private void Start()
    {
        enemyTransform = this.GetComponent<Transform>();
        if (speed < 0)
        {
            speed = 0;
            Debug.Log("Скорость не может быть отрицательной !");
        }
        localListWay.Add(new Vector2(0, 0)); 
        pathFinder = this.GetComponent<PathFinder>();
 
    }

    private void Update()
    {
        if (localListWay.Count > 1)
        {
            if (localListWay[0].x != localListWay[1].x || localListWay[0].y != localListWay[1].y)
            {
                Vector3 newRotate = rotateFinder(new Vector2(localListWay[0].x, localListWay[0].y), new Vector2(localListWay[1].x, localListWay[1].y));

             //   System.IO.File.AppendAllText(@"D:\logs.txt", "localListWay[0].x = " + localListWay[0].x + "    localListWay[0].y = " + localListWay[0].y + System.Environment.NewLine);
           //     System.IO.File.AppendAllText(@"D:\logs.txt", "localListWay[1].x = " + localListWay[1].x + "    localListWay[1].y = " + localListWay[1].y + System.Environment.NewLine);
           //     System.IO.File.AppendAllText(@"D:\logs.txt", "current Rotation = " + this.transform.rotation.eulerAngles.y + "    newRotation = " + newRotate + System.Environment.NewLine);

                
                if (this.transform.rotation.eulerAngles.y == newRotate.y)
                {

                    Vector2 newLocation = Vector2.MoveTowards(new Vector2(localListWay[0].x + xOffset, localListWay[0].y + yOffset),
                    new Vector2(localListWay[1].x + xOffset, localListWay[1].y + yOffset), Time.deltaTime * speed);

                    enemyTransform.position = new Vector3(newLocation.x, enemyTransform.position.y, newLocation.y);
                    localListWay[0] = new Vector2(newLocation.x, newLocation.y);
                }
                else
                {
                    this.transform.rotation = Quaternion.Euler(newRotate);
                }

            }

            else
            {
                localListWay.RemoveAt(0);
            }
        }
        else
        {

                int x = rnd.Next(0, pathFinder.mapWay.ChunksMap.Count - 1);
                int y = rnd.Next(0, pathFinder.mapWay.ChunksMap[0].Count - 1);

                List<Vector2> list = pathFinder.mapWay.FindWay(new Vector2(localListWay[0].y / 10, localListWay[0].x / 10), new Vector2(y, x));
                UpdateWay(list);

        }
    }

    float RoundToFraction(float value, float fraction)
    {
        return (float)System.Math.Round(value / fraction) * fraction;
    }

    public bool AddWay(List<Vector2> list)
    {
        if (list == null || list.Count == 0)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < list.Count; i++)
            {
                localListWay.Add(new Vector2(list[i].x, list[i].y));
            }
            return true;
        }
    }

    public bool UpdateWay(List<Vector2> list)
    {
        if (list == null || list.Count == 0)
        {
            return false;
        }
        else
        {
            for (int i = 0; i < localListWay.Count; i++)
            {
                localListWay.RemoveAt(0);
            }
            AddWay(list);
            return true;
        }
    }

    public bool Head(ref Vector2 headValue)
    {
        if (localListWay.Count == 0)
        {
            return false;
        }
        else
        {
            headValue.Set(localListWay[0].x, localListWay[0].y);  //возможны сбои в Vector2.Set => можно заменить на headValue=new Vector2(localListWay[0].x, localListWay[0].y);
            return true;
        }
    }


    public Vector3 rotateFinder(Vector2 start, Vector2 last)
    {
        Vector3 rotetionForBody = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        float yRotate = this.transform.rotation.eulerAngles.y;

        if (start == last)
        {
            rotetionForBody = new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
        }
        else
        {
            if (start.x == last.x && start.y != last.y)
            {
                if (start.y < last.y)
                {
                    rotetionForBody = Vector3.MoveTowards(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z),
                    new Vector3(this.transform.rotation.eulerAngles.x, 0, this.transform.rotation.eulerAngles.z), Time.deltaTime * speedRotate);


                }
                else
                {
                     rotetionForBody = Vector3.MoveTowards(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z),
                     new Vector3(this.transform.rotation.eulerAngles.x, 0/*180*/, this.transform.rotation.eulerAngles.z), Time.deltaTime * speedRotate);


                }
            }
            else
            {
                if (start.x > last.x)
                {
                    rotetionForBody = Vector3.MoveTowards(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z),
                    new Vector3(this.transform.rotation.eulerAngles.x, /*270*/90, this.transform.rotation.eulerAngles.z), Time.deltaTime * speedRotate);


                }
                else
                {
                    rotetionForBody = Vector3.MoveTowards(new Vector3(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z),
                    new Vector3(this.transform.rotation.eulerAngles.x, 90, this.transform.rotation.eulerAngles.z), Time.deltaTime * speedRotate);


                }
            }
        }

        return rotetionForBody;
    }
}
