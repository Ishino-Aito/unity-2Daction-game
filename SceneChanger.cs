using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// ゲームのシーン遷移を管理するクラス
/// </summary>
public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// ゲームを開始するメソッド（1秒待機してから実行）
    /// </summary>
    public void StartGame()
    {
        // コルーチンを開始して1秒待機後にシーン切り替え
        StartCoroutine(StartGameWithDelay());
    }

    /// <summary>
    /// 1秒待機してからゲームを開始するコルーチン
    /// </summary>
    private IEnumerator StartGameWithDelay()
    {
        // 1秒間待機
        yield return new WaitForSeconds(1.0f);

        // プレイヤーの残機を初期状態に設定
        PlayerController.currentLives = -1;

        // ゲーム開始フラグをリセット
        PlayerController.gameStarted = false;

        // ゲームシーンに遷移
        SceneManager.LoadScene("GameScene");
    }
}