using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class PuzzleDisplayy : MonoBehaviour
{
	// this puzzle texture.
	public Texture PuzzleImage;

	// the width and height of the puzzle in tiles.
	public int Height = 3;
	public int Width = 3;

	// additional scaling value.
	public Vector3 PuzzleScale = new Vector3(1.0f, 1.0f, 1.0f);

	// additional positioning offset.
	public Vector3 PuzzlePosition = new Vector3(0.0f, 0.0f, 0.0f);

	// seperation value between puzzle tiles.
	public float SeperationBetweenTiles = 0.5f;

	// the tile display object.
	public GameObject Tile;

	// the shader used to render the puzzle.

	// array of the spawned tiles.
	private GameObject[,] TileDisplayArray;
	private List<Vector3> DisplayPositions = new List<Vector3>();

	// position and scale values.
	private Vector3 Scale;
	public GameObject levelCompletePanel;

	public AudioManager audioManager;
	private Vector3 Position;
	public GameObject exitpuzzlebutton;
	// has the puzzle been completed?
	public bool Complete = false;

	public Material puzzleMaterial;

	// Use this for initialization

	public void GoBack()
	{
		SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));
	}
	private void Awake()
	{
		audioManager = AudioManager.instance;
		audioManager.PlayMusic(audioManager.levelManagerBGM);
	}
	void Start()
	{
		levelCompletePanel.SetActive(false);
		// create the games puzzle tiles from the provided image.
		CreatePuzzleTiles();

		// mix up the puzzle.
		StartCoroutine(JugglePuzzle());

	}

	// Update is called once per frame
	void Update()
	{
		// move the puzzle to the position set in the inspector.
		this.transform.localPosition = PuzzlePosition;

		// set the scale of the entire puzzle object as set in the inspector.
		this.transform.localScale = PuzzleScale;
	}

	public Vector3 GetTargetLocation(PuzzleTilee thisTile)
	{
		// check if we can move this tile and get the position we can move to.
		PuzzleTilee MoveTo = CheckIfWeCanMove((int)thisTile.GridLocation.x, (int)thisTile.GridLocation.y, thisTile);

		if (MoveTo != thisTile)
		{
			// get the target position for this new tile.
			Vector3 TargetPos = MoveTo.TargetPosition;
			Vector2 GridLocation = thisTile.GridLocation;
			thisTile.GridLocation = MoveTo.GridLocation;

			// move the empty tile into this tiles current position.
			MoveTo.LaunchPositionCoroutine(thisTile.TargetPosition);
			MoveTo.GridLocation = GridLocation;

			// return the new target position.
			return TargetPos;
		}

		// else return the tiles actual position (no movement).
		return thisTile.TargetPosition;
	}

	private PuzzleTilee CheckMoveLeft(int Xpos, int Ypos, PuzzleTilee thisTile)
	{
		// move left 
		if ((Xpos - 1) >= 0)
		{
			// we can move left, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos - 1, Ypos, thisTile);
		}

		return thisTile;
	}

	private PuzzleTilee CheckMoveRight(int Xpos, int Ypos, PuzzleTilee thisTile)
	{
		// move right 
		if ((Xpos + 1) < Width)
		{
			// we can move right, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos + 1, Ypos, thisTile);
		}

		return thisTile;
	}

	private PuzzleTilee CheckMoveDown(int Xpos, int Ypos, PuzzleTilee thisTile)
	{
		// move down 
		if ((Ypos - 1) >= 0)
		{
			// we can move down, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos, Ypos - 1, thisTile);
		}

		return thisTile;
	}

	private PuzzleTilee CheckMoveUp(int Xpos, int Ypos, PuzzleTilee thisTile)
	{
		// move up 
		if ((Ypos + 1) < Height)
		{
			// we can move up, is the space currently being used?
			return GetTileAtThisGridLocation(Xpos, Ypos + 1, thisTile);
		}

		return thisTile;
	}

	private PuzzleTilee CheckIfWeCanMove(int Xpos, int Ypos, PuzzleTilee thisTile)
	{
		// check each movement direction
		if (CheckMoveLeft(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveLeft(Xpos, Ypos, thisTile);
		}

		if (CheckMoveRight(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveRight(Xpos, Ypos, thisTile);
		}

		if (CheckMoveDown(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveDown(Xpos, Ypos, thisTile);
		}

		if (CheckMoveUp(Xpos, Ypos, thisTile) != thisTile)
		{
			return CheckMoveUp(Xpos, Ypos, thisTile);
		}

		return thisTile;
	}

	private PuzzleTilee GetTileAtThisGridLocation(int x, int y, PuzzleTilee thisTile)
	{
		for (int j = Height - 1; j >= 0; j--)
		{
			for (int i = 0; i < Width; i++)
			{
				// check if this tile has the correct grid display location.
				if ((TileDisplayArray[i, j].GetComponent<PuzzleTilee>().GridLocation.x == x) &&
				   (TileDisplayArray[i, j].GetComponent<PuzzleTilee>().GridLocation.y == y))
				{
					if (TileDisplayArray[i, j].GetComponent<PuzzleTilee>().Active == false)
					{
						// return this tile active property. 
						return TileDisplayArray[i, j].GetComponent<PuzzleTilee>();
					}
				}
			}
		}

		return thisTile;
	}

	private IEnumerator JugglePuzzle()
	{
		yield return new WaitForSeconds(1.0f);

		// hide a puzzle tile (one is always missing to allow the puzzle movement).
		TileDisplayArray[0, 0].GetComponent<PuzzleTilee>().Active = false;

		yield return new WaitForSeconds(1.0f);

		for (int k = 0; k < 20; k++)
		{
			// use random to position each puzzle section in the array delete the number once the space is filled.
			for (int j = 0; j < Height; j++)
			{
				for (int i = 0; i < Width; i++)
				{
					// attempt to execute a move for this tile.
					TileDisplayArray[i, j].GetComponent<PuzzleTilee>().ExecuteAdditionalMove();

					yield return new WaitForSeconds(0.02f);
				}
			}
		}

		// continually check for the correct answer.
		StartCoroutine(CheckForComplete());

		yield return null;
	}

	public IEnumerator CheckForComplete()
	{
		while (Complete == false)
		{
			// iterate over all the tiles and check if they are in the correct position.
			Complete = true;
			for (int j = Height - 1; j >= 0; j--)
			{
				for (int i = 0; i < Width; i++)
				{
					// check if this tile has the correct grid display location.
					if (TileDisplayArray[i, j].GetComponent<PuzzleTilee>().CorrectLocation == false)
					{
						Complete = false;
					}
				}
			}

			yield return null;
		}

		// if we are still complete then all the tiles are correct.
		if (Complete)
		{
			exitpuzzlebutton.SetActive(false);

			audioManager.PlaySFX(audioManager.OnLevelComplete);


			if (PlayerPrefs.GetInt("Arcade") != -1)
			{
				PlayerPrefs.SetInt("Arcade", 1);
				PlayerPrefs.Save();
			}

			levelCompletePanel.SetActive(true);
			yield return new WaitForSeconds(2f);
			SceneManager.LoadScene(PlayerPrefs.GetInt("curr_scene"));

			// for (int j = 0; j < Height; j++)
			// {
			// 	for (int i = 0; i < Width; i++)
			// 	{
			// 		if (TileDisplayArray[i, j] != null)
			// 		{
			// 			Destroy(TileDisplayArray[i, j]);
			// 		}
			// 	}
			// }

			// TileDisplayArray = null;
		}

		yield return null;
	}

	private Vector2 ConvertIndexToGrid(int index)
	{
		int WidthIndex = index;
		int HeightIndex = 0;

		// take the index value and return the grid array location X,Y.
		for (int i = 0; i < Height; i++)
		{
			if (WidthIndex < Width)
			{
				return new Vector2(WidthIndex, HeightIndex);
			}
			else
			{
				WidthIndex -= Width;
				HeightIndex++;
			}
		}

		return new Vector2(WidthIndex, HeightIndex);
	}

	private void CreatePuzzleTiles()
	{
		// using the width and height variables create an array.
		TileDisplayArray = new GameObject[Width, Height];

		// set the scale and position values for this puzzle.
		Scale = new Vector3(1.0f / Width, 1.0f, 1.0f / Height);
		Tile.transform.localScale = Scale;

		// used to count the number of tiles and assign each tile a correct value.
		int TileValue = 0;

		// spawn the tiles into an array.
		for (int j = Height - 1; j >= 0; j--)
		{
			for (int i = 0; i < Width; i++)
			{
				// calculate the position of this tile all centred around Vector3(0.0f, 0.0f, 0.0f).
				Position = new Vector3(((Scale.x * (i + 0.5f)) - (Scale.x * (Width / 2.0f))) * (10.0f + SeperationBetweenTiles),
									   0.0f,
									  ((Scale.z * (j + 0.5f)) - (Scale.z * (Height / 2.0f))) * (10.0f + SeperationBetweenTiles));

				// set this location on the display grid.
				DisplayPositions.Add(Position);

				// spawn the object into play.
				TileDisplayArray[i, j] = Instantiate(Tile, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.Euler(90.0f, -180.0f, 0.0f)) as GameObject;
				TileDisplayArray[i, j].gameObject.transform.parent = this.transform;

				// set and increment the display number counter.
				PuzzleTilee thisTile = TileDisplayArray[i, j].GetComponent<PuzzleTilee>();
				thisTile.ArrayLocation = new Vector2(i, j);
				thisTile.GridLocation = new Vector2(i, j);
				thisTile.LaunchPositionCoroutine(Position);
				TileValue++;

				Material thisTileMaterial = new Material(puzzleMaterial);

				// apply the puzzle image to it.
				thisTileMaterial.mainTexture = PuzzleImage;

				// set the offset and tile values for this material.
				thisTileMaterial.mainTextureOffset = new Vector2(1.0f / Width * i, 1.0f / Height * j);
				thisTileMaterial.mainTextureScale = new Vector2(1.0f / Width, 1.0f / Height);

				// assign the new material to this tile for display.
				TileDisplayArray[i, j].GetComponent<Renderer>().material = thisTileMaterial;
			}
		}

		/*
		// Enable an impossible puzzle for fun!
		// switch the second and third grid location textures.
		Material thisTileMaterial2 = TileDisplayArray[1,3].GetComponent<Renderer>().material;
		Material thisTileMaterial3 = TileDisplayArray[2,3].GetComponent<Renderer>().material;
		TileDisplayArray[1,3].GetComponent<Renderer>().material = thisTileMaterial3;
		TileDisplayArray[2,3].GetComponent<Renderer>().material = thisTileMaterial2;
		*/
	}
}
