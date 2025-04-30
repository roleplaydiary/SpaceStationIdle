using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text rewardTitle;
    [SerializeField] private TMP_Text rewardLabel;
    [SerializeField] private GameObject backgroundActive;
    [SerializeField] private GameObject backgroundInactive;

    private void Awake()
    {
        button.OnClickAsObservable().Subscribe(_ =>
        {
            GetReward();
        }).AddTo(this);
    }

    public void Initialize()
    {
        rewardTitle.text = string.Empty;
        rewardLabel.text = string.Empty;
    }

    private void GetReward()
    {
        
    }
}
