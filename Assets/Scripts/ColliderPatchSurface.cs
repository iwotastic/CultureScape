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

    private PatchSurface Outer => _outer ??= new PatchSurface(true, this);
    private PatchSurface Inner => _inner ??= new PatchSurface(false, this);
    
    public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0) =>
        Outer.Raycast(ray, out hit, maxDistance);
    
    public bool ClosestSurfacePoint(in Vector3 point, out SurfaceHit hit, float maxDistance = 0) =>
        Outer.ClosestSurfacePoint(point, out hit, maxDistance);

    public Transform Transform => Outer.Transform;
    public ISurface BackingSurface => Inner;

    private class PatchSurface : ISurface
    {
        private readonly ColliderPatchSurface _surface;
        private readonly bool _isOuter;
        
        public PatchSurface(bool isOuter, ColliderPatchSurface surface)
        {
            _isOuter = isOuter;
            _surface = surface;
        }

        private Collider SurfaceCollider => _isOuter ? _surface.outer : _surface.inner;

        public bool Raycast(in Ray ray, out SurfaceHit hit, float maxDistance = 0)
        {
            hit = new SurfaceHit();

            RaycastHit hitInfo;

            if (SurfaceCollider.Raycast(ray, out hitInfo,
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
            Vector3 closest = SurfaceCollider.ClosestPoint(point);

            Vector3 delta = closest - point;
            if (delta.sqrMagnitude < _surface.minPokeDistance * _surface.minPokeDistance)
            {
                Vector3 direction = SurfaceCollider.bounds.center - point;
                return Raycast(new Ray(point - direction,
                    direction), out hit, float.MaxValue);
            }
            
            return Raycast(new Ray(point, delta), out hit, maxDistance);
        }

        public Transform Transform => SurfaceCollider.transform;
    }
}