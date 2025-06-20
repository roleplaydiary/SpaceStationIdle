using TMPro;
using UI;
using UniRx;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private SettingsVolumePanelHandler volumePanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text playerIdLabel;
    [SerializeField] private Button copyIdButton;
    [SerializeField] private Button feedbackButton;

    private void Start()
    {
        ButtonsInitialize();
    }

    private void ButtonsInitialize()
    {
        var uIController = ServiceLocator.Get<UIController>();
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            uIController.SettingsMenuHide();
        }).AddTo(this);
        
        copyIdButton.OnClickAsObservable().Subscribe(_ =>
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            GUIUtility.systemCopyBuffer = playerId;
            uIController.PopupMessageShow("Player ID", "Your ID has been copied successfully");
        }).AddTo(this);
        
        var feedbackFormURL = "https://docs.google.com/forms/d/e/1FAIpQLSeIRcXgcGFkpMegkS-Zkt0BzPPIWpJLvnYUnYTYssggdtTF8w/viewform?usp=dialog";
        feedbackButton.OnClickAsObservable().Subscribe(_ =>
        {
            Application.OpenURL(feedbackFormURL);
        }).AddTo(this);
    }
    

    public void Show()
    {
        content.SetActive(true);
        volumePanel.LabelsInitialize();
        
        playerIdLabel.text = $"ID: {AuthenticationService.Instance.PlayerId}";
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
