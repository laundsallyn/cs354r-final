using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class HealthPack : MonoBehaviour {
    private GameObject pack;
    private float timer;
    private static int heal = 150;
    private bool started;
    private Collider col;
    void Start()
    {
        col = GetComponent<Collider>();
        pack = transform.FindChild("pack").gameObject;
        timer = 0f;
    }
    void OnCollisionEnter(Collision collision)
    {
        if (started) return;
        Debug.Log("Collision!");
        collision.collider.SendMessageUpwards("heal", heal, SendMessageOptions.DontRequireReceiver);
        started = true;
    }

    // Update is called once per frame
    void FixedUpdate () {
        if (started && pack.activeSelf)
        {
            col.enabled = !col.enabled;
            pack.SetActive(false);
        }
        timer += Time.fixedDeltaTime;

        if(started && timer >= 10f)
        {
            col.enabled = !col.enabled;
            started = false;
            pack.SetActive(true);
            timer = 0f;
        }

    }
}
