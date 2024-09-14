using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimCard.Common {
    [CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/ResourceEntity")]
    public class ResourceEntitySO : EntitySO {
        public override EntityType entityType => EntityType.Resource;
    }
}
