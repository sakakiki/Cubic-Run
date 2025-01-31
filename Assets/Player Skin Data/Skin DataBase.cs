using System.Collections.Generic;
using UnityEngine;

public class SkinDataBase : MonoBehaviour
{
    //自身のインスタンス
    public static SkinDataBase Instance;

    //スキンデータのリスト
    public List<SkinData> skinData = new List<SkinData>();



    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }
}
