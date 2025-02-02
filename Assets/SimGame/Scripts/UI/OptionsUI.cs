using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimCard.Common;
using TMPro;
using System.Linq;

namespace SimCard.SimGame {
    public interface OptionsUIListener {
        public int OptionIndex { get; }
    }

    public class OptionsUI : MonoBehaviour {
        [SerializeField] private Image optionCursor;

        private SimGameManager simGameManager;

        private CanvasGroup optionsGroup;
        private List<TextMeshProUGUI> optionTexts;

        private OptionsUIListener optionsUIListener { get; set; }

        void Awake() {
            simGameManager = GetComponentInParent<SimGameManager>();

            optionsGroup = GetComponent<CanvasGroup>();
            optionTexts = optionsGroup.GetComponentsInChildren<TextMeshProUGUI>().ToList();
        }

        void Start() {
            simGameManager.EventBus.OnDisplayInteractOptions.Event += DisplayInteractOptions;
        }

        void OnDestroy() {
            simGameManager.EventBus.OnDisplayInteractOptions.Event -= DisplayInteractOptions;
        }

        void Update() {
            if (optionsUIListener != null) {
                Vector2 pos = optionCursor.rectTransform.anchoredPosition;
                pos.y = optionTexts[optionsUIListener.OptionIndex].rectTransform.anchoredPosition.y;
                optionCursor.rectTransform.anchoredPosition = pos;
            }
        }

        void DisplayInteractOptions(EventArgs<OptionsUIListener, List<(string, bool)>> args) {
            // Reset if args are null
            if (args == null) {
                optionsUIListener = null;
                optionsGroup.alpha = 0;
                return;
            }

            (OptionsUIListener listener, List<(string, bool)> options) = args;
            optionsUIListener = listener;
            optionsGroup.alpha = 1;
            for (int i = 0; i < optionTexts.Count; i++) {
                (string option, bool allowed) = options.ElementAtOrDefault(i);
                if (option != null) {
                    optionTexts[i].gameObject.SetActive(true);
                    optionTexts[i].text = option;
                    optionTexts[i].color = allowed ? Color.white : Color.gray;
                } else {
                    optionTexts[i].gameObject.SetActive(false);
                }
            }
        }
    }
}
