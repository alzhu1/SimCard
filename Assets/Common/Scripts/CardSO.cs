using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
    public class CardSO : ScriptableObject {
        // Native properties
        public string cardName;
        public Sprite sprite;
        public int cost;
        public string flavorText;

        // Income (resource gained per turn start)
        public int income;

        // Limit (number of owner turns a card will last for)
        public int turnLimit;

        // Effects
        [SerializeReference] public List<Effect> effects;
    }

    [System.Serializable]
    public class CardMetadata {
        public CardSO cardSO;
        public int count;
    }
}
