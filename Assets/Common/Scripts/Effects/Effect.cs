using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using SimCard.CardGame;

namespace SimCard.Common {
    [System.Serializable]
    public abstract class Effect {
        // TODO: Also might need a way to indicate to outside Card
        // whether this effect is meant to apply to another card, or itself

        // TODO: Should add an optional timer object
        // Should be fine to operate on 1 card at a time
        // Include source + target, so the effect itself can keep track of whether it needs to apply it
        // Assuming it's an effect that applies on self
        public abstract void ApplyEffect(Card source, Card target);
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Effect))]
    public class EffectDrawer : PropertyDrawer {
        private static List<System.Type> EFFECT_TYPES;
        private static List<string> EFFECT_TYPE_NAMES;
        private static Dictionary<string, System.Type> TYPE_MAP;

        static EffectDrawer() {
            Debug.Log("Constructor init");
            EFFECT_TYPES = TypeCache.GetTypesDerivedFrom<Effect>().ToList();
            EFFECT_TYPES.Sort((a, b) => a.Name.CompareTo(b.Name));

            EFFECT_TYPE_NAMES = new List<string> { "null " };
            EFFECT_TYPE_NAMES.AddRange(EFFECT_TYPES.Select(x => x.Name));

            TYPE_MAP = EFFECT_TYPES.ToDictionary(x => x.Name);
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement container = new VisualElement();

            PropertyField propertyField = new PropertyField();
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            propertyField.BindProperty(property);

            // TODO: Not a fan of this approach, should look into AdvancedDropdown and a helper class
            string currPropertyTypeName = property.managedReferenceFullTypename;
            string currTypeName = currPropertyTypeName.Split(".").Last();
            int dropdownIndex = Mathf.Max(EFFECT_TYPE_NAMES.FindIndex((x) => currTypeName.Equals(x)), 0);

            DropdownField field = new DropdownField(property.name, EFFECT_TYPE_NAMES, dropdownIndex);

            container.Add(field);
            container.Add(propertyField);

            field.RegisterValueChangedCallback((changeEvent) => {
                object newObject = null;
                if (TYPE_MAP.TryGetValue(changeEvent.newValue, out System.Type type)) {
                    newObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(type);
                }
                Debug.Log($"Setting to object of type {newObject}");

                property.managedReferenceValue = newObject;
                property.serializedObject.ApplyModifiedProperties();
                property.serializedObject.Update();
                propertyField.BindProperty(property);
            });

            Debug.Log($"Outer part, binding path: {propertyField.bindingPath}");

            return container;
        }
    }
#endif
}
