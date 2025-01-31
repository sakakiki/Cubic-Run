using UnityEngine;

[CreateAssetMenu(fileName ="NewSkinData",menuName = "スキンデータ")]
public class SkinData : ScriptableObject
{
    //スキン名
    new public string name;


    //ボディの形
    public enum BodyType
    {
        Cube,
        Sphere
    }

    public BodyType bodyType;

    
    //スキンカラー
    public Color skinColor;

    //スキンのマスクを有効化するか
    public bool isEnabledMask;

    //UIのカラー
    public Color UIColor;
}
