using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �Q�[���̈ꎞ��~�V�X�e�����Ǘ�����R���|�[�l���g
/// </summary>
public class PauseSystem : MonoBehaviour
{
    [Header("UI�R���|�[�l���g")]
    [SerializeField] private GameObject pausePanel;    // �ꎞ��~��ʂ̃p�l��
    [SerializeField] private Button resumeButton;      // �Q�[���ĊJ�{�^��
    [SerializeField] private Button pauseButton;       // �Q�[���ꎞ��~�{�^��

    [Header("BGM�ݒ�")]
    [SerializeField] private AudioSource bgmAudioSource; // BGM�p��AudioSource

    [Header("�ݒ�")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;  // �ꎞ��~�Ɏg�p����L�[

    public static bool IsGamePaused { get; private set; } = false;

    private void Start()
    {
        IsGamePaused = false;
        Time.timeScale = 1f;

        InitializeComponents();
        SetupButtonListeners();

        // BGM�p��AudioSource���ݒ肳��Ă��邩�`�F�b�N
        if (bgmAudioSource == null)
        {
            Debug.LogWarning("BGM�p��AudioSource���ݒ肳��Ă��܂���BBGM�̃|�[�Y�@�\�͓��삵�܂���B");
        }
    }

    private void InitializeComponents()
    {
        if (pausePanel == null)
        {
            Debug.LogError("�ꎞ��~�p�l�����C���X�y�N�^�[�Őݒ肳��Ă��܂���I");
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
            Debug.LogError("�ĊJ�{�^�����C���X�y�N�^�[�Őݒ肳��Ă��܂���I");
        }

        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(PauseGame);
        }
        else
        {
            Debug.LogError("�|�[�Y�{�^�����C���X�y�N�^�[�Őݒ肳��Ă��܂���I");
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
    /// �Q�[�����ꎞ��~����
    /// </summary>
    public void PauseGame()
    {
        if (IsGamePaused) return;

        Time.timeScale = 0f;
        IsGamePaused = true;
        pausePanel.SetActive(true);

        // BGM���ݒ肳��Ă���΁ABGM���ꎞ��~����
        if (bgmAudioSource != null && bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Pause();
        }

        Debug.Log("�Q�[�����ꎞ��~���܂���");
    }

    /// <summary>
    /// �Q�[�����ĊJ����
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGamePaused) return;

        Time.timeScale = 1f;
        IsGamePaused = false;
        pausePanel.SetActive(false);

        // BGM���ݒ肳��Ă���΁ABGM�̍Đ����ĊJ����
        if (bgmAudioSource != null)
        {
            bgmAudioSource.UnPause();
        }

        Debug.Log("�Q�[�����ĊJ���܂���");
    }
}
