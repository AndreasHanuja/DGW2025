using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManagerAudioSource : MonoBehaviour
{
	[SerializeField]
	private Vector2 volumeMinMax = Vector2.one;
	[SerializeField]
	private Vector2 pitchMinMax = Vector2.one;

	private AudioSource ownAudioSOurce;


	private void Awake()
	{
		ownAudioSOurce = GetComponent<AudioSource>();
	}

	public void Play()
	{
		ownAudioSOurce.Stop();
		ownAudioSOurce.volume = Random.Range(volumeMinMax.x, volumeMinMax.y);
		ownAudioSOurce.pitch = Random.Range(pitchMinMax.x, pitchMinMax.y);
		ownAudioSOurce.Play();
	}
}
