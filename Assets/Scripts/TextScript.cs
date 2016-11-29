using UnityEngine;
using UnityEngine.UI;

public class TextScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetComponent<Text>().text = "FPS: " + 1.0f / Time.deltaTime;
    }
}
