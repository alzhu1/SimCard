using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.CardGame {
    [CreateAssetMenu(fileName = "SimpleAI", menuName = "ScriptableObjects/AI/SimpleAI")]
    public class SimpleAI : OpponentAI {
        protected override IEnumerator Think() {
            yield break;
        }
    }
}
