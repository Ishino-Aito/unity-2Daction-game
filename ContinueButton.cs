using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Diagnostics;

/// <summary>
/// �Q�[���𑱍s���邽�߂̃{�^���̐�����s���N���X
/// </summary>
public class ContinueButton : MonoBehaviour
{
    [SerializeField]
    private string targetSceneName = "GameScene"; // ���[�h����V�[�����i�C���X�y�N�^�[����ύX�\�j

    /// <summary>
    /// �uContinue�v�{�^���������ꂽ�Ƃ��ɌĂяo����郁�\�b�h�i1�b�ҋ@���Ă�����s�j
    /// �Q�[���V�[���ɑJ�ڂ���
    /// </summary>
    public void ContinueGame()
    {
        // �R���[�`�����J�n����1�b�ҋ@��ɃV�[���؂�ւ�
        StartCoroutine(ContinueGameWithDelay());
    }

    /// <summary>
    /// 1�b�ҋ@���Ă���Q�[���𑱍s����R���[�`��
    /// </summary>
    private IEnumerator ContinueGameWithDelay()
    {
        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);

        Debug.Log("�Q�[���𑱍s���܂�: " + targetSceneName + "�����[�h���܂�");
        SceneManager.LoadScene(targetSceneName);
    }
}