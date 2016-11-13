using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class DisplayScore : MonoBehaviour {
    public Text score;
	// Use this for initialization
	void Awake()
    {
        score.text = PlayerPrefs.GetString("Score");
    }
}
