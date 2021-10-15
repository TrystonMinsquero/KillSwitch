using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Weapon weapon;

    public SpriteRenderer weaponSR;
    public SpriteRenderer flashSR;
    public Animator weaponAnim;
    public Animator flashAnim;

    void Start()
    {
        Set();
    }

    public void SwitchWeapons(WeaponHandler weaponHandler)
    {
        if (weaponHandler.weapon == null)
            this.weapon = null;
        else
            weapon = weaponHandler.weapon;
        Set();
    }


    public void Set()
    {
        SwitchVisuals();
        if(weapon != null)
            weapon.Reset();
    }

    public void SwitchVisuals()
    {
        if (weapon == null)
        {
            flashSR.sprite = null;
            weaponSR.sprite = null;
            return;
        }
        weaponSR.color = weapon.color;
        weaponAnim.runtimeAnimatorController = weapon.weaponAoc;
        flashAnim.runtimeAnimatorController = weapon.flashAoc;
    }

    public void SwitchVisuals(Weapon newWeapon)
    {
        if (newWeapon == null)
        {
            flashSR.sprite = null;
            weaponSR.sprite = null;
            return;
        }
        weaponSR.color = newWeapon.color;
        weaponAnim.runtimeAnimatorController = newWeapon.weaponAoc;
        flashAnim.runtimeAnimatorController = newWeapon.flashAoc;
    }
    

    public void Shoot(Player player)
    {
        if(weapon != null)
        {
            if(weapon.Shoot(player))
                flashAnim.Play("Flash");
        }
        //muzzle flash start

    }

    public void SetAnimations(string stateName)
    {
        if(weapon!=null)
            weaponAnim.Play(stateName);
    }

    public List<Projectile> GetProjectiles()
    {
        return weapon.projectiles;
    }



}
