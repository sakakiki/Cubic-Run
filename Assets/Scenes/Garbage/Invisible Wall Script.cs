using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class InvisibleWallScript : MonoBehaviour
{
    private BoxCollider2D BCol;
    private GameObject Player;
    void Start()
    {
        BCol = GetComponent<BoxCollider2D>();
        Player = GameManagerScript.Instance.Player;
    }


    void Update()
    {
        BCol.size = 
            Vector2.one + 
            ((Player.GetComponent<PlayerScript>().currentPlayerState == PlayerScript.PlayerState.Squat
            || GameManagerScript.Instance.currentPlayState == GameManagerScript.PlayState.GameOver)
            ? Vector2.zero : Vector2.up);
    }
}
