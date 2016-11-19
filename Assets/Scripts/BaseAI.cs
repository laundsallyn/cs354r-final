using UnityEngine;
using System.Collections;

public class BaseAI : MonoBehaviour {

    public float targetDistance;
    public float lookDistance;
    public float attackDistance;
    public float movementSpeed;
    public float damping;
    public Transform target;
    Rigidbody theRigidBody;
    Renderer myRenderer;

	// Use this for initialization
	void Start () {
        myRenderer = GetComponent<Renderer>();
        theRigidBody = GetComponent<Rigidbody>();
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        targetDistance = Vector3.Distance(target.position, transform.position);
        if(targetDistance<lookDistance)
        {
            myRenderer.material.color = Color.yellow;
            facePlayer();
        }
        if(targetDistance<attackDistance)
        {
            attackTarget();
        }
	}

    void facePlayer()
    {
        Quaternion newRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime*damping);
    }

    void attackTarget()
    {
        theRigidBody.AddForce(transform.forward * movementSpeed);
    }

    void CheckStatus ()
    {
        //Use this to figure out if there needs to be any changes to the current
        //status and update if need be
    }
}
