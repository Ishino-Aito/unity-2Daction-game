using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// ゲームを続行するためのボタンの制御を行うクラス
/// </summary>
public class ContinueButton : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "GameScene"; // ロードするシーン名（インスペクターから変更可能）

    /// <summary>
    /// 「Continue」ボタンが押されたときに呼び出されるメソッド（1秒待機してから実行）
    /// ゲームシーンに遷移する
    /// </summary>
    public void ContinueGame()
    {
        // コルーチンを開始して1秒待機後にシーン切り替え
        StartCoroutine(ContinueGameWithDelay());
    }

    /// <summary>
    /// 1秒待機してからゲームを続行するコルーチン
    /// </summary>
    private IEnumerator ContinueGameWithDelay()
    {
        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        Debug.Log("ゲームを続行します: " + targetSceneName + "をロードします");
        SceneManager.LoadScene(targetSceneName);
    }
}