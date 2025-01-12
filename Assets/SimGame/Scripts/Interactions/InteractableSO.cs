using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    [System.Serializable]
    public class Interaction {
        // MINOR: Eventually, would be good to replace this ScriptableObject system with a scripting language.
        // Look into Jint (JS) to bring in a scriptable language that can process interactions on the fly.

        [TextArea]
        public string text;

        public List<InteractionOption> options;

        public List<InteractionTag> tags;

        public List<string> eventTriggers;
    }

    [System.Serializable]
    public class InteractionOption {
        public string option;
        public string nextInteractionPath;
        public string fallbackInteractionPath;

        public int energyCost = 0;
    }

    [System.Serializable]
    public class InteractionPath {
        public string name;
        public int startingPriority = -1;
        public List<Interaction> interactions;
        public List<InteractionTag> pathTags;
        public List<string> endingEventTriggers;

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
