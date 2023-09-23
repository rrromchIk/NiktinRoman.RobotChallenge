using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RomanNikitin.RobotChallange.Test {
    public class EnergyStationsUtilTests {
        private EnergyStationsUtil energyStationsUtil;

        public EnergyStationsUtilTests() {
            this.energyStationsUtil = new EnergyStationsUtil(new DistanceHelper(), new MapUtil());
        }

        [Fact]
        public void FindAllStationsInGivenRadiusMethodTest() {
            int radius = 9;
            int amountOfStationsInGivenRadius;
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = new Position(4, 4), Energy = radius };
            List<EnergyStation> allEnergyStations = PrepareEnergyStationsForPositionAndRadius(movingRobot.Position, radius, out amountOfStationsInGivenRadius);

            Map map = new Map() { Stations = allEnergyStations };
            List<Robot.Common.Robot> robots = new List<Robot.Common.Robot>() { new Robot.Common.Robot() };

            List<EnergyStation> expectedEnergyStations =
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

        private List<EnergyStation> PrepareEnergyStationsForPositionAndRadius(Position position, int radius, out int amountOfStationsInGivenRadius) { 
            List<EnergyStation> allEnergyStations = new List<EnergyStation>();

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
            int radius = 16;
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { Position = currentPosition, Energy = radius };

            List<EnergyStation> energyStations = new List<EnergyStation>();
            energyStations.Add(new EnergyStation() { Energy = 20, Position = new Position(7, 5), RecoveryRate = 1 });
            energyStations.Add(new EnergyStation() { Energy = 20, Position = new Position(5, 3), RecoveryRate = 4 });
            energyStations.Add(new EnergyStation() { Energy = 30, Position = new Position(3, 4), RecoveryRate = 3 });
            energyStations.Add(new EnergyStation() { Energy = 24, Position = new Position(6, 7), RecoveryRate = 5 });
            energyStations.Add(new EnergyStation() { Energy = 40, Position = new Position(4, 7), RecoveryRate = 2 });
            
            List<Robot.Common.Robot> robots = new List<Robot.Common.Robot>() { movingRobot };
            List<EnergyStation> expectedSortedStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(energyStations, robots, movingRobot, currentPosition);

            expectedSortedStations.ForEach(s => { Console.WriteLine("Station {0}, energy - {1}", s.RecoveryRate, s.Energy); });
        }

        [Fact]
        public void IsBotOnStationMethodTest() {
            Robot.Common.Robot myRobot = new Robot.Common.Robot() { Position = new Position(4, 4) };
            List<EnergyStation> energyStations = new List<EnergyStation>();

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
    }
}
