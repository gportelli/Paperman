using UnityEngine;
using System.Collections;

public class Window : MonoBehaviour {
    public GameObject ColliderStart, CollidersEnd;
    public bool IsStart = false;

    public Transform LookFrom;

    private GameController gameController;

    private bool entered = false;

    void Awake()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    void Start () {
        
	}

    void OnEnable()
    {
        CollidersEnd.SetActive(!IsStart);
        ColliderStart.SetActive(false);
    }
	
	void Update () {
	
	}

    public void EnableColliders()
    {
        StartCoroutine(_EnableColliders());
    }

    IEnumerator _EnableColliders()
    {
        yield return new WaitForSeconds(1);
        ColliderStart.SetActive(IsStart);
    }

    void OnTriggerEnter(Collider other)
    {
        if (IsStart || entered) return;

        if(other.tag == "PlayerCollider")
        {
            entered = true;

            gameController.LevelCompleted();
        }
    }
}
