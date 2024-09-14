using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class SimGameEventBus : MonoBehaviour {
        private static SimGameEventBus instance = null;

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
