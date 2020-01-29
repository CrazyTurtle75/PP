using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Plane : BasicMove
{
    // Start is called before the first frame update
    Vector2 startPosition;
    [HideInInspector]
    public float birthTime;
    public float ep;
    public float cp;
    [Range (0, 1)]
    public float epWeight;
    public float cpWeight;
    public fitnessMode mode;
    public enum fitnessMode{
        xValue,
        distanceToPoint,
        distanceFromPoint
    }
    public bool playerControlled;
    public Vector2 point;
    public bool useWind;
    public bool reverseVariableInput;
    WindField wind;
    Vector2 v;
    Rigidbody2D rig;
    public RaycastHit2D[] hits;
    //public Transform chaser;


    [Space (20)]
    public GameObject cam;

    [Space (7)]
    [Header ("Plane Statistics")]
    //[Tooltip("A ranking of how effective this plane was for the environment")]
    //public float ecoFit;
    //float ecoEffect;
    [Tooltip("A ranking of how effective this plane was for the environment")]
    public float capFit;

    public float[] input = new float[5];
    public NeuralNetwork network;
    public LayerMask maskA;
    [HideInInspector]
    public int genNum;
    public int checkPoint = 0;
    public List<GameObject> passedGates;
    public SpeciesManager spawner;
    TrailRenderer trail;
    

    
    void Start()
    {
        hits = new RaycastHit2D[5];
        checkPoint = 0;
        self = gameObject.GetComponent<Rigidbody2D>();
        lastVeloAdd = 0f;
        startPosition = transform.position;
        birthTime = Time.time;
        trail = GetComponentInChildren<TrailRenderer>();
        wind = FindObjectOfType<WindField>();

        if (wind != null){
            useWind = spawner.useWind;
            reverseVariableInput = wind.reverseVariableInput;
        }
    }

    void FixedUpdate(){
        if (self != null){
            if(network != null && !playerControlled){
                CastRays();
                Move();
            } else if (playerControlled){
                Move();
            }
        }
    }

    #if (UNITY_EDITOR)
        private void OnDrawGizmos (){
            Gizmos.color = Color.blue;
            for (int i = 0; i < hits.Length; i++)
            {
                //Vector3 newVector = Quaternion.AngleAxis(i * 45-90, new Vector3 (0, 0, 1)) *transform.up;
                //Ray ray = new Ray (transform.position, newVector);
                //Gizmos.DrawRay(ray);
                if (self != null){
                    if (hits[i]){
                        Gizmos.color = Color.red;
                        Gizmos.DrawLine(transform.position, new Vector3 (hits[i].point.x, hits[i].point.y, transform.position.z));
                        Gizmos.DrawSphere(hits[i].point, .08f);
                    } else {
                        Vector3 newVector = Quaternion.AngleAxis(i * 45-90, new Vector3 (0, 0, 1)) * transform.up * 10;
                        Gizmos.color = Color.green;
                        Ray ray = new Ray (transform.position, newVector);
                        Gizmos.DrawRay(transform.position, ray.direction * 10f);
                    }
                }
            }
            if (useWind){
                Gizmos.color = Color.yellow;
                Vector3 p = transform.position;
                Gizmos.DrawLine(transform.position, new Vector3 (p.x + v.x, p.y + v.y, p.z));
            }
        }
    #endif

    void OnCollisionEnter2D(Collision2D col){
        if(col.gameObject.GetComponent<Bound>()!= null){
            Kill();
            Debug.Log("colliding with wall");
        } else if (col.transform.parent != null){
            if (col.transform.parent.gameObject.GetComponent<Bound>() != null){
            Kill();
            Debug.Log("colliding with chaser");
            //transform.SetParent(col.transform.parent);
            }
        } else {
            Debug.Log("collision with layer #" + col.collider.gameObject.layer);
        }
    }

    void OnTriggerEnter2D(Collider2D col){
        PoliticalBoundary pb = col.transform.gameObject.GetComponent<PoliticalBoundary>();
        if(col.gameObject.GetComponent<Bound>() != null){
            Kill();
            Debug.Log("colliding with trigger");
        } else if (col.transform.parent != null){
            if (col.transform.parent.gameObject.GetComponent<Bound>() != null){
                Kill();
                Debug.Log("colliding with trigger parent");
            }
        } else if (pb != null){
            bool b = CheckPoliticalBoundary(pb);
            if (!b){
                Kill();
                Debug.Log("colliding with pb");
            }
        } else {
            Debug.Log("collision with layer #" + col.gameObject.layer);
        }
    }

    public bool CheckPoliticalBoundary (PoliticalBoundary boundary){
        if (boundary != null){
            if (boundary.speciesToBlock.Contains(spawner)){
                return false;
            } else {
                return true;
            }
        } else {
            return true;
        }
    }

    void Kill(){
        if (GetComponent<Rigidbody2D>() != null){
            Debug.Log("legally killing plane");
            if (trail != null){
                trail.time = 0;
            }
            Destroy(gameObject.GetComponent<Rigidbody2D>());
            if (spawner != null){
                spawner.livingBots--;
            }
        } else {
            Debug.LogWarning("killing plane twice but blocked");
        }
    }

    void CastRays(){
        for (int i = 0; i < 5; i++)
        {
            Vector3 newVector = Quaternion.AngleAxis(i * 45-90, new Vector3 (0, 0, 1)) *transform.up;
            RaycastHit2D hit;

            hit = Physics2D.Raycast(transform.position, new Vector2 (newVector.x, newVector.y), 10, maskA);
            hits[i] = hit;
            if(hit){
                input[i] = (10-hit.distance)/10;//distance (with 1 being very close and 0 being far)
            } else {
                input[i] = 1;
            }
        }
    }

    public void UpdateFitness(){
        if (mode == fitnessMode.xValue){
            cp += transform.position.x;
        } else if (mode == fitnessMode.distanceFromPoint){
            Vector2 v;
            v = transform.position;
            cp -= Vector2.Distance(v, point);
        } else if (mode == fitnessMode.distanceToPoint){
            Vector2 v;
            v = transform.position;
            cp += Vector2.Distance(v, point);
        }
        network.fitness = cp + ep*epWeight;
    }

    new void Move(){
        float[] output = new float[2];
        float drive = 0;

        if (playerControlled){
            if (Input.GetKey(KeyCode.LeftArrow)||(Input.GetKey(KeyCode.A))){
                //Debug.Log("keydown");
                output[1] = 1;
            } else if (Input.GetKey(KeyCode.RightArrow)||(Input.GetKey(KeyCode.D))){
                //Debug.Log("keyDown");
                output[1] = -1;
            }
            if (Input.GetKey(KeyCode.UpArrow)||(Input.GetKey(KeyCode.W))){
                //Debug.Log("Keydown");
                output[0] = 1;
            } else if (Input.GetKey(KeyCode.DownArrow)||(Input.GetKey(KeyCode.S))){
                //Debug.Log("KeyDown");
                output[0] = -1;
            }
        } else {
            //Debug.Log(input[0]);
            if (input != null){
                output = network.FeedForward(input);
            }
            //drive = (output[0]+1)/2;
        }

        drive = output[0];
            if (drive < 0){
                drive = drive/2;
            }

        self.AddForce(drive*transform.up*speed*Time.deltaTime);//controls forward backward movement
        //Debug.Log("adding force" + drive);
        transform.Rotate(0, 0, output[1]*rotationSpeed*Time.deltaTime);//controls for rotation

        if (useWind){
            Vector3 p = transform.position;
            if (reverseVariableInput){
                    v = new Vector2 (wind.P(p.y), wind.Q(p.x)).normalized;
            } else {
                    v = new Vector2 (wind.P(p.x), wind.Q(p.y)).normalized;
            }
            self.AddForce(v*Time.deltaTime);
        }
    }

}
