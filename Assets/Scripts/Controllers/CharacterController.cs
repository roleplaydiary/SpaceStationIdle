using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private CharacterStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<CharacterStateMachine>();
    }

    public void GoToWork(Vector3 workPosition)
    {
        stateMachine.SetState(new WorkState(stateMachine));
        transform.position = workPosition;
    }

    public void GoToRest(Vector3 restPosition)
    {
        //stateMachine.SetState(new RestState(stateMachine));
    }

    public void GotoIdle(Vector3 idlePosition)
    {
        //stateMachine.SetState(new IdleState(stateMachine));
    }
}
