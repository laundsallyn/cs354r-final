using UnityEngine;
using UnityEngine.SceneManagement;


public class ChangeScene : MonoBehaviour {

    public void nextScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
