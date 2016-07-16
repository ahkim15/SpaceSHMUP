using UnityEngine;
using System.Collections;

// Enemy_1 extends the Enemy class
public class Enemy_1 : Enemy {
	// because Enemy_1 extends Enemy, the ____ bool won't work
	// the same way in the Inspector pane

	// # sec for a full sine wave
	public float waveFrequency = 2;
	// sine wave width in meters
	public float waveWidth = 4;
	public float waveRotY = 45;

	private float x0 = -12345;	// the initial x value of pos
	private float birthTime;

	void Start () {
		x0 = pos.x;

		birthTime = Time.time;
	}

	// override the move function on Enemy
	public override void Move () {
		Vector3 tempPos = pos;
		float age = Time.time - birthTime;
		float theta = Mathf.PI * 2 * age / waveFrequency;
		float sin = Mathf.Sin (theta);
		tempPos.x = x0 + waveWidth * sin;
		pos = tempPos;

		// rotate a bit about y
		Vector3 rot = new Vector3 (0, sin * waveRotY, 0);
		this.transform.rotation = Quaternion.Euler (rot);
		// handles the movement down in y
		base.Move ();
	}

}
