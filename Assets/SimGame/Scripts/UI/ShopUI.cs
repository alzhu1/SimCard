using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimCard.Common;
using TMPro;
using System.Linq;

namespace SimCard.SimGame {
    public class ShopUI : MonoBehaviour {
        [SerializeField] private Image shopCursor;

        // Preview
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI lifetimeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI flavorText;

        // Inventory count
        [SerializeField] private TextMeshProUGUI inventoryText;

        private SimGameManager simGameManager;

        private CanvasGroup shopGroup;
        private List<ShopOptionUI> shopOptions;

        private List<CardMetadata> shopItemList;
        private Player player;
        private OptionsUIListener optionsUIListener { get; set; }

        void Awake() {
            simGameManager = GetComponentInParent<SimGameManager>();

            shopGroup = GetComponent<CanvasGroup>();
            shopOptions = shopGroup.GetComponentsInChildren<ShopOptionUI>().ToList();
        }

        void Start() {
            simGameManager.EventBus.OnDisplayShopOptions.Event += DisplayShopOptions;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnDisplayShopOptions.Event -= DisplayShopOptions;
        }

        void Update() {
            if (optionsUIListener != null) {
                Vector2 pos = shopCursor.rectTransform.anchoredPosition;
                pos.y = shopOptions[optionsUIListener.OptionIndex].RectTransform.anchoredPosition.y;
                shopCursor.rectTransform.anchoredPosition = pos;

                // Update preview texts
                CardSO previewCard = shopItemList[optionsUIListener.OptionIndex].cardSO;
                cardNameText.text = previewCard.cardName;
                costText.text = $"Cost: {previewCard.cost}";
                incomeText.text = $"Income: {previewCard.income}";
                lifetimeText.text = $"Turn Limit: {previewCard.turnLimit}";
                descriptionText.text = previewCard.description;
                flavorText.text = previewCard.flavorText;

                // Update inventory text
                int cardCount = player.Deck.Find(cardMetadata => cardMetadata.cardSO.Equals(previewCard))?.count ?? 0;
                inventoryText.text = $"Inventory: {cardCount}";
            }
        }

        void DisplayShopOptions(EventArgs<OptionsUIListener, List<CardMetadata>, Player> args) {
            // Reset if args are null
            if (args == null) {
                optionsUIListener = null;
                shopItemList = null;
                shopGroup.alpha = 0;
                return;
            }

            optionsUIListener = args.arg1;
            shopItemList = args.arg2;
            player = args.arg3;

            shopGroup.alpha = 1;
            for (int i = 0; i < shopOptions.Count; i++) {
                // TODO: Actually we don't want to disable things (?)
                // Or at the very least, want to cap the amount of supported items shown
                // UI might need to keep track of the CardSO list? Or a new UI listener for that
                CardMetadata cardMetadata = shopItemList.ElementAtOrDefault(i);
                if (cardMetadata != null) {
                    shopOptions[i].gameObject.SetActive(true);
                    shopOptions[i].InitText(cardMetadata.cardSO.cardName, $"${cardMetadata.count}");
                } else {
                    shopOptions[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
