using UnityEngine;
using UniRx;

public class BonusPickup : MonoBehaviour
{
    public readonly BehaviorSubject<bool> OnBonusPickedUp = new BehaviorSubject<bool>(false);

    private void OnMouseDown()
    {
        // Этот метод вызывается, когда игрок нажимает на коллайдер этого объекта
        Debug.Log("BONUS PICKUP!!!");
        OnBonusPickedUp.OnNext(true);
    }
}