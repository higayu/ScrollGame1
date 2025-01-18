using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public AudioClip bgmClip; // BGM�p�̃I�[�f�B�I�N���b�v
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource�R���|�[�l���g��������܂���B");
            return;
        }

        if (bgmClip == null)
        {
            Debug.LogError("BGM��AudioClip���ݒ肳��Ă��܂���B");
            return;
        }

        audioSource.clip = bgmClip;
        audioSource.loop = true; // ���[�v�Đ�
        //audioSource.Play(); // BGM���Đ�
    }

    public void StopBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void PauseBGM()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.UnPause();
        }
    }
}
