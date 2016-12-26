using UnityEngine;
using System.Collections;

public class statPosMng : MonoBehaviour {

	GameObject[] allHeroes;
	GameObject[] allEnemies;

	hero heroScript;
	enemy enemyScript;

	void Start () {

		// here i will put the stats and positions for each hero and enemy

		allHeroes = UnityEngine.GameObject.FindGameObjectsWithTag("hero");
		allEnemies = UnityEngine.GameObject.FindGameObjectsWithTag("enemy");

		// initialize default values
		foreach (GameObject hero_it in allHeroes) {
		
			heroScript = hero_it.GetComponent ("hero") as hero;
			heroScript.pos_actuelle = hero_it.name.Substring (4, 2); // coords come from gameObject name
			heroScript.hitPoints = 10;
			heroScript.str = 3;
			heroScript.moving = false;
		
		}

		foreach (GameObject enemy_it in allEnemies) {

			enemyScript = enemy_it.GetComponent ("enemy") as enemy;
			enemyScript.pos_actuelle = enemy_it.name.Substring (3, 2); // coords come from gameObject name
			enemyScript.hitPoints = 5;
			enemyScript.str = 1;
			enemyScript.moving = false;

		}
	
	}

}