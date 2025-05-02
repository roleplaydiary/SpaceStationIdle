using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradeMenuController : MonoBehaviour
{
    [SerializeField] private List<CargoTradeResourcePanelViewer> resourcePanels = new List<CargoTradeResourcePanelViewer>();

    [SerializeField] private GameObject content;
    [SerializeField] private Button closeButton;
    [SerializeField] private CargoBuyConfirmationMenuController cargoBuyConfirmationMenuController;
    [SerializeField] private CargoSellConfirmationMenuController cargoSellConfirmationMenuController;

    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Show()
    {
        Initialize();
        content.SetActive(true);
    }

    private void Initialize()
    {
        closeButton.OnClickAsObservable()
            .Subscribe(_ => ServiceLocator.Get<UIController>().TradeScreenHide())
            .AddTo(_disposables);
        
        var availableResources = new Resources(); // Создаем экземпляр Resources, чтобы получить список ресурсов

        if (resourcePanels == null || resourcePanels.Count != availableResources.Count())
        {
            Debug.LogError("Количество панелей ресурсов не соответствует количеству торгуемых ресурсов!");
            return;
        }

        int i = 0;
        foreach (var resourcePair in availableResources)
        {
            if (i >= resourcePanels.Count)
            {
                Debug.LogError("Недостаточно панелей ресурсов для отображения всех торгуемых ресурсов!");
                break;
            }

            var panel = resourcePanels[i];
            ResourceType resourceType = (ResourceType)Enum.Parse(typeof(ResourceType), resourcePair.Key);

            panel.Initialize(resourceType);
            

            // Подписка на кнопку покупки
            panel.GetBuyButton().OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var tradeResource = new TradeResourceClass()
                    {
                        ResourceType = resourceType,
                        ResourceAmount = 0
                    };
                    cargoBuyConfirmationMenuController.Show(tradeResource);
                })
                .AddTo(_disposables);

            // Подписка на кнопку продажи
            panel.GetSellButton().OnClickAsObservable()
                .Subscribe(_ =>
                {
                    var tradeResource = new TradeResourceClass()
                    {
                        ResourceType = resourceType,
                        ResourceAmount = 0
                    };
                    cargoSellConfirmationMenuController.Show(tradeResource);
                })
                .AddTo(_disposables);

            i++;
        }
    }

    public void Hide()
    {
        _disposables.Clear();
        content.SetActive(false);
    }
}