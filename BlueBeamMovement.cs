using UnityEngine;

/// <summary>
/// ���r�[�����L�̓���Ɛ������Ǘ�����R���|�[�l���g
/// BeamMovement�N���X���p�����Ďg�p
/// </summary>
public class BlueBeamMovement : BeamMovement
{
    /// <summary>
    /// ����������
    /// </summary>
    private void Start()
    {
        // ���r�[���p�̃^�O��ݒ�
        gameObject.tag = "Blue Beam";
    }

}