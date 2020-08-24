using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour{
    public Sound[] sounds;
    public string[] playOnStart;


    public static AudioManager instance;



    void Awake(){

        if(instance == null){
            instance = this;
        }else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds){
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start(){
        foreach(string s in playOnStart){
            Play(s);
        }
    }


    public void Play(string name){
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null){
            print("Sound: " + name + " is not registered!");
            return;
        }
        s.source.Play();
    }
}
