using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
    public class CardPreviewUI : MonoBehaviour {
        [SerializeField]
        private TextMeshProUGUI cardTitleText;

        [SerializeField]
        private TextMeshProUGUI cardFlavorText;

        private CanvasGroup canvasGroup;
        private CardGameManager cardGameManager;

        void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            cardGameManager = GetComponentInParent<CardGameManager>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardPreview.Event += UpdatePreview;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardPreview.Event -= UpdatePreview;
        }

        void UpdatePreview(EventArgs<Card, List<PlayerCardAction>> args) {
            Card card = args.arg1;

            if (card == null) {
                canvasGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;

            cardTitleText.text = card.CardSO.cardName;
            cardFlavorText.text = card.CardSO.flavorText;
        }
    }
}
