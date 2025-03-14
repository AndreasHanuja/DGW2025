using UnityEngine;

public class AudioManager : SingeltonMonoBehaviour<AudioManager>
{
    public enum Sound
    {
        PlaceBuilding = 0
    }


    [SerializeField]
    private AudioSource[] AudioSources;


	public void PlaySound(Sound sound)
    {
        AudioSources[(int)sound].Play();
	}
}
