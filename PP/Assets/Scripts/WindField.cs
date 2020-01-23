using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindField : MonoBehaviour
{
    public float[] PCo;
    public float[] QCo;
    public float strength;
    public float gizmoScale;
    public bool reverseVariableInput;
    public int maxX;
    public int minX;
    public int maxY;
    public int minY;
    public enum drawMode {
        all,
        single,
        both
    }
    public drawMode mode;

    public Vector3 Wind(float xValue, float yValue){
        float x = 0;
        float y = 0;

        x = P(xValue);
        y = Q(yValue);
        
        Vector3 windForce = new Vector3 (x, y, 0);
        return windForce;
    }

    public float P(float x){
        float p = 0;
        
        for (int i = 0; i < PCo.Length; i++)
        {
            p += Mathf.Pow(x, i) * PCo[i];
        }
        //Debug.Log(p);
        return p;
    }

    public float Q(float y){
        float q = 0;
        
        for (int i = 0; i < QCo.Length; i++)
        {
            q += Mathf.Pow(y, i) * QCo[i];
        }
        //Debug.Log(q);
        return q;
    }

    //#if (UNITY_EDITOR)
        void OnDrawGizmos(){
            if (mode == drawMode.all){
                Gizmos.color = Color.blue;
                for (int k = minY; k <= maxY; k++)
                {
                    for (int h = minX; h < maxX; h++)
                    {
                        Vector3 v;
                        if (reverseVariableInput){
                            v = new Vector3(P(k), Q(h), 0).normalized*gizmoScale;
                        } else{
                            v = new Vector3(P(h), Q(k), 0).normalized*gizmoScale;
                        }
                        
                        
                        Debug.Log(v);
                        //Debug.Log(h + ", " + k);
                        
                        Gizmos.DrawCube(new Vector3(h, k, 2f), new Vector3(.2f, .2f, .2f));
                        Gizmos.DrawLine(new Vector3(h, k, 2f), new Vector3(h + v.x, k + v.y, 2f));
                    }
                }

            } else 
            if (mode == drawMode.single){

                Gizmos.color = Color.yellow;
                float h = transform.position.x;
                float k = transform.position.y;

                Vector3 v;
                if (reverseVariableInput){
                    v = new Vector3(P(k), Q(h), 0).normalized*gizmoScale;
                } else{
                    v = new Vector3(P(h), Q(k), 0).normalized*gizmoScale;
                }

                Gizmos.DrawCube(new Vector3(h, k, 2f), new Vector3(.2f, .2f, .2f));
                Gizmos.DrawLine(new Vector3(h, k, 2f), new Vector3(h + v.x, k + v.y, 2f));

            } else 
            if (mode == drawMode.both){

                Gizmos.color = Color.blue;
                for (int k = minY; k <= maxY; k++)
                {
                    for (int h = minX; h < maxX; h++)
                    {
                        Vector3 v;
                        if (reverseVariableInput){
                            v = new Vector3(P(k), Q(h), 0).normalized*gizmoScale;
                        } else{
                            v = new Vector3(P(h), Q(k), 0).normalized*gizmoScale;
                        }
                        
                        //Debug.Log(v);
                        //Debug.Log(h + ", " + k);
                        
                        Gizmos.DrawCube(new Vector3(h, k, 2f), new Vector3(.2f, .2f, .2f));
                        Gizmos.DrawLine(new Vector3(h, k, 2f), new Vector3(h + v.x, k + v.y, 2f));
                    }
                }
                
                Gizmos.color = Color.yellow;
                float j = transform.position.x;
                float l = transform.position.y;

                Vector3 b;
                if (reverseVariableInput){
                    b = new Vector3(P(l), Q(j), 0).normalized*gizmoScale;
                } else{
                    b = new Vector3(P(j), Q(l), 0).normalized*gizmoScale;
                }
                Gizmos.DrawCube(new Vector3(j, l, 2f), new Vector3(.2f, .2f, .2f));
                Gizmos.DrawLine(new Vector3(j, l, 2f), new Vector3(j + b.x, l + b.y, 2f));
            }
        }
    //#endif
}
