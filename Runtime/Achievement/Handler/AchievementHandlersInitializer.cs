using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.GameAchievements
{
    public class AchievementHandlersInitializer : MonoBehaviour
    {
        [SerializeField] private List<AchievementHandler> _strategies;



        public void Init()
        {
            _strategies.ForEach(s => s.Init());
        }
    }
}