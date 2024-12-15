using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;

namespace SimCard.CardGame {
    public class Card : MonoBehaviour {
        [SerializeField]
        private CardSOV2 cardSO;
        public CardSOV2 CardSO => cardSO;

        public int Cost => CardSO.cost;

        private SpriteRenderer sr;

        void Awake() {
            if (cardSO == null) {
                return;
            }

            sr = GetComponent<SpriteRenderer>();
            sr.sprite = cardSO.sprite;
            gameObject.SetActive(false);
        }

        public void InitCardSO(CardSOV2 cardSO) {
            this.cardSO = cardSO;
            Awake();
        }

        public void SetSelectedColor() {
            sr.color = Color.green;
        }

        public void ResetColor() {
            Debug.Log("Resetting color");
            sr.color = Color.white;
        }
    }
}
