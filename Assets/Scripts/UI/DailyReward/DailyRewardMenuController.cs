using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardMenuController : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private DailyRewardButton dailyRewardButton;
    [SerializeField] private DailyRewardButton dailyRewardButton2;
    [SerializeField] private DailyRewardButton dailyRewardButton3;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().DailyRewardHide();
        }).AddTo(this);
    }

    public void Show()
    {
        content.SetActive(true);
        Initialize();
    }

    private void Initialize()
    {
        dailyRewardButton.Initialize();
        dailyRewardButton2.Initialize();
        dailyRewardButton3.Initialize();
    }

    public void Hide()
    {
        content.SetActive(false);
    }
}
