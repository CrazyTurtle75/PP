using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlaneNumText : LockedRotation
{
    // Start is called before the first frame update
    SpeciesManager manager;
    int numToFollow;
    
    void Start()
    {
        manager = GetComponentInParent<FollowPlane>().man;
        numToFollow = GetComponentInParent<FollowPlane>().numToFollow;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TextMeshPro>().text = manager.networks[numToFollow].n.ToString();
    }
}
