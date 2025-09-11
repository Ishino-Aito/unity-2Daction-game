using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;

/// <summary>
/// �v���C���[�̗̑͂Ɩh��V�X�e�����Ǘ�����N���X
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("�̗͐ݒ�")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float defaultDamageAmount = 34f;
    [SerializeField] private string gameOverSceneName = "GameOverScene";

    [Header("UI�ݒ�")]
    [SerializeField] private Image hpGaugeImage;
    [SerializeField] private RectTransform hpGaugeRect;
    private float originalGaugeWidth;

    [Header("���G���Ԑݒ�")]
    [SerializeField] private float invincibilityDuration = 1f; // ���G��Ԃ̎�������
    [SerializeField] private float blinkInterval = 0.1f;      // �_�ŊԊu
    private bool isInvincible = false;                        // ���G��ԃt���O
    private float invincibilityTimer = 0f;                    // ���G��ԃ^�C�}�[
    private float blinkTimer = 0f;                            // �_�Ń^�C�}�[
    private bool isVisible = true;                            // �\����ԃt���O

    [Header("�w�����b�g�ݒ�")]
    private bool hasHelmet = false;                           // �ʏ�w�����b�g�����t���O
    private bool hasBlueHelmet = false;                       // ���w�����b�g�����t���O

    // �R���|�[�l���g�Q��
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerController playerController;

    [Header("�����ݒ�")]
    [SerializeField] private AudioSource audioSource;          // AudioSource�R���|�[�l���g
    [SerializeField] private AudioClip damageSE;               // �_���[�W���ʉ� (se_dageki23)
    [SerializeField] private AudioClip powerupSE;              // �p���[�A�b�v���ʉ� (se_powerup)
    [SerializeField][Range(0f, 1f)] private float damageVolume = 0.5f;   // �_���[�W���̉���
    [SerializeField][Range(0f, 1f)] private float powerupVolume = 0.5f;  // �p���[�A�b�v���̉���

    private void Start()
    {
        // ����������
        currentHealth = maxHealth;

        // �����V�X�e���̏�����
        InitializeAudioSystem();

        // �K�v�ȃR���|�[�l���g���擾
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerController = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("Animator�R���|�[�l���g��������܂���B");
        }
        else
        {
            // ������Ԃł̓w�����b�g�𑕔����Ă��Ȃ���ԂɃ��Z�b�g
            ResetHelmetStates();
        }

        // HP�Q�[�W�̏�������ۑ�
        if (hpGaugeRect != null)
        {
            originalGaugeWidth = hpGaugeRect.sizeDelta.x;
        }

        // �̗�UI�X�V
        UpdateHealthUI();
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
                Debug.Log("PlayerHealth: AudioSource�R���|�[�l���g�������ǉ����܂����B");
            }
        }

        // AudioSource�̊�{�ݒ�
        audioSource.playOnAwake = false;  // �����Đ����Ȃ�
        audioSource.spatialBlend = 0f;    // 2D�T�E���h�i3D���ʂȂ��j
    }

    /// <summary>
    /// ���ʉ����Đ�����
    /// </summary>
    /// <param name="clip">�Đ�����AudioClip</param>
    /// <param name="volume">���ʁi0.0�`1.0�j</param>
    private void PlaySE(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            if (clip == null) Debug.LogWarning("PlayerHealth: �Đ����悤�Ƃ������ʉ����ݒ肳��Ă��܂���B");
            if (audioSource == null) Debug.LogWarning("PlayerHealth: AudioSource��������܂���B");
        }
    }

    /// <summary>
    /// �_���[�W���ʉ����Đ�����
    /// </summary>
    private void PlayDamageSE()
    {
        PlaySE(damageSE, damageVolume);
    }

    /// <summary>
    /// �p���[�A�b�v���ʉ����Đ�����
    /// </summary>
    private void PlayPowerupSE()
    {
        PlaySE(powerupSE, powerupVolume);
    }

    private void Update()
    {
        // ���G��Ԃ̏���
        if (isInvincible)
        {
            // ���G�^�C�}�[������
            invincibilityTimer -= Time.deltaTime;

            // �_�ŏ���
            blinkTimer -= Time.deltaTime;
            if (blinkTimer <= 0)
            {
                blinkTimer = blinkInterval;
                if (spriteRenderer != null)
                {
                    isVisible = !isVisible;
                    spriteRenderer.enabled = isVisible;
                }
            }

            // ���G��Ԃ��I��������
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
                if (spriteRenderer != null)
                {
                    spriteRenderer.enabled = true; // �K���\����Ԃɖ߂�
                }
                Debug.Log("���G��Ԃ��I�����܂���");
            }
        }
    }

    /// <summary>
    /// ���ׂẴw�����b�g��Ԃ����Z�b�g����
    /// </summary>
    private void ResetHelmetStates()
    {
        hasHelmet = false;
        hasBlueHelmet = false;

        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("�w�����b�g��Ԃ����Z�b�g���܂���");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �I�u�W�F�N�g�^�O�ŏ����𕪊򂳂���
        switch (other.tag)
        {
            case "Helmet":
                // �ʏ�̃w�����b�g�擾����
                EquipHelmet();
                Destroy(other.gameObject);
                break;

            case "BlueHelmet":
                // ���w�����b�g�擾����
                EquipBlueHelmet();
                Destroy(other.gameObject);
                break;

            case "Beam":
            case "Blue Beam":
                // �r�[���Ƃ̏Փˏ���
                HandleBeamCollision(other);
                break;

            case "Toge":
                // �g�Q�Ȃǂ̃_���[�W�I�u�W�F�N�g�Ƃ̏Փˏ���
                if (!isInvincible)
                {
                    TakeDamage(defaultDamageAmount);
                    ActivateInvincibility(); // �_���[�W���󂯂���ɖ��G��Ԃ�L����
                }
                break;
        }
    }

    /// <summary>
    /// ���G��Ԃ�L���ɂ���
    /// </summary>
    private void ActivateInvincibility()
    {
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        blinkTimer = blinkInterval;
        Debug.Log("���G��Ԃ��J�n���܂���: " + invincibilityDuration + "�b��");
    }

    /// <summary>
    /// �r�[���Ƃ̏Փˏ������s��
    /// </summary>
    private void HandleBeamCollision(Collider2D beam)
    {
        // �r�[���̎�ނ𔻒�
        bool isBlueBeam = beam.CompareTag("Blue Beam");

        // ���w�����b�g�������Ă��āA���r�[�������������ꍇ�̓��ʏ���
        if (hasBlueHelmet && isBlueBeam)
        {
            Debug.Log("���w�����b�g�����r�[����h�����I");
            RemoveBlueHelmet();
            Destroy(beam.gameObject);
            return;
        }
        // �ʏ�̃w�����b�g�������Ă��āA�ʏ�̃r�[�������������ꍇ
        else if (hasHelmet && !isBlueBeam)
        {
            Debug.Log("�w�����b�g���r�[����h�����I");
            RemoveHelmet();
            Destroy(beam.gameObject);
            return;
        }

        // ���G��Ԃ̏ꍇ�̓_���[�W���󂯂Ȃ�
        if (isInvincible)
        {
            Destroy(beam.gameObject);
            return;
        }

        // �w�����b�g���Ȃ��ꍇ�A�܂��͑Ή�����w�����b�g���Ȃ��ꍇ�̓_���[�W���󂯂�
        float damageAmount = defaultDamageAmount;

        // �r�[������_���[�W�ʂ��擾
        if (isBlueBeam)
        {
            BlueBeamMovement blueBeam = beam.GetComponent<BlueBeamMovement>();
            if (blueBeam != null)
            {
                damageAmount = blueBeam.GetDamageAmount();
            }
        }
        else
        {
            BeamMovement normalBeam = beam.GetComponent<BeamMovement>();
            if (normalBeam != null)
            {
                damageAmount = normalBeam.GetDamageAmount();
            }
        }

        TakeDamage(damageAmount);
        ActivateInvincibility(); // �_���[�W���󂯂���ɖ��G��Ԃ�L����
        Destroy(beam.gameObject);
    }

    /// <summary>
    /// �_���[�W���󂯂鏈��
    /// </summary>
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0f); // �̗͂�0�����ɂȂ�Ȃ��悤����
        UpdateHealthUI();

        // �_���[�W���ʉ����Đ�
        PlayDamageSE();

        if (currentHealth <= 0)
        {
            Debug.Log("�v���C���[���_���[�W�Ŏ��S");
            playerController.LoseLife();
        }
    }

    /// <summary>
    /// �O�����璼�ڃ_���[�W��^������J���\�b�h
    /// </summary>
    public void ApplyDirectDamage(float damage)
    {
        // ���G��Ԃ̏ꍇ�̓_���[�W���󂯂Ȃ��悤�ɂ���
        if (isInvincible) return;

        TakeDamage(damage);
        ActivateInvincibility(); // �_���[�W���󂯂���ɖ��G��Ԃ�L����
    }

    /// <summary>
    /// �̗�UI���X�V����
    /// </summary>
    private void UpdateHealthUI()
    {
        float healthRatio = currentHealth / maxHealth;

        // Image�^��HP�Q�[�W���X�V
        if (hpGaugeImage != null)
        {
            hpGaugeImage.fillAmount = healthRatio;
            Debug.Log("HP�\�� (Image): " + hpGaugeImage.fillAmount);
        }
        // RectTransform�^��HP�Q�[�W���X�V
        else if (hpGaugeRect != null)
        {
            Vector2 sizeDelta = hpGaugeRect.sizeDelta;
            sizeDelta.x = originalGaugeWidth * healthRatio;
            hpGaugeRect.sizeDelta = sizeDelta;
            Debug.Log("HP�\�� (RectTransform): " + healthRatio);
        }
        else
        {
            Debug.LogWarning("HP�Q�[�W�R���|�[�l���g���ݒ肳��Ă��܂���B�̗͂��\������܂���B");
        }
    }

    /// <summary>
    /// �ʏ�̃w�����b�g�𑕔�����
    /// </summary>
    public void EquipHelmet()
    {
        // ���łɃw�����b�g�������Ă���ꍇ�͈�x��������
        if (hasHelmet || hasBlueHelmet)
        {
            RemoveAllHelmets();
        }

        hasHelmet = true;
        hasBlueHelmet = false;

        // �A�j���[�^�[�̃p�����[�^��ݒ�
        if (animator != null)
        {
            animator.SetBool("HasHelmet", true);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("�ʏ�w�����b�g����: �A�j���[�V�����p�����[�^�X�V");
        }
        else
        {
            Debug.LogWarning("Animator��������܂���B�w�����b�g��Ԃ̃A�j���[�V�������X�V�ł��܂���");
        }

        // �p���[�A�b�v���ʉ����Đ��i�Ō�ɒǉ��j
        PlayPowerupSE();

    }

    /// <summary>
    /// ���w�����b�g�𑕔�����
    /// </summary>
    public void EquipBlueHelmet()
    {
        // ���łɃw�����b�g�������Ă���ꍇ�͈�x��������
        if (hasHelmet || hasBlueHelmet)
        {
            RemoveAllHelmets();
        }

        hasBlueHelmet = true;
        hasHelmet = false;

        // �A�j���[�^�[�̃p�����[�^��ݒ�
        if (animator != null)
        {
            animator.SetBool("HasBlueHelmet", true);
            animator.SetBool("HasHelmet", false);
            Debug.Log("���w�����b�g����: �A�j���[�V�����p�����[�^�X�V");
        }
        else
        {
            Debug.LogWarning("Animator��������܂���B�w�����b�g��Ԃ̃A�j���[�V�������X�V�ł��܂���");
        }

        PlayPowerupSE();

    }

    /// <summary>
    /// �ʏ�̃w�����b�g�����O��
    /// </summary>
    public void RemoveHelmet()
    {
        hasHelmet = false;

        // �A�j���[�^�[�̃p�����[�^���X�V
        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            Debug.Log("�w�����b�g����: �A�j���[�V�����p�����[�^�X�V");
        }
        else
        {
            Debug.LogWarning("Animator��������܂���B�w�����b�g��Ԃ̃A�j���[�V�������X�V�ł��܂���");
        }
    }

    /// <summary>
    /// ���w�����b�g�����O��
    /// </summary>
    public void RemoveBlueHelmet()
    {
        hasBlueHelmet = false;

        // �A�j���[�^�[�̃p�����[�^���X�V
        if (animator != null)
        {
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("���w�����b�g����: �A�j���[�V�����p�����[�^�X�V");
        }
        else
        {
            Debug.LogWarning("Animator��������܂���B�w�����b�g��Ԃ̃A�j���[�V�������X�V�ł��܂���");
        }
    }

    /// <summary>
    /// ���ׂẴw�����b�g�����O��
    /// </summary>
    private void RemoveAllHelmets()
    {
        hasHelmet = false;
        hasBlueHelmet = false;

        // �A�j���[�^�[�̃p�����[�^���X�V
        if (animator != null)
        {
            animator.SetBool("HasHelmet", false);
            animator.SetBool("HasBlueHelmet", false);
            Debug.Log("���ׂẴw�����b�g����: �A�j���[�V�����p�����[�^�X�V");
        }
        else
        {
            Debug.LogWarning("Animator��������܂���B�w�����b�g��Ԃ̃A�j���[�V�������X�V�ł��܂���");
        }
    }

    /// <summary>
    /// �ʏ�w�����b�g�𑕔����Ă��邩�ǂ�����Ԃ�
    /// </summary>
    public bool HasHelmet()
    {
        return hasHelmet;
    }

    /// <summary>
    /// ���w�����b�g�𑕔����Ă��邩�ǂ�����Ԃ�
    /// </summary>
    public bool HasBlueHelmet()
    {
        return hasBlueHelmet;
    }

    /// <summary>
    /// �����ꂩ�̃w�����b�g�𑕔����Ă��邩�ǂ�����Ԃ�
    /// </summary>
    public bool HasAnyHelmet()
    {
        return hasHelmet || hasBlueHelmet;
    }

    /// <summary>
    /// �̗͂��ő�l�Ƀ��Z�b�g����
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    /// <summary>
    /// ���G��Ԃ��ǂ�����Ԃ�
    /// </summary>
    public bool IsInvincible()
    {
        return isInvincible;
    }
}