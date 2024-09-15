using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class InteractArgs : EventArgs {
        public GameObject interactable;
        public InteractArgs(GameObject interactable) => this.interactable = interactable;
    }
}
