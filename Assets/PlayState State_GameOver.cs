using TMPro;

public class PlayStateState_GameOver : PlayStateStateBase
{
    public PlayStateState_GameOver(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //�n�`�̒�~
        TerrainManager.Instance.SetSpeed(0);
    }



    public override void Update(float deltaTime)
    {

    }



    public override void Exit()
    {

    }
}
