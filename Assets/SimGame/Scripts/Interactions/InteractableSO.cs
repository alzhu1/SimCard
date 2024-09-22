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
        private static float GlobalTypeTime => 0.08f;

        [TextArea]
        public string text;
        [SerializeField] private float typeTime;
        public InteractionAdvance advance;

        public float TypeTime => typeTime == 0f ? GlobalTypeTime : typeTime;
    }

    [CreateAssetMenu(fileName = "Interactable", menuName = "ScriptableObjects/Interactable")]
    public class InteractableSO : ScriptableObject {
        public List<Interaction> interactions;
    }
}
