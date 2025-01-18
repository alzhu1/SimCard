using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using ReorderableList = UnityEditorInternal.ReorderableList;

namespace SimCard.SimGame {
    public class InteractionEditor : EditorWindow {
        private List<TextAsset> baseFiles;

        private List<TextAsset> filteredFiles;

        [SerializeField]
        private int fileIndex;

        private VisualElement rightPane;

        // JSON
        private JsonSerializer serializer;

        // Store active json + asset
        private TextAsset currAsset;
        private InteractionJSON interactionJson;

        // ReorderableLists for various reasons
        private ReorderableList initPathOptionsList;
        private ReorderableList initPathOptionConditionsList;

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
            rightPane = new VisualElement();
            Label initText = new Label("Select an interactable to start editing.");

            // MINOR: Better style (want to center it)

            rightPane.Add(initText);
            splitView.Add(rightPane);

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
            initPathOptionConditionsList = new(new List<string>(), typeof(string), false, false, false, false) {
                drawNoneElementCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Select a path above to view conditions.");
                },
            };

            Debug.Log($"Json has been parsed for {currAsset.name}");

            rightPane.Clear();
            rightPane.Add(new Label(currAsset.name));

            // Add button to update
            Button updateButton = new Button(Temp) {
                text = "Update Interactable"
            };
            rightPane.Add(updateButton);

            // Add foldout for editing the initial settings
            Foldout initFoldout = new Foldout {
                text = "Init Options",
                value = false
            };

            // Set up reorderable list
            initPathOptionsList = new(interactionJson.Init.PathOptions, typeof(InitInteractionJSON.InitPathOptionsJSON), true, true, true, true) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Path Options");
                },
                onSelectCallback = (ReorderableList l) => {
                    InitInteractionJSON.InitPathOptionsJSON element = l.list[l.index] as InitInteractionJSON.InitPathOptionsJSON;
                    initPathOptionConditionsList = GetListForConditions(element.Conditions);
                    Debug.Log($"next path: {element.NextPath}");
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    InitInteractionJSON.InitPathOptionsJSON element = interactionJson.Init.PathOptions[index];
                    rect.y += 2;
                    element.NextPath = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"Path {index}", element.NextPath);
                }
            };

            initFoldout.Add(new IMGUIContainer(() => {
                initPathOptionsList.DoLayoutList();
                initPathOptionConditionsList.DoLayoutList();
            }));

            rightPane.Add(initFoldout);
        }

        ReorderableList GetListForConditions(Dictionary<ConditionKeyJSON, string> dict) {
            // Create a List from the dictionary, need this so that ReorderableList can wrap it/edit it
            List<(ConditionKeyJSON, string)> dictList = dict.Select(pair => (pair.Key, pair.Value)).ToList();

            return new(dictList, typeof((ConditionKeyJSON, string)), true, true, true, true) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Conditions");
                },
                onRemoveCallback = (ReorderableList l) => {
                    (ConditionKeyJSON key, string value) = ((ConditionKeyJSON, string))l.list[l.index];
                    dict.Remove(key);
                    l.list.RemoveAt(l.index);
                },
                onAddDropdownCallback = (Rect rect, ReorderableList l) => {
                    GenericMenu menu = new GenericMenu();

                    IEnumerable<ConditionKeyJSON> conditionKeysLeft = System.Enum.GetValues(typeof(ConditionKeyJSON)).Cast<ConditionKeyJSON>().Except(dict.Keys);
                    foreach (ConditionKeyJSON conditionKey in conditionKeysLeft) {
                        menu.AddItem(new GUIContent(conditionKey.ToString()), false, () => {
                            dict.Add(conditionKey, "");
                            l.list.Add((conditionKey, ""));
                        });
                    }

                    menu.ShowAsContext();
                },
                onCanAddCallback = (ReorderableList l) => {
                    return l.count < System.Enum.GetValues(typeof(ConditionKeyJSON)).Length;
                },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    (ConditionKeyJSON key, string value) element = dictList[index];

                    rect.y += 2;
                    string previousValue = element.value;
                    element.value = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.key.ToString(), element.value);

                    Debug.Log($"Prev value = {previousValue}, next value should be {element.value}");

                    if (!element.value.Equals(previousValue)) {
                        dict[element.key] = element.value;
                        dictList[index] = element;

                        Debug.Log($"Key: {element.key}, Value: {element.value}");
                    }
                }
            };
        }

        void Temp() {
            Debug.Log($"Name: {currAsset}");

            interactionJson ??= JObject.Parse(currAsset.text).ToObject<InteractionJSON>(serializer);

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
