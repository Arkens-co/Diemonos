using UnityEngine;
using System.Collections;
//using UnityStandardAssets.CrossPlatformInput;

public class Cursor : MonoBehaviour 
{
	
	public Vector3 pos;
	public Transform tr;
	private float Turned;
    private float SpeedMultiplier = 3;
	public int Layer;
	public Camera CursorCam;
    public bool PlayerControl = true;
    private int MoveDirectionX = 0;
    private int MoveDirectionY = 0;
    private int RotateDirection = 0;

    public float zoom = 1;
	//private bool StillColliding = false;
	//public static Die RolledDie;

	private bool Topdown = false;

	// Use this for initialization
	void Start () 
	{
        if (CursorCam == null)
            CursorCam = GameObject.Find("Main Camera").GetComponent<Camera>();
        pos = transform.position;
		tr = transform;
        ChangeZoom(GameControl.control.game.Zoom);
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (PlayerControl && !MainGame.main.Paused)
        {
            if (Input.GetButtonDown("Place"))
            {
                //Cursor.Layer++;
                //DieRoll ();
                if (MainGame.main.CheckLegality(MainGame.main.cursorview.RolledDie, pos))
                {
                    //Debug.Log("How many times am I running?");
                    MainGame.main.DiePlacement(MainGame.main.cursorview.RolledDie, pos);
                    MainGame.main.AdvanceTurnOrder();
                }
            }
        }

        //Track Positioning of Cursor and move accoringly
        if(Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxis("Vertical") < 0)
                MoveDirectionY = -1;
            else
                MoveDirectionY = 1;

            Move("Vertical");
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            if (Input.GetAxis("Horizontal") < 0)
                MoveDirectionX = -1;
            else
                MoveDirectionX = 1;

            Move("Horizontal");
        }

		if (Input.GetButtonDown("Rotate") && tr.position == pos) 
		{
            if (Input.GetAxis("Rotate") < 0)
                RotateDirection = -1;
            else
                RotateDirection = 1;

            Move("Rotate");
		}
       
		
		if (Input.GetButtonDown("TopDown"))
		{
            Move("Topdown");
        }

        if (Input.GetButtonDown("IncreaseSpeed"))
            SpeedMultiplier = 5;
        else
            SpeedMultiplier = 1;

        if (pos.x > MainGame.main.boardwidth / 2 - 2)
            pos.x = MainGame.main.boardwidth / 2 - 2;
        else if (pos.x < -MainGame.main.boardwidth / 2 + 1)
            pos.x = -MainGame.main.boardwidth / 2 + 1;
        else if (pos.z > MainGame.main.boardwidth / 2 - 2)
            pos.z = MainGame.main.boardwidth / 2 - 2;
        else if (pos.z < -MainGame.main.boardwidth / 2 + 1)
            pos.z = -MainGame.main.boardwidth / 2 + 1;

        Layer = MainGame.main.board[(int)pos.x + MainGame.main.boardwidth/2, (int)pos.z + MainGame.main.boardwidth/2].height;  

		if (Layer < 0)
			Layer = 0;
			
		pos.y = Layer + (float) 1;
		transform.position = pos;
	}

    public void Move(string movewhat)
    {
        if (tr.position != pos || !PlayerControl || MainGame.main.Paused)
            return;

        if (movewhat == "Vertical")
        {
            if (Turned == 0)
                pos += Vector3.forward * SpeedMultiplier * MoveDirectionY;
            else if (Turned == 1)
                pos += Vector3.right * SpeedMultiplier * MoveDirectionY;
            else if (Turned == 2)
                pos += Vector3.back * SpeedMultiplier * MoveDirectionY;
            else if (Turned == 3)
                pos += Vector3.left * SpeedMultiplier * MoveDirectionY;
            //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);

            //Debug.Log("Input amount " + Input.GetAxis("Vertical"));
            MoveDirectionY = 0;
        }

        else if (movewhat == "Horizontal")
        {
            if (Turned == 0)
                pos += Vector3.right * SpeedMultiplier * MoveDirectionX;
            else if (Turned == 1)
                pos += Vector3.back * SpeedMultiplier * MoveDirectionX;
            else if (Turned == 2)
                pos += Vector3.left * SpeedMultiplier * MoveDirectionX;
            else if (Turned == 3)
                pos += Vector3.forward * SpeedMultiplier * MoveDirectionX;
            //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
            MoveDirectionX = 0;
        }

        else if (movewhat == "Rotate")
        {
            transform.Rotate(0, 90 * RotateDirection, 0);
            Turned += RotateDirection;

            if (Turned > 3)
                Turned -= 4;

            if (Turned < 0)
                Turned += 4;

            //Debug.Log(Turned);
            //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
            RotateDirection = 0;
        }

        else if (movewhat == "Topdown")
        {
            if (Topdown)
            {
                CursorCam.gameObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(37, 0, 0);
                CursorCam.gameObject.transform.localPosition = new Vector3(0, 5*zoom, -6*zoom);
                Topdown = false;
            }
            else
            {
                CursorCam.gameObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
                CursorCam.gameObject.transform.localPosition = new Vector3(0, 7*zoom, 0);
                Topdown = true;
            }
            //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
        }

        ObjectPooler.SharedPooler.PlaySound(0,false);
    }

    public void ChangeZoom(float zoomamount)
    {
        zoom = zoomamount;
        if(!Topdown)
        {
            CursorCam.gameObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(37, 0, 0);
            CursorCam.gameObject.transform.localPosition = new Vector3(0, 5 * zoom, -6 * zoom);
        }
        else
        {
            CursorCam.gameObject.transform.localRotation = Quaternion.identity * Quaternion.Euler(90, 0, 0);
            CursorCam.gameObject.transform.localPosition = new Vector3(0, 7 * zoom, 0);
        }
        Debug.Log("zoom changed to " + zoomamount);
    }

    //Only relevent for mobile controls
    public void ChangeDirection(string whichdirection)
    {
        if (whichdirection == "Left")
            MoveDirectionX = -1;

        else if (whichdirection == "Right")
            MoveDirectionX = 1;

        else if (whichdirection == "Up")
            MoveDirectionY = 1;

        else if (whichdirection == "Down")
            MoveDirectionY = -1;

        else if (whichdirection == "RotateLeft")
            RotateDirection = 1;

        else if (whichdirection == "RotateRight")
            RotateDirection = -1;
    }
}
