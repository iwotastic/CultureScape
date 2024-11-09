using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;

public class ColliderPatchSurface : MonoBehaviour, ISurfacePatch
{
    [SerializeField] private Collider outer;
    [SerializeField] private Collider inner;
    [SerializeField] private float minPokeDistance = 0.01f;

    private PatchSurface _outer;
    private PatchSurface _inner;
    
    private void Awake()
    {
        _outer = new PatchSurface(outer, this);
        _inner = new PatchSurface(inner, this);
    }

    public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0) =>
        _outer.Raycast(ray, out hit, maxDistance);
    
    public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0) =>
        _outer.ClosestSurfacePoint(point, out hit, maxDistance);

    public Transform Transform => _outer.Transform;
    public ISurface BackingSurface => _inner;

    private class PatchSurface : ISurface
    {
        private readonly Collider _collider;
        private readonly ColliderPatchSurface _surface;
        
        public PatchSurface(Collider collider, ColliderPatchSurface surface)
        {
            _collider = collider;
            _surface = surface;
        }

        public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0)
        {
            hit = new SurfaceHit();

            RaycastHit hitInfo;

            if (_collider.Raycast(ray, out hitInfo,
                    maxDistance <= 0 ? float.MaxValue : maxDistance))
            {
                hit.Point = hitInfo.point;
                hit.Normal = hitInfo.normal;
                hit.Distance = hitInfo.distance;
                return true;
            }

            return false;
        }

        public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0)
        {
            Vector3 closest = _collider.ClosestPoint(point);

            Vector3 delta = closest - point;
            if (delta.sqrMagnitude < _surface.minPokeDistance * _surface.minPokeDistance)
            {
                Vector3 direction = _collider.bounds.center - point;
                return Raycast(new Ray(point - direction,
                    direction), out hit, float.MaxValue);
            }
            
            return Raycast(new Ray(point, delta), out hit, maxDistance);
        }

        public Transform Transform => _collider.transform;
    }
}