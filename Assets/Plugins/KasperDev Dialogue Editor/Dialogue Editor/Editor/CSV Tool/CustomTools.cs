using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace KasperDev.Dialogue.Editor
{
    public class CustomTools
    {
        [MenuItem("Tools/Dialogue Editor/Save to CSV")]
        public static void SaveToCSV()
        {
            SaveCSV saveCSV = new SaveCSV();
            saveCSV.Save();

            EditorApplication.Beep();
            Debug.Log("<color=green> Save CSV File successfully! </color>");
        }

        [MenuItem("Tools/Dialogue Editor/Load from CSV and Open Editor")]
        public static void LoadFromCSV()
        {
            // Step 1: Load data from CSV
            LoadCSV loadCSV = new LoadCSV();
            loadCSV.Load();

            // Step 2: Find the dialogue container you just loaded or updated
            List<DialogueContainerSO> dialogueContainers = Helper.FindAllDialogueContainerSO();
            
            if (dialogueContainers.Count == 0)
            {
                Debug.LogError("No Dialogue Containers found. Make sure you have created a DialogueContainerSO.");
                return;
            }

            // Step 3: Open the Dialogue Editor Window with the loaded container
            DialogueEditorWindow window = DialogueEditorWindow.OpenWindow(); // Use OpenWindow to open or get existing window

            DialogueContainerSO loadedContainer = dialogueContainers[0];
            window.LoadDialogue(loadedContainer);

            // Beep and log to indicate success
            EditorApplication.Beep();
            Debug.Log("<color=green> Loaded CSV File successfully and opened the Dialogue Editor! </color>");
        }
    }
}
