using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class DynamicObstacle : MonoBehaviour
{
    public Vector2 velocity;
    //public Vector2 acceleration;
    [Range (20f, 200f)]
    public float velocitySpeed;
    public float delay;
    
    Rigidbody2D self;
    Vector3 startPos;
    
    
    void Start(){
        self = GetComponent<Rigidbody2D>();
        if (self == null){
            self = GetComponentInParent<Rigidbody2D>();
        }
        startPos = transform.position;
        FindObjectOfType<GenusManager>().GenFinish += OnGenFinish;
        StartCoroutine("Push");
    }

    IEnumerator Push (){
        yield return new WaitForSeconds(delay);
        self.AddForce(velocity.normalized*velocitySpeed);
    }

    void OnGenFinish(){
        transform.position = startPos;
    }

}

