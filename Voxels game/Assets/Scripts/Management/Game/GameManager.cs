using System;
using Controllers.Player;
using UnityEngine;

namespace Management.Game
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private PlayerController _mainPlayer;
        public bool CantBuild { get; set; }

        public PlayerController MainPlayer
        {
            get
            {
                return _mainPlayer;
            }
        }

        private void Awake()
        {
            Instance = this;
        }
    }
}
