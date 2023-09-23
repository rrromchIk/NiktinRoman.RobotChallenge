using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikitinRoman.RobotChallenge {
    public class MapUtil {
        /// <summary>
        ///     Checks if cell on map is free (is not ocсupied by another robot)
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="movingRobot"></param>
        /// <param name="robots"></param>
        /// <returns>True or false</returns>
        public bool IsCellFree(Position cell, Robot.Common.Robot movingRobot, List<Robot.Common.Robot> robots) {
            foreach (var robot in robots) {
                if (robot != movingRobot) {
                    if (robot.Position == cell)
                        return false;
                }
            }
            return true;
        }
    }
}
