using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordDisplayController : MonoBehaviour
{
    [SerializeField] private List<GameObject> letterDisplays;


    public void ShowWord(string word)
    {
        foreach (GameObject g in letterDisplays) 
        {
            g.SetActive(false);
        }

        foreach (char c in word)
        {
            for (int i = 0; i < letterDisplays.Count; i++)
            {
                if (!letterDisplays[i].activeInHierarchy)
                {
                    letterDisplays[i].GetComponent<LetterDisplay>().ChangeLetter(c);
                    letterDisplays[i].SetActive(true);
                    break;
                }
            }
        }
    }
}
