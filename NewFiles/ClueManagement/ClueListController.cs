using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ClueListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView ClueList;
    Label CharNameLabel;
    VisualElement CharClue;

    public void InitializeCharacterList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        EnumerateAllCharacters();

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;

        // Store a reference to the character list element
        ClueList = root.Q<ListView>("clue-list");

        // Store references to the selected character info elements
        CharNameLabel = root.Q<Label>("clue-name");
        CharClue = root.Q<VisualElement>("clue-image");

        FillClueList();

        // Register to get a callback when an item is selected
        ClueList.selectionChanged += OnClueSelected;
    }

    List<ClueData> AllClues;

    void EnumerateAllCharacters()
    {
        AllClues = new List<ClueData>();
        AllClues.AddRange(Resources.LoadAll<ClueData>("Clues"));
    }

    void FillClueList()
    {
        // Set up a make item function for a list entry
        ClueList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new ClueListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        ClueList.bindItem = (item, index) =>
        {
            (item.userData as ClueListEntryController).SetClueData(AllClues[index]);
        };

        // Set a fixed item height
        ClueList.fixedItemHeight = 45;

        // Set the actual item's source list/array
        ClueList.itemsSource = AllClues;
    }

    void OnClueSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedCharacter = ClueList.selectedItem as ClueData;

        // Handle none-selection (Escape to deselect everything)
        if (selectedCharacter == null)
        {
            // Clear
            CharNameLabel.text = "";
            CharClue.style.backgroundImage = null;

            return;
        }

        // Fill in character details
        CharNameLabel.text = selectedCharacter.ClueName;
        CharClue.style.backgroundImage = new StyleBackground(selectedCharacter.ClueImage);
    }
}
