using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private DepartmentMenuViewer departmentMenuViewer;
    [SerializeField] private CargoTradeMenuController tradeMenu;
    [SerializeField] private SettingsController settingsMenu;
    [SerializeField] private PopupMessageHandler popupMessageHandler;

    public static bool UIOpen;
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void LoadingScreenShow()
    {
        loadingScreen.SetActive(true);
        UIOpen = true;
    }
    
    public void LoadingScreenHide()
    {
        loadingScreen.SetActive(false);
        UIOpen = false;
    }

    public void DepartmentScreenShow(Department department)
    {
        departmentMenuViewer.Show(department);
        UIOpen = true;
    }
    
    public void DepartmentScreenHide()
    {
        departmentMenuViewer.Hide();   
        UIOpen = false;
    }
    
    public void TradeScreenShow()
    {
        tradeMenu.Show();
        UIOpen = true;
    }
    
    public void TradeScreenHide()
    {
        tradeMenu.Hide();   
        UIOpen = false;
    }
    
    public void SettingsMenuShow()
    {
        settingsMenu.Show();
        UIOpen = true;
    }
    
    public void SettingsMenuHide()
    {
        settingsMenu.Hide();        
        UIOpen = false;
    }

    public void PopupMessageShow(string title, string message)
    {
        popupMessageHandler.ShowPopupMessage(title, message);
        UIOpen = true;
    }

    public void PopupMessageHide()
    {
        popupMessageHandler.Hide();
        UIOpen = false;
    }
}
