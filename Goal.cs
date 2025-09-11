using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// �S�[���G���A�̓���𐧌䂷��N���X
/// �v���C���[���S�[���ɐG���Ǝ��̃V�[����N���A��ʂɑJ�ڂ��܂�
/// </summary>
public class Goal : MonoBehaviour
{
    [Header("��{�ݒ�")]
    [Tooltip("���̃^�O�����I�u�W�F�N�g���S�[���ɐG���Ɣ������܂�")]
    [SerializeField] private string playerTag = "Player";

    [Header("�V�[���J�ڐݒ�")]
    [Tooltip("�L���ɂ���ƃN���A��ʂ��o�R���Ď��̃V�[���ɐi�݂܂�")]
    [SerializeField] private bool showClearScene = true;

    [Tooltip("�ŏI�X�e�[�W�N���A��ɕ\������V�[����")]
    [SerializeField] private string allClearSceneName = "AllClearScene";

    [Tooltip("�ʏ�̃N���A��ʂ̃V�[����")]
    [SerializeField] private string normalClearSceneName = "ClearScene";

    [Tooltip("�ŏI�X�e�[�W�̃V�[����")]
    [SerializeField] private string finalStageName = "GameScene3";

    /// <summary>
    /// ���̃R���C�_�[���g���K�[�ɓ������Ƃ��ɌĂ΂�郁�\�b�h
    /// </summary>
    /// <param name="other">�g���K�[�ɓ������R���C�_�[</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // �v���C���[�ƐڐG�������m�F
        if (!other.CompareTag(playerTag))
            return;

        // ���݂̃V�[�������擾
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        string currentSceneName = SceneManager.GetActiveScene().name;

        // ���̃V�[���̃C���f�b�N�X���v�Z
        int nextSceneIndex = currentSceneIndex + 1;

        // �ŏI�X�e�[�W���ǂ����𔻒�
        bool isFinalStage = currentSceneName == finalStageName;

        // ���̃V�[������PlayerPrefs�ɕۑ�
        SaveNextSceneName(nextSceneIndex, isFinalStage);

        // �K�؂ȃV�[���ɑJ��
        TransitionToNextScene(isFinalStage);
    }

    /// <summary>
    /// ���̃V�[�������擾����PlayerPrefs�ɕۑ����܂�
    /// </summary>
    /// <param name="nextSceneIndex">���̃V�[���̃C���f�b�N�X</param>
    /// <param name="isFinalStage">�ŏI�X�e�[�W���ǂ���</param>
    private void SaveNextSceneName(int nextSceneIndex, bool isFinalStage)
    {
        string nextSceneName = "GameScene"; // �f�t�H���g�l

        // �r���h�ݒ�Ɏ��̃V�[�������݂��A���ŏI�X�e�[�W�łȂ��ꍇ
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings && !isFinalStage)
        {
            // �r���h�C���f�b�N�X����V�[���p�X���擾���A�t�@�C�������������𒊏o
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextSceneIndex);
            nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
        }

        // ���̃V�[������ۑ�
        PlayerPrefs.SetString("NextGameScene", nextSceneName);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// �K�؂ȃV�[���ɑJ�ڂ��鏈��
    /// </summary>
    /// <param name="isFinalStage">�ŏI�X�e�[�W���ǂ���</param>
    private void TransitionToNextScene(bool isFinalStage)
    {
        if (showClearScene)
        {
            // �N���A��ʂ��o�R���郂�[�h
            if (isFinalStage)
            {
                // �ŏI�X�e�[�W�N���A���͑S�N���A��ʂ�
                SceneManager.LoadScene(allClearSceneName);
            }
            else
            {
                // �ʏ�X�e�[�W�̃N���A��ʂ�
                SceneManager.LoadScene(normalClearSceneName);
            }
        }
        else
        {
            // �N���A��ʂ��o�R�����ɒ��ڎ��̃V�[����
            string nextSceneName = PlayerPrefs.GetString("NextGameScene", "GameScene");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}