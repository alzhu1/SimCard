using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    public abstract class CardSO : ScriptableObject {
        // Native properties
        public string cardName;
        public Sprite sprite;
        public int cost;
        public string flavorText;

        // Base effect
        [SerializeReference] public Effect effect;
    }

    [System.Serializable]
    public class CardMetadata {
        public CardSO cardSO;
        public int count;
    }
}
