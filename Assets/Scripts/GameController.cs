using UnityEngine;
using System.Collections;
using AClockworkBerry.Unity.Splines;

public class GameController : MonoBehaviour {
    static public GameController instance;

    public bool DebugOn = true;
    public bool SkipIntro = true;
    public bool StartFromCurrent = false;
    public int StartLevel = 0;
    public bool MusicOn = false;
    public bool InvertAxis = true;
    public bool InfiniteBoost = false;
    public bool KinematicMode = false;

    public Canvas Menu;

    public Level [] Levels;

    private PlayerController player;
    private CameraController cameraController;
    private UIController ui;
    private Level level;
    public Level Level { get { return level; } }

    private GameStatus gameStatus;

    private LevelStats levelStats;

    void Awake()
    {
        instance = this;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        cameraController = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>();
        ui = GetComponent<UIController>();
        gameStatus = Helper.GetOrCreateGameStatus();

        if (gameStatus.LevelsStats == null)
            gameStatus.LevelsStats = new LevelStats[Levels.Length];

        if (DebugOn)
        {
            gameStatus.Level = StartLevel;
            gameStatus.MusicOn = MusicOn;
            gameStatus.InvertAxis = InvertAxis;
        }
    }

	// Use this for initialization
	void Start () {
        Menu.enabled = false;

        audio.enabled = gameStatus.MusicOn;

        LoadLevel();

        cameraController.Init();

        if (DebugOn && KinematicMode)
        {
            player.rigidbody.isKinematic = true;
        }

        if (KinematicMode) StartFromCurrent = true;

        if (DebugOn && (SkipIntro || StartFromCurrent))
        {
            if (!StartFromCurrent)
            {
                player.transform.position = level.StartWindow.transform.TransformPoint(0, 0, .5f);
                player.transform.rotation = level.StartWindow.transform.rotation;
                player.rigidbody.velocity = player.transform.TransformVector(new Vector3(0, 0, .5f));
            }

            ui.ShowHUD();
        }
        else
        {
            StartCoroutine(StartSequence());
        }
	}

    void LoadLevel()
    {
        for (int i = 0; i < Levels.Length; i++)
            Levels[i].LevelObject.SetActive(i == gameStatus.Level);
        
        level = Levels[gameStatus.Level];

        WindBoost windBoost = player.GetComponent<WindBoost>();
        windBoost.infiniteBoost = DebugOn ? InfiniteBoost : level.InfiniteBoost;
        windBoost.boostDuration = level.BoostDuration;
        windBoost.StartBoost = level.StartBoost;

        PlayerAerodynamicsController dynamics = player.GetComponent<PlayerAerodynamicsController>();
        if (level.OverrideDynamics)
        {
            if (level.Gravity != 0) dynamics.Gravity = level.Gravity;
            if (level.Mass != 0) dynamics.Mass = level.Mass;
            if (level.cdBottom != 0) dynamics.cdBottom = level.cdBottom;
            if (level.cdFront != 0) dynamics.cdFront = level.cdFront;
            if (level.cdRight != 0) dynamics.cdRight = level.cdRight;
            if (level.LiftCoefficient != 0) dynamics.LiftCoefficient = level.LiftCoefficient;
        }

        levelStats.Completed = false;
        levelStats.LevelName = LevelNames.Names[gameStatus.Level];
    }
	
	// Update is called once per frame
	void Update () {
        if (PlayerInput.Instance.Input.GetButtonDown("Start") && !ui.ShowingComplete())
        {
            if (Time.timeScale != 0)
            {
                ui.ShowInGameMenu("Game Paused", true);
                Time.timeScale = 0;
            }
            else
            {
                ui.HideInGameMenu();
                Time.timeScale = 1;
            }
        }
	}

    public float GetTargetDistance()
    {
        return (player.transform.position - level.EndWindow.transform.position).magnitude;
    }

    public Vector2 GetTargetInScreen()
    {
        Vector3 target = level.EndWindow.transform.position;
        Vector3 screenPos = cameraController.camera.WorldToScreenPoint(target);

        if (screenPos.z < 0)
        {
            Vector3 delta = level.EndWindow.transform.position - cameraController.camera.transform.position;
            Vector3 relativeTargetPos =  cameraController.camera.transform.InverseTransformVector(delta);

            //Debug.DrawLine(cameraController.camera.transform.position, level.EndWindow.transform.position);

            float slope, screenSlope = Screen.height / (float)Screen.width;
            
            //Debug.Log("delta = " + delta);
            //Debug.Log(relativeTargetPos.x + " " + relativeTargetPos.y + " " + relativeTargetPos.z);

            if (relativeTargetPos.x != 0)
            {
                slope = relativeTargetPos.y / relativeTargetPos.x;
                if (Mathf.Abs(slope) < screenSlope)
                {
                    screenPos.x = relativeTargetPos.x > 0 ? 1 : 0;
                    screenPos.y = 0.5f * (1 - slope / screenSlope);
                }
                else
                {
                    screenPos.y = relativeTargetPos.y > 0 ? 1 : 0;
                    screenPos.x = 0.5f * (1 + screenSlope / slope);
                }

                return new Vector2(screenPos.x, screenPos.y);
            }
        }

        return new Vector2(screenPos.x / cameraController.camera.pixelWidth, screenPos.y / cameraController.camera.pixelHeight);
    }

    public void Continue()
    {
        ui.HideInGameMenu();
        Time.timeScale = 1;                
    }

    public void NextLevel()
    {
        ui.HideInGameMenu();
        gameStatus.Level++;
        Time.timeScale = 1;
        Application.LoadLevel("City");
    }

    public void Retry()
    {
        Time.timeScale = 1;
        Application.LoadLevel("City");
    }

    public void Exit()
    {
        Time.timeScale = 1;
        Application.LoadLevel("MainMenu");
    }

    public void Gameover()
    {
        //Time.timeScale = 0;
        gameStatus.SetStats(levelStats);
        ui.ShowGameCompleteMenu(levelStats);
    }

    IEnumerator StartSequence()
    {
        float progress = 0;

        player.gameObject.SetActive(false);        

        if (level.CamPath != null && level.CamLookAtPath != null)
        {
            cameraController.SetDisabled();

            cameraController.transform.position = level.CamPath.GetPointAtIndex(0);
            cameraController.transform.LookAt(level.CamLookAtPath.GetPointAtIndex(0));

            do
            {
                progress += Time.deltaTime / 1f;

                if (PlayerInput.Instance.Input.GetButtonDown("Fire") && Time.timeScale != 0)
                {
                    StartGame();
                    yield break;
                }

                yield return null;
            }
            while (progress <= 1);

            level.CamLookAtPath.SetControlPoint(level.CamLookAtPath.curveCount * 3, level.StartWindow.transform.position - level.CamLookAtPath.transform.position);
            progress = 0;

            do
            {
                cameraController.transform.position = level.CamPath.GetPoint(progress);
                cameraController.transform.LookAt(level.CamLookAtPath.GetPoint(progress));

                progress += Time.deltaTime / level.IntroDuration;
                if (progress > 1) progress = 1;

                if (PlayerInput.Instance.Input.GetButtonDown("Fire") && Time.timeScale != 0)
                {
                    StartGame();
                    yield break;
                }

                yield return false;
            }
            while (progress < 1);
            
            cameraController.SetFixed(cameraController.transform.position, Vector3.zero);
        }
        else {
            cameraController.SetFixed(level.StartWindow.LookFrom.position, level.StartWindow.LookFrom.position);
        }

        // Positioning player
        player.transform.position = level.StartWindow.transform.position;
        player.transform.rotation = level.StartWindow.transform.rotation;
        player.rigidbody.velocity = player.transform.TransformVector(new Vector3(0, 0, .5f));

        // No user control
        player.EnableUserControl(false);
        player.gameObject.SetActive(true);

        progress = 0;
        do
        {
            progress += Time.deltaTime / 2f;

            if (PlayerInput.Instance.Input.GetButtonDown("Fire") && Time.timeScale != 0)
                break;

            yield return null;
        }
        while (progress <= 1);

        // Enable user control
        player.EnableUserControl(true);

        cameraController.SetFollow();

        level.StartWindow.EnableColliders();

        ui.ShowHUD();
    }

    private void StartGame()
    {
        // Positioning player
        player.transform.position = level.StartWindow.transform.TransformPoint(0, 0, 1f);
        player.transform.rotation = level.StartWindow.transform.rotation;
        player.rigidbody.velocity = player.transform.TransformVector(new Vector3(0, 0, .5f));

        player.EnableUserControl(true);
        player.gameObject.SetActive(true);

        cameraController.SetFollow(false);

        level.StartWindow.EnableColliders();

        ui.ShowHUD();
    }

    public void LevelCompleted()
    {
        gameStatus.SetStats(levelStats);
        levelStats.Completed = true;
        StartCoroutine(EndSequence());  
    }

    IEnumerator EndSequence()
    {
        cameraController.SetFixed(level.EndWindow.LookFrom.position, Vector3.zero);

        player.EnableUserControl(false);
        player.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);

        ui.ShowGameCompleteMenu(levelStats);
    }

    public void StarPicked(bool IsRed)
    {
        if(IsRed)
            levelStats.RedStars++;
        else
            levelStats.YellowStars++;

        ui.UpdateStats(levelStats);
    }

    public bool HasMoreLevels()
    {
        return gameStatus.Level < Levels.Length - 1;
    }
}
