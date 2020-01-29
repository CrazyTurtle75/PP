using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SceneController : MonoBehaviour
{
    Camera mainCam;
    public Vector2 screenBounds;
    public int numMarkersCreated = 0;
    public int markersToCreate;
    public GameObject marker;
    public GameObject markerParent;
    public float markerYPos;
    float realDistBetween;
    public float conDistBetween;
    public float buffer;
    public float scale = .117f;
    float width;
    
    
    void Start()
    {
        mainCam = gameObject.GetComponent<Camera>();
        screenBounds = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCam.transform.position.z));
        realDistBetween = conDistBetween/scale;
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnMarkers();
    }

    void SpawnMarkers(){
        screenBounds = mainCam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCam.transform.position.z));
        markersToCreate = ((int)Mathf.Ceil((screenBounds.x+buffer)/realDistBetween)-numMarkersCreated);
        
        
        for (int i = 0; i < markersToCreate; i++)
        {
            GameObject m = Instantiate(marker, new Vector3 ((numMarkersCreated)*realDistBetween, markerYPos, 5), new Quaternion(0, 0, 0, 0),markerParent.transform);
            m.name = marker.name + numMarkersCreated.ToString();
            var numOfMeters = ((numMarkersCreated)*conDistBetween);
            numMarkersCreated++;
            HidingSigns script = m.GetComponent<HidingSigns>();
            if (script != null){
                script.controller = gameObject.GetComponent<SceneController>();
            }
            
            //changes the text of each marker to match distance
            m.GetComponent<TextMeshPro>().text = (numOfMeters.ToString() + "km");

            //changes the parent of each marker to specified parent
            
        }
        //currentView = new Vector2((screenBounds.x + transform.position.x), screenBounds.y);
    }
}
