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
    [SerializeField] private string currentWord;
    [SerializeField] private string mixedWord;

    [SerializeField] private List<GameObject> currentLetterDisplays;
    [SerializeField] private List<GameObject> mixedLetterDisplays;
    [SerializeField] private Transform currentParent;
    [SerializeField] private Transform mixedParent;
    [SerializeField] private Transform substituteDisplay;


    private void Awake()
    {
        //INIT WORDS
        LoadWords();
        LoadLetterDisplays();
    }

    private void Start()
    {
        ChangeWord();
        UpdateDisplay();
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

    public void ChangeWord()
    {
        //GET NEW WORD
        originalWord = wordsList[UnityEngine.Random.Range(0, wordsList.Count)];
        currentWord = "";
        mixedWord = "";


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

        for (int i = 0;i < originalWord.Length; i++)
        {
            if (hideLetter[i])
            {
                mixedWord += originalWord[i];
                currentWord += " ";
            }
                
            else currentWord += originalWord[i];
        }

        //MIX NEW WORD
        mixedWord = mixedWord.ToCharArray().OrderBy(c => UnityEngine.Random.value).ToArray().ArrayToString();
    }

    private void UpdateDisplay()
    {
        foreach (GameObject g in mixedLetterDisplays)
        {
            g.SetActive(false);
        }

        foreach (char c in mixedWord)
        {
            for (int i = 0; i < mixedLetterDisplays.Count; i++)
            {
                if (!mixedLetterDisplays[i].activeInHierarchy)
                {
                    mixedLetterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(c);
                    mixedLetterDisplays[i].SetActive(true);
                    break;
                }
            }
        }

        foreach (GameObject g in currentLetterDisplays)
        {
            g.SetActive(false);
        }

        foreach (char c in currentWord)
        {
            for (int i = 0; i < currentLetterDisplays.Count; i++)
            {
                if (!currentLetterDisplays[i].activeInHierarchy)
                {
                    currentLetterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(c);
                    currentLetterDisplays[i].SetActive(true);
                    break;
                }
            }
        }
    }

    public void OnStartDragLetter(GameObject letterDisplay)
    {
        int letterDisplayIndex = letterDisplay.transform.GetSiblingIndex();

        letterDisplay.transform.SetParent(mixedParent.parent);
        substituteDisplay.gameObject.SetActive(true);
        substituteDisplay.transform.SetParent(mixedParent);
        substituteDisplay.transform.SetSiblingIndex(letterDisplayIndex);
    }

    public void OnEndDragLetter(GameObject letterDisplay)
    {
        bool found = false;
        int i;
        for (i = 0; i < currentLetterDisplays.Count; i++)
        {
            if (currentLetterDisplays[i].GetComponent<LetterDisplay>().GetLetter() == ' ')
            {
                float minDistance = currentLetterDisplays[i].GetComponent<RectTransform>().sizeDelta.x / 2;

                float distance = Vector2.Distance(currentLetterDisplays[i].transform.position, letterDisplay.transform.position);
                if (distance < minDistance) 
                {
 
                    found = true;
                    break;
                }
            }
        }
        if (found)
        {
            currentLetterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(letterDisplay.GetComponent<LetterDisplay>().GetLetter());
            substituteDisplay.gameObject.SetActive(false);
            substituteDisplay.transform.SetParent(mixedParent.parent);
            letterDisplay.SetActive(false);
            letterDisplay.transform.SetParent(mixedParent);
        }
        else
        {
            int letterDisplayIndex = substituteDisplay.transform.GetSiblingIndex();

            substituteDisplay.gameObject.SetActive(false);
            substituteDisplay.transform.SetParent(mixedParent.parent);
            letterDisplay.transform.SetParent(mixedParent);
            letterDisplay.transform.SetSiblingIndex(letterDisplayIndex);
        }
    }
    public string GetCurrentWord()
    {
        //RETURN CURRENT WORD
        return originalWord;
    }

    public string GetMixedCurrentWord()
    {
        //RETURN MIXED WORD
        return mixedWord;
    }

}
