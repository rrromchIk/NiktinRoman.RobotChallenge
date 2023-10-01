using NikitinRoman.RobotChallenge;
using Robot.Common;


namespace RomanNikitin.RobotChallange.Test {
    public class EnergyStationsUtilTests {
        private EnergyStationsUtil energyStationsUtil;

        public EnergyStationsUtilTests() {
            this.energyStationsUtil = new EnergyStationsUtil(new DistanceHelper(), new MapUtil(), 50);
        }

        [Fact]
        public void FindAllReachableStationsInGivenRadiusMethodTest() {
            int radius = 9;
            int amountOfStationsInGivenRadius;
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = new Position(4, 4), Energy = radius };
            IList<EnergyStation> allEnergyStations = PrepareEnergyStationsForPositionAndRadius(movingRobot.Position, radius, out amountOfStationsInGivenRadius);

            Map map = new Map() { Stations = allEnergyStations };
            IList<Robot.Common.Robot> robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() };

            IList<EnergyStation> expectedEnergyStations =
                energyStationsUtil.FindAllReachableStationsInGivenRadius(map, radius, movingRobot, robots);

            Assert.Equal(allEnergyStations.Count, radius * radius);
            Assert.Equal(expectedEnergyStations.Count, amountOfStationsInGivenRadius);

            movingRobot.Position = new Position(5, 5);
            allEnergyStations = PrepareEnergyStationsForPositionAndRadius(movingRobot.Position, radius, out amountOfStationsInGivenRadius);
            expectedEnergyStations = energyStationsUtil.FindAllReachableStationsInGivenRadius(map, radius, movingRobot, robots);
            Assert.Equal(allEnergyStations.Count, radius * radius);
            Assert.Equal(expectedEnergyStations.Count, amountOfStationsInGivenRadius);

            robots.Add(new Robot.Common.Robot() { Position = new Position(7, 4) });
            radius = 15;
            allEnergyStations = PrepareEnergyStationsForPositionAndRadius(movingRobot.Position, radius, out amountOfStationsInGivenRadius);
            expectedEnergyStations = energyStationsUtil.FindAllReachableStationsInGivenRadius(map, radius, movingRobot, robots);
            Assert.Equal(expectedEnergyStations.Count, 28);
        }

        private IList<EnergyStation> PrepareEnergyStationsForPositionAndRadius(Position position, int radius, out int amountOfStationsInGivenRadius) { 
            IList<EnergyStation> allEnergyStations = new List<EnergyStation>();

            amountOfStationsInGivenRadius = 0;
            for (int i = 0; i < radius; ++i) {
                for (int j = 0; j < radius; ++j) {
                    EnergyStation energyStation = new EnergyStation() { Position = new Position(i, j) };

                    if (Math.Pow(i - position.X, 2) + Math.Pow(j - position.Y, 2) <= radius) {
                        amountOfStationsInGivenRadius++;
                    }

                    allEnergyStations.Add(energyStation);
                }
            }
            return allEnergyStations;
        }

        [Fact]
        public void SortEnergyStationsByEnergyProfitMethodTest() {
            Position currentPosition = new Position(5, 5);
            int radius = 100;
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = currentPosition, Energy = radius };

            List<EnergyStation> energyStations = new List<EnergyStation>();
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(5, 3), RecoveryRate = 1 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(3, 4), RecoveryRate = 4 });
            energyStations.Add(new EnergyStation() { Energy = 1, Position = new Position(3, 5), RecoveryRate = 3 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(4, 7), RecoveryRate = 5 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(7, 7), RecoveryRate = 2 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(6, 4), RecoveryRate = 2 });

            IList<Robot.Common.Robot> robots = new List<Robot.Common.Robot>() { movingRobot };
            IList<EnergyStation> expectedSortedStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(energyStations, robots, movingRobot);

            foreach(EnergyStation s in expectedSortedStations) {
                Console.WriteLine("Station {0}, energy - {1}", s.RecoveryRate, s.Energy);
            }
        }

        [Fact]
        public void IsBotOnStationMethodTest() {
            Robot.Common.Robot myRobot = new Robot.Common.Robot() { Position = new Position(4, 4) };
            IList<EnergyStation> energyStations = new List<EnergyStation>();

            for(int i = 0;  i < 10; i++) {
                energyStations.Add(new EnergyStation() { Position = new Position(i + 1, i) });
            }

            bool isNotOnStation = energyStationsUtil.IsRobotOnTheStation(energyStations, myRobot);
            Assert.False(isNotOnStation);

            energyStations.Add(new EnergyStation() { Position = new Position(4, 4) });
            bool isOnStation = energyStationsUtil.IsRobotOnTheStation(energyStations, myRobot);
            Assert.True(isOnStation);
        }

        [Fact]
        public void IsStationFreeMethodTest() {
            EnergyStation energyStationToBeChecked = new EnergyStation() { Position = new Position(4, 4) };
            Robot.Common.Robot robot = new Robot.Common.Robot() { Position = new Position(3, 4) };
            List<Robot.Common.Robot> enemyRobots = new List<Robot.Common.Robot>();
            for(int i = 0; i < 10; ++i) {
                enemyRobots.Add(new Robot.Common.Robot() { Position = new Position(i, i+1)});
            }

            bool isFree = energyStationsUtil.IsStationFree(energyStationToBeChecked, robot, enemyRobots);
            Assert.True(isFree);

            robot.Position = new Position(4, 4);
            isFree = energyStationsUtil.IsStationFree(energyStationToBeChecked, robot, enemyRobots);
            Assert.True(isFree);

            enemyRobots.Add(new Robot.Common.Robot() { Position = robot.Position });
            bool isNotFree = energyStationsUtil.IsStationFree(energyStationToBeChecked, robot, enemyRobots);
            Assert.False(isNotFree);
        }

        [Fact]
        public void IsStationOccupiedByOwnRobotMethodTest() {
            EnergyStation energyStationToBeChecked = new EnergyStation() { Position = new Position(4, 4) };
            Robot.Common.Robot robot = new Robot.Common.Robot() { Position = new Position(3, 4), OwnerName="roma" };
            List<Robot.Common.Robot> enemyRobots = new List<Robot.Common.Robot>();
            for (int i = 0; i < 10; ++i) {
                enemyRobots.Add(new Robot.Common.Robot() { Position = new Position(i, i + 1), OwnerName = "123" + i });
            }

            bool isOccupied = energyStationsUtil.IsStationOccupiedByOwnRobot(energyStationToBeChecked, robot, enemyRobots);
            Assert.False(isOccupied);

            enemyRobots.Add(new Robot.Common.Robot() { Position = energyStationToBeChecked.Position, OwnerName = robot.OwnerName });
            isOccupied = energyStationsUtil.IsStationOccupiedByOwnRobot(energyStationToBeChecked, robot, enemyRobots);
            Assert.True(isOccupied);
        }

        [Fact]
        public void FilterStationsOccupiedByMyOwnRobotsMethodTest() {
            IList<EnergyStation> energyStations = new List<EnergyStation>();
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = new Position(3, 3), OwnerName = "roma" };
            IList<Robot.Common.Robot> robots = new List<Robot.Common.Robot>();

            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(5, 3), RecoveryRate = 1 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(3, 4), RecoveryRate = 4 });
            energyStations.Add(new EnergyStation() { Energy = 1, Position = new Position(3, 5), RecoveryRate = 3 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(4, 7), RecoveryRate = 5 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(7, 7), RecoveryRate = 2 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(6, 4), RecoveryRate = 2 });

            robots.Add(new Robot.Common.Robot() { Position =  new Position(0, 0) });

            IList<EnergyStation> filteredEnergyStations;

            filteredEnergyStations = energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(energyStations, movingRobot, robots);
            Assert.Equal(filteredEnergyStations.Count, energyStations.Count);

            robots.Add(new Robot.Common.Robot() { Position = energyStations[0].Position, OwnerName = "roma" });
            robots.Add(new Robot.Common.Robot() { Position = energyStations[1].Position, OwnerName = "roma" });
            filteredEnergyStations = energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(energyStations, movingRobot, robots);
            Assert.Equal(filteredEnergyStations.Count, energyStations.Count - 2);
        }
    }
}
