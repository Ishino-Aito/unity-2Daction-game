using UnityEngine;

/// <summary>
/// �G���v���C���[�ɐڐG�������Ƀ_���[�W��^����R���|�[�l���g
/// </summary>
public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 34f;    // �v���C���[�ɗ^����_���[�W��
    [SerializeField] private bool showDebugLog = true;    // �f�o�b�O���O��\�����邩�ǂ���

    /// <summary>
    /// ���̃R���C�_�[�ƏՓ˂������ɌĂяo����郁�\�b�h
    /// </summary>
    /// <param name="collision">�Փˏ��</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �Փ˂����I�u�W�F�N�g���v���C���[���ǂ������m�F
        if (collision.gameObject.CompareTag("Player"))
        {
            // �v���C���[�̗̑͊Ǘ��R���|�[�l���g���擾
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

            // �̗͊Ǘ��R���|�[�l���g�����݂���ꍇ�̓_���[�W��K�p
            if (playerHealth != null)
            {
                // �v���C���[�Ƀ_���[�W��^����
                playerHealth.ApplyDirectDamage(damageAmount);

                // �f�o�b�O���O��\������ݒ�̏ꍇ�̓��O���o��
                if (showDebugLog)
                {
                    Debug.Log($"{damageAmount}�_���[�W���v���C���[�ɗ^���܂���");
                }
            }
            else
            {
                // �v���C���[��PlayerHealth�R���|�[�l���g���Ȃ��ꍇ�͌x����\��
                Debug.LogWarning("�v���C���[�I�u�W�F�N�g��PlayerHealth�R���|�[�l���g��������܂���");
            }
        }
    }
}