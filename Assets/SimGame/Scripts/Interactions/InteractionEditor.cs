using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    public class InteractionEditor : EditorWindow {
        private static List<TextAsset> INTERACTABLE_FILES;

        [SerializeField]
        private int fileIndex;

        private ListView leftPane;
        private ScrollView rightPane;

        // Store active json
        private InteractionJSON interactionJson;

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

            leftPane.selectedIndex = fileIndex;
            leftPane.selectionChanged += OnFileSelection;

            Button button = new() { text = "Update" };
            button.clicked += Temp;
            rightPane.Add(button);

            // TODO: Next steps:
            //   1. Add UI to make changes to these individual elements
            //   2. Serialize the intermediate representation into JSON
            //   3. Add support for serializing into a ScriptableObject too
        }

        void OnFileSelection(IEnumerable<object> items) {
            fileIndex = leftPane.selectedIndex;
            TextAsset asset = items.First() as TextAsset;
            JObject contents = JObject.Parse(asset.text);
            interactionJson = contents.ToObject<InteractionJSON>();

            Debug.Log($"Json has been parsed for {asset.name}");
        }

        void Temp() {
            TextAsset file = INTERACTABLE_FILES[fileIndex];
            Debug.Log($"Name: {file}");

            // TODO: Remove this line, include for testing for now
            interactionJson.Paths.Add("Fake Path", new());

            using (
                System.IO.FileStream fs = System.IO.File.Open(
                    AssetDatabase.GetAssetPath(file),
                    System.IO.FileMode.OpenOrCreate
                )
            ) {
                using (System.IO.StreamWriter sw = new(fs)) {
                    using (JsonTextWriter jw = new(sw)) {
                        jw.Formatting = Formatting.Indented;
                        jw.IndentChar = ' ';
                        jw.Indentation = 4;

                        JsonSerializer serializer = JsonSerializer.Create(
                            new() {
                                Formatting = Formatting.Indented,
                                Converters = new List<JsonConverter>()
                                {
                                    new Newtonsoft.Json.Converters.StringEnumConverter(),
                                },
                            }
                        );
                        serializer.Serialize(jw, JObject.FromObject(interactionJson));
                    }
                }
            }

            EditorUtility.SetDirty(file);
            AssetDatabase.Refresh();
        }
    }
}
