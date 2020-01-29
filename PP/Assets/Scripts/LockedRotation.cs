using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedRotation : MonoBehaviour
{
    // Start is called before the first frame update
    Quaternion iniRotation;
    
    void Start()
    {
        iniRotation = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = iniRotation;
    }
}
