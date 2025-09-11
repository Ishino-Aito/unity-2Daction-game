using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// タイトル画面への遷移を管理するクラス
/// ゲーム画面からタイトル画面に戻る際に使用します
/// </summary>
public class TitleChanger : MonoBehaviour
{
    /// <summary>
    /// タイトル画面に戻るメソッド（1秒待機してから実行）
    /// </summary>
    public void StartGame()
    {
        // コルーチンを開始して2秒待機後にシーン切り替え
        StartCoroutine(LoadTitleSceneWithDelay());
    }

    /// <summary>
    /// 1秒待機してからタイトルシーンを読み込むコルーチン
    /// </summary>
    private IEnumerator LoadTitleSceneWithDelay()
    {
        // 2秒間待機
        yield return new WaitForSeconds(1.0f);

        // タイトル画面に遷移
        SceneManager.LoadScene("TitleScene");
    }
}