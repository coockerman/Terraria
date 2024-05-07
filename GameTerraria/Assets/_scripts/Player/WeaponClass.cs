using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponClass", menuName = "WeaponClass")]
public class WeaponClass : ScriptableObject
{
    public string nameWeapon;
    public Sprite sprite;
    public ItemEnum.WeaponType weaponType;
    public int dame;
    public float phamvi;
    public TileClass[] nguyenLieuCheTao;
    public GameObject bow;
}
