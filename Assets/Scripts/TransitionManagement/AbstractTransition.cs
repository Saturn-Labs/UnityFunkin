using System;
using System.Reflection;
using TransitionManager.Attributes;
using UnityEngine;

namespace TransitionManagement
{
    [Serializable]
    public abstract class AbstractTransition
    {
        public string Name { get; private set; }
        public GameObject? ConstructedTarget { get; private set; } = null;
        public bool IsConstructed => ConstructedTarget is not null;
        public bool IsActive => ConstructedTarget?.activeSelf ?? false;
        public float ActivationDuration { get; set; } = 1f;
        public float DeactivationDuration { get; set; } = 1f;

        protected AbstractTransition()
        {
            var type = GetType();
            if (type.GetCustomAttribute<TransitionTypeDeclaration>() is not { } attr)
                throw new ArgumentException("Transition type declaration attribute is missing.");
            if (string.IsNullOrEmpty(attr.Name))
                throw new ArgumentException("Transition name is missing.");
            Name = attr.Name;
        }
        
        public abstract GameObject? Construct();
        public virtual bool Activate()
        {
            if (ConstructedTarget is null)
                return false;
            ConstructedTarget?.SetActive(true);
            return true;
        }
        
        public virtual bool Deactivate()
        {
            if (ConstructedTarget is null) 
                return false;
            ConstructedTarget.SetActive(false);
            return true;
        }

        public virtual void OnSceneLoading(AsyncOperation operation)
        {
            Debug.Log($"[AbstractTransition]: Loading scene: {Name}");
        }
        
        public virtual void OnSceneLoadingProgress(AsyncOperation operation) { }

        public virtual void OnSceneLoaded(AsyncOperation operation)
        {
            Debug.Log($"[AbstractTransition]: Loaded scene: {Name}");
        }

        public virtual void OnObjectReady(GameObject obj, Canvas canvas)
        {
            obj.transform.SetParent(canvas.transform, false);
        }
    }
}