using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimCard.Common;
using TMPro;

namespace SimCard.CardGame {
    public class ResourceCountUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI playerResourceCountText;
        [SerializeField] private TextMeshProUGUI opponentResourceCountText;

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

        void HandlePlayerCurrencyUpdate(EventArgs<int, int> args) => HandleCurrencyUpdate(playerResourceCountText, args.arg1, args.arg2);
        void HandleOpponentCurrencyUpdate(EventArgs<int, int> args) => HandleCurrencyUpdate(opponentResourceCountText, args.arg1, args.arg2);

        void HandleCurrencyUpdate(TextMeshProUGUI text, int beforeCurrency, int afterCurrency) {
            Debug.Log($"(UI) Before: {beforeCurrency}, after: {afterCurrency}");
            text.text = afterCurrency.ToString();
        }
    }
}
