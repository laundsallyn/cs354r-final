using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text scores;
    private int score;
    // Use this for initialization
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > 5 && PlayerPrefs.GetInt("Pause")!=1)
            score += 1;
        scores.text = "" + score;
    }
}
