using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour
{
    private const int zoomSpeed = 40;
    private const int zoomMin = 0;
    private const int zoomMax = 1000;
    private const int boundary = 40;
    private int zoom = 400;
    private Vector3 scrollSpeed, zoomVector = Vector3.zero;
    private int width, height;

    void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void Update()
    {


        if (Input.mousePosition.x > width - boundary)
            scrollSpeed = new Vector3(30, 0, 0);
        else if (Input.mousePosition.x < 0 + boundary)
            scrollSpeed = new Vector3(-30, 0, 0);
        else if (Input.mousePosition.y < 0 + boundary)
            scrollSpeed = new Vector3(0, 0, -30);
        else if (Input.mousePosition.y > height - boundary)
            scrollSpeed = new Vector3(0, 0, 30);
        else
            scrollSpeed = Vector3.zero;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0 && zoom <= zoomMax)
        {
            zoom += zoomSpeed;
            zoomVector = new Vector3(0, zoomSpeed, 0);
        }
        else if (scroll < 0 && zoom >= zoomMin)
        {
            zoom -= zoomSpeed;
            zoomVector = new Vector3(0, -zoomSpeed, 0);
        }
        else
            zoomVector = Vector3.zero;


        transform.Translate(zoomVector * Time.deltaTime);
        transform.Translate(scrollSpeed * Time.deltaTime);
    }

}
