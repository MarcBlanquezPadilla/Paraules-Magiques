using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
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
        }
        foreach (Transform letter in mixedParent)
        {
            mixedLetterDisplays.Add(letter.gameObject);
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
        draggedLetterSourceIndex = mixedLetterDisplays.IndexOf(letterDisplay);

        // 2. Comprobación de seguridad
        // Si no lo encontramos, o el índice es mayor que las letras que tenemos, es un error.
        if (draggedLetterSourceIndex == -1 || draggedLetterSourceIndex >= mixedWord.Length)
        {
            Debug.LogError($"Error al arrastrar: Índice {draggedLetterSourceIndex} fuera de rango para mixedWord '{mixedWord}'.");
            draggedLetterSourceIndex = -1; // Invalidamos este drag
            return; // No hacemos nada más
        }

        // 3. Lógica visual (como estaba antes)
        // Usamos el índice visual (SiblingIndex) SÓLO para colocar el placeholder
        int visualIndex = letterDisplay.transform.GetSiblingIndex();

        letterDisplay.transform.SetParent(mixedParent.parent);
        substituteDisplay.gameObject.SetActive(true);
        substituteDisplay.transform.SetParent(mixedParent);
        substituteDisplay.transform.SetSiblingIndex(visualIndex);
    }

    // --- ¡FUNCIÓN MODIFICADA! ---
    public void OnEndDragLetter(GameObject letterDisplay)
    {
        // 1. Comprobar si el drag era válido
        if (draggedLetterSourceIndex == -1)
        {
            // Este drag se marcó como inválido en OnStartDrag
            // Limpiamos la UI y salimos
            substituteDisplay.gameObject.SetActive(false);
            substituteDisplay.transform.SetParent(mixedParent.parent);
            letterDisplay.transform.SetParent(mixedParent);
            UpdateDisplay(); // Para que todo vuelva a su sitio
            return;
        }

        bool foundDropTarget = false;
        int targetIndex = -1; // El índice del hueco (' ') en currentWord

        // 2. Usamos el ÍNDICE DE DATOS que guardamos
        int sourceIndex = draggedLetterSourceIndex;
        char letterToMove = mixedWord[sourceIndex]; // Obtenemos la letra correcta desde el string

        // 3. Encontrar un hueco válido para soltar (basado en distancia)
        // Añadimos comprobación 'i < currentWord.Length' por si acaso
        for (int i = 0; i < currentLetterDisplays.Count && i < currentWord.Length; i++)
        {
            if (currentWord[i] == ' ')
            {
                float minDistance = currentLetterDisplays[i].GetComponent<RectTransform>().sizeDelta.x / 2;
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
            // Modificamos los strings (el "modelo de datos")

            // Añadir letra a currentWord
            char[] currentChars = currentWord.ToCharArray();
            currentChars[targetIndex] = letterToMove;
            currentWord = new string(currentChars);

            // Quitar letra de mixedWord
            // Esta comprobación ahora debería ser siempre true, pero la dejamos por seguridad.
            if (sourceIndex >= 0 && sourceIndex < mixedWord.Length)
            {
                mixedWord = mixedWord.Remove(sourceIndex, 1);
            }
            else
            {
                Debug.LogError($"Error CRÍTICO: sourceIndex {sourceIndex} fuera de rango para mixedWord '{mixedWord}' al soltar.");
            }

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
        substituteDisplay.gameObject.SetActive(false);
        substituteDisplay.transform.SetParent(mixedParent.parent);
        letterDisplay.transform.SetParent(mixedParent); // La devolvemos al pool

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
                GameManager.Instance.CompleteWord();
            }
            else
            {
                GameManager.Instance.FailWord();
            }
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