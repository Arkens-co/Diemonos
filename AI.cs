using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Level", menuName = "AddAI", order = 1)]
public class AI : ScriptableObject
{
    public string Name;
    public int Helpme;
    public int Stacking;
    public int HurtWinner;
    public int HurtLoser;
    public int Random;
    //public int Cheater; // If there is time, AI might be able to
}
