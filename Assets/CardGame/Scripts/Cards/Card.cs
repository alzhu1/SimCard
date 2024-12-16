using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class Card : MonoBehaviour {
        [SerializeField]
        private CardSO cardSO;
        public CardSO CardSO => cardSO;

        public int Cost => CardSO.cost;

        public int Income => CardSO.income;

        private SpriteRenderer sr;

        void Awake() {
            if (cardSO == null) {
                return;
            }

            sr = GetComponent<SpriteRenderer>();
            sr.sprite = cardSO.sprite;
            gameObject.SetActive(false);
        }

        public void InitCardSO(CardSO cardSO) {
            this.cardSO = cardSO;
            Awake();
        }


    }
}
