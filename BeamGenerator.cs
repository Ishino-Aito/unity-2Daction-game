using System.Diagnostics;
using UnityEngine;

/// <summary>
/// �v���C���[�����m���Ĉ��Ԋu�Ńr�[���𔭎˂���R���|�[�l���g
/// </summary>
public class BeamGenerator : MonoBehaviour
{
    [Header("�r�[���ݒ�")]
    [SerializeField]
    private GameObject beamPrefab;  // ���˂���r�[���̃v���n�u

    [SerializeField]
    private float fireInterval = 3f;  // �r�[�����˂̊Ԋu�i�b�j

    [Header("�v���C���[���m�ݒ�")]
    [SerializeField]
    private float detectionRange = 5f;  // �v���C���[�����m����͈́i���a�j

    [SerializeField]
    private string playerTag = "Player";  // �v���C���[�̃^�O��

    [Header("�����ݒ�")]
    [SerializeField] private AudioSource audioSource;          // AudioSource�R���|�[�l���g
    [SerializeField] private AudioClip beamSE;                 // �r�[�����ˌ��ʉ�
    [SerializeField][Range(0f, 1f)] private float beamVolume = 0.5f;  // �r�[�����̉���

    // �O�񔭎ˎ������L�^����^�C�}�[
    private float timeSinceLastFire = 0f;

    /// <summary>
    /// ����������
    /// </summary>
    void Start()
    {
        // �����V�X�e���̏�����
        InitializeAudioSystem();
    }

    /// <summary>
    /// �����V�X�e���̏�����
    /// </summary>
    private void InitializeAudioSystem()
    {
        // AudioSource�R���|�[�l���g�̎擾�܂��͒ǉ�
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                Debug.Log("AudioSource�R���|�[�l���g�������ǉ����܂����B");
            }
        }

        // AudioSource�̊�{�ݒ�
        audioSource.playOnAwake = false;  // �����Đ����Ȃ�
        audioSource.spatialBlend = 0f;    // 2D�T�E���h�i3D���ʂȂ��j
    }

    /// <summary>
    /// ���ʉ����Đ�����i���̃X�N���v�g������Ăяo���\�j
    /// </summary>
    /// <param name="clip">�Đ�����AudioClip</param>
    /// <param name="volume">���ʁi0.0�`1.0�j</param>
    public void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            if (clip == null) Debug.LogWarning("�Đ����悤�Ƃ������ʉ����ݒ肳��Ă��܂���B");
            if (audioSource == null) Debug.LogWarning("AudioSource��������܂���B");
        }
    }

    /// <summary>
    /// �r�[�����ˌ��ʉ����Đ�����
    /// </summary>
    private void PlayBeamSE()
    {
        PlaySE(beamSE, beamVolume);
    }

    /// <summary>
    /// ���t���[�����s�����X�V����
    /// </summary>
    private void Update()
    {
        // �O�񔭎ˎ��������Z
        timeSinceLastFire += Time.deltaTime;

        // ���ˏ����𖞂����Ă��邩���m�F
        if (CanFireBeam())
        {
            FireBeam();
            ResetTimer();
        }
    }

    /// <summary>
    /// �r�[���𔭎˂ł���������m�F
    /// </summary>
    /// <returns>�r�[�����˂��\�Ȃ�true</returns>
    private bool CanFireBeam()
    {
        // ���ˊԊu���o�߂��Ă��āA�v���C���[�����m�͈͓��ɂ���ꍇ
        return timeSinceLastFire >= fireInterval && IsPlayerInRange();
    }

    /// <summary>
    /// �r�[���𔭎˂���
    /// </summary>
    private void FireBeam()
    {
        // �r�[�������̃I�u�W�F�N�g�̈ʒu�ɐ���
        Instantiate(beamPrefab, transform.position, Quaternion.identity);

        // �r�[�����ˌ��ʉ����Đ�
        PlayBeamSE();
    }

    /// <summary>
    /// �^�C�}�[�����Z�b�g
    /// </summary>
    private void ResetTimer()
    {
        timeSinceLastFire = 0f;
    }

    /// <summary>
    /// �v���C���[�����m�͈͓��ɂ��邩���m�F
    /// </summary>
    /// <returns>�v���C���[���͈͓��Ȃ�true�A����ȊO��false</returns>
    private bool IsPlayerInRange()
    {
        // �v���C���[�̃Q�[���I�u�W�F�N�g���擾
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        // �v���C���[�����݂��Ȃ��ꍇ�͔͈͊O�Ɣ���
        if (player == null)
        {
            return false;
        }

        // �v���C���[�Ƃ̋������v�Z
        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        // �ݒ肵���͈͓��Ƀv���C���[�����邩��Ԃ�
        return distanceToPlayer <= detectionRange;
    }

    /// <summary>
    /// �G�f�B�^��Ŋ��m�͈͂������i�G�f�B�^�ł̂ݕ\���j
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // ���m�͈͂����F�̉~�ŕ\��
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
