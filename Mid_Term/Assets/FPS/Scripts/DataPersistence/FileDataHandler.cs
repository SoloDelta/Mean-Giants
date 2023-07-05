using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FileDataHandler 
{
    private string dataDirPath = "";
    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName) 
    { 
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // use Path.Combine to account for different OS having different path seperators.
        string fullPath = Path.Combine(dataDirPath, dataFileName);
        GameData loadedData = null;
        if(File.Exists(fullPath))
        {

            try
            {
                //load serialized data from file
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using(StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                //deserializer from json to c# gameObjects
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);

            }
            catch (Exception e)
            {
                Debug.LogError("Error when trying to load data from file: " + fullPath + "\n" + e);
            }

        }
        return loadedData;

    }

    public void Save(GameData data)
    {
        // use Path.Combine to account for different OS having different path seperators.
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // create path if it doesn't exist already
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            //serialize our c# to a json
            string dataToStore = JsonUtility.ToJson(data, true);

            // write to the file
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter sWriter = new StreamWriter(stream))
                {
                    sWriter.WriteLine(dataToStore);
                }
            }

        }
        catch(Exception e)
        {
            Debug.LogError("Error when trying to save data to file: " + fullPath + "\n" + e);
        }


    }


}
