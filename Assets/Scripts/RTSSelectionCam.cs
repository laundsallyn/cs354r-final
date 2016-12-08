using UnityEngine;
using System.Collections;

public class RTSSelectionCam : MonoBehaviour {

    bool isSelecting = false;
    bool firstSelect = true;
    Vector3 mousePosition1;
    ArrayList selectedObjects = new ArrayList();
    GameObject[] allUnits;

    void Start()
    {
        allUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
    }

    void Update()
    {
        // If we press the left mouse button, save mouse location and begin selection
        if (Input.GetMouseButtonDown(0))
        {
            if (!isSelecting)
                selectedObjects.Clear();
            foreach (GameObject go in allUnits)
            {
                Debug.Log("checking this object");
                if (IsWithinSelectionBounds(go))
                {
                    Debug.Log("adding this object");
                    selectedObjects.Add(go);
                }
            }
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("RMB");
            RTSClick();
        }
    }

    void RTSClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject dest = new GameObject();
            dest.transform.position = hit.point;
            foreach(GameObject go in selectedObjects)
            {
                MoveTo mt = go.GetComponent<MoveTo>();
                mt.goal = dest.transform;
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
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }
}
