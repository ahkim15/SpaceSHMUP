using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public GameObject	poi;					// player ship
	public GameObject[]	panels;					// scrolling foregrounds
	public float		scrollSpeed = -30f;
	public float		motionMult = 0.25f;		// controls how much panels eract to player movement

	private float panelHt;						// height of each panel
	private float depth;						// depth of panels (that is, pos.z)

	void Start () {
		panelHt = panels [0].transform.localScale.y;
		depth = panels [0].transform.position.z;

		// set initial positions of panels
		panels [0].transform.position = new Vector3 (0, 0, depth);
		panels [1].transform.position = new Vector3 (0, panelHt, depth);
	}

	void Update () {
		float tY, tX = 0;
		tY = Time.time * scrollSpeed % panelHt + (panelHt * 0.5f);

		if (poi != null) {
			tX = -poi.transform.position.x * motionMult;
		}

		// position panels[0]
		panels [0].transform.position = new Vector3 (tX, tY, depth);
		// then position panels[1] where needed to make a continious starfield
		if (tY >= 0) {
			panels [1].transform.position = new Vector3(tX, tY-panelHt, depth);
		} else {
			panels [1].transform.position = new Vector3 (tX, tY+panelHt, depth);
		}
	}

}
