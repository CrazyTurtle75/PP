using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
   public GameObject objectToFollow;
   public float offset;
   public float lockedY;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(GetComponent<Camera>().orthographicSize);
        if (objectToFollow != null){
            Vector3 pos = objectToFollow.gameObject.transform.position;
            transform.position = new Vector3 ((pos.x + offset), lockedY, -10);
        }
    }
}
