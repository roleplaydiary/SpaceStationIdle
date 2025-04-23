using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CargoTradeMenuController : MonoBehaviour
{
    [SerializeField] private List<CargoTradeResourcePanelViewer> resourcePanels = new List<CargoTradeResourcePanelViewer>();

    [SerializeField] private GameObject content;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            Hide();
        }).AddTo(this);
    }

    public void Show()
    {
        Initialize();
        content.SetActive(true);
    }

    private void Initialize()
    {
        resourcePanels[0].Initialize(ResourceType.Metal);
        resourcePanels[1].Initialize(ResourceType.Glass);
        resourcePanels[2].Initialize(ResourceType.Plastic);
        resourcePanels[3].Initialize(ResourceType.Gold);
        resourcePanels[4].Initialize(ResourceType.Silver);
        resourcePanels[5].Initialize(ResourceType.Phoron);
        resourcePanels[6].Initialize(ResourceType.Uranium);
    }
    

    public void Hide()
    {
        content.SetActive(false);
    }
}
