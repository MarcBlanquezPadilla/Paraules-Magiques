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

    [SerializeField] private Mode mode;
    [SerializeField] private Difficulty difficulty;
    
    [SerializeField] private int wordsToWin;
    [SerializeField] private int timeToEnd;
    [SerializeField] private float timeCorrectWord;

    private WordsManager _wordsManager;
    [SerializeField] private BruixaBehaviour _bruixa;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject tutorialMenu;
    [SerializeField] private GameObject contadorRatoli;
    [SerializeField] private GameObject contador;

    private int wordCounter;
    private float endTimer;
    private float correctWordTimer;
    private bool changingWord;
    private bool correct;


    private void Awake() {

        difficulty = (Difficulty)DataManager.Instance.data.difficulty;
        mode = (Mode)DataManager.Instance.data.mode;
        _wordsManager = GetComponent<WordsManager>();
        wordCounter = 0;

    }

    private void Start() {

        contadorRatoli.GetComponentInChildren<TextMeshProUGUI>().text = DataManager.Instance.data.mouses.ToString();
        _wordsManager.ChangeWord();
        pauseMenu.SetActive(false);
        tutorialMenu.SetActive(false);
    }

    private void Update()
    {
        endTimer += Time.deltaTime;
        

        switch (mode)
        {
            case Mode.Normal:
                contador.GetComponentInChildren<TextMeshProUGUI>().text = wordCounter + " / " + wordsToWin;
                break;
            
            case Mode.Timed:
                contador.GetComponentInChildren<TextMeshProUGUI>().text = ((int)Mathf.Clamp(timeToEnd - endTimer, 0, int.MaxValue)).ToString();
                if (endTimer > timeToEnd)
                {
                    int divider = Mathf.Clamp(timeToEnd / 10, 1, int.MaxValue);
                    int mousesToSum = wordCounter / divider;
                    DataManager.Instance.data.mouseNum = mousesToSum;
                    DataManager.Instance.data.wordsNum = wordCounter;
                    ScenesManager.Instance.ChangeScene("MenuWin");
                }
                break;
        }

        if (changingWord)
        {
            correctWordTimer += Time.deltaTime;

            if (correctWordTimer > timeCorrectWord) 
            {
                if (correct) _wordsManager.ChangeWord();
                else _wordsManager.ReloadWord();
                changingWord = false;
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
            DataManager.Instance.data.mouseNum = 1;
            ScenesManager.Instance.ChangeScene("MenuWin");
        }
        else
        {
            correctWordTimer = 0;
            changingWord = true;
            correct = true;
            _wordsManager.ShowCorrect(correct);
            _bruixa.BruixaHappy();
        }
    }

    public void FailWord()
    {
        correct = false;
        correctWordTimer = 0;
        changingWord = true;
        _wordsManager.ShowCorrect(correct);
        _bruixa.BruixaSad();
    }

    public void ChangeScene(string s)
    {
        DataManager.Instance.SaveData();
        ScenesManager.Instance.ChangeScene(s);
    }
}
