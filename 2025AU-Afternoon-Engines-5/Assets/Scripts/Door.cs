using System;
using TMPro;
using UnityEngine;

public class Door : MonoBehaviour
{
    private static readonly int Play = Animator.StringToHash("Play");
    private static readonly int IsOpen = Animator.StringToHash("isOpen");
    private Animator _animator;
    private TextMeshProUGUI _interactionText;
    private bool _canOpen = false;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _interactionText = GameObject.Find("/UI/Interaction Text").GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.E) || !_canOpen) return;
        
        _animator.SetTrigger(Play);
        _animator.SetBool(IsOpen, !_animator.GetBool(IsOpen));
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.CompareTag("Player")) return;
        
        Debug.Log(_animator.GetCurrentAnimatorStateInfo(0).IsName("Default"));

        if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            if (_interactionText.enabled) _interactionText.enabled = false;
            _canOpen = false;
            return;
        }

        if (!_interactionText.enabled) _interactionText.enabled = true;
        
        _canOpen = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_interactionText.enabled) _interactionText.enabled = false;

        _canOpen = false;
    }
}
