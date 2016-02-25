using UnityEngine;
using System.Collections;

public class help : MonoBehaviour {
	bool hide;
	void Start () {
	
	}

	void FixedUpdate () {
	if(Input.GetKeyDown(KeyCode.H))
			hide=!hide;
	}

	void OnGUI(){
		if(!hide){
			GUI.Box (new Rect (0,0,300,50), "Press 'C' to switch cameras \n" +
		         	"Press 'H' to hide help");
		}
	}
}
