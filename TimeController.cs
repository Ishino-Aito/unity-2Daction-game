using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ゲーム内のカウントダウンタイマーを管理するためのクラス
/// 時間切れになるとプレイヤーのライフを減らす
/// </summary>
public class TimeController : MonoBehaviour
{
    [Tooltip("ゲームの制限時間（秒）")]
    [SerializeField] private float timeLimit = 100f;

    [Tooltip("現在の残り時間")]
    [SerializeField] private float currentTime;

    // TextMeshProとレガシーTextの両方をサポート
    [Header("UI要素")]
    [Tooltip("TextMeshProで時間を表示する場合")]
    [SerializeField] private TextMeshProUGUI timeTextTMP;

    [Tooltip("レガシーTextで時間を表示する場合")]
    [SerializeField] private Text timeTextLegacy;

    private PlayerController playerController;
    private bool isGameOver = false;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        // 現在の時間を制限時間に設定
        currentTime = timeLimit;

        // PlayerControllerを取得
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("シーン内にPlayerControllerが見つかりません。タイマーが正常に動作しない可能性があります。");
        }

        // UI表示を初期化
        UpdateTimeUI();
    }

    /// <summary>
    /// 毎フレーム実行される処理
    /// </summary>
    private void Update()
    {
        // ゲームオーバー状態なら処理をスキップ
        if (isGameOver) return;

        // 時間を減少させる
        currentTime -= Time.deltaTime;

        // 時間切れの判定
        if (currentTime <= 0)
        {
            currentTime = 0;
            TimeUp();
        }

        // UI表示を更新
        UpdateTimeUI();
    }

    /// <summary>
    /// 残り時間のUI表示を更新する
    /// </summary>
    private void UpdateTimeUI()
    {
        // 整数値として表示
        string timeString = Mathf.FloorToInt(currentTime).ToString();

        // TextMeshProとレガシーTextの両方に対応
        if (timeTextTMP != null)
        {
            timeTextTMP.text = timeString;
        }
        else if (timeTextLegacy != null)
        {
            timeTextLegacy.text = timeString;
        }
        else
        {
            Debug.LogWarning("テキストコンポーネントが設定されていません。時間が表示されません。");
        }
    }

    /// <summary>
    /// 時間切れ時の処理
    /// </summary>
    private void TimeUp()
    {
        // 既にゲームオーバー状態なら何もしない
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("時間切れ！プレイヤーの残機を1つ減らします");

        // プレイヤーのライフを減らす
        if (playerController != null)
        {
            playerController.LoseLife();
        }
        else
        {
            Debug.LogError("PlayerControllerが見つかりません。ライフを減らせません。");
        }
    }

    /// <summary>
    /// タイマーをリセットする（レベルリスタート時などに使用）
    /// </summary>
    public void ResetTimer()
    {
        currentTime = timeLimit;
        isGameOver = false;
        UpdateTimeUI();
    }
}