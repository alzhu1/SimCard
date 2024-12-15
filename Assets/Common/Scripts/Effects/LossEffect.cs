using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class LossEffect : Effect {
        [SerializeField] private int lossAmount;

        public override void ApplyEffect(params Card[] targets) {
            Debug.Log($"Gain effect, amount: {lossAmount}");
        }
    }
}
