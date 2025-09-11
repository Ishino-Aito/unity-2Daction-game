using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// ���̃X�e�[�W�֐i�ރ{�^���̐�����s���N���X
/// </summary>
public class NextStageButton : MonoBehaviour
{
    [SerializeField]
    private string defaultSceneName = "GameScene"; // �f�t�H���g�̃V�[�����i�C���X�y�N�^�[����ύX�\�j

    [SerializeField]
    private string playerPrefsKey = "NextGameScene"; // PlayerPrefs�Ŏg�p����L�[��

    /// <summary>
    /// �uNextStage�v�{�^���������ꂽ�Ƃ��ɌĂяo����郁�\�b�h�i1�b�ҋ@���Ă�����s�j
    /// PlayerPrefs���玟�̃X�e�[�W�����擾���ă��[�h����
    /// </summary>
    public void LoadNextStage()
    {
        // �R���[�`�����J�n����1�b�ҋ@��ɃV�[���؂�ւ�
        StartCoroutine(LoadNextStageWithDelay());
    }

    /// <summary>
    /// 1�b�ҋ@���Ă��玟�̃X�e�[�W�����[�h����R���[�`��
    /// </summary>
    private IEnumerator LoadNextStageWithDelay()
    {
        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);

        // PlayerPrefs���玟�̃V�[�������擾�i���݂��Ȃ��ꍇ�̓f�t�H���g�l���g�p�j
        string nextSceneName = PlayerPrefs.GetString(playerPrefsKey, defaultSceneName);

        Debug.Log("���̃X�e�[�W�ɐi�݂܂�: " + nextSceneName + "�����[�h���܂�");
        SceneManager.LoadScene(nextSceneName);
    }
}