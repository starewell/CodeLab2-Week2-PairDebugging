using UnityEngine;
using System.Collections;

public class RepopulateScript : MonoBehaviour {
	
	//class reference
	protected GameManagerScript gameManager;

	public virtual void Start () {
		gameManager = GetComponent<GameManagerScript>(); //assign reference
	}

	//Check for empty spaces within the grid, if found pass their positions to the GameManager
	//Called from the GameManager when there is empty space and tokens haven't been added yet
	public virtual void AddNewTokensToRepopulateGrid(){
		for(int x = 0; x < gameManager.gridWidth; x++){ //loop through all x coordinates wihtin grid
			GameObject token = gameManager.gridArray[x, gameManager.gridHeight - 1]; //check for tokens in the top row
			if(token == null){ //If a token is not found
				gameManager.AddTokenToPosInGrid(x, gameManager.gridHeight - 1, gameManager.grid); //Pass the checked position for the GameManager to add a new token
			}
		}
	}

    #region //top row repopulation post match (bug 2)
    #endregion
}
