using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader //reads the csv files and puts the output in the game data. This happens for each of the csv files in the game
{

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void OnBeforeSceneLoadRuntimeMethod() //makes the method happen on start of any scene
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "characters.csv");
        List<string[]> csvInformation = new List<string[]>();
        StreamReader strReader = new StreamReader(filePath); //gets csv file
        bool endOfFile = false;
        while (!endOfFile)
        {
            string dataString = strReader.ReadLine(); //reads csv
            if (dataString == null) //ends when the file is empty
            {
                endOfFile = true;
                break;
            }
            var dataValues = dataString.Split(","); //splits up csv
            csvInformation.Add(dataValues);
        }
        PublicData.charactersInfo = csvInformation; //sets the information in public data for public use in the game
        filePath = Path.Combine(Application.streamingAssetsPath, "names.csv");
        csvInformation = new List<string[]>();
        strReader = new StreamReader(filePath); //gets csv file
        endOfFile = false;
        while (!endOfFile)
        {
            string dataString = strReader.ReadLine(); //reads csv
            if (dataString == null) //ends when the file is empty
            {
                endOfFile = true;
                break;
            }
            var dataValues = dataString.Split(","); //splits up csv
            csvInformation.Add(dataValues);
        }
        PublicData.namesInfo = csvInformation; //sets the information in public data for public use in the game
        filePath = Path.Combine(Application.streamingAssetsPath, "records.csv");
        csvInformation = new List<string[]>();
        strReader = new StreamReader(filePath); //gets csv file
        endOfFile = false;
        while (!endOfFile)
        {
            string dataString = strReader.ReadLine(); //reads csv
            if (dataString == null) //ends when the file is empty
            {
                endOfFile = true;
                break;
            }
            var dataValues = dataString.Split(","); //splits up csv
            csvInformation.Add(dataValues);
        }
        PublicData.recordsInfo = csvInformation; //sets the information in public data for public use in the game
    }


    
}
