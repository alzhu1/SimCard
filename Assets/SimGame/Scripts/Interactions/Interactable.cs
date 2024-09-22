using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.SimGame {
    public class Interactable : MonoBehaviour {
        [field: SerializeField] public InteractableSO InteractableSO { get; private set; }
    }
}
