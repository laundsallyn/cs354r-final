using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class RigidController : NetworkBehaviour
{
    private Vector3 force = Vector3.zero;
    private Vector3 curForce = Vector3.zero;
    private float movForce = 200;
    private float maxVel = 5;
    private float sensitivityX = 1F;
    private float sensitivityY = 1F;
    private float minimumY = -60F;
    private float maximumY = 60F;
    private ConstantForce constForce;

    float rotationY = 0F;
    private Camera mainCam;
    private Camera playerCam;
    private Transform transf;

    private Rigidbody body;
    private Animation ani, fpsAni;
    private GameObject fpsArm;
    //UI
    private Canvas canvas;
    //private Image pauseMenu;
    private Text hpT, maxHpT, ammoT, maxAmmoT;
    //player stats
    private int maxAmmo = 12;
    private int maxHealth = 200;
    private int hp, ammoCnt;
    // firelock: prevent firing while reloading
    AudioSource shotAudio;
    private GameObject gunflame;
    private ParticleSystem flame;
    private bool firstPerson;
    void Start()
    {
        List<Text> txts = new List<Text>();
        //pauseMenu = canvas.GetComponentInChildren<Image>();
        //pauseMenu.enabled = false;
        transf = transform.Find("MC");
        canvas =  transform.Find("HUD").gameObject.GetComponent<Canvas>();
        gunflame = transform.Find("MC/Camera/fire").gameObject;
        fpsArm = transform.Find("MC/Camera/Arm").gameObject;
        flame = gunflame.GetComponent<ParticleSystem>();
        canvas.GetComponentsInChildren<Text>(txts);
        for (int i = 0; i < txts.Count; i++)
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
            if (name == "hp")
            {
                hpT = txts[i];
                continue;
            }
            if (name == "max hp")
            {
                maxHpT = txts[i];
            }
        }
        ani = transform.Find("MC").gameObject.GetComponent<Animation>();
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
        mainCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCam = GetComponentInChildren<Camera>();
        body = GetComponentInChildren<Rigidbody>();
        constForce = GetComponentInChildren<ConstantForce>();
        if (isLocalPlayer)
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = true;
            mainCam.enabled = false;
        }
        else
        {
            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = false;

            playerCam.enabled = false;
        }
    }

    void OnDestroy()
    {
        mainCam.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        body.AddForce(curForce - (new Vector3(System.Math.Abs(body.velocity.x) * body.velocity.x, 0, System.Math.Abs(body.velocity.z) * body.velocity.z)));
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown("w"))
        {
            ani["walk"].speed = 5;
            ani.Play("walk");
            force.z += 1;
        }
        if (Input.GetKeyUp("w"))
        {
            force.z -= 1;
            ani.Stop("walk");
        }
        if (Input.GetKeyDown("s"))
        {
            ani["walk"].speed = -5;
            ani.Play("walk");
            force.z -= 1;
        }
        if (Input.GetKeyUp("s"))
        {
            ani.Stop("walk");
            force.z += 1;
        }
        if (Input.GetKeyDown("a"))
        {
            ani.Play("Sidewalk");
            force.x -= 1;

        }
        if (Input.GetKeyUp("a"))
        {
            force.x += 1;
            ani.Stop("Sidewalk");

        }
        if (Input.GetKeyDown("d"))
        {
            ani.Play("Sidewalk");
            force.x += 1;
        }
        if (Input.GetKeyUp("d"))
        {
            ani.Stop("Sidewalk");
            force.x -= 1;
        }
        if(ammoCnt == 0)
            reload();

        if (Input.GetKeyDown("r"))
        {
            reload();
        }

        if (Input.GetMouseButtonDown(0))
            fire();
        if (Input.GetKeyDown("space") && Physics.Raycast(transf.position, transf.up * -1, 1.1f, 1 << LayerMask.NameToLayer("Ground")))
        {
            ani.Play("jump");
            body.AddForce(new Vector3(0, 300, 0));
        }
        //if (Input.GetKeyDown("f"))
        //    thirdPersonView();
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
        curForce = transf.rotation * force.normalized * movForce;
        float rX = transf.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        hpT.text = "" + hp;
        ammoT.text = "" + ammoCnt;
        playerCam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transf.localEulerAngles = new Vector3(0, rX, 0);
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
        fpsAni.Play("Reload");

        ammoCnt = maxAmmo;
    }
    private void thirdPersonView()
    {
        if (firstPerson)
        {
            firstPerson = false;
            playerCam.transform.Translate(new Vector3(1, 0, -19), Space.Self);
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = true;
            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = false;
        }
        else
        {
            playerCam.transform.Translate(new Vector3(-1, 0, 19), Space.Self);
            firstPerson = true;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = true;
        }
    }
}