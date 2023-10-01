using Robot.Common;
using rom4ik.RobotChallange;
using NikitinRoman.RobotChallenge;

namespace NiktinRoman.RobotChallenge.Test {
    public class RobotsUtilTests {
        private RobotsUtil robotsUtil;

        public RobotsUtilTests() {
            DistanceHelper distanceHelper = new DistanceHelper();
            EnergyStationsUtil energyStationsUtil = new EnergyStationsUtil(distanceHelper, new MapUtil(), 50);
            robotsUtil = new RobotsUtil(energyStationsUtil, distanceHelper, 60, 40, 700);
        }

        [Fact]
        public void CheckConditionsToCreateNewRobotMethodTest() {
            Map map = new Map();
            Robot.Common.Robot movingRobot = new Robot.Common.Robot();
            IList<Robot.Common.Robot> robots = new List<Robot.Common.Robot>();

            int createdRobots = 30;
            int roundNumber = 41;

            bool createNewRobot = robotsUtil.CheckConditionsToCreateNewRobot(map, robots, movingRobot, createdRobots, roundNumber);
            Assert.False(createNewRobot);

            createdRobots = 61;
            roundNumber = 40;
            createNewRobot = robotsUtil.CheckConditionsToCreateNewRobot(map, robots, movingRobot, createdRobots, roundNumber);
            Assert.False(createNewRobot);
        }
    }
}
