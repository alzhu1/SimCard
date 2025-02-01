using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;
using TMPro;

namespace SimCard.CardGame {
    public class ResourceCountUI : MonoBehaviour {
        // Resource counter
        [SerializeField] private TextMeshProUGUI playerResourceCountText;
        [SerializeField] private TextMeshProUGUI opponentResourceCountText;

        // Net change text
        [SerializeField] private TextMeshProUGUI playerNetChangeText;
        [SerializeField] private TextMeshProUGUI opponentNetChangeText;

        private CardGameManager cardGameManager;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCurrencyUpdate.Event += HandlePlayerCurrencyUpdate;
            cardGameManager.EventBus.OnOpponentCurrencyUpdate.Event += HandleOpponentCurrencyUpdate;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCurrencyUpdate.Event -= HandlePlayerCurrencyUpdate;
            cardGameManager.EventBus.OnOpponentCurrencyUpdate.Event -= HandleOpponentCurrencyUpdate;
        }

        void HandlePlayerCurrencyUpdate(EventArgs<int, int> args) => HandleCurrencyUpdate(playerResourceCountText, playerNetChangeText, args.arg1, args.arg2);
        void HandleOpponentCurrencyUpdate(EventArgs<int, int> args) => HandleCurrencyUpdate(opponentResourceCountText, opponentNetChangeText, args.arg1, args.arg2);

        void HandleCurrencyUpdate(TextMeshProUGUI resourceCountText, TextMeshProUGUI netChangeText, int beforeCurrency, int afterCurrency) {
            Debug.Log($"(UI) Before: {beforeCurrency}, after: {afterCurrency}");
            resourceCountText.text = afterCurrency.ToString();

            int netChange = afterCurrency - beforeCurrency;

            if (netChange != 0) {
                netChangeText.CrossFadeAlpha(1, 0f, true);
                netChangeText.text = netChange.ToString("+#;-#;0");
                netChangeText.color = netChange > 0 ? Color.green : Color.red;
                netChangeText.CrossFadeAlpha(0, 1f, false);
            }
        }
    }
}
