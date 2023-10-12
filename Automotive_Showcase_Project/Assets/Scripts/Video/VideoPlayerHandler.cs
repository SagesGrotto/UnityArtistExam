using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayerHandler : MonoBehaviour
{
    public VideoPlayer Player;
    public Slider VolumeSlider;
    public Slider PlayheadSlider;
    public Button PlayPauseButton;
    public Sprite playSprite;
    public Sprite pauseSprite;

    void Start()
    {
        Player.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Sherman.mp4");
        PlayPauseButton.GetComponent<Image>().sprite = playSprite;
        
        PlayheadSlider.onValueChanged.AddListener(value =>
        {
            Player.time = value * Player.length;
        });
        
        PlayPauseButton.onClick.AddListener(() =>
        {
            if (Player.isPlaying)
            {
                PlayPauseButton.GetComponent<Image>().sprite = pauseSprite;
                Player.Pause();
            }
            else
            {
                PlayPauseButton.GetComponent<Image>().sprite = playSprite;
                Player.Play();
            }
        });
        
        VolumeSlider.SetValueWithoutNotify(Player.GetDirectAudioVolume(0));
        VolumeSlider.onValueChanged.AddListener(volume =>
        {
            Player.SetDirectAudioVolume(0, volume);
        });
    }

    private void Update()
    {
        var time = Player.time / Player.length;

        if (double.IsNaN(time))
            time = 0;
        
        PlayheadSlider.SetValueWithoutNotify((float)time);
    }
}
