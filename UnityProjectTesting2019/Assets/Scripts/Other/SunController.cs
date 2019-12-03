using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunController : MonoBehaviour {

    public Light _sun;
    public Light _moon;

    public float _sunDefaultIntensity = 1.0f;
    public float _moonDefaultIntensity = 0.5f;
    public float _sunMoonYRotation = -30f;

    // Given a reference to a directional light posing as the sun assigned in the inspector, this function
    // changes the sun's angle accordingly; this also changes the moon's angle, but the moon has a 180 degree offset
    public void UpdateSunAndMoon(int tickCount, int ticksPerDay) {
        float timeOfDay = (float)tickCount % ticksPerDay / ticksPerDay ;
        float sunAngle = timeOfDay * 360f;
        _sun.transform.localRotation  = Quaternion.Euler(sunAngle       , _sunMoonYRotation, 0);
        _moon.transform.localRotation = Quaternion.Euler(sunAngle + 180f, _sunMoonYRotation, 0);

        // The brightnesses of the sun and moon must also change
        // This is determined by the time of day, whose range is [0, 1);
        // if it's at the zero mark, turn on the sun and turn off the moon, but if it's at
        // the halfway mark, turn off the sun and turn on the moon
        if (Mathf.Approximately(timeOfDay, 0f)) {
            _sun.intensity = _sunDefaultIntensity;
            _moon.intensity = 0;
        }
        else if (Mathf.Approximately(timeOfDay, 0.5f)) {
            _sun.intensity = 0;
            _moon.intensity = _moonDefaultIntensity;
        }
    }
}
