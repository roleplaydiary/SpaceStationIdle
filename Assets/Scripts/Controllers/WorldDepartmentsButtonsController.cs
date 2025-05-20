using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
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

        private void Start()
        {
            ServiceLocator.Get<StationEventsController>().OnMajorEventStarted.Subscribe(data =>
            {
                DepartmentButtonToggle(data.Department, false);
            }).AddTo(this);
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
            DepartmentButtonToggle(Department.Bridge, isBridgeUnlocked);
        
            var isEngineeringUnlocked = stationController.StationData.IsUnlocked(Department.Engineering);
            DepartmentButtonToggle(Department.Engineering, isEngineeringUnlocked);
        
            var isScienceUnlocked = stationController.StationData.IsUnlocked(Department.Science);
            DepartmentButtonToggle(Department.Science, isScienceUnlocked);
        
            var isCargoUnlocked = stationController.StationData.IsUnlocked(Department.Cargo);
            DepartmentButtonToggle(Department.Cargo, isCargoUnlocked);
        
            var isMedbayUnlocked = stationController.StationData.IsUnlocked(Department.Med);
            DepartmentButtonToggle(Department.Med, isMedbayUnlocked);
            
            var isSecurityUnlocked = stationController.StationData.IsUnlocked(Department.Security);
            DepartmentButtonToggle(Department.Security, isSecurityUnlocked);
        
            var isBarUnlocked = stationController.StationData.IsUnlocked(Department.Bar);
            DepartmentButtonToggle(Department.Bar, isBarUnlocked);
        }

        private void DepartmentButtonToggle(Department department, bool value)
        {
            switch (department)
            {
                case Department.Bridge:
                    bridgeMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Engineering:
                    engineeringMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Science:
                    scienceMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Cargo:
                    cargoMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Med:
                    medbayMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Security:
                    securityMenuButton.gameObject.SetActive(value);
                    break;
                case Department.Bar:
                    barMenuButton.gameObject.SetActive(value);
                    break;
                
            }
        }
    }
}
