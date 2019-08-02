//using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    //[Serializable]
	public class Player : MonoBehaviour
	{
        public StoredPlayer storeddata;
		public int Score = 0;
        public int[] DieValues = new int[3];
		public int Turnplace = 1;
        public int Position;
		public string PlayerName;
		public bool Active;
		
		public bool AI = false;
        public AI type;

        public void LoadData()
        {
            Score = storeddata.Score;
            DieValues = storeddata.DieValues;
            Turnplace = storeddata.Turnplace;
            Position = storeddata.Position;
            PlayerName = storeddata.PlayerName;
            Active = storeddata.Active;
            AI = storeddata.AI;
            switch(storeddata.AItype)
            {
                case "Moron":
                type = ObjectPooler.SharedPooler.AItypes[0];
                break;

                case "Fiend":
                type = ObjectPooler.SharedPooler.AItypes[1];
                break;

                case "Naive":
                type = ObjectPooler.SharedPooler.AItypes[2];
                break;

                case "Balancer":
                type = ObjectPooler.SharedPooler.AItypes[3];
                break;

                case "Psycho":
                type = ObjectPooler.SharedPooler.AItypes[4];
                break;

                case "Ace":
                type = ObjectPooler.SharedPooler.AItypes[5];
                break;

                default:
                type = ObjectPooler.SharedPooler.AItypes[0];
                break;
            }
        }

        public void SaveData()
        {
            storeddata.Score = Score;
            storeddata.DieValues = DieValues;
            storeddata.Turnplace = Turnplace;
            storeddata.Position = Position;
            storeddata.PlayerName = PlayerName;
            storeddata.Active = Active;
            storeddata.AI = AI;
            if (type != null)
            storeddata.AItype = type.Name;
        }

        public Vector3 AILocation(List<Vector3> Placements, int d)
        {
        //MainGame.board;

        AI weights = type;
        int[] Choicesweighted = new int[Placements.Count];

        int r = UnityEngine.Random.Range(0, Placements.Count);
        //return Placements[r];

        int bestchoice = 0;
        int choice = 0;
        Player WinningPlayer = null;
        Player LosingPlayer = null;
        for (int x = 0; x < MainGame.main.AllPlayers.Count; x++)
        {
            if (MainGame.main.AllPlayers[x].Position == 1)
                WinningPlayer = MainGame.main.AllPlayers[x];
            else if (MainGame.main.AllPlayers[x].Position == MainGame.main.LowestPosition)
            {
                if (LosingPlayer != null)
                    LosingPlayer = MainGame.main.AllPlayers[x];
                else if (Random.value > 0.5f)
                    LosingPlayer = MainGame.main.AllPlayers[x];
            }
        }
        
        //Go through every single legal move and calculate how much the AI wants to do that move.
        for(int c = 0; c < Placements.Count; c++)
        {
            Cell[,] b = MainGame.main.board;
            int x = (int)Placements[c].x + MainGame.main.boardwidth/2;
            int y = (int)Placements[c].z + MainGame.main.boardwidth/2;
            int h = (int)Placements[c].y;
            for (int p = 0; p < DieValues.Length; p++)
                {
                    if (b[x, y].Cellvalue == DieValues[p])
                    {
                        //Don't place it on yourself if Helpme is high
                        Choicesweighted[c] -= 5 * weights.Helpme;
                    }

                    if (WinningPlayer != null)
                    {
                        if (b[x, y].Cellvalue == WinningPlayer.DieValues[p])
                        {
                            //Hurt whoever is in first
                            Choicesweighted[c] += 2 * weights.HurtWinner;
                        }
                    }

                    if (LosingPlayer != null)
                    {
                        if (b[x, y].Cellvalue == LosingPlayer.DieValues[p])
                        {
                            //Stomp on whoever is in last
                            Choicesweighted[c] += 2 * weights.HurtLoser;
                        }
                    }
                }
            

            if (c == r)
            {
                //Increase odds of doing something random
                Choicesweighted[c] += 3 * weights.Random;
            }

            if (h > 0)
            {
                //If the player just likes stacking in general, this is boosted.
                Choicesweighted[c] += 1 * weights.Stacking;
            }

            if (bestchoice < Choicesweighted[c])
            {
                bestchoice = Choicesweighted[c];
                choice = c;
            }
        }
        Debug.Log(type.Name + " chose array value " + choice + " With weight of " + bestchoice);
        return Placements[choice];
        }
    }

	