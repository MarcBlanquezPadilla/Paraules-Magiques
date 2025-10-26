using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnviromentBehaviour : MonoBehaviour
{
    [SerializeField] private  List<Sprite> backgroundList;
    [SerializeField] private List<Sprite> woodList;
    
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Image wood;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (backgroundList.Count > 0)
        {
            int randomBackgroun = UnityEngine.Random.Range(0, backgroundList.Count);
            background.sprite = backgroundList[randomBackgroun]; ;
            wood.sprite = woodList[randomBackgroun];
        }
        
    }
}
