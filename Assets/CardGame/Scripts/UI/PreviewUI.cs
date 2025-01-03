using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

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

        private CanvasGroup canvasGroup;
        private CardGameManager cardGameManager;

        private PreviewUIListener previewUIListener;

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
                        // TODO: Fill in rendering
                        break;
                    }
                }
            }
        }

        void UpdatePreview(EventArgs<PreviewUIListener> args) {
            previewUIListener = args.argument;
            canvasGroup.alpha = previewUIListener == null ? 0 : 1;
        }
    }
}
