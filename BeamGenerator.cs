using System.Diagnostics;
using UnityEngine;

/// <summary>
/// プレイヤーを感知して一定間隔でビームを発射するコンポーネント
/// </summary>
public class BeamGenerator : MonoBehaviour
{
    [Header("ビーム設定")]
    [SerializeField]
    private GameObject beamPrefab;  // 発射するビームのプレハブ

    [SerializeField]
    private float fireInterval = 3f;  // ビーム発射の間隔（秒）

    [Header("プレイヤー感知設定")]
    [SerializeField]
    private float detectionRange = 5f;  // プレイヤーを感知する範囲（半径）

    [SerializeField]
    private string playerTag = "Player";  // プレイヤーのタグ名

    [Header("音響設定")]
    [SerializeField] private AudioSource audioSource;          // AudioSourceコンポーネント
    [SerializeField] private AudioClip beamSE;                 // ビーム発射効果音
    [SerializeField][Range(0f, 1f)] private float beamVolume = 0.5f;  // ビーム音の音量

    // 前回発射時刻を記録するタイマー
    private float timeSinceLastFire = 0f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        // 音響システムの初期化
        InitializeAudioSystem();
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
                Debug.Log("AudioSourceコンポーネントを自動追加しました。");
            }
        }

        // AudioSourceの基本設定
        audioSource.playOnAwake = false;  // 自動再生しない
        audioSource.spatialBlend = 0f;    // 2Dサウンド（3D効果なし）
    }

    /// <summary>
    /// 効果音を再生する（他のスクリプトからも呼び出し可能）
    /// </summary>
    /// <param name="clip">再生するAudioClip</param>
    /// <param name="volume">音量（0.0～1.0）</param>
    public void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            if (clip == null) Debug.LogWarning("再生しようとした効果音が設定されていません。");
            if (audioSource == null) Debug.LogWarning("AudioSourceが見つかりません。");
        }
    }

    /// <summary>
    /// ビーム発射効果音を再生する
    /// </summary>
    private void PlayBeamSE()
    {
        PlaySE(beamSE, beamVolume);
    }

    /// <summary>
    /// 毎フレーム実行される更新処理
    /// </summary>
    private void Update()
    {
        // 前回発射時刻を加算
        timeSinceLastFire += Time.deltaTime;

        // 発射条件を満たしているかを確認
        if (CanFireBeam())
        {
            FireBeam();
            ResetTimer();
        }
    }

    /// <summary>
    /// ビームを発射できる条件を確認
    /// </summary>
    /// <returns>ビーム発射が可能ならtrue</returns>
    private bool CanFireBeam()
    {
        // 発射間隔を経過していて、プレイヤーが感知範囲内にいる場合
        return timeSinceLastFire >= fireInterval && IsPlayerInRange();
    }

    /// <summary>
    /// ビームを発射する
    /// </summary>
    private void FireBeam()
    {
        // ビームをこのオブジェクトの位置に生成
        Instantiate(beamPrefab, transform.position, Quaternion.identity);

        // ビーム発射効果音を再生
        PlayBeamSE();
    }

    /// <summary>
    /// タイマーをリセット
    /// </summary>
    private void ResetTimer()
    {
        timeSinceLastFire = 0f;
    }

    /// <summary>
    /// プレイヤーが感知範囲内にいるかを確認
    /// </summary>
    /// <returns>プレイヤーが範囲内ならtrue、それ以外はfalse</returns>
    private bool IsPlayerInRange()
    {
        // プレイヤーのゲームオブジェクトを取得
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        // プレイヤーが存在しない場合は範囲外と判定
        if (player == null)
        {
            return false;
        }

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // 設定した範囲内にプレイヤーがいるかを返す
        return distanceToPlayer <= detectionRange;
    }

    /// <summary>
    /// エディタ上で感知範囲を可視化（エディタでのみ表示）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 感知範囲を黄色の円で表示
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
