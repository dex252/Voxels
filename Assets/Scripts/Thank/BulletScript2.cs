using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript2 : MonoBehaviour
{
    public float speed;
    public int damage;
    public float timeLife;
    public HealtPointer healthPointer;

    private double timeOfLiveBullet = 0;

    private void Start()
    {
    
    }
    // Update is called once per frame
    void FixedUpdate()
    {

        Vector3 newVelocity = this.transform.forward; //* (baseSpeed + currectVelocity.magnitude);
        newVelocity = newVelocity.normalized;
        newVelocity = newVelocity * speed * Time.deltaTime;
        this.transform.position += newVelocity;

        this.timeOfLiveBullet += Time.deltaTime;
        if (this.timeOfLiveBullet >= this.timeLife)
        {
            Destroy(this.gameObject);
            Debug.Log("Destroy bullet time");
        }
    }

    public void FireProjectile(Vector3 rotation)
    {
        this.transform.rotation= Quaternion.Euler(rotation);
    }

    void OnCollisionEnter(Collision col)
    {
        GameObject enemy = col.gameObject;

        if ( enemy.tag == "Player1" || enemy.tag == "Player2")
        {
            healthPointer = enemy.GetComponent<HealtPointer>();
            Debug.Log("Touch enemy");
            Debug.Log("Destroy bullet");
            healthPointer.TakeDamage();
            Destroy(this.gameObject);
        }

        Debug.Log("Destroy bullet");
        Destroy(this.gameObject);
    }

}
