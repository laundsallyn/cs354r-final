using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {

    public void nextScene(string scene)
    {
        if(GameObject.Find("HIT")!= null && !(scene.Equals("GameOver")))
            Destroy(GameObject.Find("HIT"));
        if(Time.timeScale==0 && PlayerPrefs.GetInt("Pause") ==1)
        {
            PlayerPrefs.SetInt("Pause", 0);
            PlayerPrefs.Save();
            Time.timeScale=1;       
            Debug.Log("WAITING UP"); 
        }
        SceneManager.LoadScene(scene);
    }

 /*
    void nextScene(string scene)
    {
        StartCoroutine(wait(scene));
    }
    IEnumerator wait(string scene)
    {
        float fadeTime = GameObject.Find("Option").GetComponent<fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(scene);

    }
    */
}
