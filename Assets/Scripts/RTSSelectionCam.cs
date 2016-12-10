using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class RTSSelectionCam : NetworkBehaviour {

    bool isSelecting = false;
    bool firstSelect = true;
    Vector3 mousePosition1;
    ArrayList selectedObjects = new ArrayList();
    GameObject[] allUnits;
    GameObject dest;

    void Start()
    {
        allUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        //selectedObjects.Add(allUnits[0]);
    }

    void Update()
    {
        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetMouseButtonDown(0))
        {
            if (!isSelecting)
                foreach(GameObject go in selectedObjects)
                {
                    go.transform.Find("Quad").gameObject.SetActive(false);
                }
                selectedObjects.Clear();
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            allUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
            isSelecting = false;
            Vector3 mousePositionT = Input.mousePosition;
            mousePosition1.y = Screen.height - mousePosition1.y;
            mousePositionT.y = Screen.height - mousePositionT.y;
            Rect rec = Utils.GetScreenRect(mousePosition1, mousePositionT);
            foreach(GameObject go in allUnits)
            {
                Camera cam = GetComponent<Camera>();
                Vector3 screenPos = cam.WorldToScreenPoint(go.transform.position);
                screenPos.z = 0;
                if(rec.Contains(screenPos))
                {
                    go.transform.Find("Quad").gameObject.SetActive(true);
                    selectedObjects.Add(go);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            mousePosition1 = Input.mousePosition;
            RTSClick();
        }

        if(Input.GetKey(KeyCode.Alpha1))
        {
            foreach(GameObject go in selectedObjects)
            {
                BasicSoldier bs = go.GetComponent<BasicSoldier>();
                bs.state = BasicSoldier.AIStates.passive;
                bs.changedState = true;
            }
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            foreach (GameObject go in selectedObjects)
            {
                BasicSoldier bs = go.GetComponent<BasicSoldier>();
                bs.state = BasicSoldier.AIStates.aggressive;
                bs.changedState = true;
            }
        }

        if (Input.GetKey(KeyCode.Alpha3))
        {
            foreach (GameObject go in selectedObjects)
            {
                BasicSoldier bs = go.GetComponent<BasicSoldier>();
                bs.state = BasicSoldier.AIStates.defensive;
                bs.changedState = true;
            }
        }
    }

    void RTSClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition1);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Object.Destroy(dest);
            dest = new GameObject();
            dest.transform.position = hit.point;
            foreach(GameObject go in selectedObjects)
            {
                BasicSoldier bs = go.GetComponent<BasicSoldier>();                
                if(bs.inCover)
                {
                    if (bs.state == BasicSoldier.AIStates.defensive)
                        bs.goal.gameObject.tag = "CoverSpot";
                    else
                        bs.goal.gameObject.tag = "AmbushSpot";
                    bs.inCover = false;
                }
                bs.goal = dest.transform;
                bs.state = BasicSoldier.AIStates.passive;
            }
        }
    }

    bool IsWithinSelectionBounds(GameObject gameObject)
    {
        Debug.Log("checking");
        if (!isSelecting)
            return false;

        Camera cam = Camera.main;
        Bounds viewportBounds = Utils.GetViewportBounds(cam, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(cam.WorldToViewportPoint(gameObject.transform.position));
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            // Create a rect from both mouse positions
            Rect rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
