using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using static GameManager;

public class WordsManager : MonoBehaviour
{
    [SerializeField] private TextAsset wordsTextAsset;
    [SerializeField] private List<string> wordsList = new List<string>();

    [SerializeField] private string originalWord;
    [SerializeField] private string startCurrentWord;
    [SerializeField] private string startMixedWord;
    [SerializeField] private string currentWord;
    [SerializeField] private string mixedWord;

    [SerializeField] private List<GameObject> currentLetterDisplays;
    [SerializeField] private List<GameObject> mixedLetterDisplays;
    [SerializeField] private List<GameObject> letterDisplays;
    [SerializeField] private Transform currentParent;
    [SerializeField] private Transform mixedParent;
    [SerializeField] private Transform substituteDisplay;
    [SerializeField] private Transform remoteSubstituteDisplay;

    //Help
    [SerializeField] private GameObject pistaDisplay;
    [SerializeField] private GameObject mussol;
    [SerializeField] private float timeToUnlockHelp;
    [SerializeField] private float timeShowingHelp;
    [SerializeField] private float pistaPositionOffset;
    bool showingHelp;
    float timerUnlockHelp;
    float timerToHideHelp;

    List<string> doneWords = new List<string>();

    private int draggedLetterSourceIndex = -1;
    private bool draggedLetterIsFromMixed = false;

    private int remoteDraggedLetterSourceIndex = -1;
    private bool remoteDraggedLetterIsFromMixed = false;

    private float lastRemoteDragTime;


    private void Awake()
    {
        //INIT WORDS
        LoadWords();
        LoadLetterDisplays();
        timerUnlockHelp = 0f;
        timerToHideHelp = 0f;
        showingHelp = false;
    }

    private void Update()
    {
        if (!showingHelp) timerUnlockHelp += Time.deltaTime;
        else
        {
            timerToHideHelp += Time.deltaTime;
            if (timerToHideHelp > timeShowingHelp)
            {
                HideHelp();
            }
        }

        if (timerUnlockHelp > timeToUnlockHelp) mussol.GetComponent<MussolBehaviour>().ShowPista();
        else mussol.GetComponent<MussolBehaviour>().HidePista();

        if (remoteDraggedLetterSourceIndex != -1 && Time.time - lastRemoteDragTime > 0.5f)
        {
            LetterDisplay l = GetLetterDisplayInstance(remoteDraggedLetterSourceIndex, remoteDraggedLetterIsFromMixed);
            EndRemoteDragVisual();
            if (l != null) l.transform.SetSiblingIndex(l.letterIndex);
        }
    }

    private void LoadWords()
    {
        if (wordsTextAsset)
        {
            //LOAD DOCUMENT
            XmlDocument wordsDocument = new XmlDocument();
            wordsDocument.LoadXml(wordsTextAsset.text);

            //LOAD WORDS
            XmlNode wordsNode = wordsDocument.SelectSingleNode("joc/paraules");

            if (wordsNode != null)
            {
                foreach (XmlNode wordNode in wordsNode)
                {
                    wordsList.Add(wordNode.InnerText);
                }
            }
        }
    }

    private void LoadLetterDisplays()
    {
        foreach (Transform letter in currentParent)
        {
            currentLetterDisplays.Add(letter.gameObject);
            letterDisplays.Add(letter.gameObject);
        }
        foreach (Transform letter in mixedParent)
        {
            mixedLetterDisplays.Add(letter.gameObject);
            letterDisplays.Add(letter.gameObject);
        }
    }

    public void ShowHelp()
    {
        if (timerUnlockHelp > timeToUnlockHelp)
        {
            bool found = false;
            int i = 0;
            int iterations = 0;
            do
            {
                iterations++;
                if (iterations > 20) break;

                i = UnityEngine.Random.Range(0, originalWord.Count());
                if (currentWord[i] != originalWord[i]) found = true;
            } while (!found && iterations < 20);

            if (found) 
            {
                pistaDisplay.SetActive(true);
                pistaDisplay.GetComponent<LetterDisplay>().ChangeLetter(originalWord[i]);
                Vector3 pistaPosition = currentLetterDisplays[i].transform.position;
                pistaPosition.y += pistaPositionOffset;
                pistaDisplay.transform.position = pistaPosition;
                showingHelp = true;
                timerUnlockHelp = 0;
            }
        }
    }

    public void HideHelp()
    {
        timerToHideHelp = 0;
        timerUnlockHelp = 0;
        pistaDisplay.SetActive(false);
        showingHelp=false;
    }

    public void ChangeWord()
    {
        if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer) return;
       
        //GET NEW WORD
        bool different;
        do
        {
            different = true;
            originalWord = wordsList[UnityEngine.Random.Range(0, wordsList.Count)];
            foreach (string word in doneWords)
            {
                if (originalWord == word) different = false;
            }
        }
        while (!different);

        startCurrentWord = "";
        startMixedWord = "";


        int lettersToHide;
        switch (GameManager.Instance.GetDifficulty())
        {
            case Difficulty.Easy:
                lettersToHide = 2;
                break;
            case Difficulty.Normal:
                lettersToHide = 3;
                break;
            case Difficulty.Hard:
                lettersToHide = 4;
                break;
            default:
                lettersToHide = 2;
                break;
        }

        lettersToHide = Mathf.Clamp(lettersToHide, 0, originalWord.Length);

        List<bool> hideLetter = new List<bool>();

        for (int i = 0; i < originalWord.Length; i++)
        {
            hideLetter.Add(false);
        }

        for (int i = 0; i < lettersToHide; i++)
        {
            int numToHide = 0;
            do
            {
                numToHide = UnityEngine.Random.Range(0, originalWord.Length);
            }
            while (hideLetter[numToHide]);

            hideLetter[numToHide] = true;
        }

        for (int i = 0; i < originalWord.Length; i++)
        {
            if (hideLetter[i])
            {
                startMixedWord += originalWord[i];
                startCurrentWord += " ";
            }

            else startCurrentWord += originalWord[i];
        }

        //MIX NEW WORD
        startMixedWord = startMixedWord.ToCharArray().OrderBy(c => UnityEngine.Random.value).ToArray().ArrayToString();

        currentWord = startCurrentWord;
        mixedWord = startMixedWord;

        UpdateDisplay();
        HideHelp();

        if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.SendWordState(originalWord, currentWord, mixedWord, startCurrentWord, startMixedWord);
        }
    }

    public void ReloadWord()
    {
        if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer) return;
        currentWord = startCurrentWord;
        mixedWord = startMixedWord;
        UpdateDisplay();
        if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
            NetworkManager.Instance.SendWordState(originalWord, currentWord, mixedWord, startCurrentWord, startMixedWord);
    }

    private void UpdateDisplay()
    {
        foreach (GameObject g in mixedLetterDisplays)
        {
            g.SetActive(false);
        }

        int lettersToDisplay = Mathf.Min(mixedWord.Length, mixedLetterDisplays.Count);
        for (int i = 0; i < lettersToDisplay; i++)
        {
            LetterDisplay letterDisplay = mixedLetterDisplays[i].GetComponent<LetterDisplay>();
            if (letterDisplay)
            {
                letterDisplay.ChangeLetter(mixedWord[i]);
                letterDisplay.letterIndex = i;
            }
            mixedLetterDisplays[i].SetActive(true);
            mixedLetterDisplays[i].transform.SetSiblingIndex(i);
        }

        foreach (GameObject g in currentLetterDisplays)
        {
            g.SetActive(false);
        }

        lettersToDisplay = Mathf.Min(currentWord.Length, currentLetterDisplays.Count);
        for (int i = 0; i < lettersToDisplay; i++)
        {
            LetterDisplay letterDisplay = currentLetterDisplays[i].GetComponent<LetterDisplay>();
            if (currentWord[i] == startCurrentWord[i]) letterDisplay.ChangeState(LetterDisplay.State.Locked);
            else letterDisplay.ChangeState(LetterDisplay.State.Dragable);
            letterDisplay.ChangeLetter(currentWord[i]);
            letterDisplay.letterIndex = i;
            currentLetterDisplays[i].SetActive(true);
            currentLetterDisplays[i].transform.SetSiblingIndex(i);
        }
    }

    public void OnStartDragLetter(GameObject letterDisplay)
    {
        LetterDisplay dragLetterDisplay = letterDisplay.GetComponent<LetterDisplay>();
        draggedLetterSourceIndex = dragLetterDisplay.letterIndex;
        draggedLetterIsFromMixed = dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed;

        if (!draggedLetterIsFromMixed)
        {
            substituteDisplay.GetComponentInChildren<Image>().enabled = true;
        }
            
        int visualIndex = letterDisplay.transform.GetSiblingIndex();
        letterDisplay.transform.SetParent(mixedParent.parent);
        substituteDisplay.gameObject.SetActive(true);
        if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed) substituteDisplay.transform.SetParent(mixedParent);
        else if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current) substituteDisplay.transform.SetParent(currentParent);
        substituteDisplay.transform.SetSiblingIndex(visualIndex);
    }

    public void OnEndDragLetter(GameObject letterDisplay)
    {
        LetterDisplay dragLetterDisplay = letterDisplay.GetComponent<LetterDisplay>();

        bool foundDropTarget = false;
        int targetIndex = -1;

        int sourceIndex = draggedLetterSourceIndex;
        char letterToMove = dragLetterDisplay.GetLetter();

        for (int i = 0; i < currentLetterDisplays.Count && i < currentWord.Length; i++)
        {
            if (currentWord[i] == ' ')
            {
                float minDistance = 150 / 2;
                float distance = Vector2.Distance(currentLetterDisplays[i].transform.position, letterDisplay.transform.position);

                if (distance < minDistance)
                {
                    foundDropTarget = true;
                    targetIndex = i;
                    break;
                }
            }
        }

        if (foundDropTarget)
        {
            AudioManager.Instance.PlayAudio("soLletres");
            letterDisplay.SetActive(false);
            HideHelp();

            if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.SendLetterMove(sourceIndex, targetIndex, dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed);
            }
            else
            {
                char[] currentChars = currentWord.ToCharArray();
                currentChars[targetIndex] = letterToMove;
                if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current) currentChars[sourceIndex] = ' ';
                else
                {
                    mixedWord = mixedWord.Remove(sourceIndex, 1);
                }
                currentWord = new string(currentChars);

                if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
                    NetworkManager.Instance.SendWordState(originalWord, currentWord, mixedWord, startCurrentWord, startMixedWord);
            }
        }

        substituteDisplay.GetComponentInChildren<Image>().enabled = false;
        substituteDisplay.gameObject.SetActive(false);
        substituteDisplay.transform.SetParent(mixedParent.parent);
        if (letterDisplay.GetComponent<LetterDisplay>().GetLetterType() == LetterDisplay.Type.Mixed) letterDisplay.transform.SetParent(mixedParent);
        else if (letterDisplay.GetComponent<LetterDisplay>().GetLetterType() == LetterDisplay.Type.Current) letterDisplay.transform.SetParent(currentParent);

        draggedLetterSourceIndex = -1;
        draggedLetterIsFromMixed = false;

        UpdateDisplay();
        CheckIfSolved();
    }


    public void CheckIfSolved()
    {
        if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer) return;

        bool ended = true;
        foreach (char c in currentWord)
        {
            if (c == ' ') ended = false;
        }

        if (ended)
        {
            if (currentWord == originalWord)
            {
                doneWords.Add(originalWord);
                UpdateDisplay();
                GameManager.Instance.CompleteWord();
                AudioManager.Instance.PlayAudio("soCorrecte");
                NetworkManager.Instance.SendText("SOLVED");
            }
            else
            {
                GameManager.Instance.FailWord();
                AudioManager.Instance.PlayAudio("soIncorrecte", 0.7f);
                NetworkManager.Instance.SendText("WRONG");
            }
        }
    }

    public void SetSolved(bool solved)
    {
        if (solved)
        {
            doneWords.Add(originalWord);
            GameManager.Instance.CompleteWord();
            ShowCorrect(true);
            AudioManager.Instance.PlayAudio("soCorrecte");
        }
        else
        {
            GameManager.Instance.FailWord();
            ShowCorrect(false);
            AudioManager.Instance.PlayAudio("soIncorrecte", 0.7f);
        }
    }

    public void ShowCorrect(bool correct)
    {
        for (int i = 0; i < currentLetterDisplays.Count; i++)
        {
            currentLetterDisplays[i].GetComponent<LetterDisplay>().ShowCorrect(correct);
        }
    }


    public string GetCurrentWord()
    {
        //RETURN CURRENT WORD
        return currentWord;
    }

    public string GetMixedCurrentWord()
    {
        //RETURN MIXED WORD
        return mixedWord;
    }
    
    public string GetOriginalWord()
    {
        //RETURN ORIGINAL WORD
        return originalWord;
    }

    public string GetStartCurrentWord()
    {
        //RETURN START CURRENT WORD
        return startCurrentWord;
    }
    
    public string GetStartMixedWord()
    {
        //RETURN START MIXED WORD
        return startMixedWord;
    }

    private LetterDisplay GetLetterDisplayInstance(int index, bool isMixed)
    {
        List<GameObject> targetList = isMixed ? mixedLetterDisplays : currentLetterDisplays;
        if (index >= 0 && index < targetList.Count)
        {
            return targetList[index].GetComponent<LetterDisplay>();
        }
        return null;
    }

    //MULTIPLAYER FUNCTIONS
    public void UpdateWordState(WordStateDTO wordStateDTO)
    {
        originalWord = wordStateDTO.originalWord;
        mixedWord = wordStateDTO.mixedWord;
        currentWord = wordStateDTO.currentWord;
        startMixedWord = wordStateDTO.startMixedWord;
        startCurrentWord = wordStateDTO.startCurrentWord;
        
        UpdateDisplay();
        HideHelp();
    }

    public void MoveLetter(LetterMoveDTO letterMoveDTO)
    {
        LetterDisplay letterDisplay = null;
        if (letterMoveDTO.isFromMixed)
        {
            for (int i = 0; i < mixedLetterDisplays.Count; i++)
            {
                LetterDisplay currentLetterDisplay = mixedLetterDisplays[i].GetComponent<LetterDisplay>();
                if (!currentLetterDisplay || currentLetterDisplay.letterIndex != letterMoveDTO.sourceIndex) continue;

                letterDisplay = currentLetterDisplay;
                break;
            }
        }
        else
        {
            for (int i = 0; i < currentLetterDisplays.Count; i++)
            {
                LetterDisplay currentLetterDisplay = currentLetterDisplays[i].GetComponent<LetterDisplay>();
                if (!currentLetterDisplay || currentLetterDisplay.letterIndex != letterMoveDTO.sourceIndex) continue;

                letterDisplay = currentLetterDisplay;
                break;
            }
        }
        
        if (!letterDisplay) return;

        char[] currentChars = currentWord.ToCharArray();
        
        if (currentChars[letterMoveDTO.targetIndex] == ' ')
        {
            currentChars[letterMoveDTO.targetIndex] = letterDisplay.GetLetter();
            if (!letterMoveDTO.isFromMixed) currentChars[letterMoveDTO.sourceIndex] = ' ';
            else
            {
                mixedWord = mixedWord.Remove(letterMoveDTO.sourceIndex, 1);
            }
            currentWord = new string(currentChars);
            letterDisplay.gameObject.SetActive(false);
            NetworkManager.Instance.SendWordState(originalWord, currentWord, mixedWord, startCurrentWord, startMixedWord);
            UpdateDisplay();
            HideHelp();
            CheckIfSolved();
        }
    }

    public void HandleRemoteDrag(LetterDragDTO letterDragDTO)
    {
        if (draggedLetterSourceIndex != -1 &&
            letterDragDTO.letterIndex == draggedLetterSourceIndex &&
            letterDragDTO.isFromMixed == draggedLetterIsFromMixed)
        {
            if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
                NetworkManager.Instance.SendDragCancel(letterDragDTO.letterIndex, letterDragDTO.isFromMixed);
            return;
        }

        LetterDisplay letter = GetLetterDisplayInstance(letterDragDTO.letterIndex, letterDragDTO.isFromMixed);
        if (letter == null) return;

        bool isNewDrag = remoteDraggedLetterSourceIndex != letterDragDTO.letterIndex ||
                         remoteDraggedLetterIsFromMixed != letterDragDTO.isFromMixed;

        if (isNewDrag)
        {
            EndRemoteDragVisual();

            remoteDraggedLetterSourceIndex = letterDragDTO.letterIndex;
            remoteDraggedLetterIsFromMixed = letterDragDTO.isFromMixed;

            int visualIndex = letter.transform.GetSiblingIndex();
            letter.transform.SetParent(mixedParent.parent);

            remoteSubstituteDisplay.GetComponentInChildren<Image>().enabled = !letterDragDTO.isFromMixed;
            remoteSubstituteDisplay.gameObject.SetActive(true);
            remoteSubstituteDisplay.SetParent(letterDragDTO.isFromMixed ? mixedParent : currentParent);
            remoteSubstituteDisplay.SetSiblingIndex(visualIndex);
        }

        letter.transform.SetAsLastSibling();
        letter.UpdateRemotePosition(new Vector2(letterDragDTO.x, letterDragDTO.y));
        letter.SetRemoteLock(true);

        lastRemoteDragTime = Time.time;
    }

    public void HandleRemoteDragEnd(LetterDragDTO letterDragDTO)
    {
        if (draggedLetterSourceIndex == letterDragDTO.letterIndex &&
        draggedLetterIsFromMixed == letterDragDTO.isFromMixed) return;

        EndRemoteDragVisual();

        LetterDisplay letter = GetLetterDisplayInstance(letterDragDTO.letterIndex, letterDragDTO.isFromMixed);
        if (letter != null)
        {
            letter.SetRemoteLock(false);
            letter.transform.SetParent(letterDragDTO.isFromMixed ? mixedParent : currentParent);
            letter.transform.SetSiblingIndex(letter.letterIndex);
        }
    }

    public void HandleDragCancel(LetterDragDTO dto)
    {
        if (draggedLetterSourceIndex != dto.letterIndex ||
            draggedLetterIsFromMixed != dto.isFromMixed) return;

        LetterDisplay letter = GetLetterDisplayInstance(dto.letterIndex, dto.isFromMixed);
        if (letter == null) return;

        letter.CancelDrag();

        substituteDisplay.GetComponentInChildren<Image>().enabled = false;
        substituteDisplay.gameObject.SetActive(false);
        substituteDisplay.SetParent(mixedParent.parent);
        letter.transform.SetParent(dto.isFromMixed ? mixedParent : currentParent);

        draggedLetterSourceIndex = -1;
        draggedLetterIsFromMixed = false;

        UpdateDisplay();
    }

    private void EndRemoteDragVisual()
    {
        if (remoteDraggedLetterSourceIndex == -1) return;

        remoteSubstituteDisplay.GetComponentInChildren<Image>().enabled = false;
        remoteSubstituteDisplay.gameObject.SetActive(false);
        remoteSubstituteDisplay.SetParent(mixedParent.parent);

        LetterDisplay letter = GetLetterDisplayInstance(remoteDraggedLetterSourceIndex, remoteDraggedLetterIsFromMixed);
        if (letter != null)
        {
            letter.SetRemoteLock(false);
            letter.transform.SetParent(remoteDraggedLetterIsFromMixed ? mixedParent : currentParent);
        }

        remoteDraggedLetterSourceIndex = -1;
        remoteDraggedLetterIsFromMixed = false;
    }
}