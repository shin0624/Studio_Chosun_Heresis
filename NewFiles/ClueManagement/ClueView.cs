using UnityEngine;
using UnityEngine.UIElements;

public class ClueView : MonoBehaviour
{
    [SerializeField] VisualTreeAsset ListEntryTemplate;

    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        var clueListController = new ClueListController();
        clueListController.InitializeCharacterList(uiDocument.rootVisualElement, ListEntryTemplate);
    }
}
