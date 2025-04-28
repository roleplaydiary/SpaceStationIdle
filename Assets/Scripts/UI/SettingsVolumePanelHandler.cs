using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsVolumePanelHandler : MonoBehaviour
{
    [SerializeField] private TMP_Text musicVolumeLabel;
    [SerializeField] private Button musicVolumeUpButton;
    [SerializeField] private Button musicVolumeDownButton;
    [SerializeField] private TMP_Text effectsVolumeLabel;
    [SerializeField] private Button effectsVolumeUpButton;
    [SerializeField] private Button effectsVolumeDownButton;
    [SerializeField] private TMP_Text ambientVolumeLabel;
    [SerializeField] private Button ambientVolumeUpButton;
    [SerializeField] private Button ambientVolumeDownButton;

    private bool isChanged = false;
    private void Awake()
    {
        ButtonsInitialize();
    }

    private void OnEnable()
    {
        isChanged = false;
    }

    private void OnDisable()
    {
        if (isChanged)
        {
            var audioManager = ServiceLocator.Get<AudioManager>();
            audioManager.SaveSoundSettings();
            isChanged = false;
        }
    }

    private void ButtonsInitialize()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        musicVolumeUpButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateBackgroundMusicVolume(AudioManager.sound_volume_constant);
            musicVolumeLabel.text = $"Music volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
        
        musicVolumeDownButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateBackgroundMusicVolume(-AudioManager.sound_volume_constant);
            musicVolumeLabel.text = $"Music volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
        
        effectsVolumeUpButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateEffectsVolume(AudioManager.sound_volume_constant);
            effectsVolumeLabel.text = $"Effects and UI volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
        
        effectsVolumeDownButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateEffectsVolume(-AudioManager.sound_volume_constant);
            effectsVolumeLabel.text = $"Effects and UI volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
        
        ambientVolumeUpButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateAmbientVolume(AudioManager.sound_volume_constant);
            ambientVolumeLabel.text = $"Ambient volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
        
        ambientVolumeDownButton.OnClickAsObservable().Subscribe(_ =>
        {
            var newVolume = audioManager.UpdateAmbientVolume(-AudioManager.sound_volume_constant);
            ambientVolumeLabel.text = $"Ambient volume: {Math.Round(newVolume * 100)}%";
            isChanged = true;
        }).AddTo(this);
    }

    public void LabelsInitialize()
    {
        var audioManager = ServiceLocator.Get<AudioManager>();
        musicVolumeLabel.text = $"Music volume: {audioManager.backgroundMusicVolume.Value * 100}%";
        effectsVolumeLabel.text = $"Effects and UI volume: {audioManager.effectsVolume.Value * 100}%";
        ambientVolumeLabel.text = $"Ambient volume: {audioManager.ambientVolume.Value * 100}%";
    }
}
