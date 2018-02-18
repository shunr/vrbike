using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float angle;
    public float distance;
    public GameObject hudText;

    private Rigidbody rb;
    private ArduinoIO io;
    private Text txt;
    private AudioSource bike;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        txt = hudText.GetComponent<Text>();
        io = GetComponent<ArduinoIO>();
        Physics.gravity = new Vector3(0, -98F, 0);
        bike = GetComponent<AudioSource>();
        io.Open();
        StartCoroutine(io.AsynchronousReadFromArduino((string raw) => {
            try
            {
                string[] args;
                args = raw.Split(' ');
                speed = float.Parse(args[0]);
                angle = float.Parse(args[1]);
            } catch {
                Debug.Log("rip");
            }
        },
        () => Debug.LogError("Error!"), // Error callback
        1000f));
    }
	
	// Update is called once per frame
	void Update () {
        txt.text = "Total distance: " + (int)distance + " m || Speed: " + speed*5 + " km/h";
    }

    void FixedUpdate() {
        Vector3 movement = rb.transform.forward * Mathf.Min(speed, 5.0f) * 3.5f;
        Vector3 rotate = new Vector3(0.0f, Mathf.Pow(speed, 0.5f)*Mathf.Sin(Mathf.Deg2Rad*angle)*0.8f, 0.0f);
        rb.velocity = movement;
        distance += Mathf.Min(speed, 5.0f) / 34;
        rb.transform.Rotate(rotate);
        if (speed <= 0 )
        {
            bike.mute = true;
        } else
        {
            bike.mute = false;
        }
    }

    void UpdateSpeed(string s) {
        Debug.Log(s);
        speed = float.Parse(s);
    }

    void UpdateRotation(string r) {
        angle = float.Parse(r);
    }
}
