using UnityEngine;
using System.Collections;

public class FPSController : MonoBehaviour {
    private Vector3 speed;
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
    // Use this for initialization
    void Start () {
        speed = Vector3.zero;
        playerCam = GetComponentInChildren<Camera>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("w"))
            speed.z += 25;
        if (Input.GetKeyUp("w"))
            speed.z -= 25;
        if (Input.GetKeyDown("s"))
            speed.z -= 25;
        if (Input.GetKeyUp("s"))
            speed.z += 25;
        if (Input.GetKeyDown("a"))
            speed.x -= 15;
        if (Input.GetKeyUp("a"))
            speed.x += 15;
        if (Input.GetKeyDown("d"))
            speed.x += 15;
        if (Input.GetKeyUp("d"))
            speed.x -= 15;
        transform.Translate(speed * Time.deltaTime,Space.Self);
        rotate();

        // buggy third person view
        //if (Input.GetKeyDown("f"))
        //    thirdPersonView();

    }
    private void thirdPersonView()
    {
        if (firstPerson)
        {
            firstPerson = false;
            playerCam.transform.Translate(new Vector3(1, 0, -9),Space.Self);
        }
        else
        {
            playerCam.transform.Translate(new Vector3(1, 0, 9),Space.Self);
            firstPerson = true;
        }
    }
    private void rotate()
    {
        if (axes == RotationAxes.MouseXAndY)
        {
            float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

            playerCam.transform.localEulerAngles = new Vector3(-rotationY,0, 0);
            transform.localEulerAngles = new Vector3(0, rotationX, 0);
        }
        //else if (axes == RotationAxes.MouseX)
        //{
        //    transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
        //}
        //else
        //{
        //    rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        //    rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        //    playerCam.transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
        //}
    }
}
