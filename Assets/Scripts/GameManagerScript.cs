using UnityEngine;
using System.Collections;

public class GameManagerScript : MonoBehaviour {

	//Grid variables
	public int gridWidth = 8;
	public int gridHeight = 8;
	public float tokenSize = 1;
	
	//Class references
	protected MatchManagerScript matchManager;
	protected InputManagerScript inputManager;
	protected RepopulateScript repopulateManager;
	protected MoveTokensScript moveTokenManager;

	//Scene references
	public GameObject grid;
	public  GameObject[,] gridArray;
	protected Object[] tokenTypes;
	//GameObject selected;


	public virtual void Start () {
		//Initialize arrays
		tokenTypes = (Object[])Resources.LoadAll("Tokens/");
		gridArray = new GameObject[gridWidth, gridHeight];
		//Initializing the grid
		MakeGrid();
		//Assigning references
		matchManager = GetComponent<MatchManagerScript>();
		inputManager = GetComponent<InputManagerScript>();
		repopulateManager = GetComponent<RepopulateScript>();
		moveTokenManager = GetComponent<MoveTokensScript>();
	}


	public virtual void Update() {
		//Check if the grid is full
		if(!GridHasEmpty()){
			//Check for matches in the MatchManagerScript
			if(matchManager.GridHasMatch()){
				matchManager.RemoveMatches();
			} else {
				//Allow the player to make a selection
				inputManager.SelectToken();
			}
		//Else there is empty space
		} else {
			//Check if a token isn't being moved, either by falling or exchanging
			if(!moveTokenManager.move){
				moveTokenManager.SetupTokenMove();
			}
			//Check that a token isn't falling
			if(!moveTokenManager.MoveTokensToFillEmptySpaces()){
				repopulateManager.AddNewTokensToRepopulateGrid();
			}
		}
	}

	//Grid initialization
	void MakeGrid() {
		grid = new GameObject("TokenGrid");
		//Loop through grid dimensions and instantiate tokens
		for(int x = 0; x < gridWidth; x++){
			for(int y = 0; y < gridHeight; y++){
				AddTokenToPosInGrid(x, y, grid);
			}
		}
	}

	// Check each space in the grid for tokens
	public virtual bool GridHasEmpty(){
		for(int x = 0; x < gridWidth; x++){
			for(int y = 0; y < gridHeight; y++){
				if(gridArray[x, y] == null){ //If no token is found
					return true;
				}
			}
		}
		//Otherwise
		return false;
	}

	//Add new token to the grid array
	public void AddTokenToPosInGrid(int x, int y, GameObject parent){
		Vector3 position = GetWorldPositionFromGridPosition(x, y);
		GameObject token = 
			Instantiate(tokenTypes[Random.Range(0, tokenTypes.Length)], 
			            position, 
			            Quaternion.identity) as GameObject;
		token.transform.parent = parent.transform;
		gridArray[x, y] = token;
	}

	//Get scene coordinates for moving Game Objects
	public Vector2 GetWorldPositionFromGridPosition(int x, int y)
	{
		return new Vector2(
			(x - gridWidth / 2) * tokenSize,
			(y - gridHeight / 2) * tokenSize);
	}

	// Returns coordinates of selected token
	public Vector2 GetPositionOfTokenInGrid(GameObject token){
		for(int x = 0; x < gridWidth; x++){
			for(int y = 0; y < gridHeight ; y++){
				if(gridArray[x, y] == token){
					return(new Vector2(x, y));
				}
			}
		}
		return new Vector2();
	}

}
