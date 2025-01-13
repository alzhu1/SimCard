using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SimCard.SimGame {
    public class InteractionEditor : EditorWindow {
        private List<TextAsset> baseFiles;

        private List<TextAsset> filteredFiles;

        [SerializeField]
        private int fileIndex;

        private ScrollView rightPane;

        // JSON
        private JsonSerializer serializer;

        // Store active json + asset
        private TextAsset currAsset;
        private InteractionJSON interactionJson;

        void OnEnable() {
            baseFiles = AssetDatabase
                .FindAssets("l:Interactable")
                .Select(guid =>
                    AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(guid))
                )
                .ToList();
            filteredFiles = baseFiles;

            serializer = JsonSerializer.Create(
                new() {
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>()
                    {
                        new Newtonsoft.Json.Converters.StringEnumConverter(),
                    },
                }
            );

            // Manually do a LINQ JSON parse to cache things on startup
            JObject.Parse(baseFiles[0].text).ToObject<InteractionJSON>(serializer);
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

            // Create left side
            VisualElement leftPane = new VisualElement();

            // Toolbar with search field
            Toolbar toolbar = new Toolbar();
            ToolbarSearchField toolbarSearchField = new ToolbarSearchField();
            toolbarSearchField.style.flexShrink = 1;
            toolbar.Add(toolbarSearchField);

            // List view containing filtered files
            ListView listView = new ListView() {
                selectionType = SelectionType.Single,
                itemsSource = filteredFiles,
                makeItem = () => new Label(),
                bindItem = (item, index) => {
                    (item as Label).text = filteredFiles[index].name;
                },
                selectedIndex = fileIndex,
            };
            listView.style.paddingLeft = 5;
            listView.style.paddingRight = 5;

            // On search update, change both list view source and filtered files
            toolbarSearchField.RegisterValueChangedCallback(evt => {
                filteredFiles = baseFiles.Where(file => file.name.Contains(evt.newValue)).ToList();
                listView.itemsSource = filteredFiles;

                // Update file index to prevent confusion (right view should still display old)
                fileIndex = filteredFiles.IndexOf(currAsset);
                listView.selectedIndex = fileIndex;

                listView.RefreshItems();
                Debug.Log($"File index is {fileIndex}");
            });

            // Run file selection on change (also update fileIndex for hot reload?)
            listView.selectionChanged += (items) => {
                fileIndex = listView.selectedIndex;
                OnFileSelection(items);
            };

            // Add toolbar on top, then list view for left side
            leftPane.Add(toolbar);
            leftPane.Add(listView);
            splitView.Add(leftPane);

            // Create right side scroll view
            rightPane = new ScrollView(ScrollViewMode.VerticalAndHorizontal);
            splitView.Add(rightPane);

            Button button = new() { text = "Update" };
            button.clicked += Temp;
            rightPane.Add(button);

            // TODO: Next steps:
            //   1. Add UI to make changes to these individual elements
            //   2. Serialize the intermediate representation into JSON
            //   3. Add support for serializing into a ScriptableObject too
        }

        void OnFileSelection(IEnumerable<object> items) {
            if (items.Count() == 0)
                return;

            currAsset = items.First() as TextAsset;
            interactionJson = JObject.Parse(currAsset.text).ToObject<InteractionJSON>(serializer);

            Debug.Log($"Json has been parsed for {currAsset.name}");
        }

        void Temp() {
            Debug.Log($"Name: {currAsset}");

            interactionJson ??= JObject.Parse(currAsset.text).ToObject<InteractionJSON>(serializer);

            // TODO: Remove this line, include for testing for now
            interactionJson.Paths.Add("Fake Path", new());

            using (
                System.IO.FileStream fs = System.IO.File.Open(
                    AssetDatabase.GetAssetPath(currAsset),
                    System.IO.FileMode.OpenOrCreate
                )
            ) {
                using (System.IO.StreamWriter sw = new(fs)) {
                    using (JsonTextWriter jw = new(sw)) {
                        jw.Formatting = Formatting.Indented;
                        jw.IndentChar = ' ';
                        jw.Indentation = 4;

                        serializer.Serialize(jw, JObject.FromObject(interactionJson));
                    }
                }
            }

            EditorUtility.SetDirty(currAsset);
            AssetDatabase.Refresh();
        }
    }
}
