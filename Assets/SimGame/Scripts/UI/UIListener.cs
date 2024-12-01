namespace SimCard.SimGame {
    // Presenter interface for UI to work with
    // This allows us to limit the methods it will work with
    public interface InteractUIListener {
        public Interaction CurrInteraction { get; }
        public int MaxVisibleCharacters { get; }
    }

    public interface OptionsUIListener {
        public int OptionIndex { get; }
    }
}
