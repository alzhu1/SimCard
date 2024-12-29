using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class Graveyard : CardHolder, CardGraphSelectable {
        public string CardName => "Graveyard";
        public string FlavorText => "Graveyard flavor text";

        public override void Spread() { }
    }
}
