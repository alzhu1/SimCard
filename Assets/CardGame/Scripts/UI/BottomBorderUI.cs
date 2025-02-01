using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
    public class BottomBorderUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI bottomBorderText;

        [SerializeField]
        private CanvasGroup cardCostCanvasGroup;

        private CardGameManager cardGameManager;
        private TextMeshProUGUI cardCostText;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
            cardCostText = cardCostCanvasGroup.GetComponentInChildren<TextMeshProUGUI>();
        }

        void Start() {
            cardGameManager.EventBus.OnTurnStart.Event += HandleTurnStart;
            cardGameManager.EventBus.OnPlayerBaseHover.Event += HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event += HandleCardEffectHover;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event += HandleActionHover;
            cardGameManager.EventBus.OnPlayerCardDiscardHover.Event += HandleCardDiscardHover;
            cardGameManager.EventBus.OnOpponentCardSummon.Event += HandleOpponentCardSummon;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnTurnStart.Event -= HandleTurnStart;
            cardGameManager.EventBus.OnPlayerBaseHover.Event -= HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event -= HandleCardEffectHover;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event -= HandleActionHover;
            cardGameManager.EventBus.OnPlayerCardDiscardHover.Event -= HandleCardDiscardHover;
            cardGameManager.EventBus.OnOpponentCardSummon.Event -= HandleOpponentCardSummon;

        }

        void HandleTurnStart(System.EventArgs args) {
            cardCostCanvasGroup.alpha = 0;
            bottomBorderText.text = "";
        }

        void HandleCardHover(EventArgs<CardGraphSelectable> args) {
            bottomBorderText.text = args.argument.PreviewName;

            if (args.argument is Card card) {
                cardCostCanvasGroup.alpha = 1;
                cardCostText.text = card.Cost.ToString();
            } else {
                cardCostCanvasGroup.alpha = 0;
            }
        }

        void HandleCardEffectHover(EventArgs<Card, Effect> args) {
            Card card = args.arg1;
            Effect effect = args.arg2;
            cardCostCanvasGroup.alpha = 0;
            string cardEffectText = effect.previewText.Length > 0 ? effect.previewText : effect.GetType().Name;
            bottomBorderText.text = $"<color=blue>{cardEffectText}</color> -> {card.CardName}";
        }

        void HandleActionHover(EventArgs<PlayerCardAction> args) {
            cardCostCanvasGroup.alpha = 0;
            bottomBorderText.text = args.argument switch {
                PlayerCardAction.Preview => "Preview",
                PlayerCardAction.Summon => "Summon",
                PlayerCardAction.Fire => "Fire",
                PlayerCardAction.Surrender => "Surrender",
                _ => "None"
            };
        }

        void HandleCardDiscardHover(EventArgs<Card> args) {
            cardCostCanvasGroup.alpha = 0;
            bottomBorderText.text = $"Discard {args.argument.PreviewName}?";
        }

        void HandleOpponentCardSummon(EventArgs<Card> args) {
            cardCostCanvasGroup.alpha = 0;
            bottomBorderText.text = args.argument.PreviewName;
        }
    }
}
