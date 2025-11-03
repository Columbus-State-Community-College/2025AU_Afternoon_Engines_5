using System;
using UnityEngine;
using UnityEngine.Animations;

public class Flashlight : MonoBehaviour
{
    private ParentConstraint _parentConstraint;
    private ConstraintSource _constraintSource;

    private Light _lightSource;
    
    private void Start()
    {
        // The constraints make it so the flashlight is a parent of the FlashlightCamera without needing to be an actual parent.
        _parentConstraint = GetComponent<ParentConstraint>();
        _constraintSource = new ConstraintSource
        {
            sourceTransform = GameObject.Find("CameraHolder").transform.Find("PlayerCamera"),
            weight = 1f
        };
        _lightSource = transform.Find("LightSource").GetComponent<Light>();
        
        _parentConstraint.AddSource(_constraintSource);
        _parentConstraint.SetTranslationOffset(0, _parentConstraint.transform.position - _constraintSource.sourceTransform.position);
        _parentConstraint.constraintActive = true;
    }
    
    private void Update()
    {
        DetectHits();
    }

    private void DetectHits()
    {
        // We need to get the light cone's radius for the sphere cast to properly detect enemies.
        var innerAngle = _lightSource.innerSpotAngle;
        var height = _lightSource.range / 8;  // Dividing by 8 here to get the flashlight range closer to how it visibly looks.
        var slantHeight = height / Math.Sin(innerAngle);
        var radius = (float)Math.Sqrt(Math.Pow(height, 2) + Math.Pow(slantHeight, 2));
        var sphereCastHits = Physics.SphereCastAll(transform.position, radius, transform.forward, height);

        foreach (var spherecastHit in sphereCastHits)
        {
            var targetDirection = spherecastHit.transform.position - transform.position;
            
            if (!spherecastHit.transform.CompareTag("Enemy")) continue;
            if (Vector3.Angle(targetDirection, transform.forward) > innerAngle) continue;  // Are we inside the flashlight cone?
            
            // Second raycast to make sure that the enemy is actually being hit and is within the flashlight cone.
            Physics.Raycast(transform.position, spherecastHit.transform.position - transform.position, out var raycastHit);
            // Debug.DrawRay(transform.position, spherecastHit.transform.position - transform.position, Color.red);
            
            if (!raycastHit.transform.CompareTag("Enemy")) continue;  // Are we still hitting the enemy, or is an object blocking?
            
            // Debug.Log("Hit enemy");
            // TODO: Implement damaging the enemy based on distance
        }
    }
}
