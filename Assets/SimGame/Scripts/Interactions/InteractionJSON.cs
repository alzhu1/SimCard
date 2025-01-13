using System.Collections.Generic;

namespace SimCard.SimGame {
    public class InteractionJSON {
        public InitInteractionJSON Init { get; set; }
        public Dictionary<string, List<InteractionNodeJSON>> Paths { get; set; }
    }

    // JSON stuff
    public enum ConditionKeyJSON {
        Bool,
        Energy,
        PathTraversedCount
    }

    public class InitInteractionJSON {
        public List<InitPathOptionsJSON> PathOptions { get; set; }

        public class InitPathOptionsJSON {
            public string NextPath { get; set; }
            public Dictionary<ConditionKeyJSON, string> Conditions { get; set; }
        }
    }

    public class InteractionNodeJSON {
        public Dictionary<ConditionKeyJSON, string> IncomingConditions { get; set; }
        public string Text { get; set; }
        public List<string> EventTriggers { get; set; }
        public List<InteractionOptionJSON> Options;

        public class InteractionOptionJSON {
            public string OptionText { get; set; }
            public string NextPath { get; set; }
            public Dictionary<ConditionKeyJSON, string> Conditions { get; set; }
        }
    }

    // struct InteractionCondition {
    //     public static string Bool = "Bool";
    //     public static string Quest = "Quest";
    //     public static string PathTraversedCount = "PathTraversedCount";
    // }


}
