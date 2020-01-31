using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlane : MonoBehaviour
{
    // Start is called before the first frame update
    public SpeciesManager man;
    //[HideInInspector]
    public int numToFollow;

    // Update is called once per frame
    void Update()
    {
        if (numToFollow < man.cars.Count){
            numToFollow = man.cars.Count-1;
            Vector3 pos = man.cars[numToFollow].gameObject.transform.position;
            transform.position = new Vector3 (pos.x, pos.y, -1f);
        } else {
            Debug.LogError("num to follow greater than cars list length");
        }
    }
}
