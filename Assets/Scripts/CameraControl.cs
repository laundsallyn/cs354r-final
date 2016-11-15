using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
    private const int zoomSpeed = 25;
    private const int zoomMin = 25;
    private const int zoomMax = 100;
    private Vector3 scrollSpeed = Vector3.zero;
    // Use this for initialization
    void Update()
    {
        Debug.Log("In CameraControl");
        if (Input.GetKeyDown("up"))
            scrollSpeed = new Vector3(0,0,30);
        if(Input.GetKeyUp("up"))
            scrollSpeed = Vector3.zero;
        if (Input.GetKeyDown("down"))
            scrollSpeed = new Vector3(0, 0, -30);
        if (Input.GetKeyUp("down"))
            scrollSpeed = Vector3.zero;
        if (Input.GetKeyDown("left"))
            scrollSpeed = new Vector3(-30, 0, 0);
        if (Input.GetKeyUp("left"))
            scrollSpeed = Vector3.zero;
        if (Input.GetKeyDown("right"))
            scrollSpeed = new Vector3(30, 0, 0);
        if (Input.GetKeyUp("right"))
            scrollSpeed = Vector3.zero;
        transform.Translate(scrollSpeed * Time.deltaTime);
        
    }
}
