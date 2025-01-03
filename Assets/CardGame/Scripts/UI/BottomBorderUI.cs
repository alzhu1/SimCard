using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
    public class BottomBorderUI : MonoBehaviour {
        [SerializeField]
        private CanvasGroup bottomBorderCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI cardTitleText;

        [SerializeField]
        private CanvasGroup cardCost;

        private CardGameManager cardGameManager;
        private TextMeshProUGUI cardCostText;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
            cardCostText = cardCost.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardHover.Event += HandleCardHover;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardHover.Event -= HandleCardHover;
        }

        void HandleCardHover(EventArgs<CardGraphSelectable, List<PlayerCardAction>> args) {
            if (args.arg1 == null) {
                bottomBorderCanvasGroup.alpha = 0;
                return;
            }

            bottomBorderCanvasGroup.alpha = 1;

            cardTitleText.text = args.arg1.PreviewName;

            // TODO: Figure out way to avoid casting
            if (args.arg1 is Card card) {
                cardCost.alpha = 1;
                cardCostText.text = card.Cost.ToString();
            } else {
                cardCost.alpha = 0;
            }
        }
    }
}
