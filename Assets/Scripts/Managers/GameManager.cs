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

    [SerializeField] GameData data;

    [SerializeField] private Mode mode;
    [SerializeField] private Difficulty difficulty;


    private WordsManager _wordsManager;
    [SerializeField] private BruixaBehaviour _bruixa;


    private void Awake() {

        difficulty = (Difficulty) data.difficulty;
        mode = (Mode) data.mode;
        _wordsManager = GetComponent<WordsManager>();
	}

    private void Start() {

        _wordsManager.ChangeWord();
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

    public void CompleteWord()
    {
        _wordsManager.ChangeWord();
        _bruixa.BruixaHappy();
    }

    public void FailWord()
    {
        _wordsManager.ReloadWord();
        _bruixa.BruixaSad();
    }
}
