using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;//In seconds
    //TODO remove from inspector later
    float movementFactor; // 0 for not moved 100 for fully moved
    //Range gives me a sliderin the inspector
    Vector3 startingPosition;

    void Start()
    {
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CycleMovement();
    }

    private void CycleMovement()
    {
        //set movement factor
        //prevent against period = 0
        if (!(period <= Mathf.Epsilon))
        {
            float cycles = Time.time / period;//grows continually from 0
            const float tau = Mathf.PI * 2f;//about 6.28
            float rawSineWave = Mathf.Sin(cycles * tau);//Varies between -1 and 1

            movementFactor = rawSineWave / 2f + 0.5f;
            Vector3 offset = movementVector * movementFactor;
            transform.position = startingPosition + offset;
        }
        else
        {
            transform.position = startingPosition + movementVector/2;
        }
    }
}
