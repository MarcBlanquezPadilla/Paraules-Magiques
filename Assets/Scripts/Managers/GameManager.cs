using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	#region Instance

	private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<GameManager>();
            }
            return _instance;
        }
    }

    #endregion

    public enum Mode
    {
        Normal,
        Timed
    }

    private Mode mode;

    private WordsManager _wordsManager;

    private void Awake() {
        
        _wordsManager = GetComponent<WordsManager>();
	}

    private void Start() {


    }

	public void Exit() {

        Application.Quit();
    }

    public void SetMode(Mode _mode)
    {
        mode = _mode;
    }
}
