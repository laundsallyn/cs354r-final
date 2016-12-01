using UnityEngine;
using UnityEngine.Networking;

public class FPSController : NetworkBehaviour
{
    private Vector3 force    = Vector3.zero;
    private Vector3 curForce = Vector3.zero;
    private float movForce     = 200;
    private float maxVel       = 5;
    private float sensitivityX = 1F;
    private float sensitivityY = 1F;
    private float minimumY     = -60F;
    private float maximumY     = 60F;
    float rotationY = 0F;
    private Camera mainCam;
    private Camera playerCam;
    private Transform transf;
    private Rigidbody body;
    // Use this for initialization
    void Start()
    {
        mainCam    = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCam  = GetComponentInChildren<Camera>();
        transf     = GetComponentsInChildren<Transform>()[1];
        body       = GetComponentInChildren<Rigidbody>();
        if (isLocalPlayer)
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
                r.enabled = false;

            mainCam.enabled = false;
        }
        else
        {
            playerCam.enabled = false;
        }
    }

    void OnDestroy()
    {
        GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
    }

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
            force.z += 1;
        if (Input.GetKeyUp("w"))
            force.z -= 1;
        if (Input.GetKeyDown("s"))
            force.z -= 1;
        if (Input.GetKeyUp("s"))
            force.z += 1;
        if (Input.GetKeyDown("a"))
            force.x -= 1;
        if (Input.GetKeyUp("a"))
            force.x += 1;
        if (Input.GetKeyDown("d"))
            force.x += 1;
        if (Input.GetKeyUp("d"))
            force.x -= 1;
        if (Input.GetKeyDown("space") && Physics.Raycast(transf.position, transf.up * -1, 1.1f, 1 << LayerMask.NameToLayer("Ground")))
            body.AddForce(new Vector3(0, 300, 0));
        curForce = transf.rotation * force.normalized * movForce;
        float rX = transf.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        playerCam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transf.localEulerAngles = new Vector3(0, rX, 0);
    }
}
