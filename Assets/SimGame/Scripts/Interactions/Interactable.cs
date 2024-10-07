using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SimCard.SimGame {
    public class Interactable : MonoBehaviour {
        private static float GlobalTypeTime => 0.08f;

        [SerializeField]
        private float typeTime;
        public float TypeTime => typeTime == 0 ? GlobalTypeTime : typeTime;

        [field: SerializeField]
        public InteractableSO InteractableSO { get; private set; }

        public Dictionary<string, List<Interaction>> InteractionPaths { get; private set; }

        public string InteractionPath { get; set; }

        void Awake() {
            InteractionPaths = InteractableSO.interactionPaths.ToDictionary(
                x => x.name,
                x => x.interactions
            );
            Assert.IsFalse(InteractionPaths.ContainsKey("Default"));
            InteractionPaths.Add("Default", InteractableSO.defaultInteractions);

            InteractionPath = "Default";
        }
    }
}
