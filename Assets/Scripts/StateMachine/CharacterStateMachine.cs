using UnityEngine;

public class CharacterStateMachine : MonoBehaviour
{
    private ICharacterState currentState;

    public void SetState(ICharacterState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    private void Update()
    {
        currentState?.Update();
    }
}