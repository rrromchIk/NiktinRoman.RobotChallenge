using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NiktinRoman.RobotChallenge {
    public class RobotsUtil {
        private EnergyStationsUtil energyStationsUtil;
        private DistanceHelper distanceHelper;
        private readonly int MAX_AMOUNT_OF_CREATED_ROBOTS;
        private readonly int MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS;
        private readonly int MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT;

        public RobotsUtil(EnergyStationsUtil energyStationsUtil, DistanceHelper distanceHelper, int maxAmountOfCreatedRobots,
                                        int maxRoundWhenCreatingRobots, int minAmountOfEnergyToCreateRobot) {
            MAX_AMOUNT_OF_CREATED_ROBOTS = maxAmountOfCreatedRobots;
            MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS = maxRoundWhenCreatingRobots;
            MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT = minAmountOfEnergyToCreateRobot;
            this.energyStationsUtil = energyStationsUtil;
            this.distanceHelper = distanceHelper; 
        }

        public bool CheckConditionsToCreateNewRobot(Map map,
                                                    IList<Robot.Common.Robot> robots,
                                                    Robot.Common.Robot movingRobot,
                                                    int createdRobots,
                                                    int roundNumber) {
            int radius = 50; //default

            if (movingRobot.Energy >= MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT && 
                createdRobots < MAX_AMOUNT_OF_CREATED_ROBOTS && 
                roundNumber <= MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS) {

                Robot.Common.Robot robotToMove = new Robot.Common.Robot() { Position = movingRobot.Position, Energy = 100 };
                IList<EnergyStation> energyStations = 
                    energyStationsUtil.FindAllReachableStationsInGivenRadius(map, radius, robotToMove, robots);

                IList<EnergyStation> sortedStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(energyStations, robots, movingRobot);

                IList<EnergyStation> notOccupied = 
                    energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(sortedStations, movingRobot, robots);

                if(notOccupied.Count >= 2) {
                    return true;
                }
            }

            return false;
        }

        public Robot.Common.Robot RobotToAttackInReachableRadius(Robot.Common.Robot movingRobot,
            IList<EnergyStation> topStationsToGo, IList<Robot.Common.Robot> robots) {

            Robot.Common.Robot robotToAttack = null;
            int maxProfit = int.MinValue;

            foreach (Robot.Common.Robot robot in robots) {
                int profitFromAttackToRobot;

                int distance = distanceHelper.FindDistance(movingRobot.Position, robot.Position);

                profitFromAttackToRobot = (int)(robot.Energy * 0.05) - distance - 50;

                if (profitFromAttackToRobot > maxProfit) {
                    maxProfit = profitFromAttackToRobot;
                    robotToAttack = robot;
                }
            }

            foreach (EnergyStation station in topStationsToGo) {
                int distanceToStation = distanceHelper.FindDistance(movingRobot.Position, station.Position);
                int profitFromCollectingEnergy = Math.Min(station.Energy, 200) - distanceToStation;

                if (profitFromCollectingEnergy > maxProfit) {
                    maxProfit = profitFromCollectingEnergy;
                    robotToAttack = null; // Going to the station is more profitable, so don't attack any robot.
                }
            }

            if(robotToAttack != null && maxProfit <= 0) {
                return null;
            }

            return robotToAttack;
        }
    }
}
