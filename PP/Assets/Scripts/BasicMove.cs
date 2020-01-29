using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class BasicMove : MonoBehaviour
{
    // Start is called before the first frame update
    #region 
        [Space (7)]
        [Header ("Speed Settings")]
        public float speedChange;
        [Range (0, 100)]
        public float speed;
        public float maxSpeed;
        public float rotationSpeed;
        public Rigidbody2D self;
        [Space (7)]
        [Header ("Plane Statistics")]
        public float currentActualSpeed;
        public float currentConvertedSpeed;
        public float averageActualSpeed;
        public float averageConvertedSpeed;
        
        List<float> veloKeys = new List<float>();
        
        [HideInInspector]
        public float lastVeloAdd;
        [HideInInspector]
        public float veloAddWait = 2f;

    #endregion
    
    void Start()
    {
        self = gameObject.GetComponent<Rigidbody2D>();
        lastVeloAdd = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Move();
        //FindSpeedData();
        //ChangeSpeed();
        
    }

    public void FindSpeedData(){
        
        currentActualSpeed =  self.velocity.magnitude;

        //converted speed from units/s to km/s of a cruising airliner
        currentConvertedSpeed = currentActualSpeed*.117f;
            
        if ((Time.time-lastVeloAdd)>=veloAddWait){  
            veloKeys.Add(self.velocity.magnitude);
            lastVeloAdd = Time.time;
        }

        if(veloKeys.Count>0){
            averageActualSpeed = veloKeys.Average();
        }
    }

    public void Move() {
        if (Input.GetKey(KeyCode.LeftArrow)||(Input.GetKey(KeyCode.A))){
            TurnLeft();
        }
        if (Input.GetKey(KeyCode.RightArrow)||(Input.GetKey(KeyCode.D))){
            TurnRight();
        }
    }

    public void TurnLeft() {
        transform.Rotate(0, 0, rotationSpeed*Time.deltaTime);
    }

    public void TurnRight() {
        transform.Rotate(0, 0, rotationSpeed*Time.deltaTime*-1);
    }


    public void ChangeSpeed() {
        if (Input.GetKey(KeyCode.UpArrow)){
            if (speed < 100){
                speed = speed + speedChange*Time.deltaTime;
            } else {
                speed = 100;
            }
                
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            if (speed > 0){
                speed = speed - speedChange*Time.deltaTime;
            } else {
                speed = 0;
            }
        }
    }
}
