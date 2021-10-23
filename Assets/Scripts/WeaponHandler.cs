using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Weapon weapon;

    [Header("Visual Componenets")]
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


    //Sets up the weapon visually and functionally
    public void Set()
    {
        SwitchVisuals();
        if(weapon != null)
            weapon.Reset();
    }

    public void EnableVisuals()
    {
        weaponSR.enabled = true;
        flashSR.enabled = true;
    }

    public void DisableVisuals()
    {
        weaponSR.enabled = false;
        flashSR.enabled = false;
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

    public void Shoot(UnityEngine.InputSystem.PlayerInput player, Vector2 direction)
    {
        if(weapon != null)
        {
            if(weapon.Shoot(player, direction))
                flashAnim.Play("Flash");
        }
        //muzzle flash start

    }

    //plays the animation of the stateName
    public void SetAnimations(string stateName)
    {
        if(weapon!=null)
            weaponAnim.Play(stateName);
    }



}
