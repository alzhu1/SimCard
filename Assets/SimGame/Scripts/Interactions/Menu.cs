using System.Collections.Generic;
using SimCard.Common;

namespace SimCard.SimGame {
    public enum MenuOptions {
        Deck,
        Exit
    }

    public class Menu : OptionsUIListener {
        public class MenuOption {
            public const string Deck = "Deck";
            public const string Exit = "Exit";
        }

        public int OptionIndex { get; private set; }

        public string SelectedOption => menuOptions[OptionIndex].Item1;

        private GameEventAction<EventArgs<OptionsUIListener, List<(string, bool)>>> DisplayInteractionOptions;
        private List<(string, bool)> menuOptions;

        public Menu(
            GameEventAction<EventArgs<OptionsUIListener, List<(string, bool)>>> DisplayInteractionOptions
        ) {
            this.DisplayInteractionOptions = DisplayInteractionOptions;

            menuOptions = new List<(string, bool)>{
                (MenuOption.Deck, true),
                (MenuOption.Exit, true)
            };
            OptionIndex = 0;
        }

        public void OpenMenu() {
            DisplayInteractionOptions.Raise(new(this, menuOptions));
        }

        public void CloseMenu() {
            DisplayInteractionOptions.Raise(null);
        }

        public void UpdateOptionIndex(int diff) {
            OptionIndex = (menuOptions.Count + OptionIndex + diff) % menuOptions.Count;
        }
    }
}
