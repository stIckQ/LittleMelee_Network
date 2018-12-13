using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager {

    AudioSource audioSource;
    GameObject hitCharacter;
    AudioClip clip;

    public SoundManager(GameObject Hitcharacter,string SoundPath)
    {
        this.hitCharacter = Hitcharacter;
        clip = Resources.Load(SoundPath) as AudioClip;

        audioSource = hitCharacter.GetComponent<AudioSource>();
        if (audioSource==null)
        {
            audioSource = hitCharacter.AddComponent<AudioSource>();
        }

        audioSource.volume = 10f;
        audioSource.clip = clip;
        
    }

	public void PlaySoundAtPosition(Vector3 playPosition)
    {
        if(clip!=null)
        {
            //Debug.Log("PlaySound");
            AudioSource.PlayClipAtPoint(clip,playPosition);
        }       
    }

    public void PlaySound()
    {
        
        if(audioSource!=null)
        {
            audioSource.Play();
        }
    }


}
