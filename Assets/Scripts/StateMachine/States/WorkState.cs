using UnityEngine;

public class WorkState : ICharacterState
{
    private readonly CharacterStateMachine machine;

    public WorkState(CharacterStateMachine machine)
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