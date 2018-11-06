using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AudioManager : MonoBehaviour
{
	private static AudioManager _instance;

	public static AudioManager Instance
	{
		get { return _instance; }
	}

	[FormerlySerializedAs("MinPitch")] public float minPitch = 0.9f;
	[FormerlySerializedAs("MaxPitch")] public float maxPitch = 1.1f;

	[FormerlySerializedAs("EfxSource")] public AudioSource efxSource;
	[FormerlySerializedAs("BgSource")]  public AudioSource bgSource;

	private void Awake()
	{
		_instance = this;
	}

	public void RandomPlay(params AudioClip[] clips)
	{
		float     pitch = Random.Range(minPitch, maxPitch);
		int       index = Random.Range(0,        clips.Length);
		AudioClip clip  = clips[index];
		efxSource.clip  = clip;
		efxSource.pitch = pitch;
		efxSource.Play();
	}

	public void DieAudio(params AudioClip[] clips)
	{
		RandomPlay(clips);
	}

	public void StopBgMusic()
	{
		bgSource.Stop();
	}

	public void PlayBgMusic()
	{
		bgSource.Play();
	}
}