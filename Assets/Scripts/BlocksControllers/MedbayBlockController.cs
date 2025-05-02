using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MedbayBlockController : StationBlockController
{
    private PlayerController playerController;
    private ReactiveProperty<bool> isProductionOn = new ReactiveProperty<bool>(false);
    private CompositeDisposable disposables = new CompositeDisposable();
    
    public override void BlockInitialization(StationBlockData _blockData)
    {
        base.BlockInitialization(_blockData);
        playerController = ServiceLocator.Get<PlayerController>();

        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        energyService.CurrentStationEnergy
            .Subscribe(value => isProductionOn.Value = value > 0)
            .AddTo(disposables);
    }
    
    private void OnDestroy()
    {
        disposables.Clear();
    }
    
    public override float GetProductionValue()
    {
        // Получаем коэфицент эффективности персонала отдела
        
        return 0f;
    }
    
    private bool IsStationEnergyEnough()
    {
        StationEnergyService energyService = ServiceLocator.Get<StationEnergyService>();
        if (energyService != null && energyService.CurrentStationEnergy.Value < 0)
        {
            Debug.Log("Недостаточно энергии для производства в " + name);
            return false;
        }
        return true;
    }
}
