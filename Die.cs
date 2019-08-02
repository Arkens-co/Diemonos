using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Die : MonoBehaviour
{
    public int SidesAdjacent = 0;
    public int Number;
    public bool Topmost = true;
    public int Layer = 0;
    public int YRotation;
    public Text Textmesh; 

    private Animator anim;
    public Renderer rend;
    public bool playanims;

    public void HideDie(bool reveal)
    {
        gameObject.SetActive(reveal);
    }

    public void Start()
    {
        if (playanims)
            anim = GetComponent<Animator>();

        if (rend == null)
            rend = GetComponent<Renderer>();
        //else
            //GetComponent<Animator>().enabled = false;
    }

    public void ChangeDie(int x)
    {
        if (x == 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
    }

    public void ChooseAnimation()
    {
        //ChangeDie(1);
        anim.SetInteger("WhichAnimation", UnityEngine.Random.Range(0, 2));
    }

    public void DoneRolling()
    {
        anim.SetTrigger("DoneRolling");
        anim.SetInteger("DieResult", 0);
        MainGame.main.Animating = false;
        MainGame.main.cursorview.gameObject.SetActive(true);
        rend.material = ObjectPooler.SharedPooler.NeededMaterials[0];
        if (GameControl.control.rules.Helpplacements)
            MainGame.main.ValidPlacementsInstantiated(true);
    }

    public void Conversion()
    {
        int temp = anim.GetInteger("DieResult");
        if (temp > 6 && temp <= 12)
        {
            temp -= 6;
            rend.material = ObjectPooler.SharedPooler.NeededMaterials[1];
        }
        else if (temp > 12 && temp <= 18)
        {
            temp -= 12;
            rend.material = ObjectPooler.SharedPooler.NeededMaterials[2];
        }
        else if (temp > 18)
        {
            temp -= 18;
            rend.material = ObjectPooler.SharedPooler.NeededMaterials[3];
        }

        anim.SetInteger("DieResult", temp);
        //Debug.Log("Converted a die result down to " + temp);
    }
}

