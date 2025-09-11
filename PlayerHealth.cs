using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

/// <summary>
/// プレイヤーの体力と防御システムを管理するクラス
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
    [SerializeField] private float invincibilityDuration = 1f; // 無敵状態の持続時間
    [SerializeField] private float blinkInterval = 0.1f;      // 点滅間隔
    private bool isInvincible = false;                        // 無敵状態フラグ
    private float invincibilityTimer = 0f;                    // 無敵状態タイマー
    private float blinkTimer = 0f;                            // 点滅タイマー
    private bool isVisible = true;                            // 表示状態フラグ

    [Header("ヘルメット設定")]
    private bool hasHelmet = false;                           // 通常ヘルメット装備フラグ
    private bool hasBlueHelmet = false;                       // 青いヘルメット装備フラグ

    // コンポーネント参照
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    [Header("音響設定")]
    [SerializeField] private AudioSource audioSource;          // AudioSourceコンポーネント
    [SerializeField] private AudioClip damageSE;               // ダメージ効果音 (se_dageki23)
    [SerializeField] private AudioClip powerupSE;              // パワーアップ効果音 (se_powerup)
    [SerializeField][Range(0f, 1f)] private float damageVolume = 0.5f;   // ダメージ音の音量
    [SerializeField][Range(0f, 1f)] private float powerupVolume = 0.5f;  // パワーアップ音の音量

    private void Start()
    {
        // 初期化処理
        currentHealth = maxHealth;

        // 音響システムの初期化
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
            // 初期状態ではヘルメットを装備していない状態にリセット
            ResetHelmetStates();
        }

        // HPゲージの初期幅を保存
        if (hpGaugeRect != null)
        {
            originalGaugeWidth = hpGaugeRect.sizeDelta.x;
        }

        // 体力UI更新
        UpdateHealthUI();
    }

    /// <summary>
    /// 音響システムの初期化
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
        audioSource.spatialBlend = 0f;    // 2Dサウンド（3D効果なし）
    }

    /// <summary>
    /// 効果音を再生する
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
    /// ダメージ効果音を再生する
    /// </summary>
    private void PlayDamageSE()
    {
        PlaySE(damageSE, damageVolume);
    }

    /// <summary>
    /// パワーアップ効果音を再生する
    /// </summary>
    private void PlayPowerupSE()
    {
        PlaySE(powerupSE, powerupVolume);
    }

    private void Update()
    {
        // 無敵状態の処理
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

            // 無敵状態が終了したら
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true; // 必ず表示状態に戻す
                }
                Debug.Log("無敵状態が終了しました");
            }
        }
    }

    /// <summary>
    /// すべてのヘルメット状態をリセットする
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
                // 通常のヘルメット取得処理
                EquipHelmet();
                Destroy(other.gameObject);
                break;

            case "BlueHelmet":
                // 青いヘルメット取得処理
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
                    ActivateInvincibility(); // ダメージを受けた後に無敵状態を有効化
                }
                break;
        }
    }

    /// <summary>
    /// 無敵状態を有効にする
    /// </summary>
    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;
        Debug.Log("無敵状態を開始しました: " + invincibilityDuration + "秒間");
    }

    /// <summary>
    /// ビームとの衝突処理を行う
    /// </summary>
    private void HandleBeamCollision(Collider2D beam)
    {
        // ビームの種類を判定
        bool isBlueBeam = beam.CompareTag("Blue Beam");

        // 青いヘルメットを持っていて、青いビームが当たった場合の特別処理
        if (hasBlueHelmet && isBlueBeam)
        {
            Debug.Log("青いヘルメットが青いビームを防いだ！");
            RemoveBlueHelmet();
            Destroy(beam.gameObject);
            return;
        }
        // 通常のヘルメットを持っていて、通常のビームが当たった場合
        else if (hasHelmet && !isBlueBeam)
        {
            Debug.Log("ヘルメットがビームを防いだ！");
            RemoveHelmet();
            Destroy(beam.gameObject);
            return;
        }

        // 無敵状態の場合はダメージを受けない
        if (isInvincible)
        {
            Destroy(beam.gameObject);
            return;
        }

        // ヘルメットがない場合、または対応するヘルメットがない場合はダメージを受ける
        float damageAmount = defaultDamageAmount;

        // ビームからダメージ量を取得
        if (isBlueBeam)
        {
            BlueBeamMovement blueBeam = beam.GetComponent<BlueBeamMovement>();
            if (blueBeam != null)
            {
                damageAmount = blueBeam.GetDamageAmount();
            }
        }
        else
        {
            BeamMovement normalBeam = beam.GetComponent<BeamMovement>();
            if (normalBeam != null)
            {
                damageAmount = normalBeam.GetDamageAmount();
            }
        }

        TakeDamage(damageAmount);
        ActivateInvincibility(); // ダメージを受けた後に無敵状態を有効化
        Destroy(beam.gameObject);
    }

    /// <summary>
    /// ダメージを受ける処理
    /// </summary>
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f); // 体力が0未満にならないよう制限
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
        // 無敵状態の場合はダメージを受けないようにする
        if (isInvincible) return;

        TakeDamage(damage);
        ActivateInvincibility(); // ダメージを受けた後に無敵状態を有効化
    }

    /// <summary>
    /// 体力UIを更新する
    /// </summary>
    private void UpdateHealthUI()
    {
        float healthRatio = currentHealth / maxHealth;

        // Image型のHPゲージを更新
        if (hpGaugeImage != null)
        {
            hpGaugeImage.fillAmount = healthRatio;
            Debug.Log("HP表示 (Image): " + hpGaugeImage.fillAmount);
        }
        // RectTransform型のHPゲージを更新
        else if (hpGaugeRect != null)
        {
            Vector2 sizeDelta = hpGaugeRect.sizeDelta;
            sizeDelta.x = originalGaugeWidth * healthRatio;
            hpGaugeRect.sizeDelta = sizeDelta;
            Debug.Log("HP表示 (RectTransform): " + healthRatio);
        }
        else
        {
            Debug.LogWarning("HPゲージコンポーネントが設定されていません。体力が表示されません。");
        }
    }

    /// <summary>
    /// 通常のヘルメットを装備する
    /// </summary>
    public void EquipHelmet()
    {
        // すでにヘルメットを持っている場合は一度解除する
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
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションを更新できません");
        }

        // パワーアップ効果音を再生（最後に追加）
        PlayPowerupSE();

    }

    /// <summary>
    /// 青いヘルメットを装備する
    /// </summary>
    public void EquipBlueHelmet()
    {
        // すでにヘルメットを持っている場合は一度解除する
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
            Debug.Log("青いヘルメット装備: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションを更新できません");
        }

        PlayPowerupSE();

    }

    /// <summary>
    /// 通常のヘルメットを取り外す
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
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションを更新できません");
        }
    }

    /// <summary>
    /// 青いヘルメットを取り外す
    /// </summary>
    public void RemoveBlueHelmet()
    {
        hasBlueHelmet = false;

        // アニメーターのパラメータを更新
        if (animator != null)
        {
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("青いヘルメット解除: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションを更新できません");
        }
    }

    /// <summary>
    /// すべてのヘルメットを取り外す
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
            Debug.Log("すべてのヘルメット解除: アニメーションパラメータ更新");
        }
        else
        {
            Debug.LogWarning("Animatorが見つかりません。ヘルメット状態のアニメーションを更新できません");
        }
    }

    /// <summary>
    /// 通常ヘルメットを装備しているかどうかを返す
    /// </summary>
    public bool HasHelmet()
    {
        return hasHelmet;
    }

    /// <summary>
    /// 青いヘルメットを装備しているかどうかを返す
    /// </summary>
    public bool HasBlueHelmet()
    {
        return hasBlueHelmet;
    }

    /// <summary>
    /// いずれかのヘルメットを装備しているかどうかを返す
    /// </summary>
    public bool HasAnyHelmet()
    {
        return hasHelmet || hasBlueHelmet;
    }

    /// <summary>
    /// 体力を最大値にリセットする
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// 無敵状態かどうかを返す
    /// </summary>
    public bool IsInvincible()
    {
        return isInvincible;
    }
}