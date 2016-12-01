using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

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
    private float ymin = 0.0f;
    float rotationY = 0F;
    private Camera playerCam;
    private bool firstPerson = true;
    private Animation ani, fpsAni;
    public GameObject fpsArm;
    //UI
    public Canvas canvas;
    //private Image pauseMenu;
    private Text hpT, maxHpT, ammoT, maxAmmoT;
    //player stats
    private int maxAmmo = 12;
    private int maxHealth = 200;
    private int hp, ammoCnt;

    // firelock: prevent firing while reloading
    AudioSource shotAudio;
    private bool fireLock;
    private float reloadStart;
    public GameObject gunflame;
    private ParticleSystem flame;
    // Use this for initialization
    void Start () {
        List<Text> txts = new List<Text>();
        fireLock = false;
        //pauseMenu = canvas.GetComponentInChildren<Image>();
        //pauseMenu.enabled = false;
        flame = gunflame.GetComponent<ParticleSystem>();
        canvas.GetComponentsInChildren<Text>(txts);
        for(int i = 0; i < txts.Count; i++)
        {
            string name = txts[i].name;
            if (name == "ammo count")
            {
                ammoT = txts[i];
                continue;
            }
            if (name == "max ammo")
            {
                maxAmmoT = txts[i];
                continue;
            }
            if(name == "hp")
            {
                hpT = txts[i];
                continue;
            }
            if(name == "max hp")
            {
                maxHpT = txts[i];
            }
        }

        speed = Vector3.zero;
        playerCam = GetComponentInChildren<Camera>();
        ani = GetComponent<Animation>();
        fpsAni = fpsArm.GetComponent<Animation>();
        shotAudio = GetComponent<AudioSource>();
        Debug.Log("Clip count is " + fpsAni.GetClipCount());
        //Debug.Log(fpsAni.GetClip("fireWeapon"));
        ani["walk"].wrapMode = WrapMode.Loop;
        ani["jump"].speed = 7;
        ani["Sidewalk"].speed = 2;
        ani["Sidewalk"].wrapMode = WrapMode.Loop;
        ani["Crouch"].speed = 4;

        hp = maxHealth;
        ammoCnt = maxAmmo;
        maxHpT.text = "" + maxHealth;
        maxAmmoT.text = "" + maxAmmo;
    }
	
	// Update is called once per frame
	void Update () {
        hpT.text = "" + hp;
        ammoT.text = "" + ammoCnt;

        if (Input.GetKeyDown("w"))
        {
            speed.z = 0;
            ani["walk"].speed = 5;
            ani.Play("walk");
            speed.z += 22;
        }
           
        if (Input.GetKeyUp("w")) {
            speed.z = 0;
            ani.Stop("walk");
        }
        if (Input.GetKeyDown("s"))
        {
            speed.z = 0;
            ani["walk"].speed = -5;
            ani.Play("walk");
            speed.z -= 20;
        }
        if (Input.GetKeyUp("s"))
        {
            speed.z = 0;
            ani.Stop("walk");
        }
        
        if (Input.GetKeyDown("a"))
        {
            speed.x = 0;
            ani.Play("Sidewalk");
            speed.x -= 15;
        }
        if (Input.GetKeyUp("a"))
        {
            speed.x = 0;
            ani.Stop("Sidewalk");
        }

        if (Input.GetKeyDown("d"))
        {
            speed.x = 0;
            ani.Play("Sidewalk");
            speed.x += 15;
        }
        if (Input.GetKeyUp("d"))
        {
            speed.x = 0;
            ani.Stop("Sidewalk");
        }
        if (Input.GetKeyDown("left ctrl"))
        {
            ani.Play("Crouch");
            ymin = -.6f;
        }
        if (Input.GetKeyUp("left ctrl"))
        {
            ani.Play("Stand");
            ymin = 0.0f;
        }
        if (Input.GetKeyDown("r"))
        {
            reload();
        }
        transform.Translate(speed * Time.deltaTime,Space.Self);
        rotate();
        if (Input.GetMouseButtonDown(0))
        {
            fire();
        }
        if (ammoCnt == 0)
        {
            reload();
        }
        // buggy third person view
        if (Input.GetKeyDown("f"))
            thirdPersonView();
        if (Input.GetKeyDown("space"))
        {
            jumpSpeed.y = 0.6f;
            ani.Play("jump");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
       }
    private void fire()
    {
        if (fpsAni.isPlaying)
            return;
        fpsAni.Play("fireWeapon");
        ammoCnt -= 1;
        flame.Emit(1);
        shotAudio.Play();
    }
    private void reload()
    {
        if (ammoCnt == maxAmmo)
            return;
        if (fpsAni.isPlaying)
            return;
        reloadStart = Time.time;
        fpsAni.Play("Reload");
        
        ammoCnt = maxAmmo;
    }



    void FixedUpdate()
    {
        
        if (transform.position.y == ymin && jumpSpeed.y < 0.1f) return;
  
        jumpSpeed.y -= gravity * Time.fixedDeltaTime;
        transform.Translate(jumpSpeed);
        if (transform.position.y < ymin)
            transform.position = new Vector3(transform.position.x, ymin, transform.position.z);
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
