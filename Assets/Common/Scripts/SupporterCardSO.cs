using UnityEngine;

namespace SimCard.Common {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card/Supporter")]
    public class SupporterCard : CardSOV2 {
        [SerializeReference] public Effect secondaryEffect;
    }
}
