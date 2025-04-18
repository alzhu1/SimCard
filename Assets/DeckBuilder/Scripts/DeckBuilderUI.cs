using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.DeckBuilder {
    public interface DeckBuilderUIListener {
        public Dictionary<CardSO, (int, int)> CardToCount { get; }
        public List<CardSO> SelectableCards { get; }
        public CardSO SelectedCard { get; }
        public int Index { get; }
        public int SubIndex { get; }
    }

    public class DeckBuilderUI : MonoBehaviour {
        [Header("Card List View")]
        [SerializeField]
        private CanvasGroup cardListView;

        [SerializeField]
        private Image cardCursor;

        [SerializeField]
        private Image upArrow;

        [SerializeField]
        private Image leftCountArrow;

        [SerializeField]
        private Image rightCountArrow;

        [SerializeField]
        private DeckBuilderCardRowUI[] cardRows;

        [SerializeField]
        private Image downArrow;

        [SerializeField]
        private Image[] options;

        [SerializeField]
        private Image optionCursor;

        [Header("Preview View")]
        [SerializeField]
        private CanvasGroup previewView;

        [SerializeField]
        private TextMeshProUGUI previewNameText;

        [SerializeField]
        private TextMeshProUGUI previewDescriptionText;

        [SerializeField]
        private TextMeshProUGUI previewFlavorText;

        [SerializeField]
        private TextMeshProUGUI previewCostText;

        [SerializeField]
        private TextMeshProUGUI previewIncomeText;

        [SerializeField]
        private TextMeshProUGUI previewLifetimeText;

        [SerializeField]
        private Image previewCardImage;

        private int topIndex;

        public DeckBuilderUIListener DeckBuilderUIListener { get; private set; }
        public void InitUIListener(DeckBuilderUIListener listener) => DeckBuilderUIListener = listener;

        void Awake() {
            topIndex = 0;
        }

        void Update() {
            if (DeckBuilderUIListener == null) {
                return;
            }

            // Reset group canvas alphas
            cardListView.alpha = 0;
            previewView.alpha = 0;

            if (DeckBuilderUIListener.SelectedCard != null) {
                UpdatePreviewViewUI(DeckBuilderUIListener.SelectedCard);
            } else {
                UpdateCardListViewUI();
            }
        }

        void UpdateCardListViewUI() {
            cardListView.alpha = 1;

            // Handle options update
            bool isOptionSelect = DeckBuilderUIListener.Index == -1;
            cardCursor.enabled = !isOptionSelect;
            leftCountArrow.enabled = !isOptionSelect;
            rightCountArrow.enabled = !isOptionSelect;

            optionCursor.enabled = isOptionSelect;

            if (isOptionSelect) {
                optionCursor.rectTransform.anchoredPosition = new Vector2(
                    options[DeckBuilderUIListener.SubIndex].rectTransform.anchoredPosition.x,
                    optionCursor.rectTransform.anchoredPosition.y
                );
            }

            // Handle top index update
            topIndex = Mathf.Min(topIndex, Mathf.Max(DeckBuilderUIListener.Index, 0));
            topIndex =
                Mathf.Max(topIndex + cardRows.Length - 1, DeckBuilderUIListener.Index)
                - (cardRows.Length - 1);

            for (int i = topIndex; i < topIndex + cardRows.Length; i++) {
                int cardRowIndex = i - topIndex;
                DeckBuilderCardRowUI cardRow = cardRows[cardRowIndex];

                if (i < DeckBuilderUIListener.SelectableCards.Count) {
                    cardRow.gameObject.SetActive(true);
                    CardSO card = DeckBuilderUIListener.SelectableCards[i];

                    // Set basic info
                    cardRow.CardNameText.text = card.cardName;
                    cardRow.CostText.text = $"${card.cost.ToString()}";
                    cardRow.IncomeText.text = $"${card.income.ToString()}";
                    cardRow.LifetimeText.text = card.turnLimit.ToString();

                    // Update the deck count text
                    (int deckCount, int totalCount) = DeckBuilderUIListener.CardToCount[card];
                    cardRow.DeckCountText.text = $"{deckCount} / {totalCount}";
                } else {
                    cardRow.gameObject.SetActive(false);
                }
            }

            // Update cursor position
            if (!isOptionSelect) {
                cardCursor.rectTransform.anchoredPosition = new Vector2(
                    cardCursor.rectTransform.anchoredPosition.x,
                    cardRows[DeckBuilderUIListener.Index - topIndex].rectTransform.anchoredPosition.y + 3 // offset
                );
            }

            upArrow.enabled = topIndex != 0;
            downArrow.enabled = topIndex + cardRows.Length < DeckBuilderUIListener.SelectableCards.Count;
        }

        void UpdatePreviewViewUI(CardSO previewCard) {
            previewView.alpha = 1;

            previewNameText.text = previewCard.cardName;
            previewDescriptionText.text = previewCard.description;
            previewFlavorText.text = previewCard.flavorText;
            previewCostText.text = $"Cost: ${previewCard.cost}";
            previewIncomeText.text = $"Income: ${previewCard.income}";
            previewLifetimeText.text = $"Turn Limit: {previewCard.turnLimit}";
            previewCardImage.sprite = previewCard.fullSprite;
        }
    }
}
