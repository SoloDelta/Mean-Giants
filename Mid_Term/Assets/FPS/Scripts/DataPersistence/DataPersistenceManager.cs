using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage")]
    [SerializeField] private string fileName;



    public static DataPersistenceManager Instance { get; private set; }

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    public void Awake()
    {
        if(Instance != null)
        {
            Debug.LogError("More than one DataPersistenceManager in the scene!");
        }
        Instance = this;
    }

    public void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        //todo load any saved data from the file handler
        this.gameData = dataHandler.Load();
        //if no data to load
        if(this.gameData == null)
        {
            Debug.Log("No Saved Game Found.");
        }
        //todo push loaded data to other scripts that need it
        foreach(IDataPersistence dataObject in dataPersistenceObjects)
        {
            dataObject.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        //pass data to other scripts to update
        foreach (IDataPersistence dataObject in dataPersistenceObjects)
        {
            dataObject.SaveData(ref gameData);
        }

        //save data to a file using FileDataHandler
        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }

}
