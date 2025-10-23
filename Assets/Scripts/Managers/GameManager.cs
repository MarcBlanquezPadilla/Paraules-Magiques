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

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    private Mode mode;
    [SerializeField]private Difficulty difficulty;

    private WordsManager _wordsManager;

    private void Awake() {
        
        _wordsManager = GetComponent<WordsManager>();
        AudioManager.Instance.PlaySong("musicaMenu");
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

    public void SetDifficulty(Difficulty _difficulty)
    {
        difficulty = _difficulty;
    }

    public Mode GetMode()
    {
        return mode;
    }

    public Difficulty GetDifficulty()
    {  
        return difficulty; 
    }
}
