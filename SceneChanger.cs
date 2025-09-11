using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// �Q�[���̃V�[���J�ڂ��Ǘ�����N���X
/// </summary>
public class SceneChanger : MonoBehaviour
{
    /// <summary>
    /// �Q�[�����J�n���郁�\�b�h�i1�b�ҋ@���Ă�����s�j
    /// </summary>
    public void StartGame()
    {
        // �R���[�`�����J�n����1�b�ҋ@��ɃV�[���؂�ւ�
        StartCoroutine(StartGameWithDelay());
    }

    /// <summary>
    /// 1�b�ҋ@���Ă���Q�[�����J�n����R���[�`��
    /// </summary>
    private IEnumerator StartGameWithDelay()
    {
        // 1�b�ԑҋ@
        yield return new WaitForSeconds(1.0f);

        // �v���C���[�̎c�@��������Ԃɐݒ�
        PlayerController.currentLives = -1;

        // �Q�[���J�n�t���O�����Z�b�g
        PlayerController.gameStarted = false;

        // �Q�[���V�[���ɑJ��
        SceneManager.LoadScene("GameScene");
    }
}