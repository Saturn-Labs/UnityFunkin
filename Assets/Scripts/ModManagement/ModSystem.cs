using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FileSystem;
using ModManagement.Abstractions;
using ModManagement.Attributes;
using ModManagement.State;
using Mono.Cecil;
using UnityEngine;
using Utils;

namespace ModManagement
{
    public class ModSystem : MonoBehaviour
    {
        #region Singleton
        private static ModSystem? _Instance = null;
        public static ModSystem Get()
        {
            if (_Instance) 
                return _Instance;
            
            // Try to find an existing instance in the scene
            _Instance = FindFirstObjectByType<ModSystem>();
            if (_Instance) 
                return _Instance;
            
            // If no instance is found, create a new one
            var prefab = Resources.Load<GameObject>("Prefabs/Modding/ModSystem");
            var obj = Instantiate(prefab);
            _Instance = obj.GetComponent<ModSystem>();
            return _Instance;
        }
        
        private void Awake()
        {
            // If the instance already exists, destroy this object
            if (_Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            // Set this object as the instance
            DontDestroyOnLoad(gameObject);
            _Instance = this;
            
        }
        #endregion

        public delegate void OnStateChangedDelegate(ModSystem system, ModSystemState state, string stateDescription);
        
        public static AppDomain ModDomain => AppDomain.CurrentDomain;
        [SerializeField]
        private string _currentModSystemStringState = "[ModSystem] Idling...";

        [SerializeField]
        private ModSystemState _currentModSystemState = ModSystemState.Idling;

        public ModSystemState CurrentModSystemState
        {
            get => _currentModSystemState;
            private set => _currentModSystemState = value;
        }

        public string CurrentModSystemStringState
        {
            get => _currentModSystemStringState;
            private set => _currentModSystemStringState = value;
        }

        public Dictionary<string, string> ListMods()
        {
            // Get all the base folders that contains the mod folders
            var baseFolders = new[]
            {
                PathUtils.Process("mods:"),
                PathUtils.Process("persistent_mods:")
            };

            return new Dictionary<string, string>();
        }
        
        public event OnStateChangedDelegate? OnStateChanged;
        private void ChangeState(ModSystemState state, string stateDescription)
        {
            CurrentModSystemState = state;
            CurrentModSystemStringState = stateDescription;
            OnStateChanged?.Invoke(this, state, stateDescription);
        }
        
        private static readonly MethodInfo? _SetCompletedMethod = typeof(AsyncResult).GetMethod("SetCompleted", BindingFlags.Instance | BindingFlags.NonPublic);
        // Reflection workaround to call the private method
        private void CallSetCompleted(AsyncResult result)
        {
            _SetCompletedMethod?.Invoke(result, null);
        }
    }
}