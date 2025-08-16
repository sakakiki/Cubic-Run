using UnityEngine;

public class PlayerState_GameOver : PlayerStateBase
{
    private float elapsedTime;
    private Vector2 tunnelScale = new Vector2(1.3f, 0.6f);

    public PlayerState_GameOver(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        //�o�ߎ��Ԃ̏�����
        elapsedTime = 0;

        //�Q�[���X�e�[�g�̑J��
        //�ʏ�v���C�Ȃ烊�U���g��
        if (TutorialStateStateBase.continueState == 
            ((GameStateState_Tutorial)gameStateMachine.state_Tutorial).tutorialStateMachine.state_Start)
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        //�`���[�g���A���Ȃ烊�g���C
        else
            gameStateMachine.ChangeState(gameStateMachine.state_TutorialRetry);

        //�g���l�����Ȃ��p�̃X�P�[����
        if (TerrainManager.Instance.currentTerrainNum == 3 && tf.position.y < 0.5)
            tf.localScale = tunnelScale;

        //�g���l���O����ʓ��Ȃ璵�˕Ԃ菈��
        else if(tf.position.y > -5)
        {
            tf.localScale = Vector2.one;
            Rigidbody2D playerRb = GameManager.Instance.playerCon.rb;
            playerRb.constraints = RigidbodyConstraints2D.None;
            playerRb.angularVelocity = TerrainManager.Instance.moveSpeed * 20;
            playerRb.linearVelocity = Vector2.up * 15 + Vector2.left * TerrainManager.Instance.moveSpeed * 0.3f;
        }

        //�Q�[���I�[�o�[�p�X�L���ɐ؂�ւ�
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinGameOver.SetActive(true);

        //�X�L���`�󂪃X�t�B�A�Ȃ�R���C�_�[�ύX
        if (SkinDataBase.Instance.skinData[GameManager.Instance.usingSkinID].bodyType == SkinData.BodyType.Sphere)
        {
            playerCon.boxCol.enabled = false;
            playerCon.capsuleCol.enabled = true;
        }

        //SE�Đ�
        AudioManager.Instance.PlaySE(AudioManager.SE.GameOver);
        
        //GameState���ɒn�`����̂��߂̑ҋ@����
        GameStateState_ResultToPlay.isPlayerCheck = false;
    }

    public override void Update()
    {
        //�o�ߎ��Ԃ̉��Z
        elapsedTime += Time.deltaTime;

        //�Q�[���X�e�[�g�̑J�ڂɍ��킹�ăX�e�[�g�J��
        if (gameStateMachine.currentState == gameStateMachine.state_ResultToMenu)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToMenu);
        else if (gameStateMachine.currentState == gameStateMachine.state_ResultToPlay ||
                 gameStateMachine.currentState == gameStateMachine.state_TutorialRetry && elapsedTime > 1)
            stateMachine.ChangeState(stateMachine.state_Model_ResultToPlay);
    }

    public override void Exit()
    {
        //�ʏ�X�L���ɐ؂�ւ�
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinGameOver.SetActive(false);
    }
}
