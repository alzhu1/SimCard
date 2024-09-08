using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.CardGame {
    public class Icons : MonoBehaviour {
        [SerializeField] private Image previewIcon;
        [SerializeField] private Image summonIcon;

        private CardGameManager cardGameManager;
        private Canvas canvas;

        private Dictionary<string, Image> iconMap;

        private Card currCard;
        private Image currIcon;

        void Awake() {
            cardGameManager = GetComponentInParent<CardGameManager>();
            canvas = GetComponentInParent<Canvas>();

            iconMap = new Dictionary<string, Image> {
                { "Preview", previewIcon },
                { "Summon", summonIcon }
            };
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event += DisplayIcons;
            cardGameManager.EventBus.OnCardIconHover.Event += HandleIconSelect;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event -= DisplayIcons;
            cardGameManager.EventBus.OnCardIconHover.Event -= HandleIconSelect;
        }

        void Update() {
            if (currIcon) {
                currIcon.rectTransform.sizeDelta = Vector2.one * (25 + Mathf.Sin(Time.time * 5) * 5);
            }
        }

        void DisplayIcons(CardArgs args) {
            currCard = args.card;
            bool isSummonAllowed = args.isSummonAllowed;

            if (currCard == null) {
                // Disable
                previewIcon.enabled = false;
                summonIcon.enabled = false;
                return;
            }

            previewIcon.enabled = true;
            summonIcon.enabled = isSummonAllowed;

            List<Image> icons = new List<Image>();
            icons.Add(previewIcon);
            if (isSummonAllowed) {
                icons.Add(summonIcon);
            }

            // Place icons above
            PositionIcons(icons);
        }

        void PositionIcons(List<Image> icons) {
            float offset = 1;
            float leftEdgeX = ((1 - icons.Count) * offset / 2) + currCard.transform.position.x;
            for (int i = 0; i < icons.Count; i++) {
                Image icon = icons[i];
                Vector3 pos = Vector3.zero;

                pos.x = (leftEdgeX + (offset * i)) / canvas.transform.localScale.x;
                pos.y = (currCard.transform.position.y + 1) / canvas.transform.localScale.y;
                pos.z /= canvas.transform.localScale.z;

                icon.rectTransform.anchoredPosition = pos;
            }
        }

        void HandleIconSelect(IconArgs args) {
            if (currIcon) {
                currIcon.rectTransform.sizeDelta = Vector2.one * 25;
            }

            currIcon = iconMap.GetValueOrDefault(args.iconName ?? "", null);
        }
    }
}
