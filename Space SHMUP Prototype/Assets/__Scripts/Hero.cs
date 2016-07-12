using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero S;	// singleton

	// these fields control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	// ship status information
	public float shieldLevel = 1;

	public bool ________________;

	public Bounds bounds;

	void Awake () {
		S = this;	// set the singleton
		bounds = Utils.CombineBoundsOfChildren (this.gameObject);
	}

	void Update () {
		// pull in information from the Input class
		float xAxis = Input.GetAxis ("Horizontal");			// 1. line 23-24 see pg 492
		float yAxis = Input.GetAxis ("Vertical");

		// change transform.position based on the axes
		Vector3 pos = transform.position;
		pos.x += xAxis * speed * Time.deltaTime;
		pos.y += yAxis * speed * Time.deltaTime;
		transform.position = pos;

		bounds.center = transform.position;		// 1. see pg 504

		// keep the ship constrained to the screen bounds
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.onScreen);	// 2. see pg 504
		if (off != Vector3.zero) {				// 3. see pg 504
			pos -= off;
			transform.position = pos;
		}

		// rotate the ship to make it feel more dynamic		// 2. see pg 492
		transform.rotation = Quaternion.Euler (yAxis * pitchMult, xAxis * rollMult, 0);
	}
}
