using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CargoSellConfirmationMenuController : MonoBehaviour
{
    [SerializeField] private GameObject content;

    [Header("Labels settings")] 
    [SerializeField] private TMP_Text titleLabel;
    [SerializeField] private TMP_Text resouceNameLabel;
    [SerializeField] private TMP_Text descriptionLabel;
    
    [Header("Buttons settings")]
    [SerializeField] private Button plusOneButton;
    [SerializeField] private Button plusFiveButton;
    [SerializeField] private Button plusTenButton;
    [SerializeField] private Button minusOneButton;
    [SerializeField] private Button minusFiveButton;
    [SerializeField] private Button minusTenButton;
    [SerializeField] private Button confirmationButton;
    [SerializeField] private Button closeButton;
    
    private TradeResourceClass tradeResourceClass = new TradeResourceClass();

    private void Awake()
    {
        confirmationButton.OnClickAsObservable().Subscribe(async _ =>
        {
            await TradeButtonClick();
        }).AddTo(this);
        
        closeButton.OnClickAsObservable().Subscribe(_ =>
        {
            CloseButtonClick();
        }).AddTo(this);

        BuyButtonsInit();
    }

    private void BuyButtonsInit()
    {
        plusOneButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount ++;
            Initialize(tradeResourceClass);
        }).AddTo(this);
        plusFiveButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount += 5;
            Initialize(tradeResourceClass);
        }).AddTo(this);
        plusTenButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount += 10;
            Initialize(tradeResourceClass);
        }).AddTo(this);
        minusOneButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount --;
            Initialize(tradeResourceClass);
        }).AddTo(this);
        minusFiveButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount -= 5;
            Initialize(tradeResourceClass);
        }).AddTo(this);
        minusTenButton.OnClickAsObservable().Subscribe(_ =>
        {
            tradeResourceClass.ResourceAmount -= 10;
            Initialize(tradeResourceClass);
        }).AddTo(this);
    }
    

    public void Show(TradeResourceClass tradeResourceClass)
    {
        content.SetActive(true);

        this.tradeResourceClass.ResourceAmount = 0; // Default value
        Initialize(tradeResourceClass);
    }

    public void Hide()
    {
        content.SetActive(false);
    }
    
    public void Initialize(TradeResourceClass tradeResourceClass)
    {
        this.tradeResourceClass = tradeResourceClass;
        
        resouceNameLabel.text = tradeResourceClass.ResourceType.ToString();
        titleLabel.text = $"Sell menu";
        
        var price = GetSellPrice();
        descriptionLabel.text = $"Do you want to sell {tradeResourceClass.ResourceAmount} amount of {tradeResourceClass.ResourceType} \n for {price}?";
    }
    
    private async Task TradeButtonClick()
    {
        if (tradeResourceClass.ResourceAmount <= 0)
        {
            Debug.LogError($"{tradeResourceClass.ResourceType} is not a valid amount.");
            return;
        }
        var resourceManager = ServiceLocator.Get<ResourceManager>();
        var playerController = ServiceLocator.Get<PlayerController>();
        
        var price = GetSellPrice();
        if (resourceManager.TryRemoveResource(tradeResourceClass.ResourceType, tradeResourceClass.ResourceAmount))
        {
            playerController.AddCredits(price);
            await resourceManager.SaveResources();
            await playerController.SavePlayerData();
            Debug.Log($"Resource sold successfully for {price}");
            Hide();
        }
        else
        {
            ServiceLocator.Get<UIController>().ShowPopupMessage("Warning", $"You do not have {tradeResourceClass.ResourceAmount} {tradeResourceClass.ResourceType}");
        }
        
    }
    
    private float GetSellPrice()
    {
        var resourcePricePerOne = ServiceLocator.Get<DataLibrary>().resourceDropData.possibleResources
            .First(entry => entry.resource == tradeResourceClass.ResourceType);
        float result = resourcePricePerOne.resourcePrice * tradeResourceClass.ResourceAmount * 0.34f;// TODO: Make an interesting price mechanic
        return result;
    }
    
    private void CloseButtonClick()
    {
        Hide();
    }
}
