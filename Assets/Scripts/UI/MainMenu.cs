using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenu : MonoBehaviour {
    public Canvas MainCanvas;
    public Canvas Options;
    public Canvas ControlsCanvas;
    public Toggle MusicOn, InvertAxis;
    private GameStatus gameStatus;

    void Awake()
    {
        gameStatus = Helper.GetOrCreateGameStatus();
        DontDestroyOnLoad(gameStatus.gameObject);
    }

	// Use this for initialization
	void Start () {

        Button [] buttons = GameObject.FindObjectsOfType<Button>();

        for (int i = 0; i < buttons.Length; i++ ) {
            if (buttons[i].name.StartsWith("Level"))
            {                
                int levelid = int.Parse(buttons[i].name.Substring("Level".Length));

                if (levelid == 1) buttons[i].Select();

                Text txt = buttons[i].gameObject.transform.FindChild("Text").gameObject.GetComponent<Text>();

                txt.text = levelid + ". " + LevelNames.Names[levelid];
            }
        }

        GotoMain();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateMusic(bool value)
    {
        gameStatus.MusicOn = value;
    }

    public void UpdateAxis(bool value)
    {
        gameStatus.InvertAxis = value;
    }

    public void OptionsMenu()
    {
        MusicOn.isOn = gameStatus.MusicOn;
        InvertAxis.isOn = gameStatus.InvertAxis;

        MainCanvas.enabled = false;
        Options.enabled = true;
    }

    public void Controls()
    {
        MainCanvas.enabled = false;
        ControlsCanvas.enabled = true;
    }

    public void GotoMain()
    {
        MainCanvas.enabled = true;
        ControlsCanvas.enabled = false;
        Options.enabled = false;
    }

    public void GotoLevel(int level)
    {
        gameStatus.Level = level;
        Application.LoadLevel("City");
    }

    public void FreeFlight()
    {
        gameStatus.Level = 0;
        Application.LoadLevel("City");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
