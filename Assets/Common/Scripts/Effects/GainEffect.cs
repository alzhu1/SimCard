using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class GainEffect : Effect {
        [SerializeField] private int gainAmount;

        public override void ApplyEffect(params Card[] targets) {
            Debug.Log($"Gain effect, amount: {gainAmount}");
        }
    }
}
