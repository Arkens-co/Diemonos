using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public bool Multipart;
    public Canvas GamePauseMenu;

    public RectTransform[] GameUIButtons;
    //0-5 PlayerScreens;
    //6 - Left Button
    //7 - Right Button
    //8 - Up Button
    //9 - Down Button
    //10 - RotateRight Button
    //11 - Rotate Left Button
    //12 - Change Camera Button
    //13 - Place Die Button
    //14 - Options Gear
    //15 - Turnmarker
    //16 - AnnounceScreen
    //17 - Pause Screen

    public Text[] Gametexts;
    //0-Announcement text
    //1-TurnText
    //2-7 - Players Score
    //8-13 - Players Number
    //14-19 - Players Name

    public Image[] GameImages;
    //0-5 - Players Screen
    //6 - Options Button

    public Button[] InteractiveButtons;

    public Slider[] Sliders;

    public void GenerateUIScreen()
    {
        float width = Screen.width;
        float height = Screen.height;
        float playerlength = width / GameControl.control.game.NumbPlayers;
        Vector3 Scale = new Vector3(1,1,1);
        for (int i = 0; i < GameControl.control.game.NumbPlayers; i++)
        {
            //GameUIButtons[i].SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, (width/ GameControl.control.game.NumbPlayers) * i, width / GameControl.control.game.NumbPlayers+10);
            //GameUIButtons[i].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, playerlength * 0.8f);
            GameUIButtons[i].SetPositionAndRotation(new Vector3 (playerlength * i + playerlength/2, height * 0.1f, 0), Quaternion.identity);
        }

        //Directional Buttons
        GameUIButtons[6].SetPositionAndRotation(new Vector3 (width * 0.3f, height * 0.5f, 0), Quaternion.identity);
        GameUIButtons[7].SetPositionAndRotation(new Vector3 (width * 0.7f, height * 0.5f, 0), Quaternion.identity);
        GameUIButtons[8].SetPositionAndRotation(new Vector3 (width * 0.5f, height * 0.75f, 0), Quaternion.identity);
        GameUIButtons[9].SetPositionAndRotation(new Vector3 (width * 0.5f, height * 0.2f, 0), Quaternion.identity);

        //Other Buttons
        GameUIButtons[10].SetPositionAndRotation(new Vector3 (width * 0.75f, height * 0.8f, 0), Quaternion.identity);
        GameUIButtons[11].SetPositionAndRotation(new Vector3 (width * 0.25f, height * 0.8f, 0), Quaternion.identity);
        GameUIButtons[12].SetPositionAndRotation(new Vector3 (width * 0.1f, height * 0.6f, 0), Quaternion.identity);
        GameUIButtons[13].SetPositionAndRotation(new Vector3 (width * 0.9f, height * 0.29f, 0), Quaternion.identity);
        GameUIButtons[14].SetPositionAndRotation(new Vector3 (width * 0.9f, height * 0.9f, 0), Quaternion.identity);
        
        GameUIButtons[15].SetPositionAndRotation(new Vector3 (width * 0.1f, height * 0.88f, 0), Quaternion.identity);
        //GameUIButtons[15].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 0.2f);
        //GameUIButtons[15].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.3f);
        
        GameUIButtons[16].SetPositionAndRotation(new Vector3 (width * 0.5f, height * 0.93f, 0), Quaternion.identity);
        //GameUIButtons[16].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 0.6f);
        //GameUIButtons[16].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.2f);

        GameUIButtons[17].gameObject.SetActive(false);
        GameUIButtons[17].SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * 0.8f);
        GameUIButtons[17].SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * 0.8f);

        //Values for the Pause Screen Options
        Sliders[0].value = GameControl.control.game.AnimationSpeed * 4;
    }

    public void ExitMainGame()
    {
        MainGame.main.Paused = false;
        if (!GameControl.control.PlayTutorial)
            GameControl.control.Save();
        //else
        //    GameControl.control.Save();
        //Options.ResetGame();
        SceneManager.LoadScene("StartScene");
        //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
    }

    public void PauseScreen(bool openclose)
    {
        GameUIButtons[17].gameObject.SetActive(openclose);
        MainGame.main.Paused = openclose;
        //if (PlayerControl = true)
        //    PlayerControl = openclose;
        //Options.LibrarySounds.PlaySound(Options.LibrarySounds.Select3, false);
    }

    public void YourTurn (int player)
    {
        for (int x = 0; x < 6; x++)
        {
            if (x != player)
                GameImages[x].color = new Color (1,1,1,.7f);
            else
                GameImages[x].color = new Color (0,1,.8f,.7f);
        }
    }

    public void UpdateScreens()
    {
        for (int x = 0; x < GameControl.control.game.NumbPlayers; x++)
        {
            Gametexts[2+x].text = MainGame.main.AllPlayers[x].Score.ToString();
            //if (GameControl.control.game.Turn != 1)
            if (MainGame.main.AllPlayers[x].Position == MainGame.main.LowestPosition)
                Gametexts[2+x].color = Color.red;
            else if (MainGame.main.AllPlayers[x].Position == 1)
                Gametexts[2+x].color = Color.green;
            else
                Gametexts[2+x].color = Color.white;
        }
    }

    public void SetScreens()
    {
        //Debug.Log(MainGame.main.AllPlayers);
        for (int x = 0; x < 6; x++)
        {
            if (x >= GameControl.control.game.NumbPlayers)
                GameImages[x].gameObject.SetActive(false);
            else
            {
                Gametexts[14+x].text = MainGame.main.AllPlayers[x].PlayerName;
                //Gametexts[8+x].text = GameControl.control.game.Players[x].PlayerName;
                //Gametexts[2+x].text = GameControl.control.game.Players[x].PlayerName;
                string temp = "";

                foreach (var p in MainGame.main.AllPlayers[x].DieValues)
                    temp += p.ToString() + ", ";
                
                Debug.Log(temp);
                temp = temp.Remove(temp.Length-2, 2);
                
                Gametexts[8+x].text = temp;
            }
            UpdateScreens();
        }
        Sliders[1].value = GameControl.control.rules.Musicvolume * 8;
        Sliders[2].value = GameControl.control.rules.Soundvolume * 8;
        Sliders[3].value = GameControl.control.game.Zoom * 4 - 0.25f;
        
    }

    public void PlayerControl(bool remove)
    {
        foreach(var b in InteractiveButtons)
        {
            b.interactable = remove;
        }
        if (MainGame.main.NoHumanPlayer)
            InteractiveButtons[5].interactable = true;
    }

    public void Announce(string text)
    {
        Gametexts[0].text = text;
        Gametexts[1].text = "Turn \n" + GameControl.control.game.Turn + "/" + GameControl.control.game.TurnLimit;
        if (GameControl.control.game.Turn > GameControl.control.game.TurnLimit)
            Gametexts[1].text = "Game \n Over!";
    }

    public IEnumerator DoneLoading()
    {
        for (int x = 0; x < 5; x++)
        {
            yield return new WaitForSeconds(0.1f);
            Gametexts[20].color = new Color(1,1,1,1-x*0.25f);
        }
        for (int y = 0; y < 11; y++)
        {
            yield return new WaitForSeconds(0.1f);
            GameImages[6].color = new Color(0,0,0,1-y*0.1f);
        }
        MainGame.main.DieAnim.SetFloat("Speed", 1);
        yield return new WaitForSeconds(3f);
        Sliders[0].value = GameControl.control.game.AnimationSpeed * 4;
        MainGame.main.DieAnim.SetFloat("Speed", GameControl.control.game.AnimationSpeed);
        MainGame.main.DieAnim.SetInteger("DieResult", MainGame.main.cursorview.RolledDie);
        if (GameControl.control.game.AnimationSpeed == 0)
        {
            MainGame.main.Animating = false;
            MainGame.main.cursorview.gameObject.SetActive(true);
            if (GameControl.control.rules.Helpplacements)
                MainGame.main.ValidPlacementsInstantiated(true);
        }
        GameImages[6].enabled = false;
        MainGame.main.Paused = false;
        if (GameControl.control.PlayTutorial)
            AdvanceTutorial(0);
            
        ObjectPooler.SharedPooler.PlaySound(9,true);
        
    }

    public void SetSliderAmount(string x)
    {
        if (GameUIButtons[17].gameObject.activeSelf)
        {
            switch (x)
            {
                case "Animation":
                    GameControl.control.game.AnimationSpeed = Sliders[0].value * 0.25f;
                    MainGame.main.DieAnim.SetFloat("Speed", GameControl.control.game.AnimationSpeed);
                    MainGame.main.DieAnim.SetTrigger("DoneRolling");
                    MainGame.main.DieAnim.SetInteger("DieResult", 0);
                    MainGame.main.Animating = false;
                    MainGame.main.cursorview.gameObject.SetActive(true);
                    if (GameControl.control.rules.Helpplacements)
                        MainGame.main.ValidPlacementsInstantiated(true);

                    break;

                case "Music":
                    GameControl.control.rules.Musicvolume = Sliders[1].value * 0.125f;
                    ObjectPooler.SharedPooler.Music.volume = GameControl.control.rules.Musicvolume;
                    break;
                
                case "Sounds":
                    GameControl.control.rules.Soundvolume = Sliders[2].value * 0.125f;
                    ObjectPooler.SharedPooler.PlaySound(0,false);
                    break;
                
                case "Zoom":
                    GameControl.control.game.Zoom = Sliders[3].value * 0.25f + 0.25f;
                    MainGame.main.cursor.ChangeZoom(GameControl.control.game.Zoom);
                    break;
            }
        }
    }

    public void AdvanceTutorial(int index)
    {
        Debug.Log(index);        
        GameImages[7].gameObject.SetActive(true);  
        PlayerControl(false);
        MainGame.main.Paused = true;
        if (index == -1 && !Multipart)
        {    
            index = MainGame.main.TutorialIndex;
            PlayerControl(true);
            if (index < 14)
                InteractiveButtons[5].interactable = false;

            MainGame.main.Paused = false;
            GameImages[7].gameObject.SetActive(false);            
        }
        else
        {
            index = MainGame.main.TutorialIndex;
            Gametexts[21].text = MainGame.main.TutorialMessages[index];
            switch(index)
            {
                case 0:
                Multipart = true;
                break;

                case 3:
                Multipart = true;
                break;

                case 4:
                Multipart = true;
                break;

                case 5:
                Multipart = false;
                GameControl.control.rules.StackSides = 2;
                MainGame.main.AllPlacements();
                break;

                case 12:
                Multipart = true;
                break;

                default:
                Multipart = false;
                break;
            }

            MainGame.main.TutorialIndex++;
        }
            
//        MainGame.main.TutorialIndex = 1;
    }

}
