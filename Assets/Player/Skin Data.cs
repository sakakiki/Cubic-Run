using System.Collections;
using System.Collections.Generic;
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


    //本体等の色
    public Color mainColor;
}
