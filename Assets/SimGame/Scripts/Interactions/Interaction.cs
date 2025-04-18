using System.Collections.Generic;

namespace SimCard.SimGame {
    public class Interaction {
        public InitInteraction Init { get; set; } = new();
        public Dictionary<string, List<InteractionNode>> Paths { get; set; } = new();
    }

    // JSON stuff
    public enum ConditionKey {
        Bool,
        PathTraversedCount,
        Cost
    }

    public class InitInteraction {
        public List<InitPathOptions> PathOptions { get; set; } = new();

        public class InitPathOptions {
            public string NextPath { get; set; }
            public Dictionary<ConditionKey, string> Conditions { get; set; } = new();
        }
    }

    public class InteractionNode {
        public Dictionary<ConditionKey, string> IncomingConditions { get; set; } = new();
        public string Text { get; set; } = "";
        public List<string> EventTriggers { get; set; } = new();
        public List<string> EndingEventTriggers { get; set; } = new();
        public List<InteractionOption> Options { get; set; } = new();

        public class InteractionOption {
            public string OptionText { get; set; } = "";
            public string NextPath { get; set; } = "";
            public string FallbackPath { get; set; } = "";
            public Dictionary<ConditionKey, string> Conditions { get; set; } = new();
        }
    }
}
