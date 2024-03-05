using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CreatingABasketballTeamJSON
{
    public class Player
    {
        public Player(string name, Stats stats)
        {
            Name = name;
            Stats = stats;
        }

        public string Name { get; set; }

        public Stats Stats { get; set; }

        public double AverageStats
        {
            get
            {
                double averageStats = (double)(Stats.Endurance + Stats.Dribble + Stats.Passes + Stats.Sprint + Stats.Shooting) / 5;

                return Math.Round(averageStats);
            }
        }
    }
}
