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

        [SerializeField]
        private int interactionPathIndex;

        private VisualElement initOptionsPane;
        private VisualElement interactionPathsPane;
        private VisualElement interactionPathEditorPane;

        // JSON
        private JsonSerializer serializer;

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

            // Create file list view and add its parent
            ListView fileListView = CreateListView(baseFiles, filteredFiles, (file) => file.name);
            fileListView.selectedIndex = fileIndex;
            fileListView.selectionChanged += (items) => {
                fileIndex = fileListView.selectedIndex;
                OnFileSelection(items);
            };
            splitView.Add(fileListView.parent);

            // Create right side scroll view
            TwoPaneSplitView rightPane = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Vertical);

            // On top, we have init options editor
            initOptionsPane = new VisualElement();
            // MINOR: Better style (want to center it)
            initOptionsPane.Add(new Label("Select an interactable to start editing."));

            // No items yet, init as empty
            interactionPathsPane = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);
            ListView interactionPathListView = CreateListView(new List<string>(), new List<string>(), (item) => item);
            interactionPathsPane.Add(interactionPathListView.parent);
            interactionPathsPane.Add(new VisualElement());

            rightPane.Add(initOptionsPane);
            rightPane.Add(interactionPathsPane);

            splitView.Add(rightPane);
        }

        void OnFileSelection(IEnumerable<object> items) {
            if (items.Count() == 0)
                return;

            TextAsset currAsset = items.First() as TextAsset;
            InteractionJSON interactionJson = JObject.Parse(currAsset.text).ToObject<InteractionJSON>(serializer);
            ReorderableList initPathOptionConditionsList = GetDefaultEmptyList("Select a path above to view conditions.");

            Debug.Log($"Json has been parsed for {currAsset.name}");

            // On top, init options pane
            initOptionsPane.Clear();
            initOptionsPane.Add(new Label(currAsset.name));

            // Add button to update
            Button updateButton = new Button() {
                text = "Update Interactable",
            };
            updateButton.clicked += () => {
                UpdateInteraction(currAsset, interactionJson);
            };
            initOptionsPane.Add(updateButton);

            // Set up reorderable list
            ReorderableList initPathOptionsList = new(interactionJson.Init.PathOptions, typeof(InitInteractionJSON.InitPathOptionsJSON), true, true, true, true) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Path Options");
                },
                onSelectCallback = (ReorderableList l) => {
                    if (l.index >= l.list.Count) {
                        initPathOptionConditionsList = GetDefaultEmptyList("Select a path above to view conditions.");
                        return;
                    }
                    InitInteractionJSON.InitPathOptionsJSON element = interactionJson.Init.PathOptions[l.index];
                    initPathOptionConditionsList = GetListForConditions(element.Conditions);
                },
                onChangedCallback = CallSelectOnChangeCallback,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    InitInteractionJSON.InitPathOptionsJSON element = interactionJson.Init.PathOptions[index];
                    rect.y += 2;
                    element.NextPath = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"Path {index}", element.NextPath);
                }
            };

            initOptionsPane.Add(new IMGUIContainer(() => {
                initPathOptionsList.DoLayoutList();
                initPathOptionConditionsList.DoLayoutList();
            }));

            // On bottom, we have interactionPaths editor
            List<string> interactionPathNames = interactionJson.Paths.Keys.ToList();

            // Recreate the interaction paths pane to avoid glitchy dragging when updating children
            VisualElement rightPane = interactionPathsPane.parent;
            rightPane.RemoveAt(1);
            interactionPathsPane = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            ListView interactionPathListView = CreateListView(interactionPathNames, interactionPathNames, (item) => item);
            interactionPathListView.selectedIndex = interactionPathIndex;
            interactionPathListView.selectionChanged += (items) => {
                interactionPathIndex = interactionPathListView.selectedIndex;
                OnInteractionPathSelection(items, interactionJson);
            };
            interactionPathsPane.Add(interactionPathListView.parent);

            // TODO: Add content to the second pane for interaction paths
            interactionPathEditorPane = new ScrollView(ScrollViewMode.Vertical);
            interactionPathsPane.Add(interactionPathEditorPane);
            rightPane.Add(interactionPathsPane);
        }

        void OnInteractionPathSelection(IEnumerable<object> items, InteractionJSON interactionJson) {
            if (items.Count() == 0)
                return;

            string currInteractionPath = items.First() as string;
            int currInteractionNodeIndex = -1;

            ReorderableList incomingConditionsList = GetDefaultEmptyList("Select an interaction to view incoming conditions.");
            ReorderableList eventTriggersList = GetDefaultEmptyList("Select an interaction to view event triggers.");
            ReorderableList optionsList = GetDefaultEmptyList("Select an interaction to view options.");
            ReorderableList optionConditionsList = null;

            interactionPathEditorPane.Clear();
            interactionPathEditorPane.Add(new Label(currInteractionPath));

            ReorderableList interactionPathNodeList = new(interactionJson.Paths[currInteractionPath], typeof(InteractionNodeJSON), true, true, true, true) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, "Interaction Path Nodes");
                },
                onSelectCallback = (ReorderableList l) => {
                    if (l.index >= l.list.Count) {
                        incomingConditionsList = GetDefaultEmptyList("Select an interaction to view incoming conditions.");
                        eventTriggersList = GetDefaultEmptyList("Select an interaction to view event triggers.");
                        optionsList = GetDefaultEmptyList("Select an interaction to view options.");
                        optionConditionsList = null;
                        return;
                    }

                    InteractionNodeJSON element = interactionJson.Paths[currInteractionPath][l.index];
                    currInteractionNodeIndex = l.index;

                    incomingConditionsList = GetListForConditions(element.IncomingConditions, "Incoming Conditions");
                    eventTriggersList = new(element.EventTriggers, typeof(string), true, true, true, true) {
                        multiSelect = false,
                        drawHeaderCallback = (Rect rect) => {
                            EditorGUI.LabelField(rect, "Event Triggers");
                        },
                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                            string eventTrigger = element.EventTriggers[index];
                            rect.y += 2;
                            element.EventTriggers[index] = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"Trigger {index}", eventTrigger);
                        }
                    };

                    optionsList = new(element.Options, typeof(InteractionNodeJSON.InteractionOptionJSON), true, true, true, true) {
                        multiSelect = false,
                        drawHeaderCallback = (Rect rect) => {
                            EditorGUI.LabelField(rect, "Options");
                        },
                        onSelectCallback = (ReorderableList ll) => {
                            if (ll.index >= ll.list.Count) {
                                optionConditionsList = null;
                                return;
                            }
                            optionConditionsList = GetListForConditions(element.Options[ll.index].Conditions);
                        },
                        onChangedCallback = CallSelectOnChangeCallback,
                        drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                            InteractionNodeJSON.InteractionOptionJSON option = element.Options[index];
                            rect.y += 2;
                            option.OptionText = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), $"Option Text {index}", option.OptionText);
                            rect.y += 2;
                            option.NextPath = EditorGUI.TextField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width, EditorGUIUtility.singleLineHeight), "Next Path", option.NextPath);
                        },
                        elementHeight = EditorGUIUtility.singleLineHeight * 3 + 2
                    };
                    optionConditionsList = GetDefaultEmptyList("Select an option to view conditions.");
                },
                onChangedCallback = CallSelectOnChangeCallback,
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => {
                    InteractionNodeJSON element = interactionJson.Paths[currInteractionPath][index];
                    rect.y += 2;
                    element.Text = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), $"Text {index}", element.Text);
                },
                elementHeight = EditorGUIUtility.singleLineHeight * 2
            };

            interactionPathEditorPane.Add(new IMGUIContainer(() => {
                interactionPathNodeList.DoLayoutList();
                incomingConditionsList.DoLayoutList();
                eventTriggersList.DoLayoutList();
                optionsList.DoLayoutList();
                optionConditionsList?.DoLayoutList();
            }));
        }

        void UpdateInteraction(TextAsset currAsset, InteractionJSON interactionJson) {
            Debug.Log($"Name: {currAsset}");

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

        // Helpers

        void CallSelectOnChangeCallback(ReorderableList l) => l.onSelectCallback.Invoke(l);

        ReorderableList GetDefaultEmptyList(string noneLabel) {
            return new(new List<string>(), typeof(string), false, false, false, false) {
                drawNoneElementCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, noneLabel);
                },
            };
        }

        ReorderableList GetListForConditions(Dictionary<ConditionKeyJSON, string> dict, string headerLabel = "Conditions") {
            // Create a List from the dictionary, need this so that ReorderableList can wrap it/edit it
            List<(ConditionKeyJSON, string)> dictList = dict.Select(pair => (pair.Key, pair.Value)).ToList();

            return new(dictList, typeof((ConditionKeyJSON, string)), true, true, true, true) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => {
                    EditorGUI.LabelField(rect, headerLabel);
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

        ListView CreateListView<T>(List<T> baseItemSource, List<T> activeItemSource, System.Func<T, string> MapName) where T : class {
            VisualElement parentPane = new VisualElement();
            T currItem = null;

            // Toolbar with search field
            Toolbar toolbar = new Toolbar();
            ToolbarSearchField toolbarSearchField = new ToolbarSearchField();
            toolbarSearchField.style.flexShrink = 1;
            toolbar.Add(toolbarSearchField);

            ListView listView = new ListView() {
                selectionType = SelectionType.Single,
                itemsSource = activeItemSource,
                makeItem = () => new Label(),
                bindItem = (item, index) => {
                    (item as Label).text = MapName(activeItemSource[index]);
                }
            };
            listView.style.paddingLeft = 5;
            listView.style.paddingRight = 5;

            // Update locally held variable to use later in searching
            listView.selectionChanged += (items) => {
                currItem = items.Count() == 0 ? null : (T)items.First();
            };

            // On search update, change both list view source and active item source
            toolbarSearchField.RegisterValueChangedCallback(evt => {
                activeItemSource = baseItemSource.Where(item => MapName(item).Contains(evt.newValue)).ToList();
                listView.itemsSource = activeItemSource;

                // Update index to prevent confusion (right view should still display old)
                listView.selectedIndex = activeItemSource.IndexOf(currItem);

                listView.RefreshItems();
            });

            // Add toolbar first, then list view
            parentPane.Add(toolbar);
            parentPane.Add(listView);

            // Return the list view, we can reference the parent from it
            return listView;
        }
    }
}
