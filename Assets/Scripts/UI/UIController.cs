using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private DepartmentMenuViewer departmentMenuViewer;
        [SerializeField] private DailyRewardMenuController dailyRewardMenu;
        [SerializeField] private CargoTradeMenuController tradeMenu;
        [SerializeField] private SettingsController settingsMenu;
        [SerializeField] private PopupMessageHandler popupMessageHandler;
        [SerializeField] private WelcomeMessageController welcomeMessageController;

        public readonly List<GameObject> OpenUIWindows = new();

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        public void LoadingScreenShow()
        {
            loadingScreen.SetActive(true);
            OpenUIWindows.Add(loadingScreen);
        }

        public void LoadingScreenHide()
        {
            loadingScreen.SetActive(false);
            OpenUIWindows.Remove(loadingScreen);
        }

        public void DepartmentScreenShow(Department department)
        {
            departmentMenuViewer.Show(department);
            OpenUIWindows.Add(departmentMenuViewer.gameObject);
        }

        public void DepartmentScreenHide()
        {
            departmentMenuViewer.Hide();
            OpenUIWindows.Remove(departmentMenuViewer.gameObject);
        }

        public void TradeScreenShow()
        {
            tradeMenu.Show();
            OpenUIWindows.Add(tradeMenu.gameObject);
        }

        public void TradeScreenHide()
        {
            tradeMenu.Hide();
            OpenUIWindows.Remove(tradeMenu.gameObject);
        }

        public void SettingsMenuShow()
        {
            settingsMenu.Show();
            OpenUIWindows.Add(settingsMenu.gameObject);
        }

        public void SettingsMenuHide()
        {
            settingsMenu.Hide();
            OpenUIWindows.Remove(settingsMenu.gameObject);
        }

        public void PopupMessageShow(string title, string message)
        {
            popupMessageHandler.ShowPopupMessage(title, message);
            OpenUIWindows.Add(popupMessageHandler.gameObject);
        }

        public void PopupMessageHide()
        {
            popupMessageHandler.Hide();
            OpenUIWindows.Remove(popupMessageHandler.gameObject);
        }
    
        public void DailyRewardShow()
        {
            dailyRewardMenu.Show();
            OpenUIWindows.Add(dailyRewardMenu.gameObject);
        }

        public void DailyRewardHide()
        {
            dailyRewardMenu.Hide();
            OpenUIWindows.Remove(dailyRewardMenu.gameObject);
        }

        public void WelcomeMessageShow()
        {
            welcomeMessageController.Initialize();
        }

        public void WelcomeMessageHide()
        {
            welcomeMessageController.Hide();           
        }

        // Вспомогательное свойство, чтобы легко проверить, открыто ли какое-либо UI окно
        public bool IsAnyUIOpen => OpenUIWindows.Count > 0;
    }
}