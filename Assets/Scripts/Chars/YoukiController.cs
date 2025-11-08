using UnityEngine;

public class YoukiController : CharacterBase
{
    [Header("Музыкальные баффы Еки")]
    public MusicBuff[] availableTracks;

    [Header("Настройки Еки")]
    public GameObject defaultBuffEffect;
    public CharacterBase[] allCharacters;

    private int currentTrackIndex = 0;
    private MusicManager musicManager;

    void Start()
    {
        musicManager = MusicManager.Instance;
        if (musicManager == null)
        {
            Debug.LogError("MusicManager не найден в сцене");
        }
        /*
        if (availableTracks.Length > 0)
        {
            PlayTrack(0);
        }*/
    }

    public override void PerformMeleeAttack()
    {
        Debug.Log("Еки: атака ножами в ближнем бою");
        PlayKnifeAttack();
        ApplyCurrentBuffToAttack();
    }

    public override void PerformRangedAttack()
    {
        Debug.Log("Еки: выстрел из револьвера");
        ShootRevolver();
        ApplyCurrentBuffToAttack();
    }

    public override void UseAbility(bool isHold)
    {
        if (!isHold)
        {
            Debug.Log("Еки: смена музыки и применение баффов");
            SwitchMusicTrack();
            ApplyAreaBuffToAllPlayers();
        }
        else musicManager.StopTrack();
    }

    public override void Dodge()
    {
        Debug.Log("Еки: музыкальный уворот");
        PerformRhythmicDodge();
    }

    private void SwitchMusicTrack()
    {
        int nextTrackIndex = (currentTrackIndex + 1) % availableTracks.Length;
        PlayTrack(nextTrackIndex);
    }

    private void PlayTrack(int trackIndex)
    {
        if (musicManager == null || availableTracks.Length == 0) return;

        currentTrackIndex = trackIndex;
        MusicBuff selectedBuff = availableTracks[currentTrackIndex];

        musicManager.PlayTrack(selectedBuff);
        Debug.Log($"Еки включил: {selectedBuff.buffName}");
    }

    private void ApplyAreaBuffToAllPlayers()
    {
        if (availableTracks.Length == 0) return;

        MusicBuff currentBuff = availableTracks[currentTrackIndex];
        Debug.Log($"Пытаюсь применить бафф: {currentBuff.buffName}");

        int buffsApplied = 0;

        foreach (CharacterBase character in allCharacters)
        {
            if (character != null)
            {
                ApplyBuffToCharacter(character, currentBuff);
                buffsApplied++;
            }
        }
        Debug.Log($"Бафф применен к {buffsApplied} персонажам");
    }

    private void ApplyBuffToCharacter(CharacterBase character, MusicBuff buff)
    {
        character.ApplyMusicBuff(buff);
        
        Debug.Log($"Применен бафф '{buff.buffName}' к {character.name}");
    }

    private void ApplyCurrentBuffToAttack()
    {
        MusicBuff currentBuff = availableTracks[currentTrackIndex];

        // TO DO: применить бафф к текущей атаке
        float modifiedDamage = 10f * currentBuff.damageMultiplier;
        Debug.Log($"Атака Еки усилена баффом '{currentBuff.buffName}'. Модификатор урона: {currentBuff.damageMultiplier}");
    }

    private void PerformRhythmicDodge()
    {
        Debug.Log($"Уворот Еки");
    }

    private void PlayKnifeAttack()
    {
        // Визуальный эффект атаки ножами
        for (int i = 0; i < 2; i++)
        {
            GameObject knife = GameObject.CreatePrimitive(PrimitiveType.Cube);
            knife.transform.position = transform.position + transform.right * (i == 0 ? -0.5f : 0.5f) + transform.forward;
            knife.transform.localScale = new Vector3(0.1f, 0.1f, 0.3f);
            knife.GetComponent<Renderer>().material.color = Color.cyan;
            Destroy(knife, 0.3f);
        }
    }

    private void ShootRevolver()
    {

        GameObject bullet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bullet.transform.position = transform.position + transform.forward;
        bullet.transform.localScale = Vector3.one * 0.2f;
        bullet.GetComponent<Renderer>().material.color = Color.blue;

        Rigidbody bulletRb = bullet.AddComponent<Rigidbody>();
        bulletRb.useGravity = false;
        bulletRb.velocity = transform.forward * 25f;

        // Добавляем коллайдер и тег для идентификации
        bullet.tag = "PlayerProjectile";
        Destroy(bullet, 2f);
    }

    // Публичные методы для управления музыкой
    public void PlaySpecificTrack(int trackIndex)
    {
        if (trackIndex >= 0 && trackIndex < availableTracks.Length)
        {
            PlayTrack(trackIndex);
        }
    }

    public MusicBuff GetCurrentTrackInfo()
    {
        if (availableTracks.Length > 0 && currentTrackIndex < availableTracks.Length)
        {
            return availableTracks[currentTrackIndex];
        }
        return null;
    }

    public string GetCurrentTrackName()
    {
        MusicBuff current = GetCurrentTrackInfo();
        return current != null ? current.buffName : "No track";
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.3f, 0.5f, 1f, 0.3f);
        Gizmos.DrawSphere(transform.position, 8f);
    }
}