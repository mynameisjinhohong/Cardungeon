using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEffect : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        
        _particleSystem.playbackSpeed = 0.5f;
    }
}
