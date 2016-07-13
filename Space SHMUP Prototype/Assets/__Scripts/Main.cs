using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;

	public GameObject [] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;	// # enemies/second
	public float enemySpawnPadding = 1.5f;		// padding for position

	public bool _______________________;

	public float enemySpawnRate;			// delay between enemy spawns

	void Awake () {
		S = this;
		// set Utils.camBounds
		Utils.SetCameraBounds (this.camera);
		// 0.5 enemies/second = enemySpawnRate of 2
		enemySpawnRate = 1f / enemySpawnPerSecond;					// 1. see pg 510
		// invoke call SpawnEnemy() once after a 2 second delay
		Invoke ("SpawnEnemy", enemySpawnRate);						// 2. see pg 510
	}

	public void SpawnEnemy () {
		// pick a random Enemy prefab to instantiate
		int ndx = Random.Range (0, prefabEnemies.Length);
		GameObject go = Instantiate (prefabEnemies [ndx]) as GameObject;
		// position the enemy above the screen with a random x position
		Vector3 pos = Vector3.zero;
		float xMin = Utils.camBounds.min.x + enemySpawnPadding;
		float xMax = Utils.camBounds.max.x - enemySpawnPadding;
		pos.x = Random.Range (xMin, xMax);
		pos.y = Utils.camBounds.max.y + enemySpawnPadding;
		go.transform.position = pos;
		// call SpawnEnemy() again in a couple of seconds
		Invoke ("SpawnEnemy", enemySpawnRate);						// 3. see pg 510
	}

}
