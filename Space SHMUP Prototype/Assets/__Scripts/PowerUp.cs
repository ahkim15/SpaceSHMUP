using UnityEngine;
using System.Collections;

public class PowerUp : MonoBehaviour {
	// this is an unusual but handy use of Vector2s. x holds a min value
	// and y a max value for a Random.Range() that will be called later
	public Vector2		rotMinMax = new Vector2(15, 90);
	public Vector2		driftMinMax = new Vector2(.25f, 2);
	public float		lifeTime = 6f;		// seconds the PowerUp exists
	public float		fadeTime = 4f;		// seconds it will then fade
	public bool _________________;
	public WeaponType	type;				// the type of the PowerUp
	public GameObject	cube;				// reference to the Cube child
	public TextMesh		letter;				// reference to the TextMesh
	public Vector3		rotPerSecond;		// euler rotation speed
	public float		birthTime;

	void Awake () {
		// find the Cube reference
		cube = transform.Find("Cube").gameObject;
		// find the TextMesh
		letter = GetComponent<TextMesh> ();

		// set a random velocity
		Vector3 vel = Random.onUnitSphere;		// get Random XYZ velocity
		// Random.onUnitSphere gives you a vector point that is somewhere on
		// the surface of the sphere with a radius of 1m around the origin
		vel.z = 0;		// flatten the vel to the XY plane
		vel.Normalize ();		//make the length of the vel 1
		// normalizing a Vector3 makes it length 1m
		vel *= Random.Range (driftMinMax.x, driftMinMax.y);
		// above sets the velocity length to something between the x and y values of driftMinMax
		rigidbody.velocity = vel;

		// set the rotation of this GameObject to R:[0,0,0]
		transform.rotation = Quaternion.identity;
		// Quaternion.identity is equal to no rotation

		// set up the rotPerSecond for the Cube child using rotMinMax x & y
		rotPerSecond = new Vector3 (Random.Range (rotMinMax.x, rotMinMax.y),
		                            Random.Range (rotMinMax.x, rotMinMax.y),
		                            Random.Range (rotMinMax.x, rotMinMax.y));

		// CheckOffscreen() every 2 seconds
		InvokeRepeating ("CheckOffscreen", 2f, 2f);

		birthTime = Time.time;
	}

	void Update () {
		// manually rotate the cube child every Update()
		// multiplying it by Time.time causes the rotation to be time-based
		cube.transform.rotation = Quaternion.Euler (rotPerSecond * Time.time);

		// fade out the PowerUp over time
		// given the default values, a PowerUp will exist for 10 seconds & then fade out every 4 sec
		float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
		// for lifeTime seconds, u will be <= 0. then it will transition to 1 over fadeTime seconds
		// if u >= 1, destroy this PowerUp
		if (u >= 1) {
			Destroy(this.gameObject);
			return;
		}
		// use u to determine the alpha value of the Cube & Letter
		if (u > 0) {
			Color c = cube.renderer.material.color;
			c.a = 1f-u;
			cube.renderer.material.color = c;
			c = letter.color;
			c.a = 1f - (u*0.5f);
			letter.color = c;
		}
	}

	// this SetType() differes from those on Weapon and Projectile
	public void SetType(WeaponType wt) {
		// grab the WeaponDefinition from Main
		WeaponDefinition def = Main.GetWeaponDefinition (wt);
		// set the color of the cube child
		cube.renderer.material.color = def.color;
		letter.text = def.letter;		// set the letter that is shown
		type = wt;		// finally actually set the type
	}

	public void AbsorbedBy (GameObject target) {
		// this function is called by the Hero class when a PowerUp is collected
		// we could tween into the target and shrink in size, but for now, just destroy this.gameObject
		Destroy (this.gameObject);
	}

	void CheckOffscreen () {
		// if the PowerUp has drifted entirely off screen ...
		if (Utils.ScreenBoundsCheck (cube.collider.bounds, BoundsTest.offScreen) != Vector3.zero) {
			// .. then destroy this GameObject
			Destroy(this.gameObject);
		}
	}


}
