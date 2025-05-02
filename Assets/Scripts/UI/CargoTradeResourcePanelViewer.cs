using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradeResourcePanelViewer : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text resourceName;
    [SerializeField] private TMP_Text resourceValue;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button sellButton;

    private CompositeDisposable disposable = new CompositeDisposable();
    public ResourceType DisplayedResourceType { get; private set; }
    
    private ResourceManager resourceManager;
    public void Initialize(ResourceType type)
    {
        resourceManager = ServiceLocator.Get<ResourceManager>();
        
        DisplayedResourceType = type;
        // icon.sprite = resource.icon
        resourceName.text = type.ToString();
        // resourceValue.text = resourceManager.GetResourceAmount(type).ToString();
        
        resourceManager.CurrentResources
         .Subscribe(resources =>
         {
             Debug.Log("CargoTradeResourcePanelViewer Subscription Event!");
             foreach (var res in resources)
             {
                 if (res.Key == DisplayedResourceType.ToString())
                 {
                     resourceValue.text = res.Value.ToString();
                     break;
                 }
             }
         })
         .AddTo(disposable);
            
    }

    private void OnDisable()
    {
        Debug.Log("CargoTradeResourcePanelViewer ОТПИСКА");
        disposable.Dispose();
    }

    public Button GetBuyButton()
    {
        return buyButton;
    }

    public Button GetSellButton()
    {
        return sellButton;
    }
}
