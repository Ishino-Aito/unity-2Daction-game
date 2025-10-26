using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// プレイヤーの体力と被ダメージシステムを管理するクラス
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("体力設定")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float defaultDamageAmount = 34f;
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    [Header("UI設定")]
    [SerializeField] private Image hpGaugeImage;
    [SerializeField] private RectTransform hpGaugeRect;
    private float originalGaugeWidth;

    [Header("無敵時間設定")]
    [SerializeField] private float invincibilityDuration = 1f; // 無敵時間の長さ
    [SerializeField] private float blinkInterval = 0.1f;      // 点滅間隔
    private bool isInvincible = false;                        // 無敵時間フラグ
    private float invincibilityTimer = 0f;                    // 無敵時間タイマー
    private float blinkTimer = 0f;                            // 点滅タイマー
    private bool isVisible = true;                            // 表示フラグ

    [Header("ヘルメット設定")]
    private bool hasHelmet = false;                           // 通常ヘルメット所持フラグ
    private bool hasBlueHelmet = false;                       // 青ヘルメット所持フラグ

    // コンポーネント取得
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    [Header("音響設定")]
    [SerializeField] private AudioSource audioSource;          // AudioSourceコンポーネント
    [SerializeField] private AudioClip damageSE;               // ダメージ効果音 (se_dageki23)
    [SerializeField] private AudioClip powerupSE;              // パワーアップ効果音 (se_powerup)
    [SerializeField] [Range(0f, 1f)] private float damageVolume = 0.5f;   // ダメージ音の音量
    [SerializeField] [Range(0f, 1f)] private float powerupVolume = 0.5f;  // パワーアップ音の音量

    private void Start()
    {
        // 体力を初期化
        currentHealth = maxHealth;

        // オーディオシステムの初期化
        InitializeAudioSystem();

        // 必要なコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animatorコンポーネントが見つかりません。");
        }
        else
        {
            // 開始時はヘルメットを装備していない状態にリセット
            ResetHelmetStates();
        }

        // HPゲージの初期幅を保存
        if (hpGaugeRect != null)
        {
            originalGaugeWidth = hpGaugeRect.sizeDelta.x;
        }

        // 体力UIを更新
        UpdateHealthUI();
    }

    /// <summary>
    /// オーディオシステムの初期化
    /// </summary>
    private void InitializeAudioSystem()
    {
        // AudioSourceコンポーネントの取得または追加
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("PlayerHealth: AudioSourceコンポーネントを自動追加しました。");
            }
        }

        // AudioSourceの基本設定
        audioSource.playOnAwake = false;  // 自動再生しない
        audioSource.spatialBlend = 0f;    // 2Dサウンド(3D効果なし)
    }

    /// <summary>
    /// 効果音を再生
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    /// <param name="volume">音量（0.0～1.0）</param>
    private void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            if (clip == null) Debug.LogWarning("PlayerHealth: 再生しようとした効果音が設定されていません。");
            if (audioSource == null) Debug.LogWarning("PlayerHealth: AudioSourceが見つかりません。");
        }
    }

    /// <summary>
    /// ダメージ効果音を再生
    /// </summary>
    private void PlayDamageSE()
    {
        PlaySE(damageSE, damageVolume);
    }

    /// <summary>
    /// パワーアップ効果音を再生
    /// </summary>
    private void PlayPowerupSE()
    {
        PlaySE(powerupSE, powerupVolume);
    }

    private void Update()
    {
        // 無敵時間の処理
        if (isInvincible)
        {
            // 無敵タイマーを減少
            invincibilityTimer -= Time.deltaTime;

            // 点滅処理
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                blinkTimer = blinkInterval;
                if (spriteRenderer != null)
                {
                    isVisible = !isVisible;
                    spriteRenderer.enabled = isVisible;
                }
            }

            // 無敵時間が終了したら
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true; // 必ず表示状態に戻す
                }
                Debug.Log("無敵時間が終了しました");
            }
        }
    }

    /// <summary>
    /// 全てのヘルメット状態をリセット
    /// </summary>
    private void ResetHelmetStates()
    {
        hasHelmet = false;
        hasBlueHelmet = false;

        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("ヘルメット状態をリセットしました");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // オブジェクトタグで処理を分岐させる
        switch (other.tag)
        {
            case "Helmet":
                // 通常のヘルメット取得
                EquipHelmet();
                Destroy(other.gameObject);
                break;

            case "BlueHelmet":
                // 青いヘルメット取得
                EquipBlueHelmet();
                Destroy(other.gameObject);
                break;

            case "Beam":
            case "Blue Beam":
                // ビームとの衝突処理
                HandleBeamCollision(other);
                break;

            case "Toge":
                // トゲなどのダメージオブジェクトとの衝突処理
                if (!isInvincible)
                {
                    TakeDamage(defaultDamageAmount);
                    ActivateInvincibility(); // ダメージを受けたら無敵時間を起動
                }
                break;
        }
    }

    /// <summary>
    /// 無敵時間を有効化
    /// </summary>
    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;
        Debug.Log("無敵時間を開始します: " + invincibilityDuration + "秒");
    }

    /// <summary>
    /// ビームとの衝突処理を実行
    /// </summary>
    private void HandleBeamCollision(Collider2D beam)
    {
        // ビームの種類を判定
        bool isBlueBeam = beam.CompareTag("Blue Beam");

        // 青いヘルメットを装備していて、青いビームに当たった場合の防御処理
        if (hasBlueHelmet && isBlueBeam)
        {
            Debug.Log("青いヘルメットが青いビームを防いだ！");
            RemoveBlueHelmet();
            Destroy(beam.gameObject);
            return;
        }
        // 通常のヘルメットを装備していて、通常のビームに当たった場合
        else if (hasHelmet && !isBlueBeam)
        {
            Debug.Log("ヘルメットがビームを防いだ！");
            RemoveHelmet();
            Destroy(beam.gameObject);
            return;
        }

        // 無敵時間の場合はダメージを受けない
        if (isInvincible)
        {
            Destroy(beam.gameObject);
            return;
        }

        // 上記の防御処理を通過した場合（＝ヘルメットが不適合、またはヘルメットがない場合）は即死
        Debug.Log("防御できないビームに接触したため、即死します。");
        playerController.LoseLife();
        Destroy(beam.gameObject);
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    private void TakeDamage(float damage)
    {
        // 青いヘルメットを装備している場合、ダメージを50%軽減
        if (hasBlueHelmet)
        {
            damage *= 0.5f;
            Debug.Log("青ヘルメット効果: ダメージを50%軽減 -> " + damage);
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f); // 体力は0未満にならないように
        UpdateHealthUI();

        // ダメージ効果音を再生
        PlayDamageSE();

        if (currentHealth <= 0)
        {
            Debug.Log("プレイヤーがダメージで死亡");
            playerController.LoseLife();
        }
    }

    /// <summary>
    /// 外部から直接ダメージを与える公開メソッド
    /// </summary>
    public void ApplyDirectDamage(float damage)
    {
        // 無敵時間の場合はダメージを受けないようにする
        if (isInvincible) return;

        TakeDamage(damage);
        ActivateInvincibility(); // ダメージを受けたら無敵時間を起動
    }

    /// <summary>
    /// 体力UIを更新
    /// </summary>
    private void UpdateHealthUI()
    {
        float healthRatio = currentHealth / maxHealth;

        // ImageタイプのHPゲージを更新
        if (hpGaugeImage != null)
        {
            hpGaugeImage.fillAmount = healthRatio;
            Debug.Log("HP表示 (Image): " + hpGaugeImage.fillAmount);
        }
        // RectTransformタイプのHPゲージを更新
        else if (hpGaugeRect != null)
        {
            Vector2 sizeDelta = hpGaugeRect.sizeDelta;
            sizeDelta.x = originalGaugeWidth * healthRatio;
            hpGaugeRect.sizeDelta = sizeDelta;
            Debug.Log("HP表示 (RectTransform): " + healthRatio);
        }
        else
        {
            Debug.LogWarning("HPゲージコンポーネントが設定されていません。体力を表示できません。");
        }
    }

    /// <summary>
    /// 通常のヘルメットを装備
    /// </summary>
    public void EquipHelmet()
    {
        // すでにヘルメットを装備している場合は一旦解除
        if (hasHelmet || hasBlueHelmet)
        {
            RemoveAllHelmets();
        }

        hasHelmet = true;
        hasBlueHelmet = false;

        // アニメーターのパラメータを設定
        if (animator != null)
        {
            animator.SetBool("HasHelmet", true);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("通常ヘルメット装備: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションが更新できません");
        }

        // パワーアップ効果音を再生（最後に再生）
        PlayPowerupSE();

    }

    /// <summary>
    /// 青いヘルメットを装備
    /// </summary>
    public void EquipBlueHelmet()
    {
        // すでにヘルメットを装備している場合は一旦解除
        if (hasHelmet || hasBlueHelmet)
        {
            RemoveAllHelmets();
        }

        hasBlueHelmet = true;
        hasHelmet = false;

        // アニメーターのパラメータを設定
        if (animator != null)
        {
            animator.SetBool("HasBlueHelmet", true);
            animator.SetBool("HasHelmet", false);
            Debug.Log("青ヘルメット装備: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションが更新できません");
        }

        PlayPowerupSE();

    }

    /// <summary>
    /// 通常のヘルメットを解除
    /// </summary>
    public void RemoveHelmet()
    {
        hasHelmet = false;

        // アニメーターのパラメータを更新
        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            Debug.Log("ヘルメット解除: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションが更新できません");
        }
    }

    /// <summary>
    /// 青いヘルメットを解除
    /// </summary>
    public void RemoveBlueHelmet()
    {
        hasBlueHelmet = false;

        // アニメーターのパラメータを更新
        if (animator != null)
        {
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("青ヘルメット解除: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションが更新できません");
        }
    }

    /// <summary>
    /// 全てのヘルメットを解除
    /// </summary>
    private void RemoveAllHelmets()
    {
        hasHelmet = false;
        hasBlueHelmet = false;

        // アニメーターのパラメータを更新
        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("全てのヘルメットを解除: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションが更新できません");
        }
    }

    /// <summary>
    /// 通常ヘルメットを装備しているか返す
    /// </summary>
    public bool HasHelmet()
    {
        return hasHelmet;
    }

    /// <summary>
    /// 青いヘルメットを装備しているか返す
    /// </summary>
    public bool HasBlueHelmet()
    {
        return hasBlueHelmet;
    }

    /// <summary>
    /// いずれかのヘルメットを装備しているか返す
    /// </summary>
    public bool HasAnyHelmet()
    {
        return hasHelmet || hasBlueHelmet;
    }

    /// <summary>
    /// 体力を最大値にリセット
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// 無敵時間中か返す
    /// </summary>
    public bool IsInvincible()
    {
        return isInvincible;
    }
}
