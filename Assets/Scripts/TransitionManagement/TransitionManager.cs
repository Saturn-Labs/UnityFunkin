using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TransitionManagement.Attributes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TransitionManagement
{
    public class TransitionManager : MonoBehaviour
    {
        private static TransitionManager? _Instance = null;
        public static TransitionManager Main
        {
            get
            {
                if (_Instance) 
                    return _Instance;
                
                // Try to find an existing instance in the scene
                _Instance = FindFirstObjectByType<TransitionManager>();
                if (_Instance) 
                    return _Instance;
                
                // If no instance is found, create a new one
                var prefab = Resources.Load<GameObject>("Prefabs/TransitionManagement/TransitionManager");
                var obj = Instantiate(prefab);
                DontDestroyOnLoad(obj);
                obj.name = "TransitionManager";
                _Instance = obj.GetComponent<TransitionManager>();
                return _Instance;
            }
        }
        
        [SerializeField]
        private Canvas? _TransitionCanvas = null;
        public Canvas? TransitionCanvas => _TransitionCanvas;
        public AbstractTransition? SelectedTransition { get; private set; } = null;
        public AbstractTransition? DefaultTransition { get; set; } = null;
        private readonly Dictionary<string, AbstractTransition> Transitions = new();
        private readonly Dictionary<string, GameObject> TransitionObjects = new();
        public bool IsBusy { get; private set; } = false;

        public void SetSelectedTransition(AbstractTransition? transition)
        {
            SetSelectedTransition(transition?.Name);
        }
        
        public AbstractTransition? SetSelectedTransition(string? transition)
        {
            if (transition is null)
            {
                SelectedTransition = null;
                return null;
            }
            
            if (!Transitions.TryGetValue(transition, out var transitionObject))
            {
                Debug.LogError($"Transition '{transition}' is not registered.");
                return null;
            }
            SelectedTransition = transitionObject;
            return SelectedTransition;
        }
        
        public AbstractTransition? CreateTransition(Type transitionType)
        {
            if (transitionType.GetCustomAttribute<TransitionTypeDeclaration>() is not { } attr)
            {
                Debug.LogError($"Transition type '{transitionType.Name}' does not have a TransitionTypeDeclaration attribute.");
                return null;
            }
            
            // Check if the transition type is already registered
            if (Transitions.ContainsKey(attr.Name))
            {
                Debug.LogError($"Transition '{attr.Name}' is already registered.");
                return null;
            }

            // Check if the transition type is abstract
            if (transitionType.IsAbstract)
            {
                Debug.LogError($"Transition '{attr.Name}' is abstract and cannot be created.");
                return null;
            }
            
            // Create an instance of the transition type
            if (Activator.CreateInstance(transitionType) is not AbstractTransition transition)
            {
                Debug.LogError($"Failed to create instance of transition: '{transitionType.Name}'.");
                return null;
            }
            
            // Add the transition to the dictionary
            Transitions[transition.Name] = transition;
            return transition;
        }
        
        public T? CreateTransition<T>() where T : AbstractTransition => CreateTransition(typeof(T)) as T;
        
        private void CreateTransitions()
        {
            // Find all transitions via reflection
            var transitionTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(AbstractTransition)) && !type.IsAbstract && type.GetCustomAttribute<TransitionTypeDeclaration>() is not null);
            
            // Create instances of each transition type
            var seenNames = new HashSet<string>();
            foreach (var type in transitionTypes)
            {
                var attribute = type.GetCustomAttribute<TransitionTypeDeclaration>();
                if (attribute is null)
                    continue; // Impossible to happen, but just in case

                if (!seenNames.Add(attribute.Name))
                {
                    Debug.LogWarning($"Transition '{type.Name}' has already been added to transition system, ignoring.");
                    continue;
                }

                CreateTransition(type);
            }
        }

        public void ConstructTransition(string transitionName, Canvas canvas)
        {
            // Check if the transition is registered
            if (!Transitions.TryGetValue(transitionName, out var transition))
            {
                Debug.LogError($"Transition '{transitionName}' is not registered.");
                return;
            }
            
            // Check if the transition type is already constructed
            if (TransitionObjects.ContainsKey(transitionName))
            {
                Debug.LogError($"Transition '{transitionName}' is already constructed.");
                return;
            }
                
            // Construct the transition
            var transitionObject = transition.Construct();
            if (!transitionObject)
            {
                Debug.LogError($"Failed to construct transition object for transition: '{transitionName}'.");
                return;
            }
            
            TransitionObjects[transitionName] = transitionObject;
            // Call the OnObjectReady method to set the parent and other properties
            transition.OnObjectReady(transitionObject, canvas);
            var property = transition.GetType().GetProperty("ConstructedTarget", BindingFlags.Public | BindingFlags.Instance);
            property = property?.DeclaringType?.GetProperty(property.Name);
            property?.GetSetMethod(true).Invoke(transition, new object[]
            {
                transitionObject.GetComponent<RectTransform>()
            });
        }

        public void ConstructTransition(AbstractTransition transition, Canvas canvas)
        {
            // Check if the transition has the TransitionTypeDeclaration attribute
            if (transition.GetType().GetCustomAttribute<TransitionTypeDeclaration>() is not { } attr)
            {
                Debug.LogError($"Transition type '{transition.GetType().Name}' does not have a TransitionTypeDeclaration attribute.");
                return;
            }
            
            // Construct the transition
            ConstructTransition(attr.Name, canvas);
        }

        private void ConstructTransitions(Canvas canvas)
        {
            // Iterate over all transitions
            foreach (var (tName, _) in Transitions)
            {
                // Try to construct each transition
                ConstructTransition(tName, canvas);
            }
        }
        
        private void Awake()
        {
            if (!TransitionCanvas)
                throw new NullReferenceException("Transition canvas is null.");
            // Create and construct the transitions
            CreateTransitions();
            ConstructTransitions(TransitionCanvas);
            DefaultTransition = Transitions.FirstOrDefault() is { Value: not null } t ? t.Value : null;
            if (SelectedTransition is null)
                SetSelectedTransition(DefaultTransition);
        }
        
        public void MakeTransitionActive(Action<AbstractTransition>? before = null, Action<AbstractTransition>? after = null)
        {
            StartCoroutine(MakeTransitionActiveCoroutine(before, after));
        }

        public IEnumerator MakeTransitionActiveCoroutine(Action<AbstractTransition>? before = null, Action<AbstractTransition>? after = null)
        {
            if (SelectedTransition is null || SelectedTransition.IsActive)
                yield break;
            before?.Invoke(SelectedTransition);
            SelectedTransition.Activate();
            yield return new WaitForSeconds(SelectedTransition.ActivationDuration);
            after?.Invoke(SelectedTransition);
        }
        
        public void MakeTransitionInactive(Action<AbstractTransition>? before = null, Action<AbstractTransition>? after = null)
        {
            StartCoroutine(MakeTransitionInactiveCoroutine(before, after));
        }

        public IEnumerator MakeTransitionInactiveCoroutine(Action<AbstractTransition>? before = null, Action<AbstractTransition>? after = null)
        {
            if (SelectedTransition is null || !SelectedTransition.IsActive)
                yield break;
            before?.Invoke(SelectedTransition);
            SelectedTransition.Deactivate();
            yield return new WaitForSeconds(SelectedTransition.DeactivationDuration);
            after?.Invoke(SelectedTransition);
        }
        
        public void LoadScene(string sceneName, AbstractTransition? transition = null, Action<AsyncOperation>? onProgress = null, Action? onComplete = null)
        {
            StartCoroutine(LoadSceneCoroutine(sceneName, transition, onProgress, onComplete));
        }

        public IEnumerator LoadSceneCoroutine(string sceneName, AbstractTransition? transition = null, Action<AsyncOperation>? onProgress = null, Action? onComplete = null)
        {
            while (IsBusy)
                yield return null;
            IsBusy = true;
            
            if (transition is not null) 
                SetSelectedTransition(transition);
            
            // Activate the transition
            yield return StartCoroutine(MakeTransitionActiveCoroutine());
            
            // Start loading scene
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation is null)
            {
                Debug.LogError($"Failed to load scene '{sceneName}'.");
                yield break;
            }
            
            // Notify transition that the scene is loading
            SelectedTransition?.OnSceneLoading(operation);

            // Wait for the scene to load
            while (!operation.isDone)
            {
                // Notify transition of loading progress
                SelectedTransition?.OnSceneLoadingProgress(operation);
                onProgress?.Invoke(operation);
                yield return null;
            }
            
            // Notify transition that the scene has loaded
            SelectedTransition?.OnSceneLoaded(operation);
            
            // Deactivate the transition
            if (SelectedTransition is not null)
            {
                SelectedTransition.Deactivate();
                yield return new WaitForSeconds(SelectedTransition.DeactivationDuration);
            }
            onComplete?.Invoke();
            IsBusy = false;
        }
    }
}
