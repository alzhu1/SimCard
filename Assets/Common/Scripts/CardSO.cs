using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card")]
    public class CardSO : ScriptableObject {
        // Native properties
        public string cardName;
        public int cost;

        [TextArea]
        public string description;

        public string flavorText;

        // Income (resource gained per turn start)
        public int income;

        // Limit (number of owner turns a card will last for)
        public int turnLimit;

        public Sprite smallSprite; // Use this in card game for preview
        public Sprite fullSprite;  // Use this for previewed images

        // Effects
        [SerializeReference] public List<Effect> effects;
    }

    [System.Serializable]
    public class CardMetadata {
        public CardSO cardSO;
        public int count;

        public CardMetadata(CardSO cardSO, int count) => (this.cardSO, this.count) = (cardSO, count);
    }

    [System.Serializable]
    public class CardPrizePool {
        public List<CardSO> cardPrizes;
        public int factor;
    }
}
