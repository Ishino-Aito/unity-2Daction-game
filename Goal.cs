using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// ゴールエリアの動作を制御するクラス
/// プレイヤーがゴールに触れると次のシーンやクリア画面に遷移します
/// </summary>
public class Goal : MonoBehaviour
{
    [Header("基本設定")]
    [Tooltip("このタグを持つオブジェクトがゴールに触れると反応します")]
    [SerializeField] private string playerTag = "Player";

    [Header("シーン遷移設定")]
    [Tooltip("有効にするとクリア画面を経由して次のシーンに進みます")]
    [SerializeField] private bool showClearScene = true;

    [Tooltip("最終ステージクリア後に表示するシーン名")]
    [SerializeField] private string allClearSceneName = "AllClearScene";

    [Tooltip("通常のクリア画面のシーン名")]
    [SerializeField] private string normalClearSceneName = "ClearScene";

    [Tooltip("最終ステージのシーン名")]
    [SerializeField] private string finalStageName = "GameScene3";

    /// <summary>
    /// 他のコライダーがトリガーに入ったときに呼ばれるメソッド
    /// </summary>
    /// <param name="other">トリガーに入ったコライダー</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // プレイヤーと接触したか確認
        if (!other.CompareTag(playerTag))
            return;

        // 現在のシーン情報を取得
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // 次のシーンのインデックスを計算
        int nextSceneIndex = currentSceneIndex + 1;

        // 最終ステージかどうかを判定
        bool isFinalStage = currentSceneName == finalStageName;

        // 次のシーン名をPlayerPrefsに保存
        SaveNextSceneName(nextSceneIndex, isFinalStage);

        // 適切なシーンに遷移
        TransitionToNextScene(isFinalStage);
    }

    /// <summary>
    /// 次のシーン名を取得してPlayerPrefsに保存します
    /// </summary>
    /// <param name="nextSceneIndex">次のシーンのインデックス</param>
    /// <param name="isFinalStage">最終ステージかどうか</param>
    private void SaveNextSceneName(int nextSceneIndex, bool isFinalStage)
    {
        string nextSceneName = "GameScene"; // デフォルト値

        // ビルド設定に次のシーンが存在し、かつ最終ステージでない場合
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings && !isFinalStage)
        {
            // ビルドインデックスからシーンパスを取得し、ファイル名部分だけを抽出
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
        }

        // 次のシーン名を保存
        PlayerPrefs.SetString("NextGameScene", nextSceneName);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 適切なシーンに遷移する処理
    /// </summary>
    /// <param name="isFinalStage">最終ステージかどうか</param>
    private void TransitionToNextScene(bool isFinalStage)
    {
        if (showClearScene)
        {
            // クリア画面を経由するモード
            if (isFinalStage)
            {
                // 最終ステージクリア時は全クリア画面へ
                SceneManager.LoadScene(allClearSceneName);
            }
            else
            {
                // 通常ステージのクリア画面へ
                SceneManager.LoadScene(normalClearSceneName);
            }
        }
        else
        {
            // クリア画面を経由せずに直接次のシーンへ
            string nextSceneName = PlayerPrefs.GetString("NextGameScene", "GameScene");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}