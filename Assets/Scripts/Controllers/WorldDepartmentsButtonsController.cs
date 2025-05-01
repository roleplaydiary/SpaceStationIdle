using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WorldDepartmentsButtonsController : MonoBehaviour
{
    [SerializeField] private Button bridgeMenuButton;
    [SerializeField] private Button engineeringMenuButton;
    [SerializeField] private Button scienceMenuButton;
    [SerializeField] private Button cargoMenuButton;
    [SerializeField] private Button medbayMenuButton;
    [SerializeField] private Button securityMenuButton;
    [SerializeField] private Button barMenuButton;

    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void Initialize()
    {
        var upgradeService = ServiceLocator.Get<UpgradeService>();
        upgradeService.OnUpgradePurchased.Subscribe(_ =>
        {
            ButtonsInitialize();
        }).AddTo(this);


        ButtonsInitialize();
    }

    private void ButtonsInitialize()
    {
        var stationController = ServiceLocator.Get<StationController>();

        var isBridgeUnlocked = stationController.StationData.IsUnlocked(Department.Bridge);
        bridgeMenuButton.gameObject.SetActive(isBridgeUnlocked);
        
        var isEngineeringUnlocked = stationController.StationData.IsUnlocked(Department.Engineering);
        engineeringMenuButton.gameObject.SetActive(isEngineeringUnlocked);
        
        var isScienceUnlocked = stationController.StationData.IsUnlocked(Department.Science);
        scienceMenuButton.gameObject.SetActive(isScienceUnlocked);
        
        var isCargoUnlocked = stationController.StationData.IsUnlocked(Department.Cargo);
        cargoMenuButton.gameObject.SetActive(isCargoUnlocked);
        
        var isMedbayUnlocked = stationController.StationData.IsUnlocked(Department.Med);
        medbayMenuButton.gameObject.SetActive(isMedbayUnlocked);
        
        var isSecurityUnlocked = stationController.StationData.IsUnlocked(Department.Security);
        securityMenuButton.gameObject.SetActive(isSecurityUnlocked);
        
        var isBarUnlocked = stationController.StationData.IsUnlocked(Department.Bar);
        barMenuButton.gameObject.SetActive(isBarUnlocked);
    }
}
