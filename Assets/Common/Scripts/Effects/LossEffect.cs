using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class LossEffect : Effect {
        [SerializeField] private int lossAmount;

        public override void ApplyEffect(Card source, Card target) {
            Debug.Log($"Gain effect, amount: {lossAmount}");
        }
    }
}
