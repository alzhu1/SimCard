using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
    public class BottomBorderUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI cardTitleText;

        private CardGameManager cardGameManager;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardHover.Event += UpdateCardText;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardHover.Event -= UpdateCardText;
        }

        void UpdateCardText(EventArgs<Card, List<PlayerCardAction>> args) {
            Card card = args.arg1;
            cardTitleText.text = card.CardSO.cardName;
        }
    }
}
