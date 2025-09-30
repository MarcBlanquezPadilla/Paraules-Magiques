using UnityEngine;
using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class WordsManager : MonoBehaviour
{
    #region Instance

    private static WordsManager _instance;
    public static WordsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<WordsManager>();
            }
            return _instance;
        }
    }

    #endregion


    [SerializeField] private TextAsset wordsTextAsset;
    [SerializeField] private List<string> wordsList = new List<string>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    private void Awake()
    {
        LoadWords();
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

    public string GetRandomWord()
    {
        string word = wordsList[UnityEngine.Random.Range(0, wordsList.Count)];
        return word;
    }
}
