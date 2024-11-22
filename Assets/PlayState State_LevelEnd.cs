using TMPro;

public class PlayStateState_LevelEnd : PlayStateStateBase
{
    private PlayerStateMachine playerStateMachine;
    private int levelUpScore;
    private TextMeshProUGUI scoreText;



    public PlayStateState_LevelEnd(PlayStateStateMachine stateMachine) : base(stateMachine)
    {
        playerStateMachine = playerCon.stateMachine;
        scoreText = GameManager.Instance.scoreText;
    }



    public override void Enter()
    {
        //���x�����㏸����X�R�A���Z�o
        levelUpScore = GM.level * GM.levelUpSpan;

        //��Q�������𖳌���
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        //�v���C���ԉ��Z
        playTime += deltaTime;

        //�ړ����̒n�`���Ǘ�
        TM.ManageMovingTerrain();

        //�X�R�A�X�V
        GM.score = (int)(playTime * 100);
        scoreText.SetText("" + GM.score);

        //�X�R�A�����x���㏸��𖞂����΃X�e�[�g�J��
        if (GM.score > levelUpScore)
            stateMachine.ChangeState(stateMachine.state_LevelStart);

        //�v���C���[��GameOver�X�e�[�g�Ȃ�GameOver�X�e�[�g�ɑJ��
        if (playerStateMachine.currentState == playerStateMachine.state_GameOver)
            stateMachine.ChangeState(stateMachine.state_GameOver);
    }



    public override void Exit()
    {

    }
}
