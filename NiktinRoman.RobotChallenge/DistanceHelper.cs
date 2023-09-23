using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikitinRoman.RobotChallenge {
    public class DistanceHelper {
        /// <summary>
        ///    Finds distance between two positions.
        /// </summary>
        /// <param name="a">First postition</param>
        /// <param name="b">Second position</param>
        /// <returns>Integer number that defines amount of energy needed to pass the distance.</returns>
        public int FindDistance(Position a, Position b) {
            return (int)(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
    }
}
