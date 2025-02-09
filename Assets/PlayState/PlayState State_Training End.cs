using UnityEngine;

public class PlayStateState_TrainingEnd: PlayStateStateBase
{
    public PlayStateState_TrainingEnd(PlayStateStateMachine stateMachine) : base(stateMachine) { }



    public override void Enter()
    {
        //��Q�������𖳌���
        TM.isCreateObstacle = false;
    }



    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        //��X�R�A�ɓ��B�����
        if (GM.score >= 5000)
        {
            //�X�R�A��␳
            GM.score = 5000;
            GM.scoreText.SetText("5000");

            //�X�R�A�Q�[�W�␳
            GM.scoreGageTf.localScale = Vector3.one * 2;

            //�N���A�񐔂����Z
            GM.AddClearTimesNum(GM.level);

            //�g���[�j���O���[�h�̍ō����x�����N���A�����Ȃ�
            if (GM.trainingLevel == GM.highestLevel)
            {
                //�g���[�j���O���[�h�̃��x����ǉ�
                GM.AddTrainingLevel();

                //���g���C�{�^���̃e�L�X�g��ύX
                GM.retryButtonText.SetText("Lv." +  GM.trainingLevel + "��");
            }
            //�N���A�ς݂̃��x���Ȃ�
            else
            {
                //���g���C�{�^���̃e�L�X�g��ύX
                GM.retryButtonText.SetText("���g���C");
            } 

            //�X�e�[�g�J��
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        }
    }



    public override void Exit()
    {

    }
}
