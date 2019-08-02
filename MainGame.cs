//using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Random;
//using UnityStandardAssets;

public class MainGame : MonoBehaviour
{
    public static MainGame main;
    public UIManager UI;
    public Cursor cursor;
    public CursorView cursorview;
    public List<Player> AllPlayers = new List<Player>();
    public bool TiedScore;
    public int LowestPosition;
    //public List<Die> AllDice = new List<Die>();
    private List<GameObject> ValidPlacements = new List<GameObject>();
    private int NumbValidPlacements = -1;
    public List<Vector3> AllPlacementsRef;
    public Animator DieAnim;

    //public Vector3[] TutorialPoints;

    public string[] TutorialMessages;
    //public Vector3[] TutorialPoints;
    public int TutorialIndex = 0;

    //public int NumbDiceRolled = 0;
    //public int PlayersJoined = 0;
    bool GameOver = false;
    public int PlayersTurn = 0;

    public int boardwidth = 0;
    public Cell[,] board;

    public bool Paused = true;
    public bool Animating = true;
    public bool NoHumanPlayer = true;

    //Called at the start to set up the board.
    void Start()
    {
        if (main == null)
            main = this;
        else
            Destroy(this);
        SetupGame();
        //DontDestroyOnLoad(gameObject);
    }

    //Called at the start in order to setup the game as per the players wishes
    public void SetupGame()
    {
        //GameControl.control.ResetGame();
        Paused = true;
        Animating = true;
        MakeBoard();
        SetupPlayers();
        FinalPlayerandBoardSetup();
        AdvanceTurnOrder();
        StartCoroutine(UI.DoneLoading());
    }

    //All Part of Setup Game

    void MakeBoard()
    {
        boardwidth = GameControl.control.game.BoardSize+2;
        board = new Cell[boardwidth, boardwidth];
        for(int i = 0; i < boardwidth - 2; i++)
        {
            for (int j = 0; j < boardwidth - 2; j++)
            {
                GameObject temp = ObjectPooler.SharedPooler.GetPooledObject(2);
                temp.transform.SetPositionAndRotation(new Vector3(1 - boardwidth * .5f + i, 0.5f, 1 - boardwidth * .5f + j), Quaternion.identity);
                temp.name = j.ToString();
                temp.SetActive(true);
            }
        }
    }
    //Give each player their turnorder and die values;
    void SetupPlayers()
    {
        //AllPlayers = ;
        int count = 0;
        int[] turnorder = new int [GameControl.control.game.NumbPlayers];
        for (int p = 0; p < 6; p++)
        {
            AllPlayers[p].storeddata = GameControl.control.game.Players[p];
            AllPlayers[p].LoadData();
        }
        AllPlayers = AllPlayers.OrderBy(Player => Player.Active).ToList();

        foreach (var p in AllPlayers)
        {
            if (p.Active)
            {
                if (!p.AI)
                    NoHumanPlayer = false;
                turnorder[count] = count;
                count++;
            }
        }

        if (!GameControl.control.game.SavedGame)
        {
            if (!GameControl.control.PlayTutorial)
            {
                if (count != GameControl.control.game.NumbPlayers)
                {
                    Debug.Log("Gamecontrol NumbPlayers and its actual list of players aren't in sync, this is a problem");
                    GameControl.control.game.NumbPlayers = count;
                }
                int[] range = new int [GameControl.control.rules.DieRangeMax];
                int numbDice = range.Length / GameControl.control.game.NumbPlayers;
                for (int w = 0; w < range.Length; w++)
                {
                    range[w] = 1 + w;
                }

                foreach (var p in AllPlayers)
                {
                    p.DieValues = new int[numbDice];
                }
                ShuffleNumbers(range);
                ShuffleNumbers(turnorder);
                for (int p = 0; p < AllPlayers.Count; p++)
                {
                    //AllPlayers[p].DieValue = AllPlayers[p].DieValues[0];
                    if (AllPlayers[p].Active)
                    {
                        count--;
                        AllPlayers[p].Turnplace = turnorder[count];   
                        for (int q = 0; q < AllPlayers[p].DieValues.Length; q++)
                        {
                            AllPlayers[p].DieValues[q] = range[p + (q * GameControl.control.game.NumbPlayers)];
                        }
                    }
                    else
                        AllPlayers[p].Turnplace = 10;
                }
            }
        }
        AllPlayers = AllPlayers.OrderBy(Player => Player.Turnplace).ToList();

        PlayersTurn = GameControl.control.game.CurrentPlayersTurn;
    }

    //A Generic Method to shuffle an array of integers
    void ShuffleNumbers(int[] deck)
    {
        for (int i = 0; i < deck.Length; i++)
        {
            int temp = deck[i];
            int randomIndex = UnityEngine.Random.Range(0, deck.Length);
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void FinalPlayerandBoardSetup()
    {
        if (!GameControl.control.game.SavedGame && !GameControl.control.PlayTutorial)
            GameControl.control.game.BatchedNumbers = EstablishBatchedNumbers();
        //GameControl.control.game.Players = AllPlayers;
        UI.GenerateUIScreen();
        UI.SetScreens();

        //board = new Board.Cell[boardwidth, boardwidth];
        if (!GameControl.control.game.SavedGame)
        {
            int range = GameControl.control.rules.DieRangeMax - GameControl.control.rules.DieRangeMin + 1;
            for (int r = 1; r <= range; r++)
            {
                Vector3 position = new Vector3(0,1,0);
                position.x += (-range / 2) + r - GameControl.control.rules.DieRangeMin;
                DiePlacement(r, position);

                //Debug.Log(AllPlayers[r].Turnplace);
            }
            GameControl.control.game.Turn = 0;
        }
        else
        {
            board = GameControl.control.game.Board;
            int midboard = boardwidth / 2;
            for (int x = 0; x < boardwidth-2; x++)
            {
                for (int y = 0; y< boardwidth-2; y++)
                {
                    if (board[x,y].height != 0)
                    {
                        int temp = board[x,y].height;
                        for(int z = 0; z < board[x,y].height; z++)
                        {
                            Vector3 temp2 = new Vector3(board[x,y].posX, z+1, board[x,y].posZ);
                            DiePlacement(board[x,y].Cellvalue, temp2);
                            board[x,y].height = temp;
                        }
                    }  
                }
            }
        }
        
    }

    int[] EstablishBatchedNumbers()
    {
        //int BatchNumbGuaranteed = 0;//Mathf.FloorToInt(Turnlimit / PlayerNumbers.Length);
        int range = GameControl.control.rules.DieRangeMax - GameControl.control.rules.DieRangeMin + 1;
        int[] temp = new int[range * GameControl.control.game.TurnLimit];

        for (int f = 0; f < range; f++)
        {
            for (int s = 0; s < GameControl.control.game.TurnLimit; s++)
            {
                int index = f + (range * s);
                temp[index] = GameControl.control.rules.DieRangeMin + f;
            }
        }

        ShuffleNumbers(temp);
        return temp;
    }

    //Called whenever something requires a die instantiated, and updates other arrays as necesary
    public void DiePlacement(int result, Vector3 position)
    {
        if (!Paused)
        {
            Debug.Log("Click");
            //ObjectPooler.SharedPooler.PlaySound(0,false);
        }
        GameObject temp = ObjectPooler.SharedPooler.GetPooledObject(0);
        Die addon = temp.GetComponent<Die>();
        addon.Layer = cursor.Layer;
        //addon.YRotation = UnityEngine.Random.Range(0, 4);
        addon.Number = result;

        temp.transform.rotation = Quaternion.identity;
        int set = Mathf.FloorToInt((result - 1) / 6);

        if (result == 2 + (set * 6))
            temp.transform.rotation = Quaternion.AngleAxis(90, Vector3.left);

        else if (result == 3 + (set * 6))
            temp.transform.rotation = Quaternion.AngleAxis(90, Vector3.back);

        else if (result == 4 + (set * 6))
            temp.transform.rotation = Quaternion.AngleAxis(90, Vector3.forward);

        else if (result == 5 + (set * 6))
            temp.transform.rotation = Quaternion.AngleAxis(90, Vector3.right);

        else if (result == 6 + (set * 6))
            temp.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);

        temp.transform.position = position;
        addon.rend.material = ObjectPooler.SharedPooler.NeededMaterials[set];

        //AllDice.Add(addon);
        temp.SetActive(true);
        //NumbDiceRolled++;

        int midboard = boardwidth / 2;

        board[midboard + (int)position.x, midboard + (int)position.z].Cellvalue = result;
        board[midboard + (int)position.x, midboard + (int)position.z].height += 1;
        board[midboard + (int)position.x, midboard + (int)position.z].posX = (int)position.x;
        board[midboard + (int)position.x, midboard + (int)position.z].posZ = (int)position.z;
    }

    //Called After a player places a die to perform maintenance around the players
    public void AdvanceTurnOrder()
    {
        if (!GameControl.control.PlayTutorial)
        {
            GameControl.control.game.SavedGame = true;
            for (int x = 0; x < AllPlayers.Count; x++)
            {
                AllPlayers[x].SaveData();
                GameControl.control.game.Players[x] = AllPlayers[x].storeddata;
            }
            GameControl.control.game.CurrentPlayersTurn = PlayersTurn;
            GameControl.control.game.Board = board;
            GameControl.control.Save();
            Debug.Log("Game has been saved?");
        }
            
        if (GameControl.control.game.Turn == 0)
        {
            PlayersTurn = 0;
            GameControl.control.game.Turn = 1;
        }
        else
        {
            if (GameControl.control.PlayTutorial)
            {
                UI.AdvanceTutorial(TutorialIndex);
            }

            if (PlayersTurn >= GameControl.control.game.NumbPlayers - 1)
            {
                PlayersTurn = 0;
                GameControl.control.game.Turn++;
            }
            else
            {
                PlayersTurn++;
            }
        }

        UpdateScores();

            //Check to see if it's the end of the game
        if (GameControl.control.game.Turn > GameControl.control.game.TurnLimit && PlayersTurn == 0)
        {
            if (TiedScore)
            {
                //GameControl.control.game.Turn--;
                GameControl.control.game.TurnLimit++;
                UI.YourTurn(PlayersTurn);
                cursorview.RolledDie = DieRoll(true);
                cursorview.ChangeView();
                AllPlacements();
                UI.Announce("We have no winner yet, the show must go on!");
            }
            else
            {
                UI.Announce("The Games are over!");
                cursor.PlayerControl = true;
                GameOver = true;
                GameControl.control.game.SavedGame = false;
                if (!GameControl.control.PlayTutorial)
                    GameControl.control.Save();
                cursorview.gameObject.SetActive(false);
                UI.PlayerControl(true);
                return;
            }
        }
        else
        {
            UI.Announce("It is now " + AllPlayers[PlayersTurn].PlayerName + "'s Turn");
            UI.YourTurn(PlayersTurn);
            cursorview.RolledDie = DieRoll(false);
            cursorview.ChangeView();
            AllPlacements();
        }

        if (NumbValidPlacements == 0)
        {
            Debug.Log("No Legal Moves");
            for (; AllPlacements().Count == 0;)
            {
                cursorview.RolledDie = DieRoll(true);
            }
            cursorview.ChangeView();
        }
        //Check if this player is an AI and proceed	
        if (AllPlayers[PlayersTurn].AI)
        {
            UI.PlayerControl(false);
            StartCoroutine(ConductAITurn());
        }
        else if (!GameControl.control.PlayTutorial)
        {
            UI.PlayerControl(true);
        }

    }

    //Called to roll a random die or a batched Die
    public int DieRoll(bool Random)
    {
        if (!Paused)
            ObjectPooler.SharedPooler.PlaySound(UnityEngine.Random.Range(1,8),false);        
        int result;

        if (GameControl.control.rules.NumbersBatched)
        {
            int index = PlayersTurn + (GameControl.control.game.NumbPlayers * (GameControl.control.game.Turn-1));
            if (GameControl.control.game.BatchedNumbers.Length <= index || Random)
            {
                Debug.Log("Die rolled was random because we wanted it to be or because we've gone past the batched limits " + index);
                result = UnityEngine.Random.Range(GameControl.control.rules.DieRangeMin, GameControl.control.rules.DieRangeMax + 1);
            }
            else
                result = GameControl.control.game.BatchedNumbers[index];

            if (result == GameControl.control.game.BatchedNumbers[GameControl.control.game.Turn - 1] && Random)
                result = DieRoll(true);
        }
        else
            result = UnityEngine.Random.Range(GameControl.control.rules.DieRangeMin, GameControl.control.rules.DieRangeMax + 1);

        if (DieAnim.GetFloat("Speed") > 0)
        {
            DieAnim.SetInteger("DieResult", result);
            Animating = true;
            cursorview.gameObject.SetActive(false);
        }
        return result;
    }

    public void PushPlace ()
    {
        if (cursor.PlayerControl && !GameOver && !Paused && !Animating)
        {
            if (CheckLegality(cursorview.RolledDie, cursor.pos))
            {
                //Debug.Log("How many times am I running?");
                DiePlacement(cursorview.RolledDie, cursor.pos);
                AdvanceTurnOrder();
            }
        }
    }
    //Updates the Scores of each player and updates their screens to be accurate.
    void UpdateScores()
    {
        for (int i = 0; i < AllPlayers.Count; i++)
        {
            int count = 0;

            for (int x = 0; x < boardwidth; x++)
            {
                for (int y = 0; y < boardwidth; y++)
                {
                    if (board[x, y].height != 0)
                    {
                        foreach (var p in AllPlayers[i].DieValues)
                        {
                            if (board[x, y].Cellvalue == p)
                                count++;
                        }
                    }
                }
            }

            AllPlayers[i].Score = count;
        }
        DeterminePositions();
        UI.UpdateScreens();
    }

    //called to determine if someone is in first or last, returns null if its target is a tie
    public void DeterminePositions()
    {
        //List<Player> storage = AllPlayers;
        //Player storage;
        AllPlayers = AllPlayers.OrderBy(Player => Player.Score).ToList();
        for (int x = 0; x < AllPlayers.Count; x++)
        {
            if (AllPlayers[x].Active)
            {
                //Debug.Log("Player with Score " + AllPlayers[x].Score);
                AllPlayers[x].Position = 0;

                if (AllPlayers[x].Score == AllPlayers[0].Score || x == 0)
                {
                    AllPlayers[x].Position = AllPlayers.Count;
                    LowestPosition = AllPlayers.Count;
                }
                else if (AllPlayers[x].Score == AllPlayers[x-1].Score)
                {
                    AllPlayers[x].Position = AllPlayers[x-1].Position;
                }
                else
                {
                    AllPlayers[x].Position = AllPlayers.Count - x;
                }
            }
        }
        if (AllPlayers[AllPlayers.Count-1].Position == AllPlayers[AllPlayers.Count-2].Position)
            TiedScore = true;
        else
            TiedScore = false;
        AllPlayers = AllPlayers.OrderBy(Player => Player.Turnplace).ToList();
    }

    //Called to check if a die can be placed at a certain location 
    //Due to all the variables involved and no desire to try splitting this behemoth to seperate pieces, this is a long one.
    public bool CheckLegality(int CN, Vector3 pos)
    {
        //CursorView cv = CursorModel.GetComponent<CursorView>();
        bool enoughaway = false;
        int sidesadjacent = 0;

        //Shorthand for this long splurge of Math
        int Away = GameControl.control.rules.HowFarAway;
        int Wrap = GameControl.control.rules.DieRangeMax - GameControl.control.rules.DieRangeMin + 1 - Away;
        bool WrapAllowed = true;//GameControl.control.rules.WrapAllowed;
        bool LegalMovesInclusive = GameControl.control.rules.LegalMovesInclusive;
        
        int OpN = 0;

        //board[pos.x, pos.z].Cellvalue;
        int x = (int)pos.x + boardwidth / 2;
        int y = (int)pos.z + boardwidth / 2;
        int ht = (int)pos.y;


        //Debug.Log("That spots cellValue is " + board[x, y].Cellvalue);

        //Check if at a wall
        if (x >= boardwidth - 1 || x <= 0 || y >= boardwidth - 1 || y <= 0)
        {
            //Debug.Log("At a wall, placement is not allowed");
            return false;
        }

        //Check if ANY dice are nearby
        if (board[x + 1, y].height + board[x - 1, y].height + board[x, y + 1].height + board[x, y - 1].height > 0)
            enoughaway = true;

        //Check the die to the east, if its height is above 0, we care about it.
        if (board[x + 1, y].height >= ht)
        {
            OpN = board[x + 1, y].Cellvalue;
            //Debug.Log("Checking the East Die, ht and Height are " + ht + " " + board[x + 1, y].height);
            sidesadjacent++;
            if (OpN + Away == CN || OpN - Away == CN)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if ((OpN + Wrap == CN || OpN - Wrap == CN) && WrapAllowed)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if (LegalMovesInclusive)
            {
                for (int l = 0; l < Away; l++)
                {
                    if (OpN + l == CN || OpN - l == CN)
                    {
                        if (enoughaway)
                            return false;
                    }
                    else if ((OpN + Wrap + l == CN || OpN - Wrap - l == CN) && WrapAllowed)
                    {
                        if (enoughaway)
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if (board[x + 1, y].height > 0 && board[x + 1, y].height == ht - 1)
            sidesadjacent++;

        ///////////////////////////////////////////////
        //
        //Now for the die to the west
        if (board[x - 1, y].height >= ht)
        {
            OpN = board[x - 1, y].Cellvalue;
            sidesadjacent++;
            if (OpN + Away == CN || OpN - Away == CN)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if ((OpN + Wrap == CN || OpN - Wrap == CN) && WrapAllowed)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if (LegalMovesInclusive)
            {
                for (int l = 0; l < Away; l++)
                {
                    if (OpN + l == CN || OpN - l == CN)
                    {
                        if (enoughaway)
                            return false;
                    }
                    else if ((board[x - 1, y].Cellvalue + Wrap + l == CN || board[x - 1, y].Cellvalue - Wrap - l == CN) && WrapAllowed)
                    {
                        if (enoughaway)
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if (board[x - 1, y].height > 0 && board[x - 1, y].height == ht - 1)
            sidesadjacent++;

        ////////////////////////////////////////////
        //
        if (board[x, y + 1].height >= ht)
        {
            OpN = board[x, y + 1].Cellvalue;
            sidesadjacent++;
            if (OpN + Away == CN || OpN - Away == CN)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if ((OpN + Wrap == CN || OpN - Wrap == CN) && WrapAllowed)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if (LegalMovesInclusive)
            {
                for (int l = 0; l < Away; l++)
                {
                    if (OpN + l == CN || OpN - l == CN)
                    {
                        if (enoughaway)
                            return false;
                    }
                    else if ((OpN + Wrap + l == CN || OpN - Wrap - l == CN) && WrapAllowed)
                    {
                        if (enoughaway)
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if (board[x, y + 1].height > 0 && board[x, y + 1].height == ht - 1)
            sidesadjacent++;

        ////////////////////////////////////////////////
        //
        //Finally the die to the south
        if (board[x, y - 1].height >= ht)
        {
            OpN = board[x, y - 1].Cellvalue;
            sidesadjacent++;
            if (OpN + Away == CN || OpN - Away == CN)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if ((OpN + Wrap == CN || OpN - Wrap == CN) && WrapAllowed)
            {
                if (enoughaway)
                    enoughaway = true;
            }
            else if (LegalMovesInclusive)
            {
                for (int l = 0; l < Away; l++)
                {
                    if (OpN + l == CN || OpN - l == CN)
                    {
                        if (enoughaway)
                            return false;
                    }
                    else if ((OpN + Wrap + l == CN || OpN - Wrap - l == CN) && WrapAllowed)
                    {
                        if (enoughaway)
                            return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if (board[x, y - 1].height > 0 && board[x, y - 1].height == ht - 1)
            sidesadjacent++;

        if (board[x, y].height > 0)
        {
            if (sidesadjacent >= GameControl.control.rules.StackSides && enoughaway)
                return true;
            else
            {
                //Debug.Log("returned false cause Sidesadjacent were " + sidesadjacent + " Or because enoughaway was " + enoughaway);
                return false;
            }
        }

        if (enoughaway)
            return true;

        //Debug.Log("Returned false because " + enoughaway + " Sides adjacent was " + sidesadjacent + " X and y were " + (x - boardwidth/2) + ", " + (y - boardwidth/2) );
        return false;

    }

    //Called to organize all the legal places a die can go, vital for AI movement and showing the helper locations
    public List<Vector3> AllPlacements()
    {
        int count = 0;
        List<Vector3> temp = new List<Vector3>();

        for (int x = 0; x < boardwidth; x++)
        {
            for (int y = 0; y < boardwidth; y++)
            {
                Vector3 pos = new Vector3(x - (boardwidth / 2), board[x, y].height + 1, y - (boardwidth / 2));
                if (CheckLegality(cursorview.RolledDie, pos))
                {
                    count++;
                    temp.Add(pos);
                }
            }
        }
        NumbValidPlacements = count;
        AllPlacementsRef = temp;

        if (GameControl.control.rules.Helpplacements && !Animating)
            ValidPlacementsInstantiated(true);
        return temp;
    }

    //Called to move the AI around, supplied with coordinates with a Player script method.
    IEnumerator ConductAITurn()
    {
        Vector3 pos = AllPlayers[PlayersTurn].AILocation(AllPlacements(), cursorview.RolledDie);
        yield return new WaitUntil(() => !Animating);
        yield return new WaitUntil(() => !Paused);
        //Debug.Log("Done Waiting");
        yield return new WaitForSeconds(GameControl.control.game.ComputerSpeed * 3);
        
        if (cursor.pos == pos)
        {
            DiePlacement(cursorview.RolledDie, pos);
            AdvanceTurnOrder();
            yield break;
        }
        for (; cursor.pos.x != pos.x;)
        {
            //Debug.Log("Tick X " +Cursor.pos.x);
            if (cursor.pos.x < pos.x)
                cursor.pos.x++;
            else if (cursor.pos.x > pos.x)
                cursor.pos.x--;
            else
                break;

            yield return new WaitUntil(() => !Paused);
            //Debug.Log("Done Waiting Again");
            yield return new WaitForSeconds(GameControl.control.game.ComputerSpeed);
        }
        for (; cursor.pos.z != pos.z;)
        {
            //Debug.Log("Tick Z " + Cursor.pos.z);
            if (cursor.pos.z < pos.z)
                cursor.pos.z++;
            else if (cursor.pos.z > pos.z)
                cursor.pos.z--;
            else
                break;

            yield return new WaitUntil(() => !Paused);
            yield return new WaitForSeconds(GameControl.control.game.ComputerSpeed);
        }
        if (cursor.pos == pos)
        {
            DiePlacement(cursorview.RolledDie, pos);
            AdvanceTurnOrder();
            yield break;
        }
    }

    public void ValidPlacementsInstantiated(bool makeordestroy)
    {
        foreach (var v in ValidPlacements)
        {
            v.SetActive(false);
            //ValidPlacements.Remove(v);
        }
        ValidPlacements = new List<GameObject>();

        if (makeordestroy)
        {
            for(var c = 0; c < AllPlacementsRef.Count; c++)
            {
                GameObject tempobject = ObjectPooler.SharedPooler.GetPooledObject(1);
                tempobject.SetActive(true);
                tempobject.transform.position = AllPlacementsRef[c];
                ValidPlacements.Add(tempobject);
            }
        }

    }
}
