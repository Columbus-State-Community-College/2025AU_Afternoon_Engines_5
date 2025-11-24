using System;
using UnityEngine;
using UnityEngine.Animations;

public class Flashlight : MonoBehaviour
{
    public float baseDamage = 2f;
    public float rangeModifier = 0.5f;
    public float angleModifier = 0.5f;

    public bool damaging = false;
    
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
    
    private void LateUpdate()
    {
        DetectHits();
    }

    private void DetectHits()
    {
        // We need to get the light cone's radius to make the sphere cast the correct size. Math, woohoo.
        var innerAngle = _lightSource.innerSpotAngle;
        var height = _lightSource.range / 8;  // Dividing by 8 here to get the flashlight range closer to how it appears.
        var slantHeight = height / Math.Sin(innerAngle);
        var radius = (float)Math.Sqrt(Math.Pow(height, 2) + Math.Pow(slantHeight, 2));
        var sphereCastHits = Physics.SphereCastAll(transform.position, radius, transform.forward, height);
        var hittingEnemy = false;

        foreach (var spherecastHit in sphereCastHits)
        {
            if (!spherecastHit.transform.CompareTag("Enemy")) continue;
            
            if (spherecastHit.transform.name.Contains("Reaper")) continue;  // Reapers are invincible, no point in checking.
            
            var targetDirection = spherecastHit.transform.position - transform.position;
            var hitAngle = Vector3.Angle(targetDirection, transform.forward);
            
            if (hitAngle > innerAngle) continue;  // Are we inside the flashlight cone?
            
            // Second raycast to make sure that the enemy is actually being hit and is within the flashlight cone.=
            Physics.Raycast(transform.position, targetDirection, out var raycastHit);
            // Debug.DrawRay(transform.position, targetDirection, Color.cyan);
            
            if (!raycastHit.transform.CompareTag("Enemy")) continue;  // Are we still hitting the enemy, or is an object blocking?
            
            ApplyDamage(hitAngle, raycastHit.distance, raycastHit.transform.gameObject);
            if (!hittingEnemy) hittingEnemy = true;
        }

        damaging = hittingEnemy;
    }

    private void ApplyDamage(float hitAngle, float distance, GameObject target)
    {
        var targetHealthComponent = target.GetComponent<HealthSystem>();

        if (targetHealthComponent is null) return;

        if (hitAngle == 0f) hitAngle = 0.00001f;
        var damage = baseDamage * Mathf.Pow(rangeModifier, distance / (_lightSource.range / 8)) * (angleModifier / hitAngle);
        
        targetHealthComponent.TakeDamage(damage);
    }
}
