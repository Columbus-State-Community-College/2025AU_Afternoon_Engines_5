using UnityEngine;
using UnityEngine.Animations;

public class Flashlight : MonoBehaviour
{
    private ParentConstraint _parentConstraint;
    private ConstraintSource _constraintSource;
    
    private void Start()
    {
        _parentConstraint = GetComponent<ParentConstraint>();
        _constraintSource = new ConstraintSource
        {
            sourceTransform = GameObject.Find("CameraHolder").transform.Find("PlayerCamera"),
            weight = 1f
        };
        
        _parentConstraint.AddSource(_constraintSource);
        _parentConstraint.SetTranslationOffset(0, _parentConstraint.transform.position - _constraintSource.sourceTransform.position);
        _parentConstraint.constraintActive = true;
    }
    
    private void Update()
    {
        
    }
}
