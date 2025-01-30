using System.Collections.Generic;

namespace SimCard.SimGame {
    public class InteractionJSON {
        public InitInteractionJSON Init { get; set; } = new();
        public Dictionary<string, List<InteractionNodeJSON>> Paths { get; set; } = new();
    }

    // JSON stuff
    public enum ConditionKey {
        Bool,
        Energy,
        PathTraversedCount
    }

    public class InitInteractionJSON {
        public List<InitPathOptionsJSON> PathOptions { get; set; } = new();

        public class InitPathOptionsJSON {
            public string NextPath { get; set; }
            public Dictionary<ConditionKey, string> Conditions { get; set; } = new();
        }
    }

    public class InteractionNodeJSON {
        public Dictionary<ConditionKey, string> IncomingConditions { get; set; } = new();
        public string Text { get; set; }
        public List<string> EventTriggers { get; set; } = new();
        public List<InteractionOptionJSON> Options { get; set; } = new();

        public class InteractionOptionJSON {
            public string OptionText { get; set; }
            public string NextPath { get; set; }
            public string FallbackPath { get; set; }
            public Dictionary<ConditionKey, string> Conditions { get; set; } = new();
        }
    }
}
