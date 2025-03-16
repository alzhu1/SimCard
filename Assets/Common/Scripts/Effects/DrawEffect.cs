using UnityEngine;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public class DrawEffect : Effect {
        public override void Apply(Card source, Card target) {
            Debug.Log("Draw card");
            target.GetCurrentHolder().HoldingDuelist.DrawCard();
        }
    }
}
