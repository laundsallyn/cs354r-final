using UnityEngine;
using System.Collections;

public class raycastForward : MonoBehaviour {

	
	// Update is called once per frame
	void Update () {
        RaycastHit hit;
        float distance;

        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
	
	}
}
