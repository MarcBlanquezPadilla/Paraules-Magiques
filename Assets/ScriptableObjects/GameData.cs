using UnityEngine;
using static GameManager;

[CreateAssetMenu(fileName = "GameData", menuName = "Scriptable Objects/GameData")]
public class GameData : ScriptableObject
{
    // Pon aquí los datos que quieres pasar
    public int mouses;
    public int difficulty;
    public int mode;
    public int mouseNum;
    public int wordsNum;
    public bool firstTime;

    // Importante: Añade una función para resetear al empezar el juego
    public void Reset()
    {
        mouses = 0;
        difficulty = 0;
        mode = 0;
        firstTime = true;
    }

    public void ChangeDifficulty(int d)
    {
        difficulty = d;
    }

    public void ChangeMode(int m)
    {
        mode = m;
    }

    public void ChangeFirstTime(bool b)
    {
        firstTime = b;
    }

    public void SumMouse()
    {
        mouses += mouseNum;
    }

    public void ChangeMouseNum(int mN)
    {
        mouseNum = mN;
    }
}
