using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
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
        if (Keyboard.current != null &&
    Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            DebugCancelDrag();
        }


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
    }
    private void DebugCancelDrag()
    {
        if (draggedLetterSourceIndex == -1)
        {
            return;
        }

        LetterDragDTO dto = new LetterDragDTO
        {
            letterIndex = draggedLetterSourceIndex,
            isFromMixed = draggedLetterIsFromMixed
        };

        HandleDragCancel(dto);
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
        showingHelp = false;
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
            SendState();
        }
    }

    public void ReloadWord()
    {
        if (NetworkManager.Instance.isMultiplayer && !NetworkManager.Instance.isServer) return;
        currentWord = startCurrentWord;
        mixedWord = startMixedWord;
        UpdateDisplay();
        if (NetworkManager.Instance.isMultiplayer && NetworkManager.Instance.isServer)
            SendState();
    }

    private void UpdateDisplay()
    {
        // MIXED
        for (int i = 0; i < mixedLetterDisplays.Count; i++)
        {
            GameObject g = mixedLetterDisplays[i];
            bool show = i < mixedWord.Length && mixedWord[i] != ' ';

            if (g.activeSelf != show) g.SetActive(show);
            if (!show) continue;

            LetterDisplay ld = g.GetComponent<LetterDisplay>();
            if (ld)
            {
                ld.ChangeLetter(mixedWord[i]);
                ld.letterIndex = i;
            }

            if (g.transform.parent == mixedParent) g.transform.SetSiblingIndex(i);
        }

        // CURRENT
        for (int i = 0; i < currentLetterDisplays.Count; i++)
        {
            GameObject g = currentLetterDisplays[i];
            bool show = i < currentWord.Length;

            if (g.activeSelf != show) g.SetActive(show);
            if (!show) continue;

            LetterDisplay ld = g.GetComponent<LetterDisplay>();
            if (i < startCurrentWord.Length && currentWord[i] == startCurrentWord[i])
                ld.ChangeState(LetterDisplay.State.Locked);
            else
                ld.ChangeState(LetterDisplay.State.Dragable);
            ld.ChangeLetter(currentWord[i]);
            ld.letterIndex = i;

            if (g.transform.parent == currentParent) g.transform.SetSiblingIndex(i);
        }
    }

    public void OnStartDragLetter(GameObject letterDisplay)
    {
        LetterDisplay letter = letterDisplay.GetComponent<LetterDisplay>();
        bool fromMixed = letter.GetLetterType() == LetterDisplay.Type.Mixed;

        draggedLetterSourceIndex = letter.letterIndex;
        draggedLetterIsFromMixed = fromMixed;

        Image substituteImage = substituteDisplay.GetComponentInChildren<Image>();
        if (substituteImage != null)
            substituteImage.enabled = !fromMixed;

        int visualIndex = letterDisplay.transform.GetSiblingIndex();

        letterDisplay.transform.SetParent(mixedParent.parent);

        substituteDisplay.gameObject.SetActive(true);
        substituteDisplay.SetParent(GetLetterParent(fromMixed));
        substituteDisplay.SetSiblingIndex(visualIndex);
    }

    public void OnEndDragLetter(GameObject letterDisplay)
    {

        if (draggedLetterSourceIndex == -1)
        {

            return;
        }

        LetterDisplay letter = letterDisplay.GetComponent<LetterDisplay>();

        int sourceIndex = draggedLetterSourceIndex;
        bool fromMixed = draggedLetterIsFromMixed;
        char letterToMove = letter.GetLetter();

        int targetIndex = -1;

        if (IsSourceStillValid(sourceIndex, fromMixed))
        {
            for (int i = 0; i < currentLetterDisplays.Count && i < currentWord.Length; i++)
            {
                if (currentWord[i] != ' ')
                    continue;

                float distance = Vector2.Distance(currentLetterDisplays[i].transform.position, letterDisplay.transform.position);

                if (distance < 75f)
                {
                    targetIndex = i;
                    break;
                }
            }
        }

        if (targetIndex != -1)
        {
            AudioManager.Instance.PlayAudio("soLletres");
            letterDisplay.SetActive(false);
            HideHelp();

            char[] currentChars = currentWord.ToCharArray();
            currentChars[targetIndex] = letterToMove;

            if (fromMixed)
                ConsumeMixedLetter(sourceIndex);
            else
                currentChars[sourceIndex] = ' ';

            currentWord = new string(currentChars);

            if (NetworkManager.Instance.isMultiplayer)
            {
                if (NetworkManager.Instance.isServer)
                    SendState();
                else
                    NetworkManager.Instance.SendLetterMove(sourceIndex, targetIndex, fromMixed);
            }
        }


        letter.transform.SetParent(GetLetterParent(fromMixed));

        ClearLocalDrag();

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

    private void ConsumeMixedLetter(int index)
    {
        char[] chars = mixedWord.ToCharArray();
        chars[index] = ' ';
        mixedWord = new string(chars);
    }

    private bool IsSourceStillValid(int index, bool fromMixed)
    {
        string w = fromMixed ? mixedWord : currentWord;
        return w != null && index >= 0 && index < w.Length && w[index] != ' ';
    }
    private Transform GetLetterParent(bool isFromMixed)
    {
        return isFromMixed ? mixedParent : currentParent;
    }

    private void RestoreLetter(LetterDisplay letter, bool isFromMixed)
    {
        if (letter == null) return;

        letter.transform.SetParent(GetLetterParent(isFromMixed));
        letter.transform.SetSiblingIndex(letter.letterIndex);
    }

    private void HideSubstitute(Transform substitute)
    {
        if (substitute == null) return;

        Image image = substitute.GetComponentInChildren<Image>();
        if (image != null) image.enabled = false;

        substitute.gameObject.SetActive(false);
        substitute.SetParent(mixedParent.parent);
    }

    //MULTIPLAYER FUNCTIONS

    private void SendState()
    {
        NetworkManager.Instance.SendWordState(originalWord, currentWord, mixedWord, startCurrentWord, startMixedWord);
    }

    public void UpdateWordState(WordStateDTO wordStateDTO)
    {
        originalWord = wordStateDTO.originalWord;
        mixedWord = wordStateDTO.mixedWord;
        currentWord = wordStateDTO.currentWord;
        startMixedWord = wordStateDTO.startMixedWord;
        startCurrentWord = wordStateDTO.startCurrentWord;

        if (remoteDraggedLetterSourceIndex != -1 &&
            !IsSourceStillValid(remoteDraggedLetterSourceIndex, remoteDraggedLetterIsFromMixed))
        {
            EndRemoteDragVisual();
        }

        UpdateDisplay();
        HideHelp();
    }

    public void MoveLetter(LetterMoveDTO dto)
    {
        bool invalid =
            dto.targetIndex < 0 || dto.targetIndex >= currentWord.Length ||
            dto.sourceIndex < 0 ||
            !IsSourceStillValid(dto.sourceIndex, dto.isFromMixed) ||
            currentWord[dto.targetIndex] != ' ';

        if (invalid)
        {
            SendState();
            return;
        }

        char letterToMove = dto.isFromMixed ? mixedWord[dto.sourceIndex] : currentWord[dto.sourceIndex];

        char[] currentChars = currentWord.ToCharArray();
        currentChars[dto.targetIndex] = letterToMove;
        if (!dto.isFromMixed) currentChars[dto.sourceIndex] = ' ';
        else ConsumeMixedLetter(dto.sourceIndex);
        currentWord = new string(currentChars);

        if (remoteDraggedLetterSourceIndex != -1 &&
            !IsSourceStillValid(remoteDraggedLetterSourceIndex, remoteDraggedLetterIsFromMixed))
        {
            EndRemoteDragVisual();
        }

        SendState();
        UpdateDisplay();
        HideHelp();
        CheckIfSolved();
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

        if (!IsSourceStillValid(letterDragDTO.letterIndex, letterDragDTO.isFromMixed)) return;

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
    }

    public void HandleRemoteDragEnd(LetterDragDTO dto)
    {
        LetterDisplay letter = GetLetterDisplayInstance(dto.letterIndex, dto.isFromMixed);

        bool isMyLocalDrag = draggedLetterSourceIndex == dto.letterIndex && draggedLetterIsFromMixed == dto.isFromMixed;

        if (isMyLocalDrag)
        {
            if (letter != null)
                letter.SetRemoteLock(false);

            return;
        }

        EndRemoteDragVisual();

        if (letter != null)
        {
            letter.SetRemoteLock(false);
            RestoreLetter(letter, dto.isFromMixed);
        }
    }


    public void HandleDragCancel(LetterDragDTO dto)
    {
        bool isMyLocalDrag = draggedLetterSourceIndex == dto.letterIndex && draggedLetterIsFromMixed == dto.isFromMixed;

        if (!isMyLocalDrag)
            return;

        LetterDisplay letter = GetLetterDisplayInstance(dto.letterIndex, dto.isFromMixed);

        if (letter != null)
        {
            letter.CancelLocalDrag();
            RestoreLetter(letter, dto.isFromMixed);
        }

        ClearLocalDrag();

        UpdateDisplay();
    }


    private void EndRemoteDragVisual()
    {
        if (remoteDraggedLetterSourceIndex == -1) return;

        LetterDisplay letter = GetLetterDisplayInstance(remoteDraggedLetterSourceIndex, remoteDraggedLetterIsFromMixed);

        HideSubstitute(remoteSubstituteDisplay);

        if (letter != null)
        {
            letter.SetRemoteLock(false);
            RestoreLetter(letter, remoteDraggedLetterIsFromMixed);
        }

        remoteDraggedLetterSourceIndex = -1;
        remoteDraggedLetterIsFromMixed = false;
    }

    public void CancelLocalDragFromDisplay(GameObject letterDisplay)
    {
        if (draggedLetterSourceIndex == -1)
            return;

        LetterDisplay letter = letterDisplay.GetComponent<LetterDisplay>();

        if (letter == null)
            return;

        bool fromMixed = letter.GetLetterType() == LetterDisplay.Type.Mixed;

        if (letter.letterIndex != draggedLetterSourceIndex ||
            fromMixed != draggedLetterIsFromMixed)
        {
            return;
        }

        ClearLocalDrag();
    }

    private void ClearLocalDrag()
    {
        HideSubstitute(substituteDisplay);

        draggedLetterSourceIndex = -1;
        draggedLetterIsFromMixed = false;
    }
}