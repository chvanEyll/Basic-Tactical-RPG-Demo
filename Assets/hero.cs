using UnityEngine;
using System.Collections;

public class hero : MonoBehaviour {

	public bool selected = false;
	public string pos_actuelle;
	public int hitPoints;
	public int str;
	public bool moving;
	moveMng moveMngScript;
	// i could add here a modifier for the peutSeDeplacer function so that each hero gets a different move

	void Start () {
		
		moveMngScript = GameObject.Find("mngHolder").GetComponent ("moveMng") as moveMng;

	}

	public bool peutSeDeplacer(string pos_destination) {
		
		char ini_y = pos_actuelle[0];
		int ini_x = int.Parse(pos_actuelle.Substring(1,1));
		char dest_y = pos_destination[0];
		int dest_x = int.Parse(pos_destination.Substring(1,1));

		int diff_x = Mathf.Abs(dest_x - ini_x);
		int diff_y =  Mathf.Abs(dest_y - ini_y);

		if ((diff_x <= 1) && (diff_y <= 1)) { //phony distance calc. there could be a specified bool here to switch around the peutSeDeplacer function

			if (moveMngScript.allEnemies != null) {

				foreach (GameObject enemy_it in moveMngScript.allEnemies) { // check if there's a foe in the way

					enemy enemyScript = enemy_it.GetComponent ("enemy") as enemy;

					if (enemyScript.pos_actuelle == pos_destination) {
						
						return false;
					}
				}

			} return true;

		} return false;
	}

	void OnMouseDown () {
		
		GameObject this_hero = this.gameObject;
		moveMngScript.select_hero(this_hero);

	}	
		
}