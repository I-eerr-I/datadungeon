using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public static bool Save(PlayerController playerController)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save";
        FileStream stream = new FileStream(path, FileMode.Create);
        SaveData data = new SaveData(playerController);
        formatter.Serialize(stream, data);
        stream.Close();
        return true;
    }

    public static SaveData Load()
    {
        string path = Application.persistentDataPath + "/save";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
            return data;
        }
        return null;
    }
}
