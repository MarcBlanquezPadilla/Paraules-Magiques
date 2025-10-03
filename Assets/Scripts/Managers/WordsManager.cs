using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class WordsManager : MonoBehaviour
{
    [SerializeField] private TextAsset wordsTextAsset;
    [SerializeField] private List<string> wordsList = new List<string>();

    [SerializeField] private string currentWord;
    [SerializeField] private string mixedCurrentWord;
    
    [SerializeField] private WordDisplayManager _wordDisplayManager;


    private void Awake()
    {
        _wordDisplayManager = FindFirstObjectByType<WordDisplayManager>();

        //INIT WORDS
        LoadWords();
    }

    private void Start()
    {
        ChangeWord();
        _wordDisplayManager.ShowWord(mixedCurrentWord);
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
        currentWord = wordsList[UnityEngine.Random.Range(0, wordsList.Count)];

        //MIX NEW WORD
        mixedCurrentWord = currentWord.ToCharArray().OrderBy(c => UnityEngine.Random.value).ToArray().ArrayToString();
    }

    public string GetCurrentWord()
    {
        //RETURN CURRENT WORD
        return currentWord;
    }

    public string GetMixedCurrentWord()
    {
        //RETURN MIXED WORD
        return mixedCurrentWord;
    }

}
