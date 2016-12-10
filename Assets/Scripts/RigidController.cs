using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class RigidController : NetworkBehaviour
{
    private class WASD
    {
        public bool w = false;
        public bool a = false;
        public bool s = false;
        public bool d = false;

        public Vector3 GetV3()
        {
            return new Vector3(System.Convert.ToInt32(d) - System.Convert.ToInt32(a),
                               0,
                               System.Convert.ToInt32(w) - System.Convert.ToInt32(s));
        }
    }
    private WASD    wasd = new WASD();
    private float speed = 5;
    private float sensitivityX = 1F;
    private float sensitivityY = 1F;
    private float minimumY = -60F;
    private float maximumY = 60F;

    float rotationY = 0F;
    private Camera mainCam;
    private Camera playerCam;
    private Transform transf;

    private int damage = 45;

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
    [SyncVar]
    private Animation animation;
    void Start()
    {

        if (isServer && isLocalPlayer)
        {
            Destroy(gameObject);
            return;
        }
        List<Text> txts = new List<Text>();
        //pauseMenu = canvas.GetComponentInChildren<Image>();
        //pauseMenu.enabled = false;
        transf    = transform.Find("MC");
        animation = transf.gameObject.GetComponent<Animation>();
        canvas    = transform.Find("HUD").gameObject.GetComponent<Canvas>();
        gunflame  = transform.Find("MC/Camera/fire").gameObject;
        fpsArm    = transform.Find("MC/Camera/Arm").gameObject;
        flame     = gunflame.GetComponent<ParticleSystem>();
        canvas.GetComponentsInChildren(txts);
        Debug.Log("" + transform.rotation + transf.localRotation);
        transf.localRotation = transform.rotation;
        transform.rotation = Quaternion.identity;
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
        //Debug.Log("Clip count is " + fpsAni.GetClipCount());
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
        mainCam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        playerCam = GetComponentInChildren<Camera>();
        body = GetComponentInChildren<Rigidbody>();
        if (isLocalPlayer)
        {
            GameObject.Find("Canvas").GetComponent<Canvas>().enabled = false;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;
            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = true;
            mainCam.GetComponent<AudioListener>().enabled = false;
            mainCam.enabled = false;
        }
        else
        {
            canvas.enabled = false;

            foreach (Renderer r in fpsArm.GetComponentsInChildren<Renderer>())
                r.enabled = false;

            playerCam.enabled = false;
        }
    }

    void OnDestroy()
    {
        GameObject.Find("Canvas").GetComponent<Canvas>().enabled = true;
        Camera mc = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        mc.enabled = true;
        mc.GetComponent<AudioListener>().enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        Vector3 move = body.rotation * wasd.GetV3().normalized * speed;
        Vector3 v    = body.velocity; v.y = 0;
        body.AddForce(move - v, ForceMode.VelocityChange);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        wasd.w = (wasd.w || Input.GetKeyDown("w")) && !Input.GetKeyUp("w");
        wasd.a = (wasd.a || Input.GetKeyDown("a")) && !Input.GetKeyUp("a");
        wasd.s = (wasd.s || Input.GetKeyDown("s")) && !Input.GetKeyUp("s");
        wasd.d = (wasd.d || Input.GetKeyDown("d")) && !Input.GetKeyUp("d");

        Vector3 mov = wasd.GetV3();
        if (mov.z != 0)
        {
            ani["walk"    ].speed = mov.z * 5;
            ani.Play("walk");
        }
        else
        if (mov.x != 0)
        {
            ani["Sidewalk"].speed = mov.x * 5;
            ani.Play("Sidewalk");
        }
        else
        {
            ani.Stop("walk");
            ani.Stop("Sidewalk");
        }

        if (Input.GetKeyDown("r"))
        {
            reload();
        }

        if (Input.GetMouseButtonDown(0))
            fire();
        if (ammoCnt == 0)
            reload();

        if (Input.GetKeyDown("space") && Physics.Raycast(transf.position, transf.up * -1, 1.1f, 1 << LayerMask.NameToLayer("Ground")))
        {
            ani.Play("jump");
            body.AddForce(new Vector3(0, 300, 0));
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

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
        Vector3 direction = playerCam.transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, direction, out hit, 300))
        {
            Vector4 dmgInfo = new Vector4();
            dmgInfo.x = hit.normal.x;
            dmgInfo.y = hit.normal.y;
            dmgInfo.z = hit.normal.z;
            dmgInfo.w = damage;
            Debug.DrawLine(playerCam.transform.position, hit.point, Color.cyan);
            Debug.Log("firing shot");
            //hit.collider.SendMessageUpwards("applyDamage", dmgInfo, SendMessageOptions.DontRequireReceiver);
            CmdSendShot(hit, dmgInfo);

        }
    }

    [Command]
    void CmdSendShot(RaycastHit hit, Vector4 dmgInfo)
    {
        Debug.Log(hit.collider.gameObject.name);
        BasicSoldier bs = hit.collider.gameObject.GetComponent<BasicSoldier>();
        if(bs)
        {
            bs.RpcApplyDamage(dmgInfo);
        }
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