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
    }

    public class PreviewUI : MonoBehaviour {
        [Header("Card Renderer")]
        [SerializeField]
        private CanvasGroup cardRendererGroup;

        [SerializeField]
        private TextMeshProUGUI cardTitleText;

        [SerializeField]
        private TextMeshProUGUI cardFlavorText;

        [Header("Graveyard Renderer")]
        [SerializeField]
        private CanvasGroup graveyardRendererGroup;

        [SerializeField]
        private Image upArrow;

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

            Assert.AreEqual(cardTexts.Length, 4);
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
                        cardTitleText.text = card.CardName;
                        cardFlavorText.text = card.FlavorText;
                        break;
                    }

                    case Graveyard graveyard: {
                        graveyardRendererGroup.alpha = 1;

                        // Handle top index update
                        graveyardTopTextIndex = Mathf.Min(graveyardTopTextIndex, previewUIListener.Index);
                        graveyardTopTextIndex = Mathf.Max(graveyardTopTextIndex + 3, previewUIListener.Index) - 3;

                        for (int i = 0; i < cardTexts.Length; i++) {
                            int cardIndex = graveyardTopTextIndex + i;
                            if (cardIndex >= graveyard.Cards.Count) {
                                cardTexts[i].enabled = false;
                            } else {
                                cardTexts[i].enabled = true;
                                cardTexts[i].text = graveyard.Cards[graveyardTopTextIndex + i].CardName;
                                cardTexts[i].color = cardIndex == previewUIListener.Index ? Color.red : Color.white;
                            }
                        }

                        // Display up/down arrows if more can be seen
                        upArrow.enabled = graveyardTopTextIndex != 0;
                        downArrow.enabled = graveyardTopTextIndex + 4 < graveyard.Cards.Count;

                        break;
                    }
                }
            }
        }

        void UpdatePreview(EventArgs<PreviewUIListener> args) {
            previewUIListener = args.argument;
            canvasGroup.alpha = previewUIListener == null ? 0 : 1;
            graveyardTopTextIndex = 0;
        }
    }
}
