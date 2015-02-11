
using UnityEngine;

class Helper
{
    static public float GetRotation(float angle)
    {
        if (angle > 180) return angle - 360;
        return angle;
    }

    static public GameStatus GetOrCreateGameStatus()
    {
        GameStatus gameStatus;

        GameObject tmp = GameObject.FindGameObjectWithTag("GameStatus");
        if (tmp != null)
        {
            gameStatus = tmp.GetComponent<GameStatus>();
        }
        else
        {
            tmp = new GameObject("GameStatus");
            tmp.tag = "GameStatus";
            tmp.AddComponent<GameStatus>();

            gameStatus = tmp.GetComponent<GameStatus>();
        }

        return gameStatus;
    }
}

