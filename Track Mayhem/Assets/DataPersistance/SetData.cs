using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetData: MonoBehaviour, IDataPersistance
{
    public void SaveData(ref GameData data)
    {
        data = PublicData.gameData;
    }

    public void LoadData(GameData data)
    {
        if (PublicData.gameData == null)
        {
            PublicData.gameData = data;
        }
    }
}
