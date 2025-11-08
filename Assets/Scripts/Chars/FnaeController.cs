using UnityEngine;

public class FnaeController : CharacterBase
{
    [Header("Ќастройки ‘на€")]
    public float burnDamage = 10f;
    public GameObject fireEffect;

    public override void PerformMeleeAttack()
    {
        Debug.Log("‘най: атака перчаткой в ближнем бою");
        CreateBurnEffect();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("‘най: атака перчаткой в дальнем бою");
        ShootFireProjectile();
    }

    public override void UseAbility(bool isHold)
    {
        Debug.Log("‘най: молотов");
        IgniteArea();
    }

    public override void Dodge()
    {
        Debug.Log("‘най: уворот");
    }

    private void CreateBurnEffect()
    {
        if (fireEffect != null)
        {
            Instantiate(fireEffect, transform.position, Quaternion.identity);
        }
    }

    private void ShootFireProjectile()
    {
        GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward;
        projectile.GetComponent<Renderer>().material.color = Color.red;
        // TO DO: добавить Rigidbody и логику полета
    }

    private void IgniteArea()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // TO DO: нанести урон горением
            }
        }
    }
}