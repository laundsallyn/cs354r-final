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
            if (firstSelect)
            {
                selectedObjects.Clear();
                firstSelect = false;
            }
            else
            {
                foreach (GameObject go in allUnits)
                {
                    if (IsWithinSelectionBounds(go) && !selectedObjects.Contains(go))
                    {
                        selectedObjects.Add(go);
                    }
                }
            }
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }
        // If we let go of the left mouse button, end selection
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            firstSelect = true;
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

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);

        return viewportBounds.Contains(
            camera.WorldToViewportPoint(gameObject.transform.position));
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
