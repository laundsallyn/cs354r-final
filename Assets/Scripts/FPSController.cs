using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {
    private Vector3 speed;
    private float gravity = 3f;
    private Vector3 jumpSpeed = Vector3.zero;
    private enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    private RotationAxes axes = RotationAxes.MouseXAndY;
    private float sensitivityX = 1F;
    private float sensitivityY = 1F;
    private float minimumX = -360F;
    private float maximumX = 360F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    float rotationY = 0F;
    private Camera playerCam;
    private bool firstPerson = true;
    private Animation ani, fpsAni;
    public GameObject fpsArm;
    public Canvas canvas;
    // Use this for initialization
    void Start () {
        speed = Vector3.zero;
        playerCam = GetComponentInChildren<Camera>();
        ani = GetComponent<Animation>();
        fpsAni = fpsArm.GetComponent<Animation>();
        Debug.Log("Clip count is " + fpsAni.GetClipCount());
        //Debug.Log(fpsAni.GetClip("fireWeapon"));
        ani["walk"].wrapMode = WrapMode.Loop;
        ani["jump"].speed = 7;
        ani["Sidewalk"].speed = 2;
        ani["Sidewalk"].wrapMode = WrapMode.Loop;
        ani["Crouch"].speed = 3;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
        {
            ani["walk"].speed = 5;
            ani.Play("walk");
            speed.z += 22;
        }
           
        if (Input.GetKeyUp("w")) { 
            ani.Stop("walk");
            speed.z -= 22;
        }
        if (Input.GetKeyDown("s"))
        {
            ani["walk"].speed = -5;
            ani.Play("walk");
            speed.z -= 20;
        }
        if (Input.GetKeyUp("s"))
        {
            ani.Stop("walk");
            speed.z += 20;
        }
        
        if (Input.GetKeyDown("a"))
        {
            ani.Play("Sidewalk");
            speed.x -= 15;
        }
        if (Input.GetKeyUp("a"))
        {
            ani.Stop("Sidewalk");
            speed.x += 15;
        }

        if (Input.GetKeyDown("d"))
        {
            ani.Play("Sidewalk");
            speed.x += 15;
        }
        if (Input.GetKeyUp("d"))
        {
            ani.Stop("Sidewalk");
            speed.x -= 15;
        }
        transform.Translate(speed * Time.deltaTime,Space.Self);
        rotate();
        if (Input.GetMouseButtonDown(0))
        {
            fpsAni.Play("fireWeapon");

        }

        // buggy third person view
        if (Input.GetKeyDown("f"))
            thirdPersonView();
        if (Input.GetKeyDown("space"))
        {
            jumpSpeed.y = 0.6f;
            ani.Play("jump");
        }
    }

    void FixedUpdate()
    {
        if (transform.position.y == 0.0f && jumpSpeed.y < 0.1f) return;

        jumpSpeed.y -= gravity * Time.fixedDeltaTime;
        transform.Translate(jumpSpeed);
        if (transform.position.y < 0.0f)
            transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
    }
    private void thirdPersonView()
    {
        if (firstPerson)
        {
            firstPerson = false;
            playerCam.transform.Translate(new Vector3(1, 0, -9),Space.Self);
            fpsArm.SetActive(false);
        }
        else
        {
            playerCam.transform.Translate(new Vector3(1, 0, 9),Space.Self);
            firstPerson = true;
            fpsArm.SetActive(true);
        }
    }
    private void rotate()
    {

        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        playerCam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);

    }
}
