using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SimCard.CardGame {
    public class CardTitleText : MonoBehaviour {
        private TextMeshProUGUI cardTitleText;
        private CardGameManager cardGameManager;

        void Awake() {
            cardTitleText = GetComponent<TextMeshProUGUI>();
            cardGameManager = GetComponentInParent<CardGameManager>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event += UpdateCardText;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event -= UpdateCardText;
        }

        void UpdateCardText(CardArgs args) {
            Card card = args.card;
            cardTitleText.text = card.CardSO.cardName;
        }
    }
}
