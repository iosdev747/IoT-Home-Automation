using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deleteitCameracontroller : MonoBehaviour {

	float speed = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical")) * speed;
		transform.Translate (velocity);
		float rotation = 0;
		if (Input.GetKey (KeyCode.A)) {
			rotation -= 5;
		}
		if (Input.GetKey (KeyCode.D)) {
			rotation += 5;
		}
		transform.Rotate (0, rotation, 0);

	}
}
