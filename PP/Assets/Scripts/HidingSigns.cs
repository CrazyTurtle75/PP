using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSigns : MonoBehaviour
{
    public SceneController controller;
    public float screenBound;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        screenBound = (controller.screenBounds.x-30);
        if(transform.position.x<(controller.screenBounds.x-30)){
            //gameObject.SetActive(false);
        } else {
            gameObject.SetActive(true);
        }
    }
}
