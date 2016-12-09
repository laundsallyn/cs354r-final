using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BasicSoldier : NetworkBehaviour {

    public Transform goal;
    public GameObject target;
    public int speed;
    public bool playerFound;
    public bool inCover;
    public bool changedState;
    private ParticleSystem flame;
    protected int hp, maxhp, maxammo, ammo, damage;
    private Rigidbody body;
    private NavMeshAgent agent;
    private Animation ani;
    
    private AudioSource shotAudio;
    private float deathCounter;
    private bool death;
    private float shootCounter;

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
        target = GameObject.FindGameObjectWithTag("Player");
        playerFound = false;
        state = AIStates.passive;
        changedState = false;
        inCover = false;
        speed = 5;
        body = GetComponent<Rigidbody>();
        maxhp = hp = 300;
        maxammo = ammo = 30;
        damage = 17;
        shotAudio = GetComponent<AudioSource>();    
        GameObject quad = transform.Find("Quad").gameObject;
        //if (!isServer)
        //    quad.GetComponent<Renderer>().enabled = false;
        quad.SetActive(false);
        ani = GetComponent<Animation>();
        Debug.Log(ani.GetClipCount());
        agent = GetComponent<NavMeshAgent>();
        deathCounter = 0.0f;
        shootCounter = 0.0f;
        flame = GetComponentInChildren<ParticleSystem>();
        flame.Stop();
    }

    void FixedUpdate()
    {
        if (!isServer) return;

        if(death)
        {
            deathCounter += Time.fixedDeltaTime;
        }
        if(death && deathCounter > 1.5f)
        {
            Network.Destroy(GetComponent<NetworkView>().viewID);
        }
    }

	
	// Update is called once per frame
	void Update () {
        if (!isServer) return;

        if(target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
        //these first few if statements are important state checks dealing with health
        //and aspects of state that the player doesn't control
        if(hp <= 25 && hp > 0)
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
            if (target != null)
            {
                ////determine if the player is within the immediate vicinity of the soldier
                //if (Vector3.Distance(transform.position, target.transform.position) < 15.0f)
                //{
                //    playerFound = true;
                //    transform.LookAt(target.transform);
                //}
                //else
                //{
                //    ////else, see if he is within the field of view.
                //    //if (Physics.Raycast(transform.position, fwd, 100))
                //    //{
                //    //    Vector3 targetDir = target.transform.position - transform.position;
                //    //    float angle = Vector3.Angle(targetDir, transform.forward);
                //    //    if (angle < 90.0f)
                //    //    {
                //    //        //Debug.Log("enemy possibly within range");
                //    //        transform.LookAt(target.transform);
                //    //        RaycastHit hit;
                //    //        Ray ray = new Ray(transform.position + transform.up, transform.forward);
                //    //        if (Physics.Raycast(ray, out hit))
                //    //        {
                //    //            if (hit.collider != null && hit.collider.tag == "Player")
                //    //            {
                //    //                playerFound = true;
                //    //            }
                //    //        }
                //    //    }
                //    //}

                //}
                float targetDist = Vector3.Distance(transform.position, target.transform.position);
                //Debug.Log(targetDist);
                if(targetDist < 75.0f)
                {
                    //Debug.Log("taret maybe?");
                    if(targetDist < 5.0f)
                    {
                        playerFound = true;
                    }
                    else
                    {
                        Vector3 targetDir = target.transform.position - transform.position;
                        //Debug.Log(Vector3.Angle(targetDir, transform.forward));
                        if(Vector3.Angle(targetDir, transform.forward) < 55.0f)
                        {
                            RaycastHit hit;
                            Ray ray = new Ray(transform.position + transform.up, targetDir);
                            if(Physics.Raycast(ray, out hit))
                            {
                                if (hit.collider.gameObject.tag == "Player")
                                    playerFound = true;
                            }
                        }
                    }
                }
            }

            if (playerFound && state != AIStates.routing)
            {
                transform.LookAt(target.transform);
                //Debug.Log("ENEMY FOUND!!!");
                //The enemy will shoot at the player, and will be totally focused on killing them
                fireWeapon();
                //goal = null; //the soldier should stop and continue shooting until death for now
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
                    if(hp > 25)
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
                if (Vector3.Distance(goal.position, transform.position) > 1.0f)
                {
                    transform.LookAt(goal);
                    if (!ani.IsPlaying("walk"))
                        ani.Play("walk");
                }
                else if (ani.IsPlaying("walk"))
                {
                    ani.Stop();
                }
                agent.destination = goal.position;
                agent.speed = speed;
            }
        }
    }

    // Use this for initialization
    public virtual void fireWeapon()
    {
        if (shootCounter > 0.0f && shootCounter < 0.5f) {
            shootCounter += Time.fixedDeltaTime;
            return;
        }
        //ani.Play("firing");
        shootCounter = 0.0f;
        ammo -= 1;
        flame.Emit(1);
        //Debug.Log(gunflame.name);
        shotAudio.Play();
        //Debug.Log("bang");
        Vector3 direction = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + transform.up, direction, out hit, 300))
        {
            //Debug.Log("bang2");
            Vector4 dmgInfo = new Vector4();
            dmgInfo.x = hit.normal.x;
            dmgInfo.y = hit.normal.y;
            dmgInfo.z = hit.normal.z;
            dmgInfo.w = damage;
            Debug.DrawLine(transform.position, hit.point, Color.cyan);
            //hit.collider.SendMessageUpwards("applyDamage", dmgInfo, SendMessageOptions.DontRequireReceiver);
            //Debug.Log("preparing to send shot");
            SendShot(hit, dmgInfo);
        }
        shootCounter += Time.fixedDeltaTime;

    }

    public void SendShot(RaycastHit hit, Vector4 dmgInfo)
    {
        //Debug.Log(hit.collider.gameObject.name);
        RigidController rc = hit.collider.gameObject.GetComponentInParent<RigidController>();
        if (rc)
        {
            //Debug.Log("Sending shot to player!");
            rc.RpcApplyDamage(dmgInfo);
        }
    }
    
    public void ApplyDamage(Vector4 dmg)
    {
        Debug.Log("OH SHIT HIT");
        if (hp <= 0)
            return;
        hp -= (int)dmg.w;
        Debug.Log("current hp is " + hp);
        Vector3 dir = new Vector3(-dmg.x, -dmg.y, -dmg.z);
        if (hp < 0)
        {
            Debug.Log("Fuck im dead");
            death = true;
            dead(dir);
        }
    }

    public void heal(int heal)
    {
        hp = hp + heal > maxhp ? maxhp : hp + heal;
    }

    public void dead(Vector3 direction)
    {
        Debug.Log(direction);
        state = AIStates.dead;
        body.mass = 0.2f;
        body.constraints = RigidbodyConstraints.None;
        //Destroy(this, 5);
        body.AddRelativeForce(direction * 100);
    }
}
