using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// �Q�[�����̃J�E���g�_�E���^�C�}�[���Ǘ����邽�߂̃N���X
/// ���Ԑ؂�ɂȂ�ƃv���C���[�̃��C�t�����炷
/// </summary>
public class TimeController : MonoBehaviour
{
    [Tooltip("�Q�[���̐������ԁi�b�j")]
    [SerializeField] private float timeLimit = 100f;

    [Tooltip("���݂̎c�莞��")]
    [SerializeField] private float currentTime;

    // TextMeshPro�ƃ��K�V�[Text�̗������T�|�[�g
    [Header("UI�v�f")]
    [Tooltip("TextMeshPro�Ŏ��Ԃ�\������ꍇ")]
    [SerializeField] private TextMeshProUGUI timeTextTMP;

    [Tooltip("���K�V�[Text�Ŏ��Ԃ�\������ꍇ")]
    [SerializeField] private Text timeTextLegacy;

    private PlayerController playerController;
    private bool isGameOver = false;

    /// <summary>
    /// ����������
    /// </summary>
    private void Start()
    {
        // ���݂̎��Ԃ𐧌����Ԃɐݒ�
        currentTime = timeLimit;

        // PlayerController���擾
        playerController = FindObjectOfType<PlayerController>();
        if (playerController == null)
        {
            Debug.LogError("�V�[������PlayerController��������܂���B�^�C�}�[������ɓ��삵�Ȃ��\��������܂��B");
        }

        // UI�\����������
        UpdateTimeUI();
    }

    /// <summary>
    /// ���t���[�����s����鏈��
    /// </summary>
    private void Update()
    {
        // �Q�[���I�[�o�[��ԂȂ珈�����X�L�b�v
        if (isGameOver) return;

        // ���Ԃ�����������
        currentTime -= Time.deltaTime;

        // ���Ԑ؂�̔���
        if (currentTime <= 0)
        {
            currentTime = 0;
            TimeUp();
        }

        // UI�\�����X�V
        UpdateTimeUI();
    }

    /// <summary>
    /// �c�莞�Ԃ�UI�\�����X�V����
    /// </summary>
    private void UpdateTimeUI()
    {
        // �����l�Ƃ��ĕ\��
        string timeString = Mathf.FloorToInt(currentTime).ToString();

        // TextMeshPro�ƃ��K�V�[Text�̗����ɑΉ�
        if (timeTextTMP != null)
        {
            timeTextTMP.text = timeString;
        }
        else if (timeTextLegacy != null)
        {
            timeTextLegacy.text = timeString;
        }
        else
        {
            Debug.LogWarning("�e�L�X�g�R���|�[�l���g���ݒ肳��Ă��܂���B���Ԃ��\������܂���B");
        }
    }

    /// <summary>
    /// ���Ԑ؂ꎞ�̏���
    /// </summary>
    private void TimeUp()
    {
        // ���ɃQ�[���I�[�o�[��ԂȂ牽�����Ȃ�
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("���Ԑ؂�I�v���C���[�̎c�@��1���炵�܂�");

        // �v���C���[�̃��C�t�����炷
        if (playerController != null)
        {
            playerController.LoseLife();
        }
        else
        {
            Debug.LogError("PlayerController��������܂���B���C�t�����点�܂���B");
        }
    }

    /// <summary>
    /// �^�C�}�[�����Z�b�g����i���x�����X�^�[�g���ȂǂɎg�p�j
    /// </summary>
    public void ResetTimer()
    {
        currentTime = timeLimit;
        isGameOver = false;
        UpdateTimeUI();
    }
}