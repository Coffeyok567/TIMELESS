using UnityEngine;

public class FinaControler : CharacterBase
{
    //[Header("Настройки Фины")]

    public override void PerformMeleeAttack()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Фина: атака палкой в ближнем бою");
    }

    public override void PerformRangedAttack()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Фина: атака копьем в дальнем бою");
        ShootSpearProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("Фина: стан");
        StunInSphere();
    }

    public override void Dodge()
    {
        Debug.Log("Фина: уворот");
        // TO DO: уворот
    }




    void ShootSpearProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward;
        projectile.GetComponent<Renderer>().material.color = Color.red;
        // TO DO: добавить Rigidbody и логику полета
    }

    void StunInSphere()
    {

    }
}