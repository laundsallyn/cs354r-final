using UnityEngine;
using System.Collections;

public class BasicSoldier : MonoBehaviour {

    public Transform goal;
    public GameObject target;
    public int health;
    public int speed;
    public bool playerFound;
    public bool inCover;
    public bool changedState;
    Quaternion startingAngle = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion stepAngle = Quaternion.AngleAxis(5, Vector3.up);

    public enum AIStates
    {
        dead = 0,
        passive = 1,
        aggressive = 2,
        defensive = 3,
        routing = 4
    };
    public AIStates state;

    // Use this for initialization
    void Start () {
        health = 200;
        playerFound = false;
        state = AIStates.passive;
        changedState = false;
        inCover = false;
        speed = 5;
    }

	
	// Update is called once per frame
	void Update () {
        //these first few if statements are important state checks dealing with health
        //and aspects of state that the player doesn't control
        if (health <= 0 && state != AIStates.dead)
        {
            state = AIStates.dead;
            Death();
        }
        else if(health <= 25)
        {
            state = AIStates.routing;
        }
        if(inCover && changedState)
        {
            if (goal.gameObject.tag == "OccupiedA")
                goal.gameObject.tag = "AmbushSpot";
            else
                goal.gameObject.tag = "CoverSpot";
        }
        if (state != AIStates.dead)
        {
            //basic raycast vision, will probably have to be made more realistic later
            Vector3 fwd = transform.TransformDirection(Vector3.forward);
            playerFound = false;
            //determine if the player is within the immediate vicinity of the soldier
            if (Vector3.Distance(transform.position, target.transform.position) < 5.0f)
            {
                playerFound = true;
                transform.LookAt(target.transform);
            }
            else
            {
                //else, see if he is within the field of view.
                if (Physics.Raycast(transform.position, fwd, 100))
                {
                    Vector3 targetDir = target.transform.position - transform.position;
                    float angle = Vector3.Angle(targetDir, transform.forward);
                    if (angle < 90.0f)
                    {
                        //Debug.Log("enemy possibly within range");
                        transform.LookAt(target.transform);
                        RaycastHit hit;
                        Ray ray = new Ray(transform.position + transform.up, transform.forward);
                        if (Physics.Raycast(ray, out hit))
                        {
                            if (hit.collider != null && hit.collider.tag == "Player")
                            {
                                playerFound = true;
                            }
                        }
                    }
                }
            }

            if (playerFound && state != AIStates.routing)
            {
                Debug.Log("ENEMY FOUND!!!");
                //The enemy will shoot at the player, and will be totally focused on killing them
                goal = null; //the soldier should stop and continue shooting until death for now
            }
            else
            {
                //this is what the soldier does in his idle state
                if(state == AIStates.passive)
                {
                    speed = 5;
                    //right now, he will do nothing except continue towards his goal, if he has one
                }
                if(state == AIStates.aggressive)
                {
                    speed = 6;
                    if (!inCover || changedState) { //In cover either means that we are on the way to cover or at it
                        //try to find the nearest ambush spot that is unoccupied, move towards it
                        changedState = false;
                        GameObject[] ambs;
                        ambs = GameObject.FindGameObjectsWithTag("AmbushSpot");
                        GameObject closest = null;
                        float distance = Mathf.Infinity;
                        foreach (GameObject go in ambs)
                        {
                            if(Vector3.Distance(transform.position, go.transform.position) < distance)
                            {
                                distance = Vector3.Distance(transform.position, go.transform.position);
                                closest = go;
                            }
                        }
                        if(closest)
                        {
                            goal = closest.transform;
                            closest.tag = "OccupiedA";
                            inCover = true;
                        }
                    }

                }

                if(state == AIStates.defensive)
                {
                    speed = 3;
                    if (!inCover || changedState)
                    { //In cover either means that we are on the way to cover or at it
                        //try to find the nearest ambush spot that is unoccupied, move towards it
                        GameObject[] covs;
                        covs = GameObject.FindGameObjectsWithTag("CoverSpot");
                        GameObject closest = null;
                        float distance = Mathf.Infinity;
                        foreach (GameObject go in covs)
                        {
                            if (Vector3.Distance(transform.position, go.transform.position) < distance)
                            {
                                distance = Vector3.Distance(transform.position, go.transform.position);
                                closest = go;
                            }
                        }
                        if (closest)
                        {
                            goal = closest.transform;
                            closest.tag = "OccupiedC";
                            inCover = true;
                        }
                    }
                }

                if(state == AIStates.routing)
                {
                    speed = 7;
                    if(health > 25)
                    {
                        state = AIStates.passive;
                    }
                    else if (!inCover || changedState) //here, incover just means has healthpack target
                    {
                        GameObject[] hps;
                        hps = GameObject.FindGameObjectsWithTag("Healthpack");
                        GameObject closest = null;
                        float distance = Mathf.Infinity;
                        foreach (GameObject go in hps)
                        {
                            if (Vector3.Distance(transform.position, go.transform.position) < distance)
                            {
                                distance = Vector3.Distance(transform.position, go.transform.position);
                                closest = go;
                            }
                        }
                        if (closest)
                        {
                            goal = closest.transform;
                            inCover = true;
                        }
                    }
                }
            }

            //this will probably have to come after he decision tree because the goal may
            //change as a result of the ai decisions
            if (goal)
            {
                //transform.LookAt(goal);
                NavMeshAgent agent = GetComponent<NavMeshAgent>();
                agent.destination = goal.position;
                agent.speed = speed;
            }
        }
    }

    void Death()
    {
        //A void method to send this soldier to the void.
        //fitting.
    }
}
