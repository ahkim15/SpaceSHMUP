using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	[SerializeField]
	private WeaponType _type;
	// this public property masks the field_type & takes action when it is set
	public WeaponType type {
		get {
			return(_type);
		}
		set {
			SetType (value);
		}
	}

	void Awake () {
		// test to see whether this has passed off screen every 2 seconds
		InvokeRepeating ("CheckOffscreen", 2f, 2f);
	}

	public void SetType (WeaponType eType) {
		// set the type
		_type = eType;
		WeaponDefinition def = Main.GetWeaponDefinition (_type);
		renderer.material.color = def.projectileColor;
	}

	void CheckOffscreen() {
		if (Utils.ScreenBoundsCheck (collider.bounds, BoundsTest.offScreen) != Vector3.zero) {
			Destroy (this.gameObject);
		}
	}
	
}
