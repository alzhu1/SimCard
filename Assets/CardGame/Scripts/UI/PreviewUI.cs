using System.Collections;
using System.Collections.Generic;
using SimCard.Common;
using TMPro;
using UnityEngine;

namespace SimCard.CardGame {
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

        void UpdatePreview(EventArgs<CardGraphSelectable, List<PlayerCardAction>> args) {
            CardGraphSelectable selectable = args.arg1;

            if (selectable == null) {
                canvasGroup.alpha = 0;
                cardRendererGroup.alpha = 0;
                graveyardRendererGroup.alpha = 0;
                return;
            }

            canvasGroup.alpha = 1;

            switch (args.arg1) {
                case Card card: {
                    cardRendererGroup.alpha = 1;
                    cardTitleText.text = selectable.CardName;
                    cardFlavorText.text = selectable.FlavorText;
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
}
