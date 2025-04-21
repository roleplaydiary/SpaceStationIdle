using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupMessageHandler : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private Button closeButton;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text message;

    private void Awake()
    {
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            Hide();
        }).AddTo(this);
    }

    public void Show(string title, string message)
    {
        this.title.text = title;
        this.message.text = message;
        content.SetActive(true);
    }

    public void Hide()
    {
        content.SetActive(false);
    }
    
}
