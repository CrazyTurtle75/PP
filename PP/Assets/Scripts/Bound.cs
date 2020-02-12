using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour
{
    //public Vector2 screenBounds;
    //public GameObject cam;
    //Camera mainCam;
    //Vector2 res;
    public enum btype { wall, chaser };
    public btype type;
    public float chaseSpeed;
    public float chaseDelay;

    // Start is called before the first frame update
    void Start()
    {
        //res = new Vector2 (Screen.width, Screen.height);
        //mainCam = cam.GetComponent<Camera>();
        //Reposition();

        if (type  == btype.chaser){
            FindObjectOfType<GenusManager>().GenFinish += OnGenFinish;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (type == btype.chaser){
                //moves position forward by chaseSpeed every second after the chaseDelay has been passed
                transform.position = new Vector3(transform.position.x + chaseSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    void OnGenFinish(){
        if (type == btype.chaser){
            transform.position = new Vector3(-7f - chaseDelay*chaseSpeed, 0, 8);
        }
    }

}
