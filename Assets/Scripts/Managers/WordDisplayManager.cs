using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordDisplayManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> letterDisplays;
    [SerializeField] private GameObject letterSubstitute;

    public void ShowWord(string mixedWord)
    {
        foreach(char c in mixedWord) 
        {
            GameObject letterDisplay = PoolingManager.Instance.GetPooledObject("LetterDisplay");
            if(letterDisplay) {
                letterDisplay.GetComponent<LetterDisplay>().ChangeLetter(c);
                letterDisplay.SetActive(true);
                letterDisplays.Add(letterDisplay);

            }
        }
    }
}
