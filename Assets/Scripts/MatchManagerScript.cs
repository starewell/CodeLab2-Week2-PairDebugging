using UnityEngine;
using System.Collections;

public class MatchManagerScript : MonoBehaviour {

	//Class reference
	protected GameManagerScript gameManager;

	public virtual void Start () {
		gameManager = GetComponent<GameManagerScript>(); //Assign reference
	}

	public virtual bool GridHasMatch(){
		bool match = false; //set to false
		
		//Loop through the grid from left to right
		for(int x = 0; x < gameManager.gridWidth; x++){
			for(int y = 0; y < gameManager.gridHeight ; y++){
				if(x < gameManager.gridWidth - 2){ //Only check tokens 2 spaces away from the right side of the grid
					match = (match || GridHasHorizontalMatch(x, y)); //Check for matches in each token, unless a match has already been found
				}
				if (y < gameManager.gridHeight - 2) {
					match = (match || GridHasVerticalMatch(x, y));
				}
			}
		}
		//Returns false if no matches, returns true if a match is found
		return match;

	}

	//Check token's rightward neighbors for matching tokens
	public bool GridHasHorizontalMatch(int x, int y){
		GameObject token1 = gameManager.gridArray[x + 0, y];
		GameObject token2 = gameManager.gridArray[x + 1, y]; //Token one space to the right of original token
		GameObject token3 = gameManager.gridArray[x + 2, y]; //Token two spaces to the right of original token
		
		if(token1 != null && token2 != null && token3 != null){ //Secondary override, make sure each token is valid
			//Get sprite renderer
			SpriteRenderer sr1 = token1.GetComponent<SpriteRenderer>();
			SpriteRenderer sr2 = token2.GetComponent<SpriteRenderer>();
			SpriteRenderer sr3 = token3.GetComponent<SpriteRenderer>();
			
			//Compare each token's sprite to each other, if matching return true
			return (sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite);
		} else { //If there was an error
			return false;
		}
	}	
	//Check token's rightward neighbors for matching tokens
	public bool GridHasVerticalMatch(int x, int y){
		GameObject token1 = gameManager.gridArray[x, y + 0];
		GameObject token2 = gameManager.gridArray[x, y + 1]; //Token one space below the original token
		GameObject token3 = gameManager.gridArray[x, y + 2]; //Token two spaces below the original token
		
		if(token1 != null && token2 != null && token3 != null){ //Secondary override, make sure each token is valid
			//Get sprite renderer
			SpriteRenderer sr1 = token1.GetComponent<SpriteRenderer>();
			SpriteRenderer sr2 = token2.GetComponent<SpriteRenderer>();
			SpriteRenderer sr3 = token3.GetComponent<SpriteRenderer>();
			
			//Compare each token's sprite to each other, if matching return true
			return (sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite);
			Debug.Log((sr1.sprite == sr2.sprite && sr2.sprite == sr3.sprite));
		} else { //If there was an error
			return false;
		}
	}

	public int GetHorizontalMatchLength(int x, int y){ //store matching token positions
		int matchLength = 1; //default value, match is at least 1 token long
		
		GameObject first = gameManager.gridArray[x, y]; //call selected token first

		if(first != null){ //if not null
			SpriteRenderer sr1 = first.GetComponent<SpriteRenderer>(); //check for component
			
			//Loop through every remaining token to the right of original pos, within the grid width
			for(int i = x + 1; i < gameManager.gridWidth; i++) {
				GameObject other = gameManager.gridArray[i, y]; //Store the next token to the right

				if(other != null){ //If there is a token
					SpriteRenderer sr2 = other.GetComponent<SpriteRenderer>(); //Store its sprite renderer

					if(sr1.sprite == sr2.sprite){ //Compare sprites between the original and this token's
						//If next token matches
						matchLength++; //Increase match length by 1
					} else { //Otherwise does not match
						break; //Exit the loop
					}
				} else {
					break;
				}
			}
		}
		
		return matchLength;
	}
	
	public int GetVerticalMatchLength(int x, int y){ //store matching token positions
		int matchLength = 1; //default value, match is at least 1 token long
		
		GameObject first = gameManager.gridArray[x, y]; //call selected token first

		if(first != null){ //if not null
			SpriteRenderer sr1 = first.GetComponent<SpriteRenderer>(); //check for component
			
			//Loop through every remaining token below the original pos, within the grid width
			for(int i = y + 1; i < gameManager.gridHeight; i++) {
				GameObject other = gameManager.gridArray[x, i]; //Store the next token to the right

				if(other != null){ //If there is a token
					SpriteRenderer sr2 = other.GetComponent<SpriteRenderer>(); //Store its sprite renderer

					if(sr1.sprite == sr2.sprite){ //Compare sprites between the original and this token's
						//If next token matches
						matchLength++; //Increase match length by 1
					} else { //Otherwise does not match
						break; //Exit the loop
					}
				} else {
					break;
				}
			}
		}
		Debug.Log(matchLength);
		return matchLength;
	}

	//is called repeatedly in GameManager update when there are no empty spaces and GridHasMatch() returns true
	public virtual int RemoveMatches(){
		int numRemoved = 0; //Default value, removed no tokens so far

		//Loop through the grid dimensions
		for(int x = 0; x < gameManager.gridWidth; x++){
			for(int y = 0; y < gameManager.gridHeight ; y++){
				//Don't check the two right most columns
				if(x < gameManager.gridWidth - 2) {
					int horizonMatchLength = GetHorizontalMatchLength(x, y); //Calculate length of the match

					if(horizonMatchLength > 2){ //If the match length is greater than 2
						//Loop through tokens within the bounds of the length in the row
						for(int i = x; i < x + horizonMatchLength; i++){
							//Store token at coord
							GameObject token = gameManager.gridArray[i, y]; 
							Destroy(token); //Destroy token at coord

							//Update manager reference grid
							gameManager.gridArray[i, y] = null;
							numRemoved++; //Store locally how many are removed
						}
					}
				}
				if (y < gameManager.gridHeight - 2) {
					int verticalMatchLength = GetVerticalMatchLength(x, y);

					if (verticalMatchLength > 2){ //If the match length is greater than 2
						//Loop through tokens within the bounds of the length in the row
						for(int i = y; i < y + verticalMatchLength; i++){
							//Store token at coord
							GameObject token = gameManager.gridArray[x, i]; 
							Destroy(token); //Destroy token at coord

							//Update manager reference grid
							gameManager.gridArray[x, i] = null;
							numRemoved++; //Store locally how many are removed
						}
					}
				}
			}
		}
		//Return number of tokens removed (unused)
		return numRemoved;
	}
}
