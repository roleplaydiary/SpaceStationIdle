using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private SettingsVolumePanelHandler volumePanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button copyIdButton;
    [SerializeField] private Button feedbackButton;

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        var uIController = ServiceLocator.Get<UIController>();
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            uIController.SettingsMenuHide();
        }).AddTo(this);
        
        copyIdButton.OnClickAsObservable().Subscribe(_ =>
        {
            
        }).AddTo(this);
        
        feedbackButton.OnClickAsObservable().Subscribe(_ =>
        {
            
        }).AddTo(this);

        volumePanel.LabelsInitialize();
    }
    

    public void Show()
    {
        content.SetActive(true);
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
