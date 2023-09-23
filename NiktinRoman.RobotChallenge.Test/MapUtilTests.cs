using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanNikitin.RobotChallange.Test {
    public class MapUtilTests {
        private MapUtil mapUtil;
        public MapUtilTests() {
            this.mapUtil = new MapUtil();
        }


        [Fact]
        public void IsCellFreeMethodTest() {
            MapUtil mapUtil = new MapUtil();

            Position positionToBeChecked = new Position(4, 4);

            Position movingRobotPosition = new Position(4, 4);
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = movingRobotPosition };

            List<Robot.Common.Robot> robots = new List<Robot.Common.Robot> {
                movingRobot
            };

            for (int i = 0; i < 10; ++i) {
                robots.Add(new Robot.Common.Robot() { Position = new Position(i + 5, i + 10) });
            }

            bool isFree = mapUtil.IsCellFree(positionToBeChecked, movingRobot, robots);
            Assert.True(isFree);

            robots.Remove(movingRobot);
            isFree = mapUtil.IsCellFree(positionToBeChecked, movingRobot, robots);
            Assert.True(isFree);

            robots.Add(new Robot.Common.Robot() { Position = positionToBeChecked });
            isFree = mapUtil.IsCellFree(positionToBeChecked, movingRobot, robots);
            Assert.False(isFree);
        }
    }
}
