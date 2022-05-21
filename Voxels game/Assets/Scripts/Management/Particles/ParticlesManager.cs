using System;
using Types.Particles;
using UnityEngine;

namespace Management.Particles
{
    public class ParticlesManager : MonoBehaviour
    {
        public static ParticlesManager Instance;
        private ParticlesType[] _types;

        private void Awake()
        {
            Instance = this;
            _types = Resources.LoadAll<ParticlesType>("Particles");
        }

        public ParticlesType GetParticle(ParticlesName name)
        {
            foreach (var particle in _types)
            {
                if (!particle.Name.Equals(name))
                    continue;
                
                return particle;
            }

            return _types[0];
        }
    }
}
