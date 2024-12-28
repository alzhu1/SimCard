using System.Collections.Generic;
using System.Linq;
using SimCard.CardGame;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.Common {
    public class EffectApplier {
        public Card source;
        public Effect effect;

        public EffectApplier(Card source, Effect effect) =>
            (this.source, this.effect) = (source, effect);

        public void ApplyEffect(Card target) => effect.Apply(source, target);
    }

    [System.Serializable]
    public abstract class Effect {
        // Self effect requires no selection for target
        public bool selfEffect;

        // Active: every owner upkeep this is triggered. Else passive: on first play.
        public bool active;

        // Timed duration of the effect, assuming active. 0 = forever.
        public int duration;

        public abstract void Apply(Card source, Card target);
    }

#if UNITY_EDITOR
    public class EffectDropdown : AdvancedDropdown {
        private static List<string> EFFECT_TYPE_NAMES;
        private static Dictionary<string, System.Type> TYPE_MAP;

        private Button effectButton;
        private SerializedProperty effectProperty;
        private PropertyField effectPropertyField;

        static EffectDropdown() {
            Debug.Log("Constructor init");
            List<System.Type> effectTypes = TypeCache.GetTypesDerivedFrom<Effect>().ToList();
            effectTypes.Sort((a, b) => a.Name.CompareTo(b.Name));

            EFFECT_TYPE_NAMES = effectTypes.Select(x => x.Name).ToList();
            TYPE_MAP = effectTypes.ToDictionary(x => x.Name);
        }

        public EffectDropdown(
            AdvancedDropdownState state,
            Button effectButton,
            SerializedProperty effectProperty,
            PropertyField effectPropertyField
        ) : base(state) {
            this.effectButton = effectButton;
            this.effectProperty = effectProperty;
            this.effectPropertyField = effectPropertyField;
        }

        protected override AdvancedDropdownItem BuildRoot() {
            AdvancedDropdownItem root = new AdvancedDropdownItem("Effects");

            foreach (string typeName in EFFECT_TYPE_NAMES) {
                root.AddChild(new AdvancedDropdownItem(typeName));
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item) {
            base.ItemSelected(item);
            Debug.Log($"Selected an item: {item} ({item.id} + {item.name})");

            object newObject = null;
            if (TYPE_MAP.TryGetValue(item.name, out System.Type type)) {
                newObject = System.Runtime.Serialization.FormatterServices.GetUninitializedObject(
                    type
                );
            }
            Debug.Log($"Setting to object of type {newObject}");

            effectButton.text = item.name;

            effectProperty.managedReferenceValue = newObject;
            effectProperty.serializedObject.ApplyModifiedProperties();
            effectProperty.serializedObject.Update();
            effectPropertyField.BindProperty(effectProperty);
        }
    }

    // Inspired by https://github.com/AlexeyTaranov/SerializeReferenceDropdown/blob/master/Editor/PropertyDrawer.cs
    [CustomPropertyDrawer(typeof(Effect))]
    public class EffectDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement container = new VisualElement();

            // Force property to update to display effect
            PropertyField propertyField = new PropertyField();
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();
            propertyField.BindProperty(property);

            // Create button dropdown
            Button button = new Button();
            string currTypeName = property.managedReferenceFullTypename.Split(".").Last();
            button.text = currTypeName?.Length > 0 ? currTypeName : "(null)";
            button.displayTooltipWhenElided = false;
            button.clickable.clicked += ShowDropdown;

            container.Add(button);
            container.Add(propertyField);

            void ShowDropdown() {
                EffectDropdown dropdown = new EffectDropdown(
                    new AdvancedDropdownState(),
                    button,
                    property,
                    propertyField
                );

                // Set position/size
                var buttonMatrix = container.worldTransform;
                var buttonRect = new Rect(
                    new Vector3(buttonMatrix.m03, buttonMatrix.m13, buttonMatrix.m23),
                    container.contentRect.size
                );
                dropdown.Show(buttonRect);
            }

            return container;
        }
    }
#endif
}
