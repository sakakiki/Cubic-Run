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

            //�g���[�j���O���[�h�̍ō����x�����N���A�����Ȃ�
            if (GM.trainingLevel == GM.highestLevel)
            {
                //�ō����B���x�����X�V
                GM.highestLevel++;

                //�g���[�j���O���[�h���x���I���p�l����ǉ�
                IM.AddLevelPanel(GM.highestLevel);

                //�g���[�j���O���[�h�̑I�����x�����X�V
                GM.SetTrainingLevel(GM.highestLevel);
                IM.button_LevelSelecters[GM.highestLevel - 1].PushButton();

                //���g���C�{�^���̃e�L�X�g��ύX
                GM.retryButtonText.SetText("Lv." +  GM.trainingLevel + "��");
            }
            //���g���C�{�^���̃e�L�X�g��ύX
            else GM.retryButtonText.SetText("���g���C");

            //�X�e�[�g�J��
            gameStateMachine.ChangeState(gameStateMachine.state_PlayToResult);
        }
    }



    public override void Exit()
    {

    }
}
