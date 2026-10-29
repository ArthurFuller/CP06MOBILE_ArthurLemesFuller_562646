public interface IGameCommand
{
    void Execute();
}

public sealed class StartGameCommand : IGameCommand
{
    private readonly GameFacade facade;
    public StartGameCommand(GameFacade gameFacade) => facade = gameFacade;
    public void Execute() => facade.StartGame();
}

public sealed class RestartGameCommand : IGameCommand
{
    private readonly GameFacade facade;
    public RestartGameCommand(GameFacade gameFacade) => facade = gameFacade;
    public void Execute() => facade.RestartGame();
}
