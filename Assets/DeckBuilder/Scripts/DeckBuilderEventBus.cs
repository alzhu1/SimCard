using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.DeckBuilder {
    public class DeckBuilderEventBus : MonoBehaviour {
        private static DeckBuilderEventBus instance = null;

        void Awake() {
            if (instance == null) {
                instance = this;
            } else {
                Destroy(gameObject);
                return;
            }
        }
    }
}
