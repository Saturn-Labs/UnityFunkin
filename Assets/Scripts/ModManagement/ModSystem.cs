using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FileSystem;
using ModManagement.Abstractions;
using ModManagement.Attributes;
using ModManagement.Default;
using ModManagement.State;
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
        
        [SerializeField]
        public static AppDomain ModDomain => AppDomain.CurrentDomain;
        private readonly List<AbstractMod> _Mods = new();
        private readonly List<Assembly> _LoadedModAssemblies = new();
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

        public event OnStateChangedDelegate? OnStateChanged;
        
        public AbstractMod InstantiateMod(Type modType, string baseDirectory)
        {
            // Check if the type is a subclass of AbstractMod
            if (!modType.IsSubclassOf(typeof(AbstractMod)))
                throw new ArgumentException($"[ModSystem]: Type '{modType.Name}' is not a subclass of AbstractMod.");

            // Check if the type has ModDefinition attribute
            var attr = modType.GetCustomAttribute<ModDefinition>();
            if (attr is null)
                throw new ArgumentException($"[ModSystem]: Type '{modType.Name}' doesn't have a mod definition attribute.");
            
            // Check if the mod is already loaded
            if (_Mods.Any(m => m.GetType() == modType))
            {
                throw new Exception($"[ModSystem]: Mod '{attr.Name}' is already instantiated.");
            }
            
            ChangeState(ModSystemState.InstantiatingMod, $"[ModSystem] Instantiating mod type '{modType.FullName}'...");
            // Create an instance of the mod
            var modInstance = (AbstractMod?)Activator.CreateInstance(modType);
            if (modInstance is null)
            {
                throw new Exception($"[ModSystem]: Failed to instantiate mod type '{modType.FullName}'.");
            }
            
            // Load the mod and add it to the list
            modType.GetProperty("Directory")?.GetSetMethod(true).Invoke(modInstance, new object[]
            {
                baseDirectory
            });
            return modInstance;
        }
        
        public void LoadModAssembly(string assemblyFile, string baseDirectory)
        {
            var assembly = Assembly.Load(File.ReadAllBytes(assemblyFile));
            
            _LoadedModAssemblies.Add(assembly);
            // Get all types in the assembly that inherit from AbstractMod and have the ModDefinition attribute
            var modTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(AbstractMod)) && !t.IsAbstract && t.GetCustomAttribute<ModDefinition>() is not null)
                .ToList();

            if (modTypes.Count == 0)
            {
                Debug.LogWarning($"[ModSystem]: Couldn't find any class that inherits from AbstractMod and have the ModDefinition attribute on assembly '{assembly.GetName().Name}'.");
            }

            foreach (var modType in modTypes)
            {
                if (InstantiateMod(modType, baseDirectory) is {} mod)
                    _Mods.Add(mod);
            }
        }
        
        public AsyncResult LoadModAssembliesAsync()
        {
            var result = new AsyncResult();
            IEnumerator LoadModAssembliesCoroutine()
            {
                // Check if the mod assemblies are already loaded
                if (_LoadedModAssemblies.Count > 0)
                {
                    Debug.LogWarning("[ModSystem]: Mod assemblies are already loaded.");
                    yield break;
                }
                
                // Get the mod directory from the path system
                var modsDirectory = PathUtils.Process("mods:");
                if (!Directory.Exists(modsDirectory))
                {
                    Debug.LogWarning($"[ModSystem]: Mod directory '{modsDirectory}' does not exist.");
                    yield break;
                }
            
                // Get all DLL files in the mod directory and its subdirectories
                var modDirectories = Directory.GetDirectories(modsDirectory, "*", SearchOption.AllDirectories);
                foreach (var modDirectory in modDirectories)
                {
                    var assemblyFiles = Directory.GetFiles(modDirectory, "*.dll", SearchOption.AllDirectories);
                    foreach (var assemblyFile in assemblyFiles)
                    {
                        // Check if the assembly is already loaded
                        if (_LoadedModAssemblies.Any(a => a.Location == assemblyFile))
                        {
                            Debug.LogWarning($"[ModSystem]: Assembly '{assemblyFile}' is already loaded.");
                            continue;
                        }
                        
                        // Load the assembly and add it to the list
                        ChangeState(ModSystemState.LoadingAssembly, $"[ModSystem] Loading mod assembly '{Path.GetFileName(assemblyFile)}'...");
                        try
                        {
                            LoadModAssembly(assemblyFile, modDirectory);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }
                    }
                }

                ChangeState(ModSystemState.Idling, $"[ModSystem] Idling...");
                CallSetCompleted(result);
            }
            StartCoroutine(LoadModAssembliesCoroutine());
            return result;
        }

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