using TMPro;
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
    
    [SerializeField] private int wordsToWin;
    [SerializeField] private int timeToEnd;

    private WordsManager _wordsManager;
    [SerializeField] private BruixaBehaviour _bruixa;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject contadorRatoli;
    [SerializeField] private GameObject contadorTemps;

    private int wordCounter;
    private float endTimer;


    private void Awake() {

        difficulty = (Difficulty) data.difficulty;
        mode = (Mode) data.mode;
        _wordsManager = GetComponent<WordsManager>();
        wordCounter = 0;
        endTimer = 0;

    }

    private void Start() {

        contadorRatoli.GetComponentInChildren<TextMeshProUGUI>().text = data.mouses.ToString();
        _wordsManager.ChangeWord();
        pauseMenu.SetActive(false);
        
        if(mode==Mode.Timed) contadorTemps.SetActive(true);
        else contadorTemps.SetActive(false);
    }

    private void Update()
    {
        endTimer += Time.deltaTime;
        if (mode == Mode.Timed)
        {
            contadorTemps.GetComponentInChildren<TextMeshProUGUI>().text = ((int)Mathf.Clamp(timeToEnd-endTimer, 0, int.MaxValue)).ToString();
            if (endTimer > timeToEnd)
            {
                int divider = Mathf.Clamp(timeToEnd / 10, 1, int.MaxValue);
                int mousesToSum = wordCounter / divider;
                data.ChangeMouseNum(mousesToSum);
                ScenesManager.Instance.ChangeScene("MenuWin");
            }
        }
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
        wordCounter++;
        if (wordCounter >= wordsToWin && mode == Mode.Normal)
        {
            data.ChangeMouseNum(1);
            ScenesManager.Instance.ChangeScene("MenuWin");
        }
        else
        {
            _wordsManager.ChangeWord();
            _bruixa.BruixaHappy();
        }
    }

    public void FailWord()
    {
        _wordsManager.ReloadWord();
        _bruixa.BruixaSad();
    }

    public void ChangeScene(string s)
    {
        ScenesManager.Instance.ChangeScene(s);
    }
}
