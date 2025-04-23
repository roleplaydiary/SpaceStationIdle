using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradeResourcePanelViewer : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text resourceName;
    [SerializeField] private TMP_Text resourceValue;

    public void Initialize(ResourceType resource)
    {
        var resouceManager = ServiceLocator.Get<ResourceManager>();
        // icon.sprite = 
        
        resourceName.text = resource.ToString();
        resourceValue.text = resouceManager.GetResourceAmount(resource).ToString();
    }
    
}
