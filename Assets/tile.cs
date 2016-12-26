using UnityEngine;
using System.Collections;

public class tile : MonoBehaviour {

	void Start () {}

	void OnMouseDown() {
		
		moveMng moveMngScript = GameObject.Find("mngHolder").GetComponent ("moveMng") as moveMng;

		if (moveMngScript.allHeroes != null) {
		
			moveMngScript.move_hero(name);
		
		}
	}

}
