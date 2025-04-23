using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private DepartmentMenuViewer departmentMenuViewer;
    [SerializeField] private CargoTradeMenuController tradeMenu;
    [SerializeField] private PopupMessageHandler popupMessageHandler;
    
    private void Awake()
    {
        ServiceLocator.Register(this);
    }

    public void LoadingScreenShow()
    {
        loadingScreen.SetActive(true);
    }
    
    public void LoadingScreenHide()
    {
        loadingScreen.SetActive(false);
    }

    public void DepartmentScreenShow(Department department)
    {
        departmentMenuViewer.Show(department);
    }
    
    public void DepartmentScreenHide()
    {
        departmentMenuViewer.Hide();        
    }
    
    public void TradeScreenShow()
    {
        tradeMenu.Show();
    }
    
    public void TradeScreenHide()
    {
        tradeMenu.Hide();        
    }

    public void ShowPopupMessage(string title, string message)
    {
        popupMessageHandler.ShowPopupMessage(title, message);
    }

    public void HidePopupMessage()
    {
        popupMessageHandler.Hide();
    }
}
