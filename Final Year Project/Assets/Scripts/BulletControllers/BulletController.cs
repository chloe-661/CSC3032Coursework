using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float SPEED = 50f;

    private Rigidbody rb;
    void Start()
    {
        this.rb = this.GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

//METHODS --------------------------------------------------------------------------------------------------------

    public void move(Vector3 direction){
        this.rb.velocity = direction * this.SPEED;
    }

//COLLISION METHODS -----------------------------------------------------------------------------------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground" 
            || other.gameObject.tag == "RedPlayer" || other.gameObject.tag == "BluePlayer")
        {
            Destroy(this.gameObject);
            Debug.Log("Destroying bullet - trigger");
        }
    }

    private void OnCollisionEnter (Collision other) {
        if (other.gameObject.tag == "Wall" || other.gameObject.tag == "Ground"){    
            Destroy(this.gameObject);
        }
        else if (other.gameObject.tag == "RedPlayer" || other.gameObject.tag == "BluePlayer"){
            Destroy(this.gameObject);
            Debug.Log("Destroying bullet");
        }
    }
}
