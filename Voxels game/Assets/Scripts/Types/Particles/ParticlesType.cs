using UnityEngine;

namespace Types.Particles
{
    [CreateAssetMenu(fileName="new_Particles", menuName = "New particles type")]
    public class ParticlesType : ScriptableObject
    {
        public ParticlesName Name;
        public ParticleSystem System;
        public Material ParticleMaterial;
    }
}
