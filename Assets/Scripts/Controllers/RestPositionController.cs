using UnityEngine;

public class RestPositionController : MonoBehaviour
{
    public bool IsUnlocked { get; private set; } = false;
    public bool IsOccupied { get; private set; } = false;
    [SerializeField] private CharacterController crewMember = null;

    public void OccupyRestPosition(CharacterController crewMember)
    {
        this.crewMember = crewMember;
        IsOccupied = true;
    }

    public CharacterController GetRestCrewMember()
    {
        return crewMember;
    }

    public void ReleaseRestPosition()
    {
        IsOccupied = false;
        crewMember = null;
    }

    public void UnlockRestPosition()
    {
        IsUnlocked = true;
    }

    public void LockRestPosition()
    {
        IsUnlocked = false;
    }
}
