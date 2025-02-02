using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimCard.Common;
using UnityEngine;
using UnityEngine.UI;

namespace SimCard.CardGame {
    public class IconsUI : MonoBehaviour {
        [SerializeField]
        private Image previewIcon;

        [SerializeField]
        private Image summonIcon;

        [SerializeField]
        private Image fireIcon;

        [SerializeField]
        private Image surrenderIcon;

        [SerializeField]
        private float iconSize = 25;

        [SerializeField]
        private float iconPulseSpeed = 5;

        [SerializeField]
        private float iconAmplitude = 5;

        private Canvas canvas;
        private CardGameManager cardGameManager;

        private Dictionary<PlayerCardAction, Image> iconMap;

        private CardGraphSelectable currItem;
        private Image currIcon;

        void Awake() {
            canvas = GetComponentInParent<Canvas>();
            cardGameManager = GetComponentInParent<CardGameManager>();

            iconMap = new Dictionary<PlayerCardAction, Image>
            {
                { PlayerCardAction.None, null },
                { PlayerCardAction.Preview, previewIcon },
                { PlayerCardAction.Summon, summonIcon },
                { PlayerCardAction.Fire, fireIcon },
                { PlayerCardAction.Surrender, surrenderIcon }
            };
        }

        void Start() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event += DisplayIcons;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event += SelectIcon;
        }

        void OnDestroy() {
            cardGameManager.EventBus.OnPlayerCardSelect.Event -= DisplayIcons;
            cardGameManager.EventBus.OnPlayerCardActionHover.Event -= SelectIcon;
        }

        void Update() {
            if (currIcon) {
                currIcon.rectTransform.sizeDelta =
                    Vector2.one
                    * (iconSize + Mathf.Sin(Time.time * iconPulseSpeed) * iconAmplitude);
            }
        }

        void DisplayIcons(EventArgs<CardGraphSelectable, List<PlayerCardAction>> args) {
            (CardGraphSelectable selectable, List<PlayerCardAction> actions) = args;
            currItem = selectable;
            List<Image> icons = actions.Select(action => {
                Image icon = iconMap[action];
                icon.enabled = true;
                return icon;
            })
            .ToList();

            if (currItem == null) {
                foreach (Image icon in iconMap.Values) {
                    if (icon != null) {
                        icon.enabled = false;
                    }
                }
                return;
            }

            // Place icons above
            PositionIcons(icons);
        }

        void PositionIcons(List<Image> icons) {
            float offset = 1;
            float leftEdgeX = ((1 - icons.Count) * offset / 2) + currItem.transform.position.x;
            for (int i = 0; i < icons.Count; i++) {
                Image icon = icons[i];
                Vector3 pos = Vector3.zero;

                pos.x = (leftEdgeX + (offset * i)) / canvas.transform.localScale.x;
                pos.y = (currItem.transform.position.y + 1) / canvas.transform.localScale.y;
                pos.z /= canvas.transform.localScale.z;

                icon.rectTransform.anchoredPosition = pos;
            }
        }

        void SelectIcon(EventArgs<PlayerCardAction> args) {
            if (currIcon) {
                currIcon.rectTransform.sizeDelta = Vector2.one * iconSize;
            }
            currIcon = iconMap[args.argument];
        }
    }
}
