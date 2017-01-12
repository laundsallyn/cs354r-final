using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class TrainingUnits : NetworkBehaviour {
    //private Canvas can;
    private Camera cam;
    public Transform unitPrefab;
    public Texture2D cursorTexture;
    private List<Image> unitIcons;
    private Button unit;
    private Image unitTraining;
    private Text unitRdy;
    private Animation ani;
    private AudioSource training, unitReady;
    private bool ready, placing;
    float start;
    // Use this for initialization
    void Start () {
        cam = GameObject.Find("CameraTarget/Camera").GetComponent<Camera>();
        //Debug.Log("Camera name"+ cam.name);
        ready = placing = false;
        unit = GetComponentInChildren<Button>();
        unitTraining = unit.transform.FindChild("Progress").gameObject.GetComponent<Image>();
        unitRdy = unit.GetComponentInChildren<Text>();
        unitRdy.enabled = false;
        //marineTraining.enabled = false;
        start = 0f;
        ani = unit.GetComponent<Animation>();
        unitReady = GameObject.Find("Audio/unitready").GetComponent<AudioSource>();
        training = GameObject.Find("Audio/training").GetComponent<AudioSource>();
        // Debug.Log(ani.GetClipCount());

    }

    // Update is called once per frame
    void FixedUpdate () {        
        if (ani.IsPlaying("TrainMarine") && unit.interactable)
        {
            unit.interactable = false;
        }
        if (start >= 8.9)
        {
            unit.interactable = true;
            ready = true;
            unitReady.Play();
            //unitTraining.enabled = false;
        }else if (unitRdy.enabled)
            start += Time.fixedDeltaTime;

        if (placing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(Input.mousePosition.x + " " + Input.mousePosition.y);
                Vector3 mousePositionT = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePositionT);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Debug.Log(hit.point.x + " " + hit.point.y);
                    NetworkServer.Spawn(((Transform)Instantiate(unitPrefab, hit.point, Quaternion.identity)).gameObject);

                    reset();
                    Debug.Log("in Raycasting");
                    Cursor.SetCursor(null, Input.mousePosition, CursorMode.Auto);

                }

            }
            ani.Rewind();
        }


    }

     public void click ()
    {
        if (!ready)
        {
            unitRdy.enabled = true;
            //marineTraining.enabled = true;
            Debug.Log("Button Clicked");
            training.Play();
            ani.Play();
        }
        else
        {
            Debug.Log("In else clause");
            Cursor.SetCursor(cursorTexture, new Vector2(cursorTexture.width/2, cursorTexture.height/2), CursorMode.Auto);
            placing = true;
        }

    }
    private void reset()
    {
        unitRdy.enabled = false;
        ready = placing = false;
        start = 0f;
    }
}
