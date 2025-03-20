using UnityEngine;

public class TutorialStateStateMachine
{
    public GameStateStateMachine gameStateMachine;

    public TutorialStateStateBase currentState {  get; private set; }

    public TutorialStateStateBase state_Start { get; private set; }
    public TutorialStateStateBase state_Jump_1 { get; private set; }
    public TutorialStateStateBase state_Jump_2 { get; private set; }
    public TutorialStateStateBase state_Jump_3 { get; private set; }
    public TutorialStateStateBase state_Squat_1 { get; private set; }
    public TutorialStateStateBase state_Squat_2 { get; private set; }
    public TutorialStateStateBase state_Squat_3 { get; private set; }
    public TutorialStateStateBase state_Attack_1 { get; private set; }
    public TutorialStateStateBase state_Attack_2 { get; private set; }
    public TutorialStateStateBase state_Attack_3 { get; private set; }



    public TutorialStateStateMachine(GameStateStateMachine gameStateMachine)
    {
        this.gameStateMachine = gameStateMachine;
        state_Start = new TutorialStateState_Start(this);
        state_Jump_1 = new TutorialStateState_Jump_1(this);
        state_Jump_2 = new TutorialStateState_Jump_2(this);
        state_Jump_3 = new TutorialStateState_Jump_3(this);
        state_Squat_1 = new TutorialStateState_Squat_1(this);
        state_Squat_2 = new TutorialStateState_Squat_2(this);
        state_Squat_3 = new TutorialStateState_Squat_3(this);
        state_Attack_1 = new TutorialStateState_Attack_1(this);
        state_Attack_2 = new TutorialStateState_Attack_2(this);
        state_Attack_3 = new TutorialStateState_Attack_3(this);
        TutorialStateStateBase.continueState = state_Start;
    }

    public void Initialize(TutorialStateStateBase firstState)
    {
        currentState = firstState;
        firstState.Enter();
    }

    public void ChangeState(TutorialStateStateBase newState)
    {
        currentState.Exit();
        currentState = newState;
        newState.Enter();
    }

    public void Update(float deltaTime)
    {
        //ステートに応じたUpdateの実行
        if (currentState != null) currentState.Update(deltaTime);
    }
}
