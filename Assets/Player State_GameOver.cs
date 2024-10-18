using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_GameOver : PlayerStateBase
{
    public PlayerState_GameOver(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        tf.localScale = Vector2.one;
        playerCon.SkinDefault.SetActive(false);
        playerCon.SkinGameOver.SetActive(true);
    }
    public override void Update()
    {

    }
    public override void Exit()
    {
        playerCon.SkinDefault.SetActive(true);
        playerCon.SkinGameOver.SetActive(false);
    }
}
