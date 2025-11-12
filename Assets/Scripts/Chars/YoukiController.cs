using System.Collections;
using TMPro;
using UnityEngine;

public class YoukiController : CharacterBase
{
    [Header("Музыкальные баффы Еки")]
    public MusicBuff[] availableTracks;

    [Header("Настройки Еки")]
    public TMP_Text songText;
    public GameObject defaultBuffEffect;
    public CharacterBase[] allCharacters;

    private int currentTrackIndex = 0;
    private MusicManager musicManager;

    void Start()
    {
        musicManager = MusicManager.Instance;
    }

    public override void PerformMeleeAttack()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log($"Еки: атака ножами в ближнем бою");
        //PlayKnifeAttack();
        ShootRevolver();
    }

    public override void PerformRangedAttack()
    {
        // TODO: добавить мультипликаторы урона к итоговой реализации
        Debug.Log("Еки: выстрел из револьвера");
        ShootRevolver();
    }

    public override void UseAbility(bool isHold)
    {
        if (isHold)
        {
            Debug.Log("Еки: смена музыки и применение баффов");
            SwitchMusicTrack();
            ApplyAreaBuffToAllPlayers();
        }
        else
        {
            musicManager.StopTrack();
            CancelBuffsToAllPlayers();
        }
    }

    public override void Dodge()
    {
        Debug.Log("Еки: уворот");
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
        // Создаем луч из центра экрана игрока
        Ray ray = this.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        // Проверяем попадание луча в объект
        if (Physics.Raycast(ray, out hitInfo, weaponRange))
        {
            /*PlayerHealth hitPlayerHealth = hitInfo.collider.GetComponent<PlayerHealth>(); // Получаем компонент PlayerHealth на объекте, в который попал луч
            if (hitPlayerHealth != null) // Если объект имеет компонент PlayerHealth, наносим ему урон
            {
                hitPlayerHealth.TakeDamage(dealingDamage);
                Debug.Log($"Нанесено {dealingDamage} урона");
            }*/

            GameObject impactObj = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)); // Воспроизводим эффект попадания
            Destroy(impactObj, impactEffectDuration); // Уничтожаем эффект через определенное время
            Debug.Log("Попадание");

            // Отображаем след пули
            LineRenderer tracer = Instantiate(tracerEffect, gunEnd.position, Quaternion.identity);
            StartCoroutine(ShowTracerEffect(tracer, gunEnd.position, hitInfo.point));
        }
        else
        {
            Debug.Log("Промах");
            Vector3 endPoint = ray.origin + ray.direction * Mathf.Min(weaponRange, weaponRange); // Определяем точку, где луч должен закончиться
            GameObject impactObj = Instantiate(impactEffect, endPoint, Quaternion.identity); // Воспроизводим эффект попадания на этой точке
            Destroy(impactObj, impactEffectDuration); // Уничтожаем эффект через определенное время

            // Отображаем след пули
            LineRenderer tracer = Instantiate(tracerEffect, gunEnd.position, Quaternion.identity);
            StartCoroutine(ShowTracerEffect(tracer, gunEnd.position, endPoint));
        }
    }

    // Публичные методы для управления музыкой
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
        songText.text = $"Now playing:\n{selectedBuff.buffName}";
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
                character.ApplyMusicBuff(currentBuff);
                buffsApplied++;
            }
        }
        Debug.Log($"Бафф применен к {buffsApplied} персонажам");
    }

    private void CancelBuffsToAllPlayers()
    {
        if (availableTracks.Length == 0) return;

        foreach (CharacterBase character in allCharacters)
        {
            if (character != null)
            {
                character.ResetBuffs();
            }
        }
        Debug.Log($"Баффы сняты");
    }

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

/*TODO
 * добавить снижение разрешения при выборе родиона
 * скачать и вставить прозрачные бандикам и фуллхд
 * начать делать врагов
 * добавить рейкаст кому надо и баллистику кому надо (своровать из курсового)
 * сделать радиальное меню для выбора треков Еки
 */