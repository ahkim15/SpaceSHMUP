using UnityEngine;
using System.Collections;

public class Hero : MonoBehaviour {
	static public Hero S;	// singleton

	public float gameRestartDelay = 2f;

	// these fields control the movement of the ship
	public float speed = 30;
	public float rollMult = -45;
	public float pitchMult = 30;

	// ship status information
	[SerializeField]
	private float _shieldLevel = 1;

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

	// this variable holds a reference to the last triggering GameObject
	public GameObject lastTriggerGo = null;				// 1. see pg 517

	void OnTriggerEnter (Collider other) {
		// find the tag of other.gameObject or its parent GameObjects
		GameObject go = Utils.FindTaggedParent (other.gameObject);
		// if there is a parent with a tag
		if (go != null) {
			// make sure it's not the same triggering go as last time
			if (go == lastTriggerGo) {					// 2. see pg 517
				return;
			}
			lastTriggerGo = go;							// 3. see pg 517

			if (go.tag == "Enemy") {
				// if the shield was triggered by an enemy
				// decrease the level of the shield by 1
				shieldLevel--;
				// destroy the enemy
				Destroy (go);							// 4. see pg 517
			} else {
				print ("Triggered: " + go.name);
			}
		} else {
			// otherwise announce the original other.gameObject
			print ("Triggered: " + other.gameObject.name);
		}
	}

	public float shieldLevel {
		get {
			return (_shieldLevel);						// 1. see pg 518
		}
		set {
			_shieldLevel = Mathf.Min (value, 4);		// 2. see pg 518
			// if the shield is going to be set to less than zero
			if (value < 0) {							// 3. see pg 518
				Destroy (this.gameObject);
				// tell Main.Sto restart the game after a delay
				Main.S.DelayedRestart (gameRestartDelay);
			}
		}
	}
	
}
