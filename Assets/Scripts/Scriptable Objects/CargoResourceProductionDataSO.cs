using UnityEngine;

[CreateAssetMenu(fileName = "CargoResourceData", menuName = "Game Data/Cargo Resource Data")]
public class CargoResourceProductionDataSO : ScriptableObject
{
    [System.Serializable]
    public struct ResourceEntry
    {
        public ResourceType resource;
        public Sprite icon;
        public float resourcePrice;
        [Range(0f, 1f)] public float dropProbability; // Вероятность выпадения (от 0 до 1)
        public float minAmount;
        public float maxAmount;
    }

    public ResourceEntry[] possibleResources;
}