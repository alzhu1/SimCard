using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    [CreateAssetMenu(fileName = "DummyAI", menuName = "ScriptableObjects/AI/DummyAI")]
    public class DummyAI : OpponentAI {
        protected override IEnumerator Think() {
            // The only action it should give is EndAction
            yield return new WaitForSeconds(1f);

            actions.Add(new EndAction());
        }
    }
}
