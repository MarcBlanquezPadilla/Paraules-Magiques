using UnityEngine;
using System.IO;
using System.Xml.Serialization;
using System;

[System.Serializable]
public class GameSaveData
{
    public int mouses;
    public int difficulty;
    public int mode;
    public int mouseNum;
    public int wordsNum;
    public bool firstTime;

    public GameSaveData() { }

    public GameSaveData(bool esLaPrimeraVez)
    {
        mouses = 0;
        difficulty = 0;
        mode = 0;
        mouseNum = 0;
        wordsNum = 0;
        firstTime = esLaPrimeraVez;
    }
}


public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public GameSaveData data;

    private string saveFilePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);

            saveFilePath = Path.Combine(Application.persistentDataPath, "gameData.xml");

            LoadData();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void LoadData()
    {
        if (File.Exists(saveFilePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
            using (FileStream stream = new FileStream(saveFilePath, FileMode.Open))
            {
                data = serializer.Deserialize(stream) as GameSaveData;
            }
            Debug.Log("Datos cargados desde: " + saveFilePath);
        }
        else
        {
            
            Debug.Log("No se encontró archivo de guardado. Creando datos nuevos.");
            data = new GameSaveData(true);
            SaveData(); 
        }
    }

    public void SaveData()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(GameSaveData));
        using (FileStream stream = new FileStream(saveFilePath, FileMode.Create)) 
        {
            serializer.Serialize(stream, data);
        }
        Debug.Log("Datos guardados en: " + saveFilePath);
    }

    public void ChangeDifficulty(int i)
    {
        data.difficulty = i;
        Debug.Log("Diff"+ i);
    }

    public void ChangeMode(int i)
    {
        data.mode = i;
        Debug.Log("Mode" + i);
    }
}