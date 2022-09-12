using UnityEngine;
using System.Collections;

public class MoveTokensScript : MonoBehaviour {

	//Class references
	protected GameManagerScript gameManager;
	protected MatchManagerScript matchManager;

	//Flag for tokens being moved
	public bool move = false;

	//Animation speed settings
	public float lerpPercent;
	public float lerpSpeed;

	//Reversable flag
	bool userSwap;

	//Token references
	protected GameObject exchangeToken1;
	GameObject exchangeToken2;
	Vector2 exchangeGridPos1;
	Vector2 exchangeGridPos2;

	//Initialize references and lerp step
	public virtual void Start () {
		gameManager = GetComponent<GameManagerScript>();
		matchManager = GetComponent<MatchManagerScript>();
		lerpPercent = 0;
	}

	public virtual void Update () {
		//If tokens are currently being lerped
		if(move){
			//Step through the lerp by increment
			lerpPercent += lerpSpeed;

			//Clamp lerp to max
			if(lerpPercent >= 1){
				lerpPercent = 1;
			}

			//If exchange setup occured, exchange tokens
			if(exchangeToken1 != null){
				ExchangeTokens();
			}
		}
	}

	//Reset lerp step and indicate a move is in progress
	public void SetupTokenMove(){
		move = true;
		lerpPercent = 0;
	}

	//Recieve data from Input class and store it locally to be used in exchange
	//Reversable bool indicates if the move was done by a player and can be reversed by the game manager
	public void SetupTokenExchange(GameObject token1, Vector2 pos1,
	                               GameObject token2, Vector2 pos2, bool reversable){
		SetupTokenMove();

		exchangeToken1 = token1;
		exchangeToken2 = token2;

		exchangeGridPos1 = pos1;
		exchangeGridPos2 = pos2;


		this.userSwap = reversable;
	}

	//Called from update if SetupTokenExchange was already called, and an exchange token is assigned
	public virtual void ExchangeTokens(){

		//Get scene/world positions of targeted tokens
		Vector3 startPos = gameManager.GetWorldPositionFromGridPosition((int)exchangeGridPos1.x, (int)exchangeGridPos1.y);
		Vector3 endPos = gameManager.GetWorldPositionFromGridPosition((int)exchangeGridPos2.x, (int)exchangeGridPos2.y);

//		Vector3 movePos1 = Vector3.Lerp(startPos, endPos, lerpPercent);
//		Vector3 movePos2 = Vector3.Lerp(endPos, startPos, lerpPercent);

		//Lerp positions using custom animation function
		Vector3 movePos1 = SmoothLerp(startPos, endPos, lerpPercent);
		Vector3 movePos2 = SmoothLerp(endPos, startPos, lerpPercent);

		//Assign token positions to in progress lerp positions
		exchangeToken1.transform.position = movePos1;
		exchangeToken2.transform.position = movePos2;

		//If lerp step has reached the end
		if(lerpPercent == 1){
			//Update the array of tokens in the GameManager grid
			gameManager.gridArray[(int)exchangeGridPos2.x, (int)exchangeGridPos2.y] = exchangeToken1;
			gameManager.gridArray[(int)exchangeGridPos1.x, (int)exchangeGridPos1.y] = exchangeToken2;

			//If there is no match and the user initiated the movement
			if(!matchManager.GridHasMatch() && userSwap){
				SetupTokenExchange(exchangeToken1, exchangeGridPos2, exchangeToken2, exchangeGridPos1, false); //Undo the movement, with reversable false bc the computer makes the change
			} else { //Otherwise, movement is valid, and local references are purged so that flags in Update are false
				exchangeToken1 = null;
				exchangeToken2 = null;
				move = false;
			}
		}
	}

	//Move animation
	private Vector3 SmoothLerp(Vector3 startPos, Vector3 endPos, float lerpPercent){
		return new Vector3(
			Mathf.SmoothStep(startPos.x, endPos.x, lerpPercent),
			Mathf.SmoothStep(startPos.y, endPos.y, lerpPercent),
			Mathf.SmoothStep(startPos.z, endPos.z, lerpPercent));
	}

	//Called from MoveTokenToFillEmptySpace(), when a token needs to be lerped to a position under it
	public virtual void MoveTokenToEmptyPos(int startGridX, int startGridY,
	                                int endGridX, int endGridY,
	                                GameObject token){
	
		Vector3 startPos = gameManager.GetWorldPositionFromGridPosition(startGridX, startGridY);
		Vector3 endPos = gameManager.GetWorldPositionFromGridPosition(endGridX, endGridY);

		Vector3 pos = Vector3.Lerp(startPos, endPos, lerpPercent);

		token.transform.position =	pos;

		if(lerpPercent == 1){
			gameManager.gridArray[endGridX, endGridY] = token;
			gameManager.gridArray[startGridX, startGridY] = null;
		}
	}

	//Returns true when/if a token in the grid's target position has been calculated
	//Called repeatedly from GameManager when there is empty space in the grid
	public virtual bool MoveTokensToFillEmptySpaces(){
		bool movedToken = false; //Declare false while the lerp operates

		//Loop through the grid
		for(int x = 0; x < gameManager.gridWidth; x++){
			for(int y = 1; y < gameManager.gridHeight ; y++){
				//Check if any spaces in the grid are empty
				if(gameManager.gridArray[x, y - 1] == null){
					//Loop through tokens above the missing token
					for(int pos = y; pos < gameManager.gridHeight; pos++){
						GameObject token = gameManager.gridArray[x, pos];
						if(token != null){  //If there is a token in this space
							MoveTokenToEmptyPos(x, pos, x, pos - 1, token); //Move it to the target empty space one space below
							movedToken = true; //Movement has been calculated
						}
					}
				}
			}
		}
		//Once the move animation is complete, set move to false
		if(lerpPercent == 1){
			move = false;
		}

		//Return completed check - 
		return movedToken;
	}
}
