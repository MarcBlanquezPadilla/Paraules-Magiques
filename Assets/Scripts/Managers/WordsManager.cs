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
    }

    public void ReloadWord()
    {
        currentWord = startCurrentWord;
        mixedWord = startMixedWord;
        UpdateDisplay ();
    }

    private void UpdateDisplay()
    {
        // --- Actualizar MixedLetterDisplays ---

        // 1. Desactivarlos todos primero
        foreach (GameObject g in mixedLetterDisplays)
        {
            g.SetActive(false);
        }

        // 2. Activar y asignar letra según el string mixedWord
        int lettersToDisplay = Mathf.Min(mixedWord.Length, mixedLetterDisplays.Count);
        for (int i = 0; i < lettersToDisplay; i++)
        {
            mixedLetterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(mixedWord[i]);
            mixedLetterDisplays[i].SetActive(true);

            // --- ¡AÑADE ESTA LÍNEA! ---
            // Forzamos el orden visual para que coincida con el orden de los datos
            mixedLetterDisplays[i].transform.SetSiblingIndex(i);
        }

        // --- Actualizar CurrentLetterDisplays ---

        // 1. Desactivarlos todos primero
        foreach (GameObject g in currentLetterDisplays)
        {
            g.SetActive(false);
        }

        // 2. Activar y asignar letra según el string currentWord
        lettersToDisplay = Mathf.Min(currentWord.Length, currentLetterDisplays.Count);
        for (int i = 0; i < lettersToDisplay; i++)
        {
            if (currentWord[i] == startCurrentWord[i]) currentLetterDisplays[i].GetComponent<LetterDisplay>().ChangeState(LetterDisplay.State.Locked);
            else currentLetterDisplays[i].GetComponent<LetterDisplay>().ChangeState(LetterDisplay.State.Dragable);
            currentLetterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(currentWord[i]);
            currentLetterDisplays[i].SetActive(true);

            // --- ¡AÑADE ESTA LÍNEA TAMBIÉN! ---
            // (Por consistencia y para evitar bugs futuros)
            currentLetterDisplays[i].transform.SetSiblingIndex(i);
        }
    }

    public void OnStartDragLetter(GameObject letterDisplay)
    {
        // 1. Encontrar el ÍNDICE DE DATOS
        // Buscamos el objeto que arrastramos en nuestra lista de displays
        LetterDisplay dragLetterDisplay = letterDisplay.GetComponent<LetterDisplay>();

        if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed) draggedLetterSourceIndex = mixedLetterDisplays.IndexOf(letterDisplay);
        else if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current)
        {
            substituteDisplay.GetComponentInChildren<Image>().enabled = true;
            draggedLetterSourceIndex = currentLetterDisplays.IndexOf(letterDisplay);
        }
            

        int visualIndex = letterDisplay.transform.GetSiblingIndex();

        letterDisplay.transform.SetParent(mixedParent.parent);
        substituteDisplay.gameObject.SetActive(true);
        if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed) substituteDisplay.transform.SetParent(mixedParent);
        else if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current) substituteDisplay.transform.SetParent(currentParent);
        substituteDisplay.transform.SetSiblingIndex(visualIndex);
    }

    // --- ¡FUNCIÓN MODIFICADA! ---
    public void OnEndDragLetter(GameObject letterDisplay)
    {
        LetterDisplay dragLetterDisplay = letterDisplay.GetComponent<LetterDisplay>();

        bool foundDropTarget = false;
        int targetIndex = -1;

        int sourceIndex = draggedLetterSourceIndex;
        char letterToMove = ' ';

        if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Mixed) letterToMove = mixedWord[sourceIndex];
        else if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current) letterToMove = currentWord[sourceIndex];

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

        // 4. Si encontramos un hueco...
        if (foundDropTarget)
        {
            AudioManager.Instance.PlayAudio("soLletres");
            // Modificamos los strings (el "modelo de datos")

            // Añadir letra a currentWord
            char[] currentChars = currentWord.ToCharArray();
            currentChars[targetIndex] = letterToMove;
            if (dragLetterDisplay.GetLetterType() == LetterDisplay.Type.Current) currentChars[sourceIndex] = ' ';
            else
            {
                mixedWord = mixedWord.Remove(sourceIndex, 1);
            }
            currentWord = new string(currentChars);
            
            // Limpiar la UI del drag
            letterDisplay.SetActive(false); // Ya no la necesitamos, UpdateDisplay creará las nuevas
            HideHelp();
        }
        else
        {
            // Si no encontramos hueco, no hacemos nada a los strings
            // La letra volverá a su sitio en el UpdateDisplay()
        }

        // 5. Limpiamos la UI del drag (tanto si acierta como si falla)
        substituteDisplay.GetComponentInChildren<Image>().enabled = false;
        substituteDisplay.gameObject.SetActive(false);
        substituteDisplay.transform.SetParent(mixedParent.parent);
        if (letterDisplay.GetComponent<LetterDisplay>().GetLetterType() == LetterDisplay.Type.Mixed) letterDisplay.transform.SetParent(mixedParent); // La devolvemos al pool
        else if (letterDisplay.GetComponent<LetterDisplay>().GetLetterType() == LetterDisplay.Type.Current) letterDisplay.transform.SetParent(currentParent); // La devolvemos al pool

        // 6. Resetear el índice de drag
        draggedLetterSourceIndex = -1;

        // 7. Actualizar TODA la UI basada en los strings (la fuente de la verdad)
        UpdateDisplay();

        // 8. Comprobar si se ha resuelto
        CheckIfSolved();
       
    }


    public void CheckIfSolved()
    {
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
            }
            else
            {
                GameManager.Instance.FailWord();
                AudioManager.Instance.PlayAudio("soIncorrecte", 0.7f);
            }
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
}