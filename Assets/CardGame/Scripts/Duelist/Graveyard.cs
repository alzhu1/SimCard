using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class Graveyard : CardHolder, CardGraphSelectable {
        public string PreviewName => "Graveyard";

        public override void Organize() {
            foreach (Card card in cards) {
                card.transform.position = transform.position;
                card.Hidden = hidden;
            }
        }
    }
}
