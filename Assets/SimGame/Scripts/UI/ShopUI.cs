using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimCard.Common;
using TMPro;
using System.Linq;

namespace SimCard.SimGame {
    public class ShopUI : MonoBehaviour {
        // Shop
        [SerializeField] private Image shopCursor;
        [SerializeField] private Image upArrow;
        [SerializeField] private Image downArrow;

        // Preview
        [SerializeField] private TextMeshProUGUI cardNameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private TextMeshProUGUI incomeText;
        [SerializeField] private TextMeshProUGUI lifetimeText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI flavorText;
        [SerializeField] private Image previewCardImage;

        // Player
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private TextMeshProUGUI inventoryText;

        private SimGameManager simGameManager;

        private CanvasGroup shopGroup;
        private List<ShopOptionUI> shopOptions;
        private int topIndex;

        private List<CardMetadata> shopItemList;
        private Player player;
        private OptionsUIListener optionsUIListener { get; set; }

        void Awake() {
            simGameManager = GetComponentInParent<SimGameManager>();

            shopGroup = GetComponent<CanvasGroup>();
            shopOptions = shopGroup.GetComponentsInChildren<ShopOptionUI>().ToList();
            topIndex = 0;
        }

        void Start() {
            simGameManager.EventBus.OnDisplayShopOptions.Event += DisplayShopOptions;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnDisplayShopOptions.Event -= DisplayShopOptions;
        }

        void Update() {
            if (optionsUIListener != null) {
                int optionIndex = optionsUIListener.OptionIndex;

                // Handle top index update (for display)
                topIndex = Mathf.Min(topIndex, optionIndex);
                topIndex = Mathf.Max(topIndex + shopOptions.Count - 1, optionIndex) - shopOptions.Count + 1;

                for (int i = 0; i < shopOptions.Count; i++) {
                    int itemIndex = topIndex + i;

                    CardMetadata cardMetadata = shopItemList.ElementAtOrDefault(itemIndex);
                    if (cardMetadata != null) {
                        shopOptions[i].InitText(cardMetadata.cardSO.cardName, $"${cardMetadata.count}");
                    } else {
                        shopOptions[i].InitText("", "");
                    }
                }

                // Display up/down arrows if more can be seen
                upArrow.enabled = topIndex != 0;
                downArrow.enabled = topIndex + shopOptions.Count < shopItemList.Count;

                Debug.Log($"Top index: {topIndex}");

                int uiOptionIndex = optionIndex - topIndex;
                ShopOptionUI shopOption = shopOptions[uiOptionIndex];
                shopCursor.rectTransform.anchoredPosition = new Vector2(
                    shopCursor.rectTransform.anchoredPosition.x,
                    shopOption.RectTransform.anchoredPosition.y
                );

                // Update preview params
                CardSO previewCard = shopItemList[optionIndex].cardSO;
                cardNameText.text = previewCard.cardName;
                costText.text = $"Cost: ${previewCard.cost}";
                incomeText.text = $"Income: ${previewCard.income}";
                lifetimeText.text = $"Turn Limit: {previewCard.turnLimit}";
                descriptionText.text = previewCard.description;
                flavorText.text = previewCard.flavorText;
                previewCardImage.sprite = previewCard.fullSprite;

                // Update inventory text
                currencyText.text = $"Money: ${player.Currency}";
                inventoryText.text = $"Card Count: {player.GetCardCount(previewCard)}";
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

            // Initialize via deconstruction
            (optionsUIListener, shopItemList, player) = args;

            shopGroup.alpha = 1;
        }
    }
}
