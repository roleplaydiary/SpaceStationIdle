using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPressSound : MonoBehaviour
{
    [SerializeField] private string buttonPressSound = "button_click_1";
    private Button button;
    
    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        button = gameObject.GetComponent<Button>();
        button.OnClickAsObservable().Subscribe(_ =>
        {
            var audioManager = ServiceLocator.Get<AudioManager>();
            var dataLibrary = ServiceLocator.Get<DataLibrary>();
            audioManager.PlayUISound(dataLibrary.soundLibrary.GetUISound(buttonPressSound));
        }).AddTo(this);
    }
}
