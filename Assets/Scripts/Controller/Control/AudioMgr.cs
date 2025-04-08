using UnityEngine;
using System.Collections.Generic;
using static ManyKnivesDefine;

public class AudioMgr
{
    GameScene sceneMgr;
    float BGMVolume = 1, FXVolume = 1;
    bool hitFlag;
    float hitTimer;
    bool knifeFightFlag;
    float knifeFightTimer;
    bool knifePickFlag;
    float knifePickTimer;
    bool freezeFlag;
    float freezeTimer;
    const float hitTimeCD = 0.1f;
    const float knife_fightTimeCD = 0.1f;
    const float knifePickTimeCD = 0.1f;
    const float freezeTimeCD = 0.5f;

    // 没有的默认0.5
    readonly Dictionary<string, float> fxVolumePair = new()
    {
        [AudioClips.monster_dash] = 0.4f,
        [AudioClips.knife_fight] = 0.7f,
        [AudioClips.bingdong] = 0.4f,
        [AudioClips.BGM2] = 1,
    };

    float GetFXVolume(string clip)
    {
        if (!fxVolumePair.TryGetValue(clip, out var value)) value = 0.5f;
        return value;
    }

    public void Init(GameScene scene)
    {
        sceneMgr = scene;
    }
    public void Ready()
    {
        sceneMgr.AS_BGM.volume = BGMVolume * GetFXVolume(AudioClips.BGM2);
        sceneMgr.AS_BGM.clip = ResManager.LoadAudioClip(AudioClips.BGM2);
        sceneMgr.AS_FX.volume = FXVolume;
        sceneMgr.AS_FX_Loop_Craze.volume = FXVolume * GetFXVolume(AudioClips.violent);
        sceneMgr.AS_FX_Loop_Craze.clip = ResManager.LoadAudioClip(AudioClips.violent);
        sceneMgr.pauseBind.Add(PauseListener);
        //sceneMgr.rolePlayer.crazeBind.Add(value =>TODO
        //{
        //    if (value) sceneMgr.AS_FX_Loop_Craze.Play(0);
        //    else sceneMgr.AS_FX_Loop_Craze.Stop();
        //});
        ResetGame();
    }

    void PauseListener(bool pause)
    {
        if (pause)
        {
            sceneMgr.AS_BGM.Pause();
            sceneMgr.AS_FX.Pause();
            sceneMgr.AS_FX_Loop_Craze.Pause();
        }
        else
        {
            sceneMgr.AS_BGM.Play();
            sceneMgr.AS_FX.Play();
            //if (sceneMgr.rolePlayer.crazeBind.value) sceneMgr.AS_FX_Loop_Craze.Play();TODO
        }
    }

    public void PlayOneShot(string clip)
    {
        sceneMgr.AS_FX.PlayOneShot(ResManager.LoadAudioClip(clip), GetFXVolume(clip));
    }

    public void PlayFreezeSound()
    {
        if (freezeFlag) return;
        PlayOneShot(AudioClips.bingdong);
        freezeFlag = true;
        freezeTimer = 0;
    }

    public void PlayKnifePickSound()
    {
        if (knifePickFlag) return;
        PlayOneShot(AudioClips.pick_knife);
        knifePickFlag = true;
        knifePickTimer = 0;
    }

    public void PlayKnifeFightSound(int bladeType)
    {
        if (knifeFightFlag) return;
        PlayOneShot(AudioClips.knife_fight);
        knifeFightFlag = true;
        knifeFightTimer = 0;
    }

    public void PlayHitSound()
    {
        if (hitFlag) return;
        PlayOneShot(AudioClips.hit);
        hitFlag = true;
        hitTimer = 0;
    }

    public void Update()
    {
        var deltaTime = Time.deltaTime;
        if (hitFlag)
        {
            hitTimer += deltaTime;
            if (hitTimer >= hitTimeCD)
            {
                hitFlag = false;
                hitTimer = 0;
            }
        }

        if (knifeFightFlag)
        {
            knifeFightTimer += deltaTime;
            if (knifeFightTimer >= knife_fightTimeCD)
            {
                knifeFightFlag = false;
                knifeFightTimer = 0;
            }
        }

        if (knifePickFlag)
        {
            knifePickTimer += deltaTime;
            if (knifePickTimer >= knifePickTimeCD)
            {
                knifePickFlag = false;
                knifePickTimer = 0;
            }
        }

        if (freezeFlag)
        {
            freezeTimer += deltaTime;
            if (freezeTimer >= freezeTimeCD)
            {
                freezeFlag = false;
                freezeTimer = 0;
            }
        }
    }

    public void ResetGame()
    {
        sceneMgr.AS_BGM.Play(0);
        sceneMgr.AS_FX.Stop();
        sceneMgr.AS_FX_Loop_Craze.Stop();
        hitFlag = false;
        knifeFightFlag = false;
        knifePickFlag = false;
        freezeFlag = false;
    }

    public void StartBattle() { }
    public void OverBattle()
    {
        sceneMgr.AS_FX_Loop_Craze.Stop();
    }
}
