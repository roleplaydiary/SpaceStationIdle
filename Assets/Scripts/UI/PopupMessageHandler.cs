using TMPro;
using UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageHandler : MonoBehaviour
{
    [SerializeField] private GameObject windowContent;
    [SerializeField] private Button closeWindowButton;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text message;

    private void Awake()
    {
        closeWindowButton.OnClickAsObservable().Subscribe(_ =>
        {
            ServiceLocator.Get<UIController>().PopupMessageHide();
        }).AddTo(this);
    }

    public void ShowPopupMessage(string title, string message)
    {
        windowContent.SetActive(true);
        this.title.text = title;
        this.message.text = message;
    }

    public void Hide()
    {
        windowContent.SetActive(false);
    }
    
}
