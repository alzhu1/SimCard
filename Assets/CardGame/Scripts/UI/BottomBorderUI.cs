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
        private TextMeshProUGUI bottomBorderText;

        [SerializeField]
        private CanvasGroup cardCost;

        private CardGameManager cardGameManager;
        private TextMeshProUGUI cardCostText;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
            cardCostText = cardCost.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerBaseHover.Event += HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event += HandleCardEffectHover;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerBaseHover.Event -= HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event -= HandleCardEffectHover;
        }

        void HandleCardHover(EventArgs<CardGraphSelectable> args) {
            if (args.argument == null) {
                bottomBorderCanvasGroup.alpha = 0;
                cardCost.alpha = 0;
                return;
            }

            bottomBorderCanvasGroup.alpha = 1;
            bottomBorderText.text = args.argument.PreviewName;

            if (args.argument is Card card) {
                cardCost.alpha = 1;
                cardCostText.text = card.Cost.ToString();
            } else {
                cardCost.alpha = 0;
            }
        }

        void HandleCardEffectHover(EventArgs<Card, Effect> args) {
            Card card = args.arg1;
            Effect effect = args.arg2;
            cardCost.alpha = 0;

            if (card == null) {
                bottomBorderCanvasGroup.alpha = 0;
                return;
            }

            bottomBorderCanvasGroup.alpha = 1;

            string cardEffectText = effect.previewText.Length > 0 ? effect.previewText : effect.GetType().Name;
            bottomBorderText.text = $"<color=blue>{cardEffectText}</color> -> {card.CardName}";
        }
    }
}
