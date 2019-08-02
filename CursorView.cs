using UnityEngine;
using System.Collections;

public class CursorView : MonoBehaviour 
{
	public int RolledDie;

    public void ChangeView () 
	{
        //NumbText.text = "";
		gameObject.transform.localRotation = Quaternion.identity;
        //Debug.Log("Change the View");

        int set = Mathf.FloorToInt((RolledDie - 1) / 6);

        if (RolledDie == 1 + (set * 6))
            gameObject.transform.Rotate(Vector3.zero);

        else if (RolledDie == 2 + (set * 6))
            gameObject.transform.Rotate(new Vector3(-90, 0, 0));

        else if (RolledDie == 3 + (set * 6))
            gameObject.transform.Rotate(new Vector3(0, 0, -90));

        else if (RolledDie == 4 + (set * 6))
            gameObject.transform.Rotate(new Vector3(0, 0, 90));

        else if (RolledDie == 5 + (set * 6))
            gameObject.transform.Rotate(new Vector3(90, 0, 0));

        else if (RolledDie == 6 + (set * 6))
            gameObject.transform.Rotate(new Vector3(180, 180, 0));

        gameObject.GetComponent<Renderer>().material.SetTexture("_MainTex", ObjectPooler.SharedPooler.NeededMaterials[set].GetTexture("_MainTex"));

	}
}
