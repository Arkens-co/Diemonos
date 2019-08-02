using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq;

public class Startscreen : MonoBehaviour
{
    //GameControl backup;

    public Text[] Starttexts;
    // 0 - Help Text for Options;
    // 1 - 
    // 2 - 
    // 3 - 
    // 4 - 
    // 5-10 - Player Names;
    // 11-? - Options Stuff

    public Canvas[] Screens;
    // 0 - StartScreen;
    // 1 - PlayerSelect;
    // 2 - OptionsScreen;
    // 3 - CustomSettings;
    // 4 - CreditsScreen;

    public Button[] Buttons;
    // 0 - Continue

    //public int HowtoPlayPage = 0;
    //public GameObject[] Pages = new GameObject[5];

    public Dropdown[] Players;

    public Slider[] Sliders;

    public Toggle[] Toggles;

    public GameObject[] OptionScreens;
    //public AI[] AiTypes;
    private static int Playerspot = 3;
    public static int direction = 0;

    void Start()
    {
        Screens[0].enabled = true;
        Screens[1].enabled = false;
        Screens[4].enabled = false;
        Screens[2].enabled = false;
        //float temp = GameControl.control.rules.Soundvolume;
        //Debug.Log(temp);
        //GameControl.control.rules.Soundvolume = 0;
        SetPlayers();
        SetOptions();
        //Sliders[6].value = temp * 8;
        //GameControl.control.rules.Soundvolume = temp;
        if (GameControl.control.game != null)
            Buttons[0].interactable = GameControl.control.game.SavedGame;
        else
            Buttons[0].interactable = false;
        //backup = gameObject.AddComponent<GameControl>();
        //Screens[3].enabled = false;
        ObjectPooler.SharedPooler.PlaySound(9,true);
    }

    public void OpenScreen(int x)
    {
        ObjectPooler.SharedPooler.PlaySound(2,false);
        Screens[x].enabled = true;
        Buttons[0].interactable = false;
        Buttons[3].interactable = false;
        Buttons[4].interactable = false;
        Buttons[5].interactable = false;
        Buttons[6].interactable = false;
        //Buttons[7].interactable = false;
    }

    public void CloseScreen (int x)
    {
        ObjectPooler.SharedPooler.PlaySound(2,false);
        Buttons[0].interactable = GameControl.control.game.SavedGame;
        Buttons[3].interactable = true;
        Buttons[4].interactable = true;
        Buttons[5].interactable = true;
        Buttons[6].interactable = true;
        //Buttons[7].interactable = true;
        Screens[x].enabled = false;
    }

    public void StartGame(string kind)
    {
        ObjectPooler.SharedPooler.PlaySound(1,false);
        if (kind == "Continue")
        {
            GameControl.control.Load();
            GameControl.control.game.SavedGame = true;
            GameControl.control.PlayTutorial = false;
        }
            
        else if (kind == "New")
        {
            GameControl.control.Save();
            GameControl.control.game.SavedGame = false;
            GameControl.control.PlayTutorial = false;
        }
        else if (kind == "Tutorial")
        {
            GameControl.control.PlayTutorial = true;
        }
        SceneManager.LoadScene("GameScene");
    }

    public void SetPlayerName(int index)
    {
        if (Screens[1].enabled)
            ObjectPooler.SharedPooler.PlaySound(1,false);
        GameControl.control.game.Players[index].PlayerName = Starttexts[16+index].text;
        Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
        Starttexts[1].text = Starttexts[16+index].text + " has joined the battle";
        //GameControl.control.game.Players[x].Active
    }

    public void SetPlayerStatus(int index)
    {
        if(Screens[1].enabled)
            ObjectPooler.SharedPooler.PlaySound(1,false);
        switch(Players[index].value)
        {
            case 0:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = false;
            Starttexts[1].text = "Whenever you are ready, press Start New Game";
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 1:
            GameControl.control.game.Players[index].Active = false;
            Debug.Log("Index " + index + " at value " + Players[index].value);
            Starttexts[1].text = "This Player will sit out of this game.";
            break;

            case 2:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Moron";
            Starttexts[1].text = "Has a bad habit of stacking on top of its own numbers and sabotaging itself.";
            //GameControl.control.game.Players[index].PlayerName = "MoronAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 3:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Psycho";
            Starttexts[1].text = "Purely random, it delivers no thought in its moves.";

            //GameControl.control.game.Players[index].PlayerName = "PsychoAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 4:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Naive";
            Starttexts[1].text = "A passive AI that will avoid conflict, even if it will clearly benefit it.";
            //GameControl.control.game.Players[index].PlayerName = "NaiveAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 5:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Balancer";
            Starttexts[1].text = "Aggressively attacks the leader and never harms the player in last, no matter who they be.";
            //GameControl.control.game.Players[index].PlayerName = "BalancerAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 6:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Fiend";
            Starttexts[1].text = "A bully and a brute, it enjoys picking on everyone else, no matter what position they are in.";
            //GameControl.control.game.Players[index].PlayerName = "FiendAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;

            case 7:
            GameControl.control.game.Players[index].Active = true;
            GameControl.control.game.Players[index].AI = true;
            GameControl.control.game.Players[index].AItype = "Ace";
            Starttexts[1].text = "An aggressive AI that makes few clear errors, it leaves nothing to chance and will be the best.";
            //GameControl.control.game.Players[index].PlayerName = "AceAI";
            //Starttexts[16+index].text = GameControl.control.game.Players[index].PlayerName;
            //Debug.Log("Index " + index + " at value " + Players[index].value);
            break;
            
        }
        int count = 0;
        foreach (var p in GameControl.control.game.Players)
        {
            if (p.Active)
                count++;
        }
        GameControl.control.game.NumbPlayers = count;
    }

    public void ToggleOption(string option)
    {
        switch(option)
        {
            case "BoardSize":
            if ((int)Sliders[0].value * 8 > GameControl.control.rules.DieRangeMax)
            {
                GameControl.control.game.BoardSize = (int)Sliders[0].value * 8;
                Starttexts[0].text = "How big are the boundaries of the game?";
                Starttexts[11].text = "Size of Board: " + GameControl.control.game.BoardSize + " Spaces";
            }
            else
            {
                Starttexts[0].text = "You can't have the board be smaller than your Die Range.";
            }
            break;

            case "StackSides":
            GameControl.control.rules.StackSides = (int)Sliders[2].value;
            Starttexts[0].text = "How much support does a die need in order to stack on top of it?";
            Starttexts[13].text = "Sides necesary to Stack: " + GameControl.control.rules.StackSides;
            break;

            case "TurnLimit":
            GameControl.control.game.TurnLimit = (int)Sliders[1].value * 5;
            Starttexts[0].text = "How long does the game last?";
            Starttexts[12].text = "Game Lasts: " + GameControl.control.game.TurnLimit + " Turns";
            break;

            case "Distance":
            GameControl.control.rules.HowFarAway = (int)Sliders[3].value;
            Starttexts[0].text = "How far away must each adjacent die be to place?";
            Starttexts[14].text = "Distance between dice: " + GameControl.control.rules.HowFarAway;
            break;

            case "Range":
            if ((int)Sliders[4].value >= GameControl.control.game.BoardSize)
            {
                Starttexts[0].text = "You can't have the board be smaller than your Die Range.";                
            }
            else
            {
                GameControl.control.rules.DieRangeMax = (int)Sliders[4].value;
                Starttexts[0].text = "How high do the dice go?";
                Starttexts[15].text = "Range of each Die: 1 - " + GameControl.control.rules.DieRangeMax;
            }
            break;

            case "Music":
            GameControl.control.rules.Musicvolume = Sliders[5].value * 0.125f;
            ObjectPooler.SharedPooler.Music.volume = GameControl.control.rules.Musicvolume;
            Starttexts[0].text = "How loud is the Music?";
            //Starttexts[16].text = "Game Lasts: " + GameControl.control.game.TurnLimit + " Turns";
            break;

            case "Sounds":
            GameControl.control.rules.Soundvolume = (int)Sliders[6].value * 0.125f;
            Starttexts[0].text = "How loud are the sound effects?";
            //Starttexts[17].text = "Game Lasts: " + GameControl.control.game.TurnLimit + " Turns";
            break;

            case "Computer":
            GameControl.control.game.ComputerSpeed = (int)Sliders[7].value * 0.125f;
            Starttexts[0].text = "How quickly does the AI play: ";
            if ((int)Sliders[7].value == 1 || (int)Sliders[7].value == 0)
                Starttexts[0].text += "Fast";
            else if ((int)Sliders[7].value == 2 || (int)Sliders[7].value == 3 || (int)Sliders[7].value == 4)
                Starttexts[0].text += "Decent";
            else if ((int)Sliders[7].value == 5 || (int)Sliders[7].value == 6)
                Starttexts[0].text += "Slow";
            else if ((int)Sliders[7].value == 7 || (int)Sliders[7].value == 8)
                Starttexts[0].text += "Glacial";
            //Starttexts[18].text = "Game Lasts: " + GameControl.control.game.TurnLimit + " Turns";
            break;

            case "Animation":
            GameControl.control.game.AnimationSpeed = (int)Sliders[8].value* 0.25f;
            Starttexts[0].text = "How fast do the Animations play: ";
            if ((int)Sliders[8].value == 1 || (int)Sliders[8].value == 0)
                Starttexts[0].text += "Fast";
            else if ((int)Sliders[8].value == 2 || (int)Sliders[8].value == 3 || (int)Sliders[8].value == 4)
                Starttexts[0].text += "Decent";
            else if ((int)Sliders[8].value == 5 || (int)Sliders[8].value == 6)
                Starttexts[0].text += "Slow";
            else if ((int)Sliders[8].value == 7 || (int)Sliders[8].value == 8)
                Starttexts[0].text += "Glacial";
            //Starttexts[19].text = "Game Lasts: " + GameControl.control.game.TurnLimit + " Turns";
            break;

            case "Inclusive":
            GameControl.control.rules.LegalMovesInclusive = Toggles[0].isOn;
            //Debug.Log(Toggles[0].isOn);
            if (GameControl.control.rules.HowFarAway == 1)
                Starttexts[0].text = "This does nothing on or off while Distance between Dice is 1";
            else
                Starttexts[0].text = "All numbers between the needed distance are legal to place as well";
            break;

            case "OnePlusAssigned":
            GameControl.control.rules.MoreOneNumberAssigned = Toggles[1].isOn;
            Starttexts[0].text = "As many numbers will be assigned to players as possible, regarding only balance";
            break;

            case "BatchedNumbers":
            GameControl.control.rules.NumbersBatched = Toggles[2].isOn;
            Starttexts[0].text = "All die results are predetermined and randomized only in when they appear. Turn off if one wants more randomness";
            break;

            case "HelpPlacements":
            GameControl.control.rules.Helpplacements = Toggles[3].isOn;
            Starttexts[0].text = "The Game will show you all legal moves the player is able to make";
            break;

            case "Rules":
            OptionScreens[0].SetActive(true);
            Buttons[1].interactable = false;
            Buttons[2].interactable = true;
            OptionScreens[1].SetActive(false);
            OptionScreens[2].SetActive(false);
            Starttexts[0].text = "Set options that change the rules of the game itself";
            break;

            case "Options":
            OptionScreens[0].SetActive(false);
            Buttons[2].interactable = false;
            Buttons[1].interactable = true;
            OptionScreens[1].SetActive(true);
            OptionScreens[2].SetActive(false);
            Starttexts[0].text = "Set options that affect the UI and otherwise more generic aspects.";
            break;

            case "Reset":
            OptionScreens[2].SetActive(true);
            OptionScreens[1].SetActive(false);
            OptionScreens[0].SetActive(false);
            Buttons[1].interactable = true;
            Buttons[2].interactable = true;
            Starttexts[0].text = "Are you sure? All settings will revert to default and any saved games you had will be erased.";
            //Buttons[3].interactable = true;
            break;

            case "TrueWipe":
            GameControl.control.ResetGame();
            GameControl.control.Save();
            SceneManager.LoadScene("StartScene");
            //SceneManager.LoadScene("StartScreen");
            break;

        }
        if(Screens[2].enabled)
            ObjectPooler.SharedPooler.PlaySound(0,false);
        Debug.Log("Modified Option");
    }

    void SetPlayers()
    {
        GameControl.control.game.Players = GameControl.control.game.Players.OrderBy(StoredPlayer => StoredPlayer.OriginalOrder).ToList();
        //List<StoredPlayer> temp = GameControl.control.game.Players;
        for (int x = 0; x < 6; x++)
        {
            if (GameControl.control.game.Players[x] == null)
                Debug.Log("This is null");
            Starttexts[5+x].text = GameControl.control.game.Players[x].PlayerName;

            if (!GameControl.control.game.Players[x].Active)
                Players[x].value = 1;
            else if (!GameControl.control.game.Players[x].AI)
                Players[x].value = 0;
            else if (GameControl.control.game.Players[x].AItype == "Moron")
                Players[x].value = 2;
            else if (GameControl.control.game.Players[x].AItype == "Psycho")
                Players[x].value = 3;
            else if (GameControl.control.game.Players[x].AItype == "Naive")
                Players[x].value = 4;
            else if (GameControl.control.game.Players[x].AItype == "Balancer")
                Players[x].value = 5;
            else if (GameControl.control.game.Players[x].AItype == "Fiend")
                Players[x].value = 6;
            else if (GameControl.control.game.Players[x].AItype == "Ace")
                Players[x].value = 7;

            GameControl.control.game.Players[x].OriginalOrder = x;
        }

        if (GameControl.control.game.SavedGame)
            Starttexts[1].text = "Warning: Starting a new game will delete your currently saved one";
        else
            Starttexts[1].text = "Change any individual player settings and begin!";

    }

    void SetOptions()
    {
        //ObjectPooler.SharedPooler.Sound.volume = 0;
        if (GameControl.control.game.SavedGame)
            Starttexts[0].text = "Warning: Any changes made here will not be saved if you continue your game";
        Sliders[0].value = GameControl.control.game.BoardSize / 8;
        Sliders[1].value = GameControl.control.game.TurnLimit / 5;
        Sliders[2].value = GameControl.control.rules.StackSides;
        Sliders[3].value = GameControl.control.rules.HowFarAway;
        Sliders[4].value = GameControl.control.rules.DieRangeMax;
        Sliders[5].value = GameControl.control.rules.Musicvolume * 8;
        Sliders[7].value = GameControl.control.game.ComputerSpeed * 8;
        Sliders[8].value = GameControl.control.game.AnimationSpeed * 4;
        Toggles[0].isOn = GameControl.control.rules.LegalMovesInclusive;
        Toggles[1].isOn = GameControl.control.rules.MoreOneNumberAssigned;
        Toggles[2].isOn = GameControl.control.rules.NumbersBatched;
        Toggles[3].isOn = GameControl.control.rules.Helpplacements;
        //GameControl.control.rules.Soundvolume = temp;
        Sliders[6].value = GameControl.control.rules.Soundvolume * 8;
        //ObjectPooler.SharedPooler.Sound.volume = 1;
    }

/*

    public void GameModeChange(int mode)
    {
        /*
        if (mode == 0)
        {
            Options.DieRangeMin = 1;
            Options.DieRangeMax = 6;
            Options.HowFarAway = 1;
            Options.StackSides = 2;
            CustomSettings.enabled = false;
            HelpText.text = "Traditional, must place 1 away, need two dice adjacent to the die in order to stack on top of it.";
        }
        if (mode == 1)
        {
            Options.DieRangeMin = 1;
            Options.DieRangeMax = 8;
            Options.HowFarAway = 2;
            Options.StackSides = 2;
            CustomSettings.enabled = false;
            HelpText.text = "Eight sides on the die instead of six, must place 2 away, need two dice adjacent to stack";
        }
        if (mode == 2)
        {
            Options.DieRangeMin = 1;
            Options.DieRangeMax = 6;
            Options.HowFarAway = 1;
            Options.StackSides = 1;
            CustomSettings.enabled = false;
            HelpText.text = "Only need one die adjacent in order to stack on top of it";
        }
        if (mode == 3)
        {
            Options.DieRangeMin = 1;
            Options.DieRangeMax = 12;
            Options.HowFarAway = 1;
            Options.StackSides = 3;
            CustomSettings.enabled = false;
            HelpText.text = "12 sides on the die, need three dice adjacent to stack instead of 2";
        }
        if (mode == 4)
        {
            Options.DieRangeMin = 1;
            Options.DieRangeMax = 6;
            Options.HowFarAway = 1;
            Options.StackSides = 2;
            //CustomSettings.enabled = true;
            HelpText.text = "Customize your own version";
        }
        
        //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
    }
    */
    

}
