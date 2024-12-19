using System;
using System.Collections.Generic;
using System.IO;
using SaveSystem.Scripts.Runtime.Core;
using SaveSystem.Scripts.Runtime.Data;
using UnityEngine;

namespace SaveSystem.Scripts.Runtime
{
    [CreateAssetMenu(fileName = "GameData", menuName = "SaveSystem/GameData", order = 0)]
    public class GameData : ScriptableObject
    {
        [SerializeField] private string m_FileName;
        [NonSerialized] private bool isDataLoaded;
        [SerializeField, HideInInspector] private string m_Path;
        private Dictionary<string, object> m_Data = new Dictionary<string, object>();
        public bool hasPreviousSave => File.Exists(m_Path);

        [ContextMenu("Delete Save")]
        private void DeleteSave()
        {
            if (hasPreviousSave)
            {
                File.Delete(m_Path);
            }
        }

        public void Save(ISavable<SceneData> scene)
        {
            SceneData data = scene.data;
            m_Data[nameof(SceneData)] = data;
        }

        public void Load(ISavable<SceneData> scene)
        {
            if (isDataLoaded)
            {
                scene.Load(m_Data[nameof(SceneData)] as SceneData);
            }
            else
            {
                Debug.LogWarning("Data have not been loaded!");
            }
        }

        public void LoadFromBinaryFile()
        {
            if (!isDataLoaded)
            {
                if (hasPreviousSave)
                {
                    FileManager.LoadFromBinaryFile(m_Path, out object data);
                    m_Data = data as Dictionary<string, object>;
                }

                isDataLoaded = true;
            }
            else
            {
                Debug.Log("Data has already been loaded!");
            }
        }

        public void SaveToBinaryFile()
        {
            FileManager.SaveToBinaryFile(m_Path, m_Data);
        }

        private void OnValidate()
        {
            m_Path = Path.Combine(Application.persistentDataPath, m_FileName);
        }
    }
}