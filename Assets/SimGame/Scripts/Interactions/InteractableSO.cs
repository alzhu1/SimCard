using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    [System.Serializable]
    public class InteractionOption {
        public string option;
        public string nextInteractionPath;
    }

    [System.Serializable]
    public class Interaction {
        [TextArea]
        public string text;

        public List<InteractionOption> options;
    }

    [System.Serializable]
    public class InteractionPath {
        public string name;
        public List<Interaction> interactions;
    }

    [CreateAssetMenu(fileName = "Interactable", menuName = "ScriptableObjects/Interactable")]
    public class InteractableSO : ScriptableObject {
        public List<Interaction> defaultInteractions;
        public List<InteractionPath> interactionPaths;
    }
}
