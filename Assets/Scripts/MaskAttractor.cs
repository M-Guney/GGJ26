using StarterAssets;
using UnityEngine;
using System.Collections.Generic;

public class MaskAttractor : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The AttractableArea object with a trigger collider that defines the attraction zone.")]
    [SerializeField] private GameObject _attractableArea;
    [Tooltip("Speed at which masks move towards the attraction center")]
    [SerializeField] private float _attractSpeed = 15f;

    private StarterAssetsInputs _input;
    private HashSet<Transform> _masksInRange = new HashSet<Transform>();
    private Transform _attractionCenter;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
        if (_input == null)
        {
            Debug.LogError("MaskAttractor: StarterAssetsInputs component missing!");
        }

        if (_attractableArea != null)
        {
            _attractionCenter = _attractableArea.transform;
            
            // Add this script's trigger callbacks to the AttractableArea
            var areaCollider = _attractableArea.GetComponent<Collider>();
            if (areaCollider == null)
            {
                Debug.LogError("MaskAttractor: AttractableArea has no Collider component!");
            }
            else if (!areaCollider.isTrigger)
            {
                Debug.LogWarning("MaskAttractor: AttractableArea's Collider should be set to 'Is Trigger'");
            }

            // Add a helper component to the AttractableArea to detect masks
            var detector = _attractableArea.GetComponent<MaskDetector>();
            if (detector == null)
            {
                detector = _attractableArea.AddComponent<MaskDetector>();
            }
            detector.Initialize(this);
        }
        else
        {
            Debug.LogError("MaskAttractor: AttractableArea is not assigned!");
        }
    }

    private void Update()
    {
        if (_input == null || !_input.attract) return;

        AttractMasks();
    }

    private void AttractMasks()
    {
        if (_attractionCenter == null) return;

        foreach (var maskTransform in _masksInRange)
        {
            if (maskTransform == null) continue;

            // Skip masks that are already picked up (have a parent)
            if (maskTransform.parent != null) continue;

            // Move the mask towards the center
            Vector3 direction = (_attractionCenter.position - maskTransform.position).normalized;
            maskTransform.position += direction * _attractSpeed * Time.deltaTime;
        }
    }

    public void OnMaskEnter(Transform mask)
    {
        _masksInRange.Add(mask);
    }

    public void OnMaskExit(Transform mask)
    {
        _masksInRange.Remove(mask);
    }

    // Helper component to detect masks in the AttractableArea
    private class MaskDetector : MonoBehaviour
    {
        private MaskAttractor _attractor;

        public void Initialize(MaskAttractor attractor)
        {
            _attractor = attractor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_attractor != null && other.CompareTag("Mask"))
            {
                _attractor.OnMaskEnter(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_attractor != null && other.CompareTag("Mask"))
            {
                _attractor.OnMaskExit(other.transform);
            }
        }
    }
}
