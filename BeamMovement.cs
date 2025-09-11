using UnityEngine;

/// <summary>
/// �r�[���̊�{�I�ȓ���Ɛ������Ǘ�����R���|�[�l���g
/// </summary>
public class BeamMovement : MonoBehaviour
{
    [Header("�r�[���ݒ�")]
    [Tooltip("�r�[���̈ړ����x")]
    [SerializeField] private float speed = 5f;

    [Tooltip("�r�[�����^����_���[�W��")]
    [SerializeField] private float damageAmount = 100f;

    [Tooltip("��ʊO�����臒l�i���̒l��艺�ɍs���ƃr�[���I�u�W�F�N�g���j�������j")]
    [SerializeField] private float destroyThreshold = -10f;

    /// <summary>
    /// ����������
    /// </summary>
    private void Start()
    {
        // �r�[���Ƀ^�O��ݒ�
        gameObject.tag = "Beam";
    }

    /// <summary>
    /// ���t���[�����s����鏈��
    /// </summary>
    private void Update()
    {
        // �r�[�����������Ɉړ�������
        MoveBeam();

        // ��ʊO����
        CheckIfOffScreen();
    }

    /// <summary>
    /// �r�[�����ړ������鏈��
    /// </summary>
    protected virtual void MoveBeam()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    /// <summary>
    /// ��ʊO�ɏo�����ǂ������`�F�b�N���A�o�Ă���Δj������
    /// </summary>
    private void CheckIfOffScreen()
    {
        if (transform.position.y < destroyThreshold)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �r�[���̃_���[�W�ʂ��擾����
    /// </summary>
    /// <returns>�_���[�W��</returns>
    public virtual float GetDamageAmount()
    {
        return damageAmount;
    }
}