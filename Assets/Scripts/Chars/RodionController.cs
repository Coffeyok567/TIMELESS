using UnityEngine;
using UnityEngine.InputSystem;

public class RodionController : CharacterBase
{
    //[Header("Настройки Родиона")]
    /*
    public override void OnCharacterSelected()
    {
        if (playerInput != null)
            playerInput.enabled = true;
        Screen.SetResolution(640, 360, true);
        //Debug.Log($"{type} выбран, ввод включен");
    }

    public override void OnCharacterDeselected()
    {
        if (playerInput != null)
            playerInput.enabled = false;
        Screen.SetResolution(Screen.width, Screen.height, true);
        //Debug.Log($"{type} отменен, ввод выключен");
    }*/

    public override void PerformMeleeAttack()
    {
        Debug.Log("Родион: атака лобзиком в ближнем бою");
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Родион: выстрел аннигилятором в дальнем бою");
        ShootAnnihilator();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Родион: SMOKIN` SEXY STYLE");
        //TO DO: как сделать систему стиля?
    }

    public override void Dodge()
    {
        Debug.Log("Родион: уворот");
        // TO DO: уворот
    }




    void ShootAnnihilator()
    {
        //TO DO: своровать рейкаст из револьвера еки
    }
}