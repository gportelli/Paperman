using UnityEngine;
using System.Collections;

public struct LevelStats
{
    public string LevelName;
    public int YellowStars;
    public int RedStars;
    public float Time;
    public bool Completed;
}

public class GameStatus : MonoBehaviour {
    static public GameStatus instance;

    public int Level = 0;
    public bool MusicOn = true;
    public bool InvertAxis = false;

    public LevelStats[] LevelsStats;

    void Awake()
    {
        instance = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetStats(LevelStats levelStats)
    {
        this.LevelsStats[Level] = levelStats;
    }
}
