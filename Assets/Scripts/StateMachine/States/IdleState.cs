using UnityEngine;

public class IdleState : ICharacterState
{
    private readonly CharacterStateMachine machine;

    public IdleState(CharacterStateMachine machine)
    {
        this.machine = machine;
    }

    public void Enter()
    {
        // Включить анимацию работы
    }

    public void Update()
    {
        // Работа может длиться определённое время
    }

    public void Exit()
    {
        // Остановить анимацию
    }
}
