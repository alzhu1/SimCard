using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    public class InteractionEditor : EditorWindow {
        private static List<TextAsset> INTERACTABLE_FILES;

        private ListView leftPane;
        private ScrollView rightPane;

        private int fileIndex;

        void OnEnable() {
            INTERACTABLE_FILES = AssetDatabase
                .FindAssets("l:Interactable")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid))
                )
                .ToList();
        }

        [MenuItem("Window/Interactions/Interaction Editor")]
        public static void ShowMyEditor() {
            EditorWindow wnd = GetWindow<InteractionEditor>();
            wnd.titleContent = new GUIContent("Interaction Editor");
            wnd.minSize = new Vector2(450, 200);
            wnd.maxSize = new Vector2(1920, 720);
        }

        public void CreateGUI() {
            TwoPaneSplitView splitView = new TwoPaneSplitView(
                0,
                250,
                TwoPaneSplitViewOrientation.Horizontal
            );
            rootVisualElement.Add(splitView);

            leftPane = new ListView() {
                selectionType = SelectionType.Single,
                itemsSource = INTERACTABLE_FILES,
                makeItem = () => new Label(),
                bindItem = (item, index) => {
                    (item as Label).text = INTERACTABLE_FILES[index].name;
                },
                selectedIndex = fileIndex,
            };
            splitView.Add(leftPane);

            rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            splitView.Add(rightPane);

            leftPane.selectedIndicesChanged += (items) => {
                fileIndex = items.First();
            };

            Button button = new() { text = "Update" };
            button.clicked += Temp;
            rightPane.Add(button);

            // TODO: Next steps:
            //   1. Convert the JSON into something parseable (e.g. some kind of intermediate representation) (deserialization)
            //   2. Add UI to make changes to these individual elements
            //   3. Serialize the intermediate representation into JSON
            //   4. Add support for serializing into a ScriptableObject too
        }

        void Temp() {
            Debug.Log($"Name: {INTERACTABLE_FILES[fileIndex]}");

            string json = $"{{\"random\": {Random.Range(0, 1000)}}}";

            System.IO.File.WriteAllText(
                AssetDatabase.GetAssetPath(INTERACTABLE_FILES[fileIndex]),
                json
            );
            EditorUtility.SetDirty(INTERACTABLE_FILES[fileIndex]);
            AssetDatabase.Refresh();
        }
    }
}
