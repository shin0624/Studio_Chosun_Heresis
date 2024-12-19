using UnityEngine.UIElements;

public class ClueListEntryController
{
    Label NameLabel;

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("clue-name");
    }

    //This function receives the character whose name this list 
    //element displays.Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetClueData(ClueData ClueData)
    {
        NameLabel.text = ClueData.ClueName;
    }
}