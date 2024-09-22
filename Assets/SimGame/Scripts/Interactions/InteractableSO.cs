using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    [System.Serializable]
    public class InteractionAdvance {
        // TODO: Would like to make this abstract and have different types of advance, to avoid bloat
        public List<string> options;
    }

    [System.Serializable]
    public class Interaction {
        [TextArea]
        public string text;
        public InteractionAdvance advance;
        public float typeTime = 0.1f;
    }

    [CreateAssetMenu(fileName = "Interactable", menuName = "ScriptableObjects/Interactable")]
    public class InteractableSO : ScriptableObject {
        public List<Interaction> interactions;
    }
}
