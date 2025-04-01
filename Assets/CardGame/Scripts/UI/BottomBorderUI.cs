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
        private CanvasGroup cardPropertiesCanvasGroup;

        [SerializeField]
        private TextMeshProUGUI incomeTurnText;

        [SerializeField]
        private TextMeshProUGUI cardCostText;

        private CardGameManager cardGameManager;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
        }

        void Start() {
            cardGameManager.EventBus.OnGameStart.Event += HandleClear;
            cardGameManager.EventBus.OnTurnStart.Event += HandleClear;
            cardGameManager.EventBus.OnPlayerBaseHover.Event += HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event += HandleCardEffectHover;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event += HandleActionHover;
            cardGameManager.EventBus.OnPlayerCardDiscardHover.Event += HandleCardDiscardHover;
            cardGameManager.EventBus.OnOpponentCardSummon.Event += HandleOpponentCardSummon;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnGameStart.Event -= HandleClear;
            cardGameManager.EventBus.OnTurnStart.Event -= HandleClear;
            cardGameManager.EventBus.OnPlayerBaseHover.Event -= HandleCardHover;
            cardGameManager.EventBus.OnPlayerCardEffectHover.Event -= HandleCardEffectHover;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event -= HandleActionHover;
            cardGameManager.EventBus.OnPlayerCardDiscardHover.Event -= HandleCardDiscardHover;
            cardGameManager.EventBus.OnOpponentCardSummon.Event -= HandleOpponentCardSummon;

        }

        void HandleClear(System.EventArgs args) {
            cardPropertiesCanvasGroup.alpha = 0;
            bottomBorderText.text = "";
        }

        void HandleCardHover(EventArgs<CardGraphSelectable> args) {
            CardGraphSelectable selectable = args.argument;
            bottomBorderText.text = selectable.PreviewName;

            if (selectable is Card card) {
                cardPropertiesCanvasGroup.alpha = 1;
                cardCostText.text = $"${card.Cost}";
                incomeTurnText.text = $"${card.Income}||T:{card.ActiveTurns}";
            } else {
                cardPropertiesCanvasGroup.alpha = 0;
            }
        }

        void HandleCardEffectHover(EventArgs<Card, Effect> args) {
            (Card card, Effect effect) = args;
            cardPropertiesCanvasGroup.alpha = 0;
            string cardEffectText = effect.previewText.Length > 0 ? effect.previewText : effect.GetType().Name;
            bottomBorderText.text = $"<color=blue>{cardEffectText}</color> -> {card.CardName}";
        }

        void HandleActionHover(EventArgs<PlayerCardAction> args) {
            cardPropertiesCanvasGroup.alpha = 0;
            bottomBorderText.text = args.argument switch {
                PlayerCardAction.Preview => "Preview",
                PlayerCardAction.Summon => "Summon",
                PlayerCardAction.Fire => "Fire",
                PlayerCardAction.Surrender => "Surrender",
                _ => ""
            };
        }

        void HandleCardDiscardHover(EventArgs<Card> args) {
            cardPropertiesCanvasGroup.alpha = 0;
            bottomBorderText.text = $"Discard {args.argument.PreviewName}?";
        }

        void HandleOpponentCardSummon(EventArgs<Card> args) {
            cardPropertiesCanvasGroup.alpha = 0;
            bottomBorderText.text = args.argument.PreviewName;
        }
    }
}
