using UniRx;
using UnityEngine;

namespace Controllers
{
    public class WorldDepartmentsButtonsController : MonoBehaviour
    {
        [SerializeField] private WorldDepartmentButtonHandler bridgeMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler engineeringMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler scienceMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler cargoMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler medbayMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler securityMenuButton;
        [SerializeField] private WorldDepartmentButtonHandler barMenuButton;

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void Start()
        {
            ServiceLocator.Get<StationEventsController>().OnMajorEventStarted.Subscribe(DepartmentHazardButtonToggle).AddTo(this);
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
                    bridgeMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Engineering:
                    engineeringMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Science:
                    scienceMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Cargo:
                    cargoMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Med:
                    medbayMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Security:
                    securityMenuButton.DepartmentButtonToggle(value);
                    break;
                case Department.Bar:
                    barMenuButton.DepartmentButtonToggle(value);
                    break;
            }
        }

        private void DepartmentHazardButtonToggle(MajorEventData data)
        {
            bool value = data.StationMajorEventType != StationMajorEventType.None;
            switch (data.Department)
            {
                case Department.Bridge:
                    bridgeMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Engineering:
                    engineeringMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Science:
                    scienceMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Cargo:
                    cargoMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Med:
                    medbayMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Security:
                    securityMenuButton.HazardButtonToggle(value);
                    break;
                case Department.Bar:
                    barMenuButton.HazardButtonToggle(value);
                    break;
            }
        }
    }
}
