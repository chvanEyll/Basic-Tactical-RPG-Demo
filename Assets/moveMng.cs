using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class moveMng : MonoBehaviour {
	
	struct moving_unit {
		
		public Vector3 targetPos;
		public GameObject unit;
		public string target_pos_coord;
	
	}

	moving_unit current_moving_unit;

	public static string turn;

	smallFightMng fightMngScript;

	public GameObject[] allHeroes;
	public GameObject[] allEnemies;

	GameObject null_gameobject;

	void Start () {
		
		fightMngScript = GameObject.Find("mngHolder").GetComponent ("smallFightMng") as smallFightMng;

		// grab and count all characters on the game
		allHeroes = UnityEngine.GameObject.FindGameObjectsWithTag("hero");
		allEnemies = UnityEngine.GameObject.FindGameObjectsWithTag("enemy");

		turn = "hero"; // starting turn

		null_gameobject = new GameObject ();

	}

	// HERO FUNCTIONS

	public void select_hero(GameObject hero_obj) {
		
		if (turn=="hero") {
			
			hero heroScript = hero_obj.GetComponent ("hero") as hero;
			SpriteRenderer hero_renderer = hero_obj.GetComponent<SpriteRenderer>();
			Sprite spr_reg = Resources.Load<Sprite> ("hero");
			Sprite spr_hl = Resources.Load<Sprite> ("hero_highlight");

			//highlight hero on click
			remettreCasesBlanches ();

			if (heroScript.selected) {
				hero_renderer.sprite = spr_reg;
				heroScript.selected = false;
			} else {
				hero_renderer.sprite = spr_hl;
				heroScript.selected = true;
				highlight_available_tiles (hero_obj); // in movemng

			}
		}

	}

	void highlight_available_tiles(GameObject hero_obj) {

		hero heroScript = hero_obj.GetComponent ("hero") as hero;
		Sprite case_hl = Resources.Load<Sprite> ("tile_hl");

		// highlight every possible move
		foreach (Transform child in GameObject.Find("damier").transform) {
			
			if (heroScript.peutSeDeplacer(child.gameObject.name)) {

				child.GetComponent<SpriteRenderer> ().sprite = case_hl;

			};
		}
	}

	public void move_hero(string tile_coord) {

		hero heroScript;

		if (find_current_moving_unit() == null_gameobject) { // no hero must be moving
			
			// check if one of the heros is selected
			foreach (GameObject hero_it in allHeroes) {
				
				heroScript = hero_it.GetComponent ("hero") as hero;

				if (heroScript.selected) {

					if (heroScript.peutSeDeplacer(tile_coord)) {
						
						// start update move
						Vector3 tile_position = GameObject.Find(tile_coord).transform.position;
						current_moving_unit.targetPos = new Vector3 (tile_position.x, tile_position.y, -3);
						current_moving_unit.unit = hero_it;
						current_moving_unit.target_pos_coord = tile_coord;

						heroScript.moving = true; 
						break;

					} 

				}

			}

		}

	}

	void moveHero() {

		if (find_current_moving_unit().tag == "hero") {
			hero movingHeroScript;
			movingHeroScript = current_moving_unit.unit.GetComponent ("hero") as hero;

			current_moving_unit.unit.transform.position = Vector3.Lerp(current_moving_unit.unit.transform.position, current_moving_unit.targetPos, Time.deltaTime*15);

			// hero reached target
			if (current_moving_unit.unit.transform.position == current_moving_unit.targetPos){

				remettreCasesBlanches ();
				movingHeroScript.selected = false;
				movingHeroScript.pos_actuelle = current_moving_unit.target_pos_coord;

				// remettre héros correct
				Sprite spr_reg = Resources.Load<Sprite> ("hero");
				SpriteRenderer rend = current_moving_unit.unit.GetComponent<SpriteRenderer>();
				rend.sprite = spr_reg;

				movingHeroScript.moving = false;

				if (fightMngScript.Encounter_exists().Count != 0) {

					turn = "off";
					fightMngScript.gen_fight_window ();

				} else {

					if (allEnemies != null) {

						turn = "enemy";
						move_random_enemy ();

					}

				}

			}
		}

	}

	// ENEMY FUNCTIONS

	GameObject find_hero_closest_to_enemy(GameObject chosen_enemy) {

		hero heroScript;
		GameObject best_hero = null;
		float best_distance_to_hero = 100;

		foreach(GameObject hero_it in allHeroes) {

			heroScript = hero_it.GetComponent ("hero") as hero;
			float distance_to_hero = Vector3.Distance (chosen_enemy.transform.position,hero_it.transform.position);

			if (distance_to_hero < best_distance_to_hero) {

				best_hero = hero_it;
				best_distance_to_hero = distance_to_hero;

			}

		}

		hero bestHeroScript = best_hero.GetComponent ("hero") as hero;
		GameObject best_hero_tile = GameObject.Find (bestHeroScript.pos_actuelle);

		return best_hero_tile;

	}

	GameObject find_tile_closest_to_hero(List<GameObject> reachable_tiles, GameObject best_hero_tile) {

		GameObject best_tile = null;
		float best_distance_to_tile = 100;

		foreach(GameObject tile_it in reachable_tiles) {

			GameObject tile_obj = GameObject.Find (tile_it.name);
			float distance_to_tile = Vector3.Distance (best_hero_tile.transform.position ,tile_obj.transform.position);

			if (distance_to_tile < best_distance_to_tile) {

				best_tile = tile_obj;
				best_distance_to_tile = distance_to_tile;

			}

		}

		return best_tile;

	}
		
	void move_random_enemy () { 

		Sprite case_hl = Resources.Load<Sprite> ("tile_hl");

		// pick a random enemy
		System.Random rnd = new System.Random();
		int random_enemy_index = rnd.Next (0, allEnemies.Length);
		GameObject chosen_enemy = allEnemies [random_enemy_index];
		enemy chosenEnemyScript = chosen_enemy.GetComponent ("enemy") as enemy;

		// move the random enemy
		if ((turn == "enemy") && (find_current_moving_unit() == null_gameobject)) { // control the turn elsewehere!


			//met dans une liste tous les mouvements disponibles

			List<GameObject> reachable_tiles = new List<GameObject> ();

			foreach (Transform child in GameObject.Find("damier").transform)
				
			{
				if (chosenEnemyScript.peutSeDeplacer(child.gameObject.name)) {
					
					child.GetComponent<SpriteRenderer> ().sprite = case_hl;
					reachable_tiles.Add(child.gameObject);

				}

			}

			// find best move
			GameObject closest_hero = find_hero_closest_to_enemy (chosen_enemy);
			GameObject closest_tile_to_closest_hero = find_tile_closest_to_hero(reachable_tiles, closest_hero);

			// activate move. 
			current_moving_unit.targetPos = new Vector3 (closest_tile_to_closest_hero.transform.position.x, closest_tile_to_closest_hero.transform.position.y, -3);
			current_moving_unit.target_pos_coord = closest_tile_to_closest_hero.name;
			current_moving_unit.unit = chosen_enemy;
			chosenEnemyScript.moving = true;

		}

	}

	void moveEnemy() {

		enemy movingEnemyScript;
		movingEnemyScript = current_moving_unit.unit.GetComponent ("enemy") as enemy;

		current_moving_unit.unit.transform.position = Vector3.Lerp(current_moving_unit.unit.transform.position, current_moving_unit.targetPos, Time.deltaTime*15);

		// enemy reached target
		if (current_moving_unit.unit.transform.position == current_moving_unit.targetPos){

			// modifications
			remettreCasesBlanches ();
			movingEnemyScript.pos_actuelle = current_moving_unit.target_pos_coord;

			movingEnemyScript.moving = false;

			if (fightMngScript.Encounter_exists().Count != 0) {

				turn = "off";
				fightMngScript.gen_fight_window ();

			} else {

				if (allHeroes != null) {

					turn = "hero";

				}
			}

		}

	}

	// GENERAL FUNCTIONS

	public void remettreCasesBlanches() {

		Sprite case_reg = Resources.Load<Sprite> ("tile");

		foreach(Transform child in GameObject.Find("damier").transform)
		{

			child.GetComponent<SpriteRenderer> ().sprite = case_reg;

		}

	}
	
	GameObject find_current_moving_unit() {
		
		// iterate through heroes
		hero heroScript;

		if (allHeroes != null) {
			
			foreach (GameObject hero_it in allHeroes) {

				if (allHeroes != null) {
					
					heroScript = hero_it.GetComponent ("hero") as hero;

					if (heroScript.moving) {

						return hero_it;

					}

				}
			}

		}
		
		// iterate through enemies
		enemy enemyScript;

		if (allEnemies != null) {
			
			foreach (GameObject enemy_it in allEnemies) {

				enemyScript = enemy_it.GetComponent ("enemy") as enemy;

				if (enemyScript.moving) {

					return enemy_it;

				}
			}

		} return null_gameobject;
		
	}

	// update is used to move characters with frame by frame Lerp
	void Update () {

		if (find_current_moving_unit() != null_gameobject) {

			moveHero ();

		}

		if (find_current_moving_unit().tag == "enemy") {

				moveEnemy ();

		}

	}

}