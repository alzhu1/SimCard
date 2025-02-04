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
        private Image upArrow;

        [SerializeField]
        private DeckBuilderCardRowUI[] cardRows;

        [SerializeField]
        private Image downArrow;

        [SerializeField]
        private Image[] options;

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

            for (int i = 0; i < options.Length; i++) {
                options[i].color = DeckBuilderUIListener.Index == -1 && DeckBuilderUIListener.SubIndex == i ? Color.red : Color.white;
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
                    cardRow.CostText.text = card.cost.ToString();
                    cardRow.IncomeText.text = card.income.ToString();
                    cardRow.LifetimeText.text = card.turnLimit.ToString();

                    // Update the deck count text
                    (int deckCount, int totalCount) = DeckBuilderUIListener.CardToCount[card];
                    cardRow.DeckCountText.text = $"{deckCount} / {totalCount}";

                    // Red text if it's the current index
                    cardRows[cardRowIndex]
                        .SetTextColor(i == DeckBuilderUIListener.Index ? Color.red : Color.white);
                } else {
                    cardRow.gameObject.SetActive(false);
                }
            }

            upArrow.enabled = topIndex != 0;
            downArrow.enabled = topIndex + cardRows.Length < DeckBuilderUIListener.SelectableCards.Count;
        }

        void UpdatePreviewViewUI(CardSO previewCard) {
            previewView.alpha = 1;

            previewNameText.text = previewCard.cardName;
            previewDescriptionText.text = previewCard.description;
            previewFlavorText.text = previewCard.flavorText;
            previewCostText.text = $"Cost: {previewCard.cost}";
            previewIncomeText.text = $"Income: {previewCard.income}";
            previewLifetimeText.text = $"Turn Limit: {previewCard.turnLimit}";
        }
    }
}
