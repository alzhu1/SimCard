using UnityEngine;

namespace SimCard.Common {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Card/Supporter")]
    public class SupporterCard : CardSO {
        [SerializeReference] public Effect secondaryEffect;
    }
}
