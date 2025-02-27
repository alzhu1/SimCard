using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace SimCard.CardGame {
    public interface PreviewUIListener {
        public CardGraphSelectable PreviewedItem { get; }
        public int Index { get; }
        public string PreviewHeader { get; }
    }

    public class PreviewUI : MonoBehaviour {
        [Header("Card Renderer")]
        [SerializeField]
        private CanvasGroup cardRendererGroup;

        [SerializeField]
        private TextMeshProUGUI cardTitleText;

        [SerializeField]
        private TextMeshProUGUI cardIncomeText;

        [SerializeField]
        private TextMeshProUGUI cardTurnsLeftText;

        [SerializeField]
        private TextMeshProUGUI cardDescriptionText;

        [SerializeField]
        private TextMeshProUGUI cardFlavorText;

        [SerializeField]
        private TextMeshProUGUI cardEffectText;

        [SerializeField]
        private Image cardSpriteImage;

        [Header("Graveyard Renderer")]
        [SerializeField]
        private CanvasGroup graveyardRendererGroup;

        [SerializeField]
        private Image graveyardCursor;

        [SerializeField]
        private Image upArrow;

        [SerializeField]
        private TextMeshProUGUI graveyardTitleText;

        [SerializeField]
        private TextMeshProUGUI[] cardTexts;

        [SerializeField]
        private Image downArrow;

        private CanvasGroup canvasGroup;
        private CardGameManager cardGameManager;

        private PreviewUIListener previewUIListener;

        // Graveyard focused
        private int graveyardTopTextIndex;

        void Awake() {
            canvasGroup = GetComponent<CanvasGroup>();
            cardGameManager = GetComponentInParent<CardGameManager>();

            Assert.AreEqual(cardTexts.Length, 5);
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardPreview.Event += UpdatePreview;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardPreview.Event -= UpdatePreview;
        }

        void Update() {
            if (previewUIListener != null && previewUIListener.PreviewedItem != null) {
                cardRendererGroup.alpha = 0;
                graveyardRendererGroup.alpha = 0;

                switch (previewUIListener.PreviewedItem) {
                    case Card card: {
                        cardRendererGroup.alpha = 1;
                        cardTitleText.text = previewUIListener.PreviewHeader;
                        cardIncomeText.text = $"Income: {card.Income}";
                        cardTurnsLeftText.text = card.ActiveTurns > 0 ? $"Turns Left: {card.ActiveTurns}" : "<color=red>RETIRED</color>";
                        cardDescriptionText.text = card.Description;
                        cardFlavorText.text = card.FlavorText;
                        cardSpriteImage.sprite = card.FullSprite;

                        // TODO: Figure out Effect Text (likely just a list of applied effects)
                        // cardEffectText.text =

                        break;
                    }

                    case Graveyard graveyard: {
                        graveyardRendererGroup.alpha = 1;
                        graveyardTitleText.text = previewUIListener.PreviewHeader;
                        graveyardCursor.enabled = graveyard.Cards.Count > 0;

                        // Handle top index update
                        graveyardTopTextIndex = Mathf.Min(graveyardTopTextIndex, previewUIListener.Index);
                        graveyardTopTextIndex = Mathf.Max(graveyardTopTextIndex + cardTexts.Length - 1, previewUIListener.Index) - (cardTexts.Length - 1);

                        for (int i = 0; i < cardTexts.Length; i++) {
                            int cardIndex = graveyardTopTextIndex + i;
                            if (cardIndex >= graveyard.Cards.Count) {
                                cardTexts[i].enabled = false;
                            } else {
                                cardTexts[i].enabled = true;
                                cardTexts[i].text = graveyard.Cards[graveyardTopTextIndex + i].CardName;
                            }
                        }

                        graveyardCursor.rectTransform.anchoredPosition = new Vector2(
                            graveyardCursor.rectTransform.anchoredPosition.x,
                            cardTexts[previewUIListener.Index - graveyardTopTextIndex].rectTransform.anchoredPosition.y + 3 // offset
                        );

                        // Display up/down arrows if more can be seen
                        upArrow.enabled = graveyardTopTextIndex != 0;
                        downArrow.enabled = graveyardTopTextIndex + cardTexts.Length < graveyard.Cards.Count;

                        break;
                    }
                }
            }
        }

        void UpdatePreview(EventArgs<PreviewUIListener> args) {
            previewUIListener = args.argument;
            canvasGroup.alpha = previewUIListener == null ? 0 : 1;
            graveyardTopTextIndex = 0;

            // Run Update once to initialize UI immediately
            Update();
        }
    }
}
