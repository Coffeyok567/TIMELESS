using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CharacterBase : MonoBehaviour
{
    [Header("Информация о персонаже")]
    public string charname;

    [Header("Компоненты")]
    protected Rigidbody rb;
    protected PlayerInput playerInput;
    protected Animator animator;
    [SerializeField] protected CameraFollow cameraFollow;

    [Header("Настройки")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Боевые настройки")]
    public WeaponSlot[] weaponSlots = new WeaponSlot[2];
    public int currentWeaponIndex = 0; // 0-ближнее, 1-дальнее

    [System.Serializable]
    public class WeaponSlot
    {
        public string slotName;
        public GameObject weaponObject;
        public bool isAvailable = true;
    }

    public float baseDamage = 10f;
    public float attackInterval = 0.5f;
    public float meleeRange = 2f;
    public float rangedRange = 10f;

    [Header("Текущие баффы")]
    public MusicBuff activeMusicBuff;
    public float damageMultiplier = 1f;
    public float speedMultiplier = 1f;
    public float attackSpeedMultiplier = 1f;

    protected Vector2 moveInput;
    protected Vector2 lookInput;
    private Vector3 movement;
    private bool isGrounded;
    private Transform cameraTransform;

    // Общие анимации - ИСПРАВЛЕНО: правильные названия параметров
    private readonly int isMovingHash = Animator.StringToHash("IsMoving");
    private readonly int jumpTriggerHash = Animator.StringToHash("Jump");

    // melee-анимации
    private readonly int meleeAttack1Hash = Animator.StringToHash("FirstMeleeAttack");
    private readonly int meleeAttack2Hash = Animator.StringToHash("SecondMeleeAttack");

    // ranged-анимации
    private readonly int rangedAttackHash = Animator.StringToHash("RangedAttack");
    private readonly int rangedIdleHash = Animator.StringToHash("RangedIdle");

    public GameObject meleeModel;
    public GameObject rangedModel;

    // Переменные для управления атакой
    private bool canAttack = true;
    private float lastAttackTime = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        FindActiveAnimator();
        animator = GetComponent<Animator>();

        if (Camera.main != null)
            cameraTransform = Camera.main.transform;

        InitializeWeapons();
    }

    private void FindActiveAnimator()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("Animator not found in character model!");
        }
        else
        {
            Debug.Log($"Animator found: {animator.name}");
        }
    }

    private void InitializeWeapons()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].weaponObject != null)
            {
                weaponSlots[i].weaponObject.SetActive(i == currentWeaponIndex);
            }
        }

        // Принудительно обновляем аниматор при старте
        SwitchCharacterModel();
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = moveInput.magnitude > 0.1f;

        // ОТЛАДКА: выводим состояние движения
        Debug.Log($"UpdateAnimations - isMoving: {isMoving}, Weapon: {currentWeaponIndex}, Animator: {animator.name}");

        // ОБЩИЕ анимации для обеих моделей
        animator.SetBool(isMovingHash, isMoving);

        // Специфические анимации для дальнего боя
        if (currentWeaponIndex == 1)
        {
            animator.SetBool(rangedIdleHash, !isMoving);
        }
        else
        {
            // Для meleeModel гарантируем, что rangedIdle выключен
            animator.SetBool(rangedIdleHash, false);
        }
    }

    void Update()
    {
        Move();
        UpdateAnimations();

        // Проверяем кулдаун атаки
        if (Time.time - lastAttackTime >= attackInterval)
        {
            canAttack = true;
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && canAttack)
        {
            if (currentWeaponIndex == 0)
            {
                PlayMeleeAnimation();
                PerformMeleeAttack();
            }
            else
            {
                PlayRangedAnimation();
                PerformRangedAttack();
            }

            canAttack = false;
            lastAttackTime = Time.time;
        }
    }

    private void PlayMeleeAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null in PlayMeleeAnimation!");
            return;
        }

        int attackIndex = Random.Range(1, 3);

        if (attackIndex == 1)
            animator.SetTrigger(meleeAttack1Hash);
        else
            animator.SetTrigger(meleeAttack2Hash);

        Debug.Log("Melee attack animation played");
    }

    private void PlayRangedAnimation()
    {
        if (animator == null)
        {
            Debug.LogError("Animator is null in PlayRangedAnimation!");
            return;
        }

        animator.SetTrigger(rangedAttackHash);
        Debug.Log("Ranged attack animation played");
    }

    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Vector2 scrollValue = context.ReadValue<Vector2>();
            if (scrollValue != Vector2.zero)
            {
                SwitchWeapon(scrollValue);
            }
        }
    }

    private void SwitchWeapon(Vector2 direction)
    {
        int newIndex = currentWeaponIndex;
        do
        {
            newIndex = (newIndex + (int)direction.y + weaponSlots.Length) % weaponSlots.Length;
        }
        while (!weaponSlots[newIndex].isAvailable && newIndex != currentWeaponIndex);

        if (weaponSlots[newIndex].isAvailable && newIndex != currentWeaponIndex)
        {
            SetCurrentWeapon(newIndex);
        }
    }

    private void SetCurrentWeapon(int newIndex)
    {
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            weaponSlots[currentWeaponIndex].weaponObject.SetActive(false);
        }

        currentWeaponIndex = newIndex;
        if (weaponSlots[currentWeaponIndex].weaponObject != null)
        {
            weaponSlots[currentWeaponIndex].weaponObject.SetActive(true);
        }

        SwitchCharacterModel();
        Debug.Log($"Переключено на оружие: {weaponSlots[currentWeaponIndex].slotName}");
    }

    private void SwitchCharacterModel()
    {
        if (meleeModel == null || rangedModel == null) return;

        bool isMeleeWeapon = currentWeaponIndex == 0;
        meleeModel.SetActive(isMeleeWeapon);
        rangedModel.SetActive(!isMeleeWeapon);

        // Обновляем аниматор при смене модели
        Animator newAnimator = isMeleeWeapon ?
            meleeModel.GetComponent<Animator>() :
            rangedModel.GetComponent<Animator>();

        if (newAnimator != null)
        {
            animator = newAnimator;
            Debug.Log($"Аниматор переключен на: {(isMeleeWeapon ? "Melee" : "Ranged")} - {animator.name}");

            // Сбрасываем все анимации при смене модели
            ResetAllAnimations();

            // Принудительно обновляем аниматор
            animator.Rebind();
            animator.Update(0f);
        }
        else
        {
            Debug.LogError($"Не удалось найти аниматор для {(isMeleeWeapon ? "Melee" : "Ranged")} модели!");
        }
    }

    // Метод для сброса всех анимаций
    private void ResetAllAnimations()
    {
        if (animator == null) return;

        // Сбрасываем все триггеры
        animator.ResetTrigger(meleeAttack1Hash);
        animator.ResetTrigger(meleeAttack2Hash);
        animator.ResetTrigger(rangedAttackHash);
        animator.ResetTrigger(jumpTriggerHash);

        // Сбрасываем bool параметры
        animator.SetBool(isMovingHash, false);
        animator.SetBool(rangedIdleHash, false);

        Debug.Log("Все анимации сброшены");
    }

    public void SetWeaponByIndex(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index].isAvailable)
        {
            SetCurrentWeapon(index);
        }
    }

    public void SetWeaponAvailable(int index, bool available)
    {
        if (index >= 0 && index < weaponSlots.Length)
        {
            weaponSlots[index].isAvailable = available;
            if (!available && currentWeaponIndex == index)
            {
                SwitchToFirstAvailableWeapon();
            }
        }
    }

    private void SwitchToFirstAvailableWeapon()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (weaponSlots[i].isAvailable)
            {
                SetCurrentWeapon(i);
                return;
            }
        }
    }

    public virtual void ApplyMusicBuff(MusicBuff buff)
    {
        activeMusicBuff = buff;
        damageMultiplier = buff.damageMultiplier;
        speedMultiplier = buff.moveSpeedMultiplier;
        attackSpeedMultiplier = buff.attackSpeedMultiplier;
        Debug.Log($"{name} получил бафф: {buff.buffName}");
    }

    public virtual void ResetBuffs()
    {
        activeMusicBuff = null;
        damageMultiplier = 1f;
        speedMultiplier = 1f;
        attackSpeedMultiplier = 1f;
    }

    public virtual void OnCharacterSelected()
    {
        if (playerInput != null)
            playerInput.enabled = true;

        ResetAllAnimations();
    }

    public virtual void OnCharacterDeselected()
    {
        if (playerInput != null)
            playerInput.enabled = false;

        ResetAllAnimations();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log($"OnMove called: {moveInput}");
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
            Jump();
    }

    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UseAbility(true);
        }
        else if (context.canceled)
        {
            UseAbility(false);
        }
    }

    private void Move()
    {
        if (cameraTransform == null)
        {
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
            else
                return;
        }

        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1));
        Vector3 cameraRight = Vector3.Scale(cameraTransform.right, new Vector3(1, 0, 1));

        movement = (cameraForward * moveInput.y + cameraRight * moveInput.x).normalized;

        float currentMoveSpeed = moveSpeed * speedMultiplier;
        Vector3 targetVelocity = movement * currentMoveSpeed;
        targetVelocity.y = rb.velocity.y;

        rb.velocity = targetVelocity;

        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    public virtual void Jump()
    {
        if (isGrounded && animator != null)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger(jumpTriggerHash);
            Debug.Log("Jump trigger activated!");
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.contacts.Length > 0)
        {
            float angle = Vector3.Angle(collision.contacts[0].normal, Vector3.up);
            if (angle < 45f)
            {
                isGrounded = true;
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public abstract void PerformMeleeAttack();
    public abstract void PerformRangedAttack();
    public abstract void UseAbility(bool isHold);
    public abstract void Dodge();
}