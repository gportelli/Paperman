using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Rewired;

public class UIController : MonoBehaviour {
    public static UIController instance;

    GameObject player;

    public Canvas HUD;
    public Text Speed, Altitude, Boost;
    public Image DistanceIndicator;
    public Text DistanceText;
    public Text RedStarsCount;
    public Text YellowStarsCount;

    public Canvas InGameMenu;
    public Text Title;
    public Text Stats;
    public Button ContinueButton;
    public Button RetryButton;
    public Button ExitButton;
    public Button NextLevelButton;

    private bool showingComplete;

    private WindBoost windBoost;

    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        windBoost = player.GetComponent<WindBoost>();
    }

	// Use this for initialization
	void Start () {
        HUD.enabled = false;
        RedStarsCount.text = "0";
        YellowStarsCount.text = "0";
	}

    public void UpdateStats(LevelStats stats)
    {
        RedStarsCount.text = "" + stats.RedStars;
        YellowStarsCount.text = "" + stats.YellowStars;
    }

    public void  ShowHUD(bool show = true)
    {
        HUD.enabled = show;
    }

    public bool ShowingComplete()
    {
        return showingComplete;
    }

    public void ShowInGameMenu(string title, bool showContinue)
    {
        InGameMenu.enabled = true;
        ContinueButton.gameObject.SetActive(showContinue);
        NextLevelButton.gameObject.SetActive(false);

        if (showContinue) ContinueButton.Select();

        Stats.text = "";
        Title.text = title;
    }

    public void ShowGameCompleteMenu(LevelStats stats)
    {
        showingComplete = true;
        InGameMenu.enabled = true;
        ContinueButton.gameObject.SetActive(false);
        NextLevelButton.gameObject.SetActive(stats.RedStars > 0 && GameController.instance.HasMoreLevels() && stats.Completed);

        if (NextLevelButton.gameObject.activeInHierarchy) NextLevelButton.Select();
        else RetryButton.Select();

        if(stats.RedStars > 0 && stats.Completed) {
            Title.text = "Level \"" + stats.LevelName + "\" Completed!";
        }
        else
        {
            Title.text = "Level " + stats.LevelName + " Failed";
        }

        Stats.text = "Red Stars: " + stats.RedStars + "  Yellow Stars: " + stats.YellowStars;
    }

    public void HideInGameMenu()
    {
        InGameMenu.enabled = false;
        showingComplete = false;
    }

	// Update is called once per frame
	void Update () {
        if (PlayerInput.Instance.Input.GetButtonDown("Fire") && InGameMenu.enabled)
        {
            if (NextLevelButton.gameObject.activeInHierarchy) GameController.instance.NextLevel();
            else if (ContinueButton.gameObject.activeInHierarchy) GameController.instance.Continue();
            else if (RetryButton.gameObject.activeInHierarchy) GameController.instance.Retry();
        }

        Speed.text = "Speed " + (player.activeInHierarchy ? (int)(player.rigidbody.velocity.magnitude * 3.6) : 0) + " Km/h";
        Altitude.text = "Altitude " + ((int)player.transform.position.y) + "m";
        Boost.text = "Wind Boost " + (windBoost.infiniteBoost ? "Infinite" : ""+ Mathf.Ceil(windBoost.GetAvailableBoost() * 100) + "%");

        if (GameController.instance.Level.EndWindow != null)
        {
            float targetDistance = GameController.instance.GetTargetDistance();

            if (targetDistance > 5)
            {
                DistanceIndicator.enabled = true;
                DistanceText.enabled = true;

                DistanceText.text = ((int)targetDistance) + " M";

                Vector2 targetPosition = GameController.instance.GetTargetInScreen();

                //Debug.Log("received " + targetPosition.x + " " + targetPosition.y);

                float padding = 0.1f;
                if (targetPosition.x < padding) targetPosition.x = padding;
                else if (targetPosition.x > 1 - padding)
                {
                    targetPosition.x = 1 - padding;
                    targetPosition.y *= 1 - padding;
                }

                if (targetPosition.y < padding) targetPosition.y = padding;
                else if (targetPosition.y > 1 - padding)
                {
                    targetPosition.y = 1 - padding;
                    targetPosition.x *= 1 - padding;
                }

                DistanceIndicator.rectTransform.anchoredPosition = new Vector3(Screen.width * (targetPosition.x - 0.5f), Screen.height * (targetPosition.y - 0.5f), 0);
            }
            else
            {
                DistanceIndicator.enabled = false;
                DistanceText.enabled = false;
            }
        }
        else
        {
            DistanceIndicator.enabled = false;
            DistanceText.enabled = false;
        }
	}
}
