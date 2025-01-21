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
            initOptionsPane = new ScrollView(ScrollViewMode.Vertical);
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
            ReorderableList initPathOptionConditionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select a path above to view conditions.");

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
            ReorderableList initPathOptionsList =
                InteractionEditorListBuilder.NewBuilder(interactionJson.Init.PathOptions, "Path Options")
                    .WithOnSelectCallback(
                        (ReorderableList l) => {
                            if (l.index >= l.list.Count) {
                                initPathOptionConditionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select a path above to view conditions.");
                                return;
                            }
                            initPathOptionConditionsList = GetListForConditions(interactionJson.Init.PathOptions[l.index].Conditions);
                        }
                    )
                    .WithDrawElementCallback(
                        (Rect rect, int index, bool isActive, bool isFocused) => {
                            InitInteractionJSON.InitPathOptionsJSON element = interactionJson.Init.PathOptions[index];
                            rect.y += 2;
                            element.NextPath = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"Path {index}", element.NextPath);
                        }
                    )
                    .Build();

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

            interactionPathEditorPane = new ScrollView(ScrollViewMode.Vertical);
            interactionPathsPane.Add(interactionPathEditorPane);
            rightPane.Add(interactionPathsPane);
        }

        void OnInteractionPathSelection(IEnumerable<object> items, InteractionJSON interactionJson) {
            if (items.Count() == 0)
                return;

            string currInteractionPath = items.First() as string;
            int currInteractionNodeIndex = -1;

            ReorderableList incomingConditionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view incoming conditions.");
            ReorderableList eventTriggersList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view event triggers.");
            ReorderableList optionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view options.");
            ReorderableList optionConditionsList = null;

            interactionPathEditorPane.Clear();
            interactionPathEditorPane.Add(new Label(currInteractionPath));

            ReorderableList interactionPathNodeList =
                InteractionEditorListBuilder.NewBuilder(interactionJson.Paths[currInteractionPath], "Interaction Path Nodes")
                    .WithOnSelectCallback((ReorderableList l) => {
                        if (l.index >= l.list.Count) {
                            incomingConditionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view incoming conditions.");
                            eventTriggersList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view event triggers.");
                            optionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an interaction to view options.");
                            optionConditionsList = null;
                            return;
                        }

                        InteractionNodeJSON element = interactionJson.Paths[currInteractionPath][l.index];
                        currInteractionNodeIndex = l.index;

                        incomingConditionsList = GetListForConditions(element.IncomingConditions, "Incoming Conditions");


                        eventTriggersList =
                            InteractionEditorListBuilder.NewBuilder(element.EventTriggers, "Event Triggers")
                                .WithDrawElementCallback((Rect rect, int index, bool isActive, bool isFocused) => {
                                    string eventTrigger = element.EventTriggers[index];
                                    rect.y += 2;
                                    element.EventTriggers[index] = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), $"Trigger {index}", eventTrigger);
                                })
                                .Build();

                        optionsList =
                            InteractionEditorListBuilder.NewBuilder(element.Options, "Options")
                                .WithOnSelectCallback((ReorderableList ll) => {
                                    if (ll.index >= ll.list.Count) {
                                        optionConditionsList = null;
                                        return;
                                    }
                                    optionConditionsList = GetListForConditions(element.Options[ll.index].Conditions);
                                })
                                .WithDrawElementCallback((Rect rect, int index, bool isActive, bool isFocused) => {
                                    InteractionNodeJSON.InteractionOptionJSON option = element.Options[index];
                                    rect.y += 2;
                                    option.OptionText = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), $"Option Text {index}", option.OptionText);
                                    rect.y += 2;
                                    option.NextPath = EditorGUI.TextField(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight * 2, rect.width, EditorGUIUtility.singleLineHeight), "Next Path", option.NextPath);
                                })
                                .WithElementHeight(EditorGUIUtility.singleLineHeight * 3 + 2)
                                .Build();

                        optionConditionsList = InteractionEditorListBuilder.GetDefaultEmptyList("Select an option to view conditions.");
                    })
                    .WithDrawElementCallback((Rect rect, int index, bool isActive, bool isFocused) => {
                        InteractionNodeJSON element = interactionJson.Paths[currInteractionPath][index];
                        rect.y += 2;
                        element.Text = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight * 2), $"Text {index}", element.Text);
                    })
                    .WithElementHeight(EditorGUIUtility.singleLineHeight * 2)
                    .Build();

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

        ReorderableList GetListForConditions(Dictionary<ConditionKeyJSON, string> dict, string headerLabel = "Conditions") {
            // Create a List from the dictionary, need this so that ReorderableList can wrap it/edit it
            List<(ConditionKeyJSON, string)> dictList = dict.Select(pair => (pair.Key, pair.Value)).ToList();

            return InteractionEditorListBuilder.NewBuilder(dictList, headerLabel)
                    .WithOnCanAddCallback((ReorderableList l) => {
                        return l.count < System.Enum.GetValues(typeof(ConditionKeyJSON)).Length;
                    })
                    .WithOnAddDropdownCallback((Rect rect, ReorderableList l) => {
                        GenericMenu menu = new GenericMenu();

                        IEnumerable<ConditionKeyJSON> conditionKeysLeft = System.Enum.GetValues(typeof(ConditionKeyJSON)).Cast<ConditionKeyJSON>().Except(dict.Keys);
                        foreach (ConditionKeyJSON conditionKey in conditionKeysLeft) {
                            menu.AddItem(new GUIContent(conditionKey.ToString()), false, () => {
                                dict.Add(conditionKey, "");
                                l.list.Add((conditionKey, ""));
                            });
                        }

                        menu.ShowAsContext();
                    })
                    .WIthOnRemoveCallback((ReorderableList l) => {
                        (ConditionKeyJSON key, string value) = ((ConditionKeyJSON, string))l.list[l.index];
                        dict.Remove(key);
                        l.list.RemoveAt(l.index);
                    })
                    .WithDrawElementCallback((Rect rect, int index, bool isActive, bool isFocused) => {
                        (ConditionKeyJSON key, string value) element = dictList[index];

                        rect.y += 2;
                        string previousValue = element.value;
                        element.value = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element.key.ToString(), element.value);

                        if (!element.value.Equals(previousValue)) {
                            dict[element.key] = element.value;
                            dictList[index] = element;
                        }
                    })
                    .Build();
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
                currItem = items.Count() == 0 ? null : items.First() as T;
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

    public class InteractionEditorListBuilder {
        public static InteractionEditorListBuilder<T> NewBuilder<T>(List<T> elements, string label) {
            return new(elements, label);
        }

        public static ReorderableList GetDefaultEmptyList(string noneLabel) {
            return NewBuilder(new List<string>(), "").WithDrawNoneElementCallback((Rect rect) => {
                EditorGUI.LabelField(rect, noneLabel);
            }).Build(false);
        }
    }

    public class InteractionEditorListBuilder<T> {
        // Must be provided at start
        private List<T> elements;
        private string label;

        // Optionally added callbacks (type from ReorderableList), just a subset for what we need

        // Visual callbacks/fields
        private ReorderableList.DrawNoneElementCallback drawNoneElementCallback;
        private ReorderableList.ElementCallbackDelegate drawElementCallback;
        private float elementHeight = 21; // Default height of ReorderableList

        // Selection callback
        private ReorderableList.SelectCallbackDelegate onSelectCallback;

        // Add/remove callbacks
        private ReorderableList.CanAddCallbackDelegate onCanAddCallback;
        private ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback;
        private ReorderableList.RemoveCallbackDelegate onRemoveCallback;

        public InteractionEditorListBuilder(List<T> elements, string label) {
            this.elements = elements;
            this.label = label;
        }

        public InteractionEditorListBuilder<T> WithDrawNoneElementCallback(ReorderableList.DrawNoneElementCallback drawNoneElementCallback) {
            this.drawNoneElementCallback = drawNoneElementCallback;
            return this;
        }

        public InteractionEditorListBuilder<T> WithDrawElementCallback(ReorderableList.ElementCallbackDelegate drawElementCallback) {
            this.drawElementCallback = drawElementCallback;
            return this;
        }

        public InteractionEditorListBuilder<T> WithElementHeight(float elementHeight) {
            this.elementHeight = elementHeight;
            return this;
        }

        public InteractionEditorListBuilder<T> WithOnSelectCallback(ReorderableList.SelectCallbackDelegate onSelectCallback) {
            this.onSelectCallback = onSelectCallback;
            return this;
        }

        public InteractionEditorListBuilder<T> WithOnCanAddCallback(ReorderableList.CanAddCallbackDelegate onCanAddCallback) {
            this.onCanAddCallback = onCanAddCallback;
            return this;
        }

        public InteractionEditorListBuilder<T> WithOnAddDropdownCallback(ReorderableList.AddDropdownCallbackDelegate onAddDropdownCallback) {
            this.onAddDropdownCallback = onAddDropdownCallback;
            return this;
        }

        public InteractionEditorListBuilder<T> WIthOnRemoveCallback(ReorderableList.RemoveCallbackDelegate onRemoveCallback) {
            this.onRemoveCallback = onRemoveCallback;
            return this;
        }

        public ReorderableList Build(bool interactable = true) {
            return new ReorderableList(elements, typeof(T), interactable, interactable, interactable, interactable) {
                multiSelect = false,
                drawHeaderCallback = (Rect rect) => EditorGUI.LabelField(rect, label),

                // Visual callbacks/fields
                drawNoneElementCallback = drawNoneElementCallback,
                drawElementCallback = drawElementCallback,
                elementHeight = elementHeight,

                // Seletion callback
                onSelectCallback = onSelectCallback,
                onChangedCallback = (ReorderableList l) => l.onSelectCallback?.Invoke(l), // Want this by default

                // Add/remove callbacks
                onCanAddCallback = onCanAddCallback,
                onAddDropdownCallback = onAddDropdownCallback,
                onRemoveCallback = onRemoveCallback
            };
        }
    }
}
