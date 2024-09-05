using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    public class DummyAI : OpponentAI {
        public override IEnumerator Think() {
            // The only action it should give is EndAction
            yield return new WaitForSeconds(1f);

            actions.Add(new EndAction());
        }
    }
}
