using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    public enum ScriptTag {
        Bool,
        Quest,
        PathTraversedCount,
    }

    [System.Serializable]
    public class InteractionTag {
        public ScriptTag tag;
        public bool boolArg;
        public string strArg;
        public int intArg;

        // TODO: Need more fine grained control on conditions?
        // e.g. negating condition, or equals for int
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(InteractionTag))]
    public class InteractionTagDrawer : PropertyDrawer {
        public override VisualElement CreatePropertyGUI(SerializedProperty property) {
            VisualElement container = new VisualElement();

            PropertyField tagField = new PropertyField(property.FindPropertyRelative("tag"));
            container.Add(tagField);

            PropertyField boolArgField = new PropertyField(
                property.FindPropertyRelative("boolArg")
            );
            PropertyField strArgField = new PropertyField(property.FindPropertyRelative("strArg"));
            PropertyField intArgField = new PropertyField(property.FindPropertyRelative("intArg"));
            PropertyField[] argFields = new[] { boolArgField, strArgField, intArgField };

            // Add all fields above to the container
            Array.ForEach(argFields, container.Add);

            // This should "repaint" on change (i.e. show and hide elements)
            tagField.RegisterValueChangeCallback(
                (changeEvent) => {
                    ScriptTag scriptTag = (ScriptTag)changeEvent.changedProperty.intValue;

                    foreach (PropertyField argField in argFields) {
                        argField.style.display = DisplayStyle.None;
                    }

                    switch (scriptTag) {
                        case ScriptTag.Bool:
                            boolArgField.style.display = DisplayStyle.Flex;
                            break;

                        case ScriptTag.PathTraversedCount:
                            intArgField.style.display = DisplayStyle.Flex;
                            strArgField.style.display = DisplayStyle.Flex;
                            break;

                        default:
                            strArgField.style.display = DisplayStyle.Flex;
                            break;
                    }
                }
            );

            return container;
        }
    }
#endif
}
