using UnityEngine;
using System.Collections;

// part is another serializable data storage class just like WeaponDefinition
[System.Serializable]
public class Part {
	// these three fields need to be defined in the Inspector pane
	public string	name;			// the name of this part
	public float	health;			// the amount of health this part has
	public string[]	protectedBy;	// the other parts that protect this

	// these two fields are set automatically in Start ()
	// caching like this makes it faster and easier to find these later

	public GameObject	go;		// the GameObject of this part
	public Material		mat;	// the material to show damage
}

public class Enemy_4 : Enemy {
	// enemy_4 will start offscreen and then pick a random point on screen to move to
	// oncce it has arrived, it will pick another random point and continue until the 
	// player has shot it down
	public Vector3[]	points;			// stores the p0 & p1 for interpolation
	public float		timeStart;		// birth time for this enemy_4
	public float		duration = 4;	// duration of movement

	public Part []		parts;			// the array of ship Parts

	void Start () {
		points = new Vector3[2];
		// there is already an initial position chosen by Main.SpawnEnemy()
		// so add it to points as the initial p0 & p1
		points [0] = pos;
		points [1] = pos;

		InitMovement ();

		// cache GameObject & Material of each Part in parts
		Transform t;
		foreach (Part prt in parts) {
			t = transform.Find (prt.name);
			if (t != null) {
				prt.go = t.gameObject;
				prt.mat = prt.go.renderer.material;
			}
		}

	}

	void InitMovement () {
		// pick a new point to move to that is on screen
		Vector3 p1 = Vector3.zero;
		float esp = Main.S.enemySpawnPadding;
		Bounds cBounds = Utils.camBounds;
		p1.x = Random.Range (cBounds.min.x + esp, cBounds.max.x - esp);
		p1.y = Random.Range (cBounds.min.y + esp, cBounds.max.y - esp);

		points [0] = points [1];	// shift points [1] to points [0]
		points [1] = p1;			// add p1 as points[1]

		// reset the time
		timeStart = Time.time;
	}

	public override void Move () {
		// this completely overrides Enemy.Move() with a linear interpolation

		float u = (Time.time - timeStart) / duration;
		if (u >= 1) {	// if u>=1...
			InitMovement ();		// .. then initialize movement to a new point
			u = 0;
		}

		u = 1 - Mathf.Pow (1 - u, 2);		// apply ease out easing to u

		pos = (1 - u) * points [0] + u * points [1];	// simple linear interpolation
	}

	// this will override the OnCollisionEnter tht is part of Enemy.cs
	// because of the way that Monobehavior declares common Unity functions
	void OnCollisionEnter (Collision coll) {
		GameObject other = coll.gameObject;
		switch (other.tag) {
		case "ProjectileHero":
			Projectile p = other.GetComponent <Projectile>();
			// enemies don't take damage unless they're on screen
			// this stops the player from shooting them before they are visible
			bounds.center = transform.position + boundsCenterOffset;
			if (bounds.extents == Vector3.zero || Utils.ScreenBoundsCheck(bounds, BoundsTest.offScreen) != Vector3.zero) {
				Destroy(other);
				break;
			}

			// hurt this enemy
			// find the GameObject that was hit
			// the collison coll has contacts[], an array of ContactPoints because there was a collision,
			// we're guaranteed that there is at least a contacts[0], and ContactPoints have a reference to
			// thisCollider, which will be the collider for the part of the Enemy_4 that was hit
			GameObject goHit = coll.contacts[0].thisCollider.gameObject;
			Part prtHit = FindPart(goHit);
			if (prtHit == null) {		// if prt wasn't found
				goHit = coll.contacts[0].otherCollider.gameObject;
				prtHit = FindPart(goHit);
			}
			// check wheather this part is still protected
			if (prtHit.protectedBy != null) {
				foreach (string s in prtHit.protectedBy) {
					// if one of the protecting parts hasn't been destroyed ...
					if (!Destroyed(s)) {
						// .. then don't damange this part yet
						Destroy (other);	// destroy the ProjectileHero
						return;		// return before causing damage
					}
				}
			}
			// it's not protected, so make it take damage
			// get the damage amount from the Projectile.type & Main.W_DEFS
			prtHit.health -= Main.W_DEFS[p.type].damageOnHit;
			// show damage on the part
			ShowLocalizedDamage(prtHit.mat);
			if (prtHit.health <= 0) {
				// instead of destroying this enemy, disable the damaged part
				prtHit.go.SetActive (false);
			}
			// check to see if the whole ship is destroyed
			bool allDestroyed = true;	// assume it is destroyed
			foreach (Part prt in parts) {
				if (!Destroyed(prt)) {	// if a part still exists
					allDestroyed = false;	//.. change allDestroyed to false
					break;		// and break out of the foreach loop
				}
			}
			if (allDestroyed) {		// if it IS completely destroyed
				// tell the Main singleton that his ship has been destroyed
				Main.S.ShipDestroyed (this);
				// Destroy this Enemy
				Destroy(this.gameObject);
			}
			Destroy(other);	// destroy the projectileHero
			break;
		}
	}

	// these two functions find a part in this.parts by name or GameObject
	Part FindPart (string n) {
		foreach (Part prt in parts) {
			if (prt.name == n) {
				return (prt);
			}
		}
		return (null);
	}
	Part FindPart (GameObject go) {
		foreach (Part prt in parts) {
			if (prt.go == go) {
				return(prt);
			}
		}
		return (null);
	}

	// these functions return true if the part has been destroyed
	bool Destroyed(GameObject go) {
		return (Destroyed (FindPart (go)));
	}
	bool Destroyed(string n) {
		return (Destroyed (FindPart (n)));
	}
	bool Destroyed(Part prt) {
		if (prt == null) {		// if no real Part was passed on
			return (true);		// return true (meaning yes, it was destroyed)
		}
		// returns the result of the comparison: prt.health <= 0
		// if prt.health is 0 or less, returns true (yes, it was destroyed)
		return (prt.health <= 0);
	}

	// this changes the color of just one Part to red instead of the whole ship
	void ShowLocalizedDamage (Material m) {
		m.color = Color.red;
		remainingDamageFrames = showDamageForFrames;
	}

}
