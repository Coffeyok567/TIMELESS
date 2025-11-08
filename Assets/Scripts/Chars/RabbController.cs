using UnityEngine;

public class RabbController : CharacterBase
{
    //[Header("Настройки Крола")]

    public override void PerformMeleeAttack()
    {
        Debug.Log("Крол: атака тростью в ближнем бою");
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Крол: выстрелы томпсоном в дальнем бою");
        ShootTompson();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Крол: миньоны");
        //TO DO: как сделать систему миньонов?
    }

    public override void Dodge()
    {
        Debug.Log("Крол: уворот");
        // TO DO: уворот
    }




    void ShootTompson()
    {
        //TO DO: своровать рейкаст из револьвера еки
    }
}