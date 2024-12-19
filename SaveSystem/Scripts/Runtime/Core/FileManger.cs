using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SaveSystem.Scripts.Runtime.Core
{
    public class FileManager
    {
        public static void SaveToBinaryFile(string path, object data)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(path, FileMode.Create);

            try
            {
                formatter.Serialize(file, data);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to save file at {path}");
            }
            finally
            {
                file.Close();
            }
        }

        public static void LoadFromBinaryFile(string path, out object data)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream file = File.Open(path, FileMode.Open);

            try
            {
                data = formatter.Deserialize(file);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to load file at {path}");
                data = new object();
            }
            finally
            {
                file.Close();
            }
        }
    }
}