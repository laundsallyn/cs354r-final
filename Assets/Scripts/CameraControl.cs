﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraControl : MonoBehaviour {
    private const int zoomSpeed = 5;
    private const int zoomMin = 25;
    private const int zoomMax = 100;
    private const int boundary = 40;
    private Vector3 scrollSpeed = Vector3.zero;
    private int width, height;
    //public Text debugInfo;
    void Start()
    {
        width = Screen.width;
        height = Screen.height;
    }

    void Update()
    {
        //debugInfo.text = "width: " + width + "\nheight: " + height+"\n" 
        //    + Input.mousePosition;

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


        transform.Translate(scrollSpeed * Time.deltaTime);
    }

}
