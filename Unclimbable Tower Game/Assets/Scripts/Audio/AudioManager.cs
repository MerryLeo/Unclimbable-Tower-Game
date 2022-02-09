// Script that contains all audio sounds for the player and the objects

using UnityEngine;

public class AudioManager : MonoBehaviour {
    [Header("Ground Audio Settings")]
    [SerializeField] GroundSound[] groundAudios;

    [Header("Prop Audio Settings")]
    [SerializeField] PropSound[] propAudios;
    public GroundSound[] GroundAudios => groundAudios;
    public PropSound[] PropAudios => propAudios;
}

[System.Serializable]
public class GroundSound {
    public Sound RunSound;
    public Sound WalkSound;
    public Sound JumpSound;
    public Sound LandSound;
    public int Mask;
}

[System.Serializable]
public class PropSound {
    public Sound CollisionSound;
    public int Mask;
}

[System.Serializable]
public class Sound {
    public AudioClip clip;
    public float volume = 1;
    public float pitch = 1;
}
