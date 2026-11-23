using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PinPop {
 public sealed class HtmlGameAudio:MonoBehaviour {
  PinPopGame game;AudioSource source;readonly Dictionary<string,AudioClip> clips=new Dictionary<string,AudioClip>();
  public void Build(PinPopGame owner){game=owner;source=gameObject.AddComponent<AudioSource>();source.playOnAwake=false;source.volume=1;}
  public void Tone(float frequency,float duration=.1f,bool triangle=false){if(game.Muted)return;string key=frequency+":"+duration+":"+triangle;if(!clips.TryGetValue(key,out var clip)){int n=Mathf.CeilToInt(44100*duration);var samples=new float[n];double phase=0;for(int i=0;i<n;i++){float t=i/44100f;phase+=frequency*Mathf.Pow(.65f,t/duration)*Mathf.PI*2/44100;float wave=Mathf.Sin((float)phase);if(triangle)wave=2/Mathf.PI*Mathf.Asin(wave);samples[i]=wave*.1f*Mathf.Pow(.01f,t/duration);}clip=AudioClip.Create("HTML tone "+key,n,1,44100,false);clip.SetData(samples,0);clips[key]=clip;}source.PlayOneShot(clip);}
  public void Chime(bool won=true){StartCoroutine(Notes(won?new[]{523f,659,784,1046}:new[]{300f,240,180}));}
  IEnumerator Notes(float[] notes){foreach(float n in notes){Tone(n,.22f);yield return new WaitForSecondsRealtime(.11f);}}
 }
}
