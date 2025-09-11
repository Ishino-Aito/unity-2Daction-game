using UnityEngine;

/// <summary>
/// �v���C���[��ǐՂ���J�����̐�����s���R���|�[�l���g
/// </summary>
public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject player;       // �ǐՂ���v���C���[�I�u�W�F�N�g
    [SerializeField] private float minY = 0f;         // �J������Y���W�̍ŏ��l�i�����艺�ɂ̓J�������ړ����Ȃ��j

    /// <summary>
    /// ����������
    /// </summary>
    private void Start()
    {
        // �v���C���[�I�u�W�F�N�g���ݒ肳��Ă��Ȃ��ꍇ�͖��O�Ō���
        if (player == null)
        {
            player = GameObject.Find("Player");

            // �v���C���[��������Ȃ��ꍇ�̓��O���o��
            if (player == null)
            {
                Debug.LogWarning("�v���C���[�I�u�W�F�N�g��������܂���ł����B�J�����̒ǐՑΏۂ�ݒ肵�Ă��������B");
            }
        }
    }

    /// <summary>
    /// ���t���[�����s�����X�V����
    /// </summary>
    private void Update()
    {
        // �v���C���[�����݂��Ȃ��ꍇ�͉������Ȃ�
        if (player == null) return;

        // �v���C���[�̌��݈ʒu���擾
        Vector3 playerPos = player.transform.position;

        // �J�����̌��݈ʒu���擾
        Vector3 currentPosition = transform.position;

        // Y���W���ŏ��l��艺�ɂȂ�Ȃ��悤�ɐ���
        float newY = Mathf.Max(playerPos.y, minY);

        // �J�����̈ʒu���X�V�iX,Z���W�͕ύX�����AY���W�̂ݍX�V�j
        transform.position = new Vector3(
            currentPosition.x,
            newY,
            currentPosition.z
        );
    }
}