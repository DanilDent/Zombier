using UnityEngine;

namespace Prototype.Data
{
    [CreateAssetMenu(fileName = "New Projectile Trail Data", menuName = "Data/Projectile Trail Data")]
    public class ProjectileTrailData : ScriptableObject
    {
        public AnimationCurve WidthCurve;
        public float Time = 0.5f;
        public float MinVertexDistance = 0.1f;
        public Gradient ColorGradient;
        public Material Material;
        public int NumCornverVertices;
        public int numEndCapVertices;

        public void SetupTrail(TrailRenderer trailRenderer)
        {
            trailRenderer.widthCurve = WidthCurve;
            trailRenderer.time = Time;
            trailRenderer.minVertexDistance = MinVertexDistance;
            trailRenderer.colorGradient = ColorGradient;
            trailRenderer.sharedMaterial = Material;
            trailRenderer.numCornerVertices = NumCornverVertices;
            trailRenderer.numCapVertices = numEndCapVertices;
        }
    }
}