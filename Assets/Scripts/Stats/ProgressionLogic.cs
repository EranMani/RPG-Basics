using UnityEngine;
using System.Collections.Generic;
using System;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]
    public class ProgressionLogic : ScriptableObject
    {
        // A list of progression character class
        [SerializeField] ProgressionCharacterClass[] characterClasses;

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> lookupTable = null;

        public float GetStat(Stat stat, CharacterClass characterClass, int level)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];

            if (levels.Length < level)
            {
                return 0;
            }

            return levels[level - 1];
        }

        public int GetLevels(Stat stat, CharacterClass characterClass)
        {
            BuildLookup();

            float[] levels = lookupTable[characterClass][stat];
            return levels.Length;
        }

        private void BuildLookup()
        {
            if (lookupTable != null) return;

            // Init the main dictionary
            lookupTable = new Dictionary<CharacterClass, Dictionary<Stat, float[]>>();

            // For each character class as key, find the stats and levels that are relevant
            foreach (ProgressionCharacterClass progressionCharacterClass in characterClasses)
            {
                // Init the second dictionary
                var statLookupTable = new Dictionary<Stat, float[]>();

                // For each stat that are assigned, add it as key and find the relevant level values
                foreach (ProgressionStat progressionStat in progressionCharacterClass.stats)
                {
                    statLookupTable[progressionStat.stat] = progressionStat.levels;
                }

                // Use the character class as a key, and the stat with level values as the value
                lookupTable[progressionCharacterClass.characterClass] = statLookupTable;
            }
           
        }

        [System.Serializable] // -> allows for the class to be serialized/visible in the inspector
        class ProgressionCharacterClass
        {
            // Each character class has its own health amount according to a certain level
            public CharacterClass characterClass;
            public ProgressionStat[] stats;
        }

        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
    }
}

