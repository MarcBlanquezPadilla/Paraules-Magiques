using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using static GameManager;

public class WordsManager : MonoBehaviour
{
    [SerializeField] private TextAsset wordsTextAsset;
    [SerializeField] private List<string> wordsList = new List<string>();

    [SerializeField] private string originalWord;
    [SerializeField] private string currentWord;
    [SerializeField] private string mixedCurrentWord;


    [SerializeField] private WordDisplayController _mixedWordDisplayController;
    [SerializeField] private WordDisplayController _currentWordDisplayController;


    private void Awake()
    {
        //INIT WORDS
        LoadWords();
    }

    private void Start()
    {
        ChangeWord();
        _currentWordDisplayController.ShowWord(currentWord);
        _mixedWordDisplayController.ShowWord(mixedCurrentWord);
    }

    void LoadWords()
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

    public void ChangeWord()
    {
        //GET NEW WORD
        originalWord = wordsList[UnityEngine.Random.Range(0, wordsList.Count)];
        currentWord = "";
        mixedCurrentWord = "";


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
                mixedCurrentWord += originalWord[i];
                currentWord += " ";
            }
                
            else currentWord += originalWord[i];
        }

        //MIX NEW WORD
        mixedCurrentWord = mixedCurrentWord.ToCharArray().OrderBy(c => UnityEngine.Random.value).ToArray().ArrayToString();
    }

    public string GetCurrentWord()
    {
        //RETURN CURRENT WORD
        return originalWord;
    }

    public string GetMixedCurrentWord()
    {
        //RETURN MIXED WORD
        return mixedCurrentWord;
    }

}
