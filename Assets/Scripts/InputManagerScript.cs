using UnityEngine;
using System.Collections;

public class InputManagerScript : MonoBehaviour {

	//class references
	protected GameManagerScript gameManager;
	protected MoveTokensScript moveManager;
	protected GameObject selected = null;

	//calling components
	public virtual void Start () {
		moveManager = GetComponent<MoveTokensScript>();
		gameManager = GetComponent<GameManagerScript>();
	}
	
	//is called repeatedly in GameManager update when there are no empty spaces
	public virtual void SelectToken(){
		if(Input.GetMouseButtonDown(0)){ //checking for input when mouse button is pressed
			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //store mouse position
			
			Collider2D collider = Physics2D.OverlapPoint(mousePos); //get a collider based on mouse position

			if(collider != null){ //if there is a collider
				if(selected == null){ //if nothing is selected
					selected = collider.gameObject; //select the collider
				} else { //otherwise, we have already selected a token
					//store positions of the 2 selected tokens
					Vector2 pos1 = gameManager.GetPositionOfTokenInGrid(selected); 
					Vector2 pos2 = gameManager.GetPositionOfTokenInGrid(collider.gameObject);

					//check if selected tokens are a valid movement
					//FIXED; corrected the mathF syntax
					if ((Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y)) == 1){ //check token positions, so that there is only 1 space difference
						//if valid, move tokens through another class
						moveManager.SetupTokenExchange(selected, pos1, collider.gameObject, pos2, true); //pass stored data to other class for use
					}
					selected = null; //operation is done, no token currently selected
				}
			}
		}

	}

}
