using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour 
{

	public static GameControl control;
	public bool PlayTutorial;
//	public int Turn;
	public CurrentGame game;
	public Rules rules = new Rules();

	public AudioSource Music;
	public AudioSource Sound;

	void Awake () 
	{
		if (control == null) 
		{
			DontDestroyOnLoad (gameObject);
			control = this;
			
			//ResetGame();
			Load();
		} 
		else if (control != null) 
		{
			if (control.game == null || control.rules == null)
			{
				control.game = game;
				control.rules = rules;
				Debug.Log("Default Gamecontrol Loaded into Control");
			}
			else if (control.PlayTutorial)
			{
				control.game = game;
				control.rules = rules;
				Debug.Log("Tutorial GameControl has taken over");
				if (!PlayTutorial)
					Load();
			}
			Debug.Log("Gamecontrol deleted");
			Destroy (this);
		}
	}

	public void Save()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();
		data.rules = rules;
		data.PlayTutorial = PlayTutorial;
		data.game = game;

		bf.Serialize (file, data);
		file.Close ();
		Debug.Log("Save Succesful");
	}

	public void Load()
	{
		if (File.Exists (Application.persistentDataPath + "/playerInfo.dat")) 
		{
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			PlayerData data = (PlayerData)bf.Deserialize (file);
			file.Close ();

			//Debug.Log(game);
			if (data.game != null && data.rules != null)
			{
				rules = data.rules;
				PlayTutorial = data.PlayTutorial;
				game = data.game;
				Debug.Log("Load Succesful");
			}
			else
			{
				Debug.Log("No data to load");
				//game = new CurrentGame();
				//rules = new Rules();
				//PlayTutorial = false;
			}
		}
		else
		{
			Debug.Log("No File to Load");
		}
	}

	public void ResetGame()
	{
		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/playerInfo.dat");

		PlayerData data = new PlayerData ();

		rules = data.rules;
		PlayTutorial = data.PlayTutorial;
		game = data.game;

		bf.Serialize (file, data);
		file.Close ();
	}
}

[Serializable]
class PlayerData
{
	public Rules rules;

	public CurrentGame game;

	public bool PlayTutorial;
}

[Serializable]
public class Rules
{
	//Variables that are NOT expected to change their value during a game.
	public bool NumbersBatched = true; //Guarantees the same amount of each number will appear
	public bool WrapAllowed = true; //1 is allowed to go to 6, always allowed on second thought;
	public bool MoreOneNumberAssigned = true; //Players can be given more than 1 number.
	public bool LegalMovesInclusive = false; //You do not need to be exact with how far away you are
	public bool Helpplacements = true; //Game will inform you of all legal moves.
	//public bool CheatMode = false; //Not availible in game

	public int DieRangeMin = 1; //The Lowest the Die can go
	public int DieRangeMax = 6; //The Highest the Die can go

	public int HowFarAway = 1; //How far the number must be away to be legal
	public int StackSides = 2; //How many dice must be adjacent before you can stack on top of the die

	public float Musicvolume = 1f; //How loud the Music is
	public float Soundvolume = 1f; //How loud the Sound Effects are
}


[Serializable]
public class CurrentGame
{
	//Variables that are expected to change in the middle of the game.
	public bool SavedGame = false;// Is there a Game in storage?
	public int Turn = 0;
	public List<StoredPlayer> Players;
	public int CurrentPlayersTurn = 0;

	public int NumbPlayers = 6; //How many Players are playing this game
	public int TurnLimit = 10; //How long the game goes
	public int BoardSize = 16; //How big the map is.

	public Cell[,] Board; //The Actual Board

	public int[] BatchedNumbers; //The Batched numbers in the game.

	public float ComputerSpeed = 0.25f; //How fast the Computer moves;
	public float AnimationSpeed = 1; //How fast the Animations play;
	public float Zoom = 1; //How zoomed in the Camera is;
}
[Serializable]
public struct Cell
{
	public int Cellvalue;
	//public string player;
	public int height;
	public int posX;
	public int posZ;
}

[Serializable]
public class StoredPlayer
{
	public int Score;
	public int[] DieValues;
	public int Turnplace;
	public int Position;
	public string PlayerName;
	public bool Active;
	public bool AI;
	public string AItype;
	public int OriginalOrder;
}