using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
 	public float speed = 10f;		// the speed in m/s
	public float fireRate = 0.3f;	// seconds/shot (unused)
	public float health = 10;
	public int score = 100;			// points earned for destroying this

	public bool ________________;

	public Bounds bounds;			// the bounds of this and its children
	public Vector3 boundsCenterOffset;	// dist of bounds.center from position

	void Awake () {
		InvokeRepeating ("CheckOffscreen", 0f, 2f);
	}

	// update is called once per frames
	void Update () {
		Move ();
	}

	public virtual void Move() {
		Vector3 tempPos = pos;
		tempPos.y -= speed * Time.deltaTime;
		pos = tempPos;
	}

	// this is a property: a method that acts like a field
	public Vector3 pos {
		get {
			return (this.transform.position);
		}
		set {
			this.transform.position = value;
		}
	}

	void CheckOffscreen () {
		// if bounds are sill their default value ...
		if (bounds.size == Vector3.zero) {
			// then set them
			bounds = Utils.CombineBoundsOfChildren (this.gameObject);
			// also find the diff between bounds.center & transform.position
			boundsCenterOffset = bounds.center - transform.position;
		}

		// every time, update the bounds to the current position
		bounds.center = transform.position + boundsCenterOffset;
		// check to see whether the bounds are completely offscreen
		Vector3 off = Utils.ScreenBoundsCheck (bounds, BoundsTest.offScreen);
		if (off != Vector3.zero) {
			// if this enemy has gone off the bottom edge of the screen
			if (off.y < 0) {
				// then destroy it
				Destroy (this.gameObject);
			}
		}
	}

}
