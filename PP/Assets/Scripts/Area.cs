using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    // Start is called before the first frame update
    public float cps;
    public float eps;
    public float radius;
    public enum shape {
        circle,
        square
    }
    public shape type;

    public List<Plane> planes;

    void Start(){
        FindObjectOfType<GenusManager>().GenFinish += OnGenFinish;
        SpeciesManager[] sm = FindObjectsOfType<SpeciesManager>();
        for (int i = 0; i < sm.Length; i++)
        {
            Debug.Log("doing stuff");
            planes.AddRange(sm[i].cars);
        }
    }

    // Update is called once per frame
    void Update()
    {
        FindInternalPlanes();
    }

    void OnGenFinish (){
        Debug.Log("gen finish");
        StartCoroutine("LateOnGenFinish");
    }

    IEnumerator LateOnGenFinish (){
        planes = new List<Plane>();
        Debug.Log("late gen finish");
        yield return new WaitForSeconds (.1f);
        SpeciesManager[] sm = FindObjectsOfType<SpeciesManager>();
        for (int i = 0; i < sm.Length; i++)
        {
            Debug.Log("doing stuff");
            planes.AddRange(sm[i].cars);
        }
    }

    void FindInternalPlanes(){
        if (type == shape.circle){
            foreach (Plane p in planes)
            {
                if (p != null){
                    if (p.GetComponent<Rigidbody2D>() != null){
                        if (Vector3.Distance(p.transform.position, transform.position) <= radius){
                            DoDamage(p);
                        }
                    }
                }
            }
        } else if (type == shape.square){
            foreach (Plane p in planes)
            {
                if (p != null){
                    if (p.GetComponent<Rigidbody2D>() != null){
                        Vector3 pos = p.transform.position;
                        Vector3 myPos = transform.position;
                        if (Mathf.Abs(pos.x-myPos.x) <= radius || Mathf.Abs(pos.y-myPos.y) <= radius){
                            DoDamage(p);
                        }
                    }
                }
            }
        } else{
            Debug.LogWarning("no shape found for area");
        }
    }

    void DoDamage(Plane plane){
        plane.ep += eps * Time.deltaTime;
        plane.cp += cps * Time.deltaTime;
    }
    #if (UNITY_EDITOR)
        void OnDrawGizmos(){
        Gizmos.color = Color.red;

        if (type == shape.circle){
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        if (type == shape.square){
            float r = 2*radius;
            Gizmos.DrawWireCube(transform.position, new Vector3 (r, r, r));
        }
    }
    #endif
}
