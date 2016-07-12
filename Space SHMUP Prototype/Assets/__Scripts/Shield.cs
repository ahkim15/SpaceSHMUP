using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {
	public float rotationsPerSecond = 0.1f;
	public bool ___________________;
	public int levelShown = 0;

	void Update () {
		// read the current shield level from the Hero Singleton
		int currLevel = Mathf.FloorToInt (Hero.S.shieldLevel);				// 1. see pg 495
		// if this is different from levelShown ...
		if (levelShown != currLevel) {
			levelShown = currLevel;
			Material mat = this.renderer.material;
			// adjust the texture offset to show different shield level
			mat.mainTextureOffset = new Vector2 (0.2f * levelShown, 0);		// 2. see pg 495
		}
		//rotate the shield a bit every second
		float rZ = (rotationsPerSecond * Time.time * 360) % 360f;			// 3. see pg 495
		transform.rotation = Quaternion.Euler (0, 0, rZ);
	}
}
