using UnityEngine;
using System.Collections;

public static class AudioSourceExtensions 
{
	public static void playClip( this AudioSource audioSource, AudioClip audioClip )
	{
		audioSource.clip = audioClip;
		audioSource.Play();
	}
}
