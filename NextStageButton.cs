using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// 次のステージへ進むボタンの制御を行うクラス
/// </summary>
public class NextStageButton : MonoBehaviour
{
    [SerializeField]
    private string defaultSceneName = "GameScene"; // デフォルトのシーン名（インスペクターから変更可能）

    [SerializeField]
    private string playerPrefsKey = "NextGameScene"; // PlayerPrefsで使用するキー名

    /// <summary>
    /// 「NextStage」ボタンが押されたときに呼び出されるメソッド（1秒待機してから実行）
    /// PlayerPrefsから次のステージ名を取得してロードする
    /// </summary>
    public void LoadNextStage()
    {
        // コルーチンを開始して1秒待機後にシーン切り替え
        StartCoroutine(LoadNextStageWithDelay());
    }

    /// <summary>
    /// 1秒待機してから次のステージをロードするコルーチン
    /// </summary>
    private IEnumerator LoadNextStageWithDelay()
    {
        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        // PlayerPrefsから次のシーン名を取得（存在しない場合はデフォルト値を使用）
        string nextSceneName = PlayerPrefs.GetString(playerPrefsKey, defaultSceneName);

        Debug.Log("次のステージに進みます: " + nextSceneName + "をロードします");
        SceneManager.LoadScene(nextSceneName);
    }
}