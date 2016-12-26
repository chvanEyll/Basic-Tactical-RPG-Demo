using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class smallFightMng : MonoBehaviour {

	public static bool echange_coups;
	public static bool moving_in;
	public static bool moving_out;
	public static GameObject attacker;
	public static GameObject target;
	public static GameObject hero_f;
	public static GameObject foe_f;
	public static GameObject fight_bg;
	public static TextMesh hero_hpTxt;
	public static TextMesh foe_hpTxt;
	public static Vector3 attacker_initial_pos;

	public GameObject fighting_hero;
	public GameObject fighting_enemy;

	moveMng moveMngScript;
	enemy enemyScript;
	hero heroScript;

	void Start () {

		echange_coups = false;
		hero_f = GameObject.Find("hero_combat");
		foe_f = GameObject.Find("foe_combat");
		fight_bg = GameObject.Find ("fight_bg");
		hero_hpTxt = GameObject.Find ("hero_hpTxt").GetComponent<TextMesh>();
		foe_hpTxt = GameObject.Find ("foe_hpTxt").GetComponent<TextMesh>();
		moveMngScript = GameObject.Find("mngHolder").GetComponent ("moveMng") as moveMng;

	}

	public List<GameObject> Encounter_exists() {

		//creating null list to return false
		List<GameObject> encounter_units = new List<GameObject> ();

		foreach (GameObject hero_it in moveMngScript.allHeroes) {

			heroScript = hero_it.GetComponent ("hero") as hero;
			char hx = heroScript.pos_actuelle[0];
			int hy = int.Parse(heroScript.pos_actuelle.Substring(1,1));

			if (moveMngScript.allEnemies != null) {

				foreach (GameObject enemy_it in moveMngScript.allEnemies) {

					enemyScript = enemy_it.GetComponent ("enemy") as enemy;
					char ex = enemyScript.pos_actuelle[0];
					int ey = int.Parse(enemyScript.pos_actuelle.Substring(1,1));

					// test for collision
					if (hx == ex) {

						if ((hy == ey+1) ||  (hy == ey-1)) {
							
							encounter_units.Add (hero_it);
							encounter_units.Add (enemy_it);
							fighting_hero = hero_it;
							fighting_enemy = enemy_it;
							return encounter_units;

						}

					} else if (hy == ey) {

						if ((hx == ex+1) ||  (hx == ex-1)) {
							
							encounter_units.Add (hero_it);
							encounter_units.Add (enemy_it);
							fighting_hero = hero_it;
							fighting_enemy = enemy_it;
							return encounter_units;

						}
					}
				} 

			}
		} return encounter_units; // return empty list

	}

	bool fighter_is_dead() {

		heroScript = fighting_hero.GetComponent ("hero") as hero;
		enemyScript = fighting_enemy.GetComponent ("enemy") as enemy;

		if (heroScript.hitPoints <= 0) {

			return true;

		} else if (enemyScript.hitPoints <= 0) {

			return true;

		} return false;

	}

	public void gen_fight_window() {

		heroScript = fighting_hero.GetComponent ("hero") as hero;
		enemyScript = fighting_enemy.GetComponent ("enemy") as enemy;

		// instantiate initial fight window
		hero_hpTxt.text = "HP: " + heroScript.hitPoints;
		foe_hpTxt.text = "HP: " + enemyScript.hitPoints;
		fight_bg.transform.position = new Vector3 (0.03F, 0.1F, -3);

		// initial attack order before loop
		attacker = foe_f;
		target = hero_f;

		//starting attack loop
		alterner_coups();

	}

	void remove_from_list (ref GameObject[] old_unit_list, GameObject unit) {

		GameObject[] new_unit_list = null;
		int i = 0;
			
			foreach (GameObject unit_it in old_unit_list) {

				if (unit_it != unit) {

					new_unit_list[i] = unit_it;
					i++;
				}
		}

		old_unit_list = new_unit_list;

	}

	void alterner_coups() {
		
		if (!fighter_is_dead()) {

			//alternate attack order
			if ((attacker == hero_f) && (target == foe_f)) {

				attacker = foe_f;
				target = hero_f;

			} else {

				attacker = hero_f;
				target = foe_f;

			}

			attacker_initial_pos = attacker.transform.position;

			moving_in = true;

		} else if (fighter_is_dead()) {
			
			Destroy (fight_bg);

			if (target == foe_f) {
			
				remove_from_list (ref moveMngScript.allEnemies, fighting_enemy);
				Destroy (fighting_enemy);
				moveMng.turn = "hero";

			} else {

				remove_from_list (ref moveMngScript.allHeroes, fighting_hero);
				Destroy (fighting_hero);
				moveMng.turn = "enemy";

			}

		}
			
	}

	void move_in() {

		enemyScript = fighting_enemy.GetComponent ("enemy") as enemy;
		heroScript = fighting_hero.GetComponent ("hero") as hero;

		if (attacker.transform.position == target.transform.position) {

			// modify HP values
			if (target == foe_f) {

				enemyScript.hitPoints = enemyScript.hitPoints - heroScript.str;

			} else if (target == hero_f) {

				heroScript.hitPoints = heroScript.hitPoints - enemyScript.str;

			}

			hero_hpTxt.text = "HP: " + heroScript.hitPoints;
			foe_hpTxt.text = "HP: " + enemyScript.hitPoints;

			moving_out = true;
			moving_in = false;

		}

		attacker.transform.position = Vector3.Lerp (attacker.transform.position, target.transform.position,15*Time.deltaTime); // move in

	}

	void move_out() {

		enemyScript = fighting_enemy.GetComponent ("enemy") as enemy;
		heroScript = fighting_hero.GetComponent ("hero") as hero;

		if (attacker.transform.position == attacker_initial_pos) {

			moving_out = false;
			alterner_coups ();

		}

		attacker.transform.position = Vector3.Lerp (attacker.transform.position, attacker_initial_pos,15*Time.deltaTime); // move out

	}

	// attack rotation in this update
	void Update () {

		if (moving_in) {

			move_in ();
		
		}

		 else if (moving_out) {
				
			move_out ();

		}

	}

}