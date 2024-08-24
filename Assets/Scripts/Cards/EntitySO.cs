using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EntityType {
    NORMAL,
    RESOURCE
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/Entity")]
public class EntitySO : ScriptableObject {
    public string entityName;

    public virtual EntityType entityType => EntityType.NORMAL;

    public bool IsResource() {
        return entityType == EntityType.RESOURCE;
    }
}

[CreateAssetMenu(fileName = "Card", menuName = "ScriptableObjects/ResourceEntity")]
public class ResourceEntitySO : EntitySO {
    public override EntityType entityType => EntityType.RESOURCE;
}