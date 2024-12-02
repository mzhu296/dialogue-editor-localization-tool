using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace KasperDev.Dialogue.Editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        // Updated to public to make it accessible from other classes
        public DialogueContainerSO currentDialogueContainer;

        private DialogueGraphView graphView;
        private DialogueSaveAndLoad saveAndLoad;

        private LanguageType selectedLanguage = LanguageType.English; // Current selected language in the dialogue editor window.
        private ToolbarMenu languagesDropdownMenu; // Languages toolbar menu in the top of dialogue editor window.
        private Label nameOfDialogueContainer; // Name of the current open dialogue container.
        private string graphViewStyleSheet = "USS/EditorWindow/EditorWindowStyleSheet"; // Name of the graph view style sheet.

        // Language Property - Public for access
        public LanguageType SelectedLanguage { get => selectedLanguage; set => selectedLanguage = value; }

        // Static method to open or get the dialogue editor window
        [MenuItem("Tools/Dialogue Editor/Open empty Dialogue Editor")]
        public static DialogueEditorWindow OpenWindow()
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>("Dialogue Editor");
            window.minSize = new Vector2(500, 250);
            window.Show();
            return window;
        }

        // Callback for opening an asset in Unity
        [OnOpenAsset(0)]
        public static bool ShowWindow(int instanceId, int line)
        {
            UnityEngine.Object item = EditorUtility.InstanceIDToObject(instanceId);

            if (item is DialogueContainerSO dialogueContainer)
            {
                DialogueEditorWindow window = OpenWindow(); // Use the existing method to show the editor window
                window.LoadDialogue(dialogueContainer);
                return true;
            }

            return false; // Asset type not handled
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            if (currentDialogueContainer != null)
            {
                Load();
            }
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(graphView);
        }

        /// <summary>
        /// Construct graph view
        /// </summary>
        private void ConstructGraphView()
        {
            graphView = new DialogueGraphView(this);
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
            saveAndLoad = new DialogueSaveAndLoad(graphView);
        }

        /// <summary>
        /// Generate the toolbar you will see in the top left of the dialogue editor window.
        /// </summary>
        private void GenerateToolbar()
        {
            StyleSheet styleSheet = Resources.Load<StyleSheet>(graphViewStyleSheet);
            rootVisualElement.styleSheets.Add(styleSheet);

            Toolbar toolbar = new Toolbar();

            // Save button
            Button saveBtn = new Button() { text = "Save" };
            saveBtn.clicked += () => Save();
            toolbar.Add(saveBtn);

            // Load button
            Button loadBtn = new Button() { text = "Load" };
            loadBtn.clicked += () => Load();
            toolbar.Add(loadBtn);

            // Dropdown menu for languages
            languagesDropdownMenu = new ToolbarMenu();
            foreach (LanguageType language in (LanguageType[])Enum.GetValues(typeof(LanguageType)))
            {
                languagesDropdownMenu.menu.AppendAction(language.ToString(), new Action<DropdownMenuAction>(x => Language(language)));
            }
            toolbar.Add(languagesDropdownMenu);

            // Name of current DialogueContainer
            nameOfDialogueContainer = new Label("");
            toolbar.Add(nameOfDialogueContainer);
            nameOfDialogueContainer.AddToClassList("nameOfDialogueContainer");

            rootVisualElement.Add(toolbar);
        }

        /// <summary>
        /// Load the current selected dialogue container.
        /// </summary>
        public void Load()
        {
            if (currentDialogueContainer != null)
            {
                Language(LanguageType.English);
                nameOfDialogueContainer.text = "Name: " + currentDialogueContainer.name;
                saveAndLoad.Load(currentDialogueContainer);
            }
        }

        /// <summary>
        /// Load the specified DialogueContainer into the editor.
        /// </summary>
        /// <param name="dialogueContainer">The DialogueContainer to load.</param>
        public void LoadDialogue(DialogueContainerSO dialogueContainer)
        {
            currentDialogueContainer = dialogueContainer;
            Load();
        }

        /// <summary>
        /// Save the current changes to dialogue container.
        /// </summary>
        private void Save()
        {
            if (currentDialogueContainer != null)
            {
                saveAndLoad.Save(currentDialogueContainer);
                EditorUtility.SetDirty(currentDialogueContainer);
                AssetDatabase.SaveAssets();
            }
        }

        /// <summary>
        /// Change the language in the dialogue editor window.
        /// </summary>
        /// <param name="language">Language that you want to change to</param>
        private void Language(LanguageType language)
        {
            languagesDropdownMenu.text = "Language: " + language.ToString();
            selectedLanguage = language;
            graphView.ReloadLanguage();
        }
    }
}
