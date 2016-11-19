using UnityEngine;
using UnityEngine.Networking;

public class FPSController : NetworkBehaviour
{
    private Vector3 speed      = Vector3.zero;
    private Vector3 jumpSpeed  = Vector3.zero;
    private float gravity      = 9.8f;
    private float maxSpeed     = 25;
    private float sensitivityX = 1F;
    private float sensitivityY = 1F;
    private float minimumY     = -60F;
    private float maximumY     = 60F;
    float rotationY = 0F;
    private Camera mainCam;
    private Camera playerCam;
    // Use this for initialization
    void Start()
    {
        mainCam   = GameObject.Find("Main Camera").GetComponent<Camera>();
        playerCam = GetComponentInChildren<Camera>();
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

    // Used for jump pseudophysics
    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (transform.position.y == 1.0f && jumpSpeed.y < 3.0f) return;

        jumpSpeed.y -= gravity * Time.fixedDeltaTime;
        transform.Translate(jumpSpeed);
        if (transform.position.y < 1.0f)
            transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown("w"))
            speed.z += 1;
        if (Input.GetKeyUp("w"))
            speed.z -= 1;
        if (Input.GetKeyDown("s"))
            speed.z -= 1;
        if (Input.GetKeyUp("s"))
            speed.z += 1;
        if (Input.GetKeyDown("a"))
            speed.x -= 1;
        if (Input.GetKeyUp("a"))
            speed.x += 1;
        if (Input.GetKeyDown("d"))
            speed.x += 1;
        if (Input.GetKeyUp("d"))
            speed.x -= 1;
        if (Input.GetKeyDown("space"))
            jumpSpeed.y = 3.0f;
        transform.Translate(speed.normalized * maxSpeed * Time.deltaTime);

        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        playerCam.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);
        transform.localEulerAngles = new Vector3(0, rotationX, 0);
    }
}
