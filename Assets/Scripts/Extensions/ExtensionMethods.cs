using System;
using System.Linq;
using UnityEngine;

namespace Extensions
{
    public static class ExtensionMethods
    {
        public static T? GetComponentInChildren<T>(this GameObject obj, Predicate<GameObject>? childSelector = null, bool recurse = false) where T : Component
        {
            if (childSelector is null)
            {
                return obj.GetComponentInChildren<T>();
            }

            foreach (Transform child in obj.transform)
            {
                if (childSelector(child.gameObject))
                {
                    if (child.TryGetComponent(out T component))
                    {
                        return component;
                    }
                }
                else if (recurse && child.childCount > 0)
                {
                    var result = child.gameObject.GetComponentInChildren<T>(childSelector, recurse);
                    if (result is not null)
                    {
                        return result;
                    }
                }
            }
            return null;
        }
    }
}