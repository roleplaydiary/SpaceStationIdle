using System.Threading.Tasks;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private StationController stationController;
    [SerializeField] private PlayerController playerController;

    [SerializeField] private DataLibrary dataLibrary;

    private async void Start()
    {
        ServiceLocator.Register(dataLibrary);
        await GameInitialization();
    }

    private async Task GameInitialization()
    {
        await ServiceLocator.Get<CloudController>().Autentication();
        await playerController.PlayerInitialization();
        await stationController.StationInitializate();
    }
}
