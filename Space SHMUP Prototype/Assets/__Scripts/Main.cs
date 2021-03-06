﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {
	static public Main S;
	static public Dictionary<WeaponType, WeaponDefinition> W_DEFS;

	public GameObject [] prefabEnemies;
	public float enemySpawnPerSecond = 0.5f;	// # enemies/second
	public float enemySpawnPadding = 1.5f;		// padding for position
	public WeaponDefinition[] weaponDefinitions;
	public GameObject prefabPowerUp;
	public WeaponType[] powerUpFrequency = new WeaponType[] {
		WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield };

	public bool _______________________;

	public WeaponType[] activeWeaponTypes;
	public float enemySpawnRate;			// delay between enemy spawns

	void Awake () {
		S = this;
		// set Utils.camBounds
		Utils.SetCameraBounds (this.camera);
		// 0.5 enemies/second = enemySpawnRate of 2
		enemySpawnRate = 1f / enemySpawnPerSecond;					// 1. see pg 510
		// invoke call SpawnEnemy() once after a 2 second delay
		Invoke ("SpawnEnemy", enemySpawnRate);						// 2. see pg 510

		// a generic Dictionary with WeaponType as the key
		W_DEFS = new Dictionary<WeaponType, WeaponDefinition> ();
		foreach (WeaponDefinition def in weaponDefinitions) {
			W_DEFS [def.type] = def;
		}
	}

	static public WeaponDefinition GetWeaponDefinition (WeaponType wt) {
		// check to make sure that hte key exists in the Dictionary
		// attempting to retrieve a key that didn't exist, would throw an error,
		// the following if statment is important
		if (W_DEFS.ContainsKey (wt)) {
			return (W_DEFS [wt]);
		}
		// this will return a definition for WeaponType.none, which means
		// it has failed to find the WeaponDefinition
		return (new WeaponDefinition ());
	}

	void Start () {
		activeWeaponTypes = new WeaponType[weaponDefinitions.Length];
		for (int i=0; i < weaponDefinitions.Length; i++) {
			activeWeaponTypes[i] = weaponDefinitions[i].type;
		}
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

	public void ShipDestroyed (Enemy e) {
		// potentially generate a PowerUp
		if (Random.value <= e.powerUpDropChance) {
			// random.value generates a value between 0 & 1 (though never == 1) if the e.powerUpDropChance
			// is 0.50f, a PowerUp will be generated 50% of the time. For testing, it's now set to 1f

			// choose which PowerUp to pick
			// pick one from the possibilities in powerUpFrequency
			int ndx = Random.Range (0, powerUpFrequency.Length);
			WeaponType puType = powerUpFrequency [ndx];

			// spawn a PowerUp
			GameObject go = Instantiate (prefabPowerUp) as GameObject;
			PowerUp pu = go.GetComponent<PowerUp> ();
			// set it to the proper WeaponType
			pu.SetType (puType);

			// set it to the position of the destroyed ship
			pu.transform.position = e.transform.position;
		}
	}

	public void DelayedRestart (float delay) {
		// invoke the Restart () method in the delay seconds
		Invoke ("Restart", delay);
	}

	public void Restart () {
		// reload _Scene_0 to restart the game
		Application.LoadLevel ("_Scene_0");
	}

}
