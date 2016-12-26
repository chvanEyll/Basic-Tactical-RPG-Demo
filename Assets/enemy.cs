using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class enemy : MonoBehaviour {
	
	public string pos_actuelle;
	public int hitPoints;
	public int str;
	public bool moving;
	moveMng moveMngScript;

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

			if (moveMngScript.allHeroes != null) {

				foreach (GameObject hero_it in moveMngScript.allHeroes) { // check if there's a hero in the way

					hero heroScript = hero_it.GetComponent ("hero") as hero;

					if (heroScript.pos_actuelle == pos_destination) {

						return false;

					}
				}

			} return true;

		} return false;
	
	}

}