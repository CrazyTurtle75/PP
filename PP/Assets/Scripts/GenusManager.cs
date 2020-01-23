using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;

public class GenusManager : MonoBehaviour
{
    // Start is called before the first frame update
    public SpeciesManager[] managers;
    public float timeframe;
    float lastSpawnTime;
    public event Action GenFinish;
    [Range(0.1f, 20f)] public float gameSpeed = 1f;
    public bool demoMode;
    public int generationNumber = 0;
    public bool PC;
    Plane b;
    public Material pcMAT;
    public int livingBots;
    public GameObject numText;
    public GameObject numTextParent;
    public GameObject bot;
    int totalBots;
    

    [Header("Objects")]
    public TextMeshProUGUI genSign;
    public TextMeshProUGUI botNumSign;
    public Slider timeSlider;
    public Slider botsSlider;
    public TextMeshProUGUI[] fitnessSigns; 
    public CameraFollow cam;
    [Tooltip("The location of the generation UI text")]
    public Transform genLoc;
    public Bound chaser;
    const string letters = "abcdefghijklmnopqrstuvwxyz";



    void Awake(){
        UpdateManagers();
        for (int i = 0; i < managers.Length; i++)
        {
            GameObject f = Instantiate(numText, new Vector3 (0, 0, 0), new Quaternion (0, 0, 0, 0), numTextParent.transform);
            FollowPlane j = f.GetComponent<FollowPlane>();
            j.man = managers[i];
            if (j.man.popSize == 0){
                j.man.popSize = 60/managers.Length;
            }
            f.name = "TextHolder" + letters[i];
            j.numToFollow = j.man.popSize-1;
            j.enabled = true;
            totalBots += managers[i].popSize;
        }
        for (int i = 0; i < fitnessSigns.Length; i++)
        {
            if (i < managers.Length){
                fitnessSigns[i].color = managers[i].goodMat.color;
            } else {
                Debug.Log("removing");
                fitnessSigns[i].gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        //bool check = b.GetComponent<Rigidbody2D>() == null;
        timeSlider.value = (Time.time-lastSpawnTime)/timeframe;
        botNumSign.text = "living bots:" + livingBots;
        botsSlider.value = (float)livingBots/(float)totalBots;
        Debug.Log((float)livingBots/(float)totalBots);

        if ((CheckManagers()) || (Time.time-lastSpawnTime >= timeframe)){
            
            if (GenFinish != null){
                FinishGen();
            } else {
                Debug.LogError("nobody subscribed to genfinish");
            }
            UpdateUI();
        }

        Time.timeScale = gameSpeed;

        int j = 0;
        for (int i = 0; i < managers.Length; i++)
        {
            j += managers[i].livingBots;
        }
        livingBots = j;
    }

    public void UpdateUI(){
        genSign.text = "gen #"+generationNumber;

        for (int i = 0; i < fitnessSigns.Length; i++)
        {
            if (i < managers.Length){
                fitnessSigns[i].text = Math.Round(managers[i].bestFit, 2).ToString();
            }
        }
    }

    public void UpdateManagers(){
        managers = FindObjectsOfType<SpeciesManager>().OrderBy( go => go.name ).ToArray();
        for (int i = 0; i < managers.Length; i++)
        {
            managers[i].n = letters[i].ToString();
            managers[i].num = i + 1;
        }
    }

    void FinishGen(){
        GenFinish();
        lastSpawnTime = Time.time;
        StartCoroutine("LateOnGenFinish");
        generationNumber++;
    }

    IEnumerator LateOnGenFinish (){
        yield return new WaitForSeconds (.1f);
        float highestFit = 0;
        for (int i = 0; i < managers.Length; i++){
            if (managers[i].bestFit>highestFit){
                cam.objectToFollow = managers[i].cars[managers[i].popSize-1].gameObject;
                highestFit = managers[i].bestFit;
            }
        }
        if (PC){
            b = Instantiate(bot, new Vector3 (0, 0, 0), new Quaternion (0, 0, -0.7f, 0.7f)).GetComponent<Plane>();
            b.playerControlled = true;
            b.gameObject.name = "pc";
            b.gameObject.GetComponent<Renderer>().material = pcMAT;
        }
    }
    //cam.objectToFollow = car.gameObject;

    bool CheckManagers(){
        foreach (SpeciesManager man in managers)
        {
            if (man.livingBots>0){
                return false;
            }
        }
        return true;
    }

    public void KillAll(){
        FinishGen();
    }
}
