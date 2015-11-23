using UnityEngine;
using System.Collections;

public class SoundPlayer : MonoBehaviour {

	public int maxSounds;
	static AudioSource[] mainAudioSource;
	AudioSource[] audioSource;

	void Awake() {
		audioSource = new AudioSource[maxSounds];
		for (int i = 0; i < maxSounds; i++) {
			audioSource[i] = gameObject.AddComponent<AudioSource> ();
		}
		setAudioSource (audioSource);
	}

	static void setAudioSource(AudioSource[] newAudioSource) {
		mainAudioSource = newAudioSource;
	}

	public static AudioSource[] getAudioSource() {
		return mainAudioSource;
	}

	public static void PlayClip (AudioClip clip) {
		if (mainAudioSource != null) {
			for (int i = 0; i < mainAudioSource.Length; i++) {
				if (!mainAudioSource[i].isPlaying) {
					print (i);
					mainAudioSource[i].PlayOneShot (clip);
					//mainAudioSource[i].clip = clip;
					//mainAudioSource[i].Play ();
					break;
				}
			}
		}
	}
}
