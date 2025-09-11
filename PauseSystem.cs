using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ゲームの一時停止システムを管理するコンポーネント
/// </summary>
public class PauseSystem : MonoBehaviour
{
    [Header("UIコンポーネント")]
    [SerializeField] private GameObject pausePanel;    // 一時停止画面のパネル
    [SerializeField] private Button resumeButton;      // ゲーム再開ボタン
    [SerializeField] private Button pauseButton;       // ゲーム一時停止ボタン

    [Header("BGM設定")]
    [SerializeField] private AudioSource bgmAudioSource; // BGM用のAudioSource

    [Header("設定")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;  // 一時停止に使用するキー

    public static bool IsGamePaused { get; private set; } = false;

    private void Start()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;

        InitializeComponents();
        SetupButtonListeners();

        // BGM用のAudioSourceが設定されているかチェック
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("BGM用のAudioSourceが設定されていません。BGMのポーズ機能は動作しません。");
        }
    }

    private void InitializeComponents()
    {
        if (pausePanel == null)
        {
            Debug.LogError("一時停止パネルがインスペクターで設定されていません！");
            return;
        }
        pausePanel.SetActive(false);
    }

    private void SetupButtonListeners()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogError("再開ボタンがインスペクターで設定されていません！");
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        else
        {
            Debug.LogError("ポーズボタンがインスペクターで設定されていません！");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pauseKey))
        {
            if (IsGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// ゲームを一時停止する
    /// </summary>
    public void PauseGame()
    {
        if (IsGamePaused) return;

        Time.timeScale = 0f;
        IsGamePaused = true;
        pausePanel.SetActive(true);

        // BGMが設定されていれば、BGMを一時停止する
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }

        Debug.Log("ゲームを一時停止しました");
    }

    /// <summary>
    /// ゲームを再開する
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGamePaused) return;

        Time.timeScale = 1f;
        IsGamePaused = false;
        pausePanel.SetActive(false);

        // BGMが設定されていれば、BGMの再生を再開する
        if (bgmAudioSource != null)
        {
            bgmAudioSource.UnPause();
        }

        Debug.Log("ゲームを再開しました");
    }
}
