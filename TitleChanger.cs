using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// �^�C�g����ʂւ̑J�ڂ��Ǘ�����N���X
/// �Q�[����ʂ���^�C�g����ʂɖ߂�ۂɎg�p���܂�
/// </summary>
public class TitleChanger : MonoBehaviour
{
    /// <summary>
    /// �^�C�g����ʂɖ߂郁�\�b�h�i1�b�ҋ@���Ă�����s�j
    /// </summary>
    public void StartGame()
    {
        // �R���[�`�����J�n����2�b�ҋ@��ɃV�[���؂�ւ�
        StartCoroutine(LoadTitleSceneWithDelay());
    }

    /// <summary>
    /// 1�b�ҋ@���Ă���^�C�g���V�[����ǂݍ��ރR���[�`��
    /// </summary>
    private IEnumerator LoadTitleSceneWithDelay()
    {
        // 2�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);

        // �^�C�g����ʂɑJ��
        SceneManager.LoadScene("TitleScene");
    }
}