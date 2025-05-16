using System;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioBeatClock : MonoBehaviour
    {
        private AudioSource _Source = null!;

        [SerializeField]
        private float _BeatsPerMinute = 1f;
        public float BeatsPerMinute => _BeatsPerMinute;
        public double SecondsPerBeat => 60d / _BeatsPerMinute;
        public float Time => _Source.time;
        
        [SerializeField]
        private ulong _CurrentBeat = 0;
        public ulong CurrentBeat => _CurrentBeat;

        [SerializeField]
        private double _LastTime = 0;
        
        [SerializeField]
        private double _NextBeatTime = 0;

        private double _StartTime;

        private void Awake()
        {
            _Source = GetComponent<AudioSource>();
            _StartTime = AudioSettings.dspTime;
        }

        private void Update()
        {
            double currentTime = AudioSettings.dspTime;
            _CurrentBeat = (ulong)((currentTime - _StartTime) / SecondsPerBeat);
        }
        
        public void SetBeatsPerMinute(float bpm, bool reset = false)
        {
            _BeatsPerMinute = bpm;
            if (reset)
                ResetValues();
        }

        public void ResetValues()
        {
            _CurrentBeat = 0;
            _LastTime = 0;
            _NextBeatTime = 0;
        }
    }
}