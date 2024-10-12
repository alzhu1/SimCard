using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    [System.Serializable]
    public class InteractionOption {
        public string option;
        public string nextInteractionPath;
        public string fallbackInteractionPath;
    }

    [System.Serializable]
    public class Interaction {
        // TODO: To add conditions, we need to define something on the dialogue itself
        // Maybe the dialogue, or the InteractionPath
        // Could have general conditions on path, and more specific conditions on Interaction

        // Couple options:
        // 1. Use a scripting language (e.g. JS with Jint)
        //    This would be attached as a property on InteractionPath or Interaction
        // 2. Adding a List<string> for "scriptTags"
        //    And basically building my own "scripting language"
        //    Different starting characters/enum can represent different options
        //    e.g. checking if you talked with someone else, or if quest complete, or story progress (i.e. day 7 reached)

        [TextArea]
        public string text;

        public List<InteractionOption> options;

        public List<InteractionTag> tags;
    }

    [System.Serializable]
    public class InteractionPath {
        public string name;
        public int startingPriority = -1;
        public List<Interaction> interactions;
        public List<InteractionTag> pathTags;

        public static InteractionPath CreateDefaultPath(List<Interaction> defaultInteractions) {
            return new InteractionPath {
                name = "Default",
                startingPriority = 0,
                interactions = defaultInteractions,
            };
        }
    }

    [CreateAssetMenu(fileName = "Interactable", menuName = "ScriptableObjects/Interactable")]
    public class InteractableSO : ScriptableObject {
        public List<Interaction> defaultInteractions;
        public List<InteractionPath> interactionPaths;
    }
}
