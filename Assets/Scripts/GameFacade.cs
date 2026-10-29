using UnityEngine;

public sealed class GameFacade : MonoBehaviour
{
    [SerializeField] private GameMediator mediator;

    public void StartGame() => mediator.Begin();
    public void RestartGame() => mediator.Begin();
}
