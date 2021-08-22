using System;
using System.Collections.Generic;
using RPG.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

// use in any gameobject that has an ISaveable that we want to keep track of
// this class generates unique identifiers with no overlaps inside the scene
namespace RPG.Saving
{
    [ExecuteAlways]
    public class SaveableEntity : MonoBehaviour
    {
        // create a default, empty identifier which should not change
        [SerializeField] string uniqueIdentifier = "";

        // the entities UUID will be registered to this dictionary
        static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

        public string GetUniqueIdentifier()
        {
            return uniqueIdentifier;
        }

        // why returning an "object"?
        // we dont have to be concerned with what type is actually returned, as long as it is serializable
        public object CaptureState()
        {
            Dictionary<string, object> state = new Dictionary<string, object>();
            // calling the ISaveable component on the object that uses it
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                state[saveable.GetType().ToString()] = saveable.CaptureState();
            }
            return state;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
            foreach (ISaveable saveable in GetComponents<ISaveable>())
            {
                string typeString = saveable.GetType().ToString();
                if (stateDict.ContainsKey(typeString))
                {
                    saveable.RestoreState(stateDict[typeString]);
                }
            }
        }

        // UNITY_EDITOR -> everything between these two statements (#endif) is not going to be included
        // in the C# build for a project that is not being built for the editor.
#if UNITY_EDITOR
        private void Update() {
            if (Application.IsPlaying(gameObject)) return;
            // the given path will return the current object path, if it is in a scene or in another mode (like prefab window)
            // if this value is null, it means that we might be in a different window the scene, and because of that
            // it should not update the UUID of the entity.
            if (string.IsNullOrEmpty(gameObject.scene.path)) return;

            // in order to change the values that are being stored in the scene file or prefab,
            // we need to use a serialized object and serialized property and modify those rather
            // modifing the values directly on our objects.
            SerializedObject serializedObject = new SerializedObject(this);
            SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");
            
            // modify the property only it is empty or null
            // update the property with a string value, which will generate a new UUID for that saveable entity
            if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
            {
                property.stringValue = System.Guid.NewGuid().ToString();
                // inform unity that a change has been made to the property
                serializedObject.ApplyModifiedProperties();
            }

            globalLookup[property.stringValue] = this;
        }
#endif

        private bool IsUnique(string candidate)
        {
            if (!globalLookup.ContainsKey(candidate)) return true;

            if (globalLookup[candidate] == this) return true;

            if (globalLookup[candidate] == null)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
            {
                globalLookup.Remove(candidate);
                return true;
            }

            return false;
        }
    }
}