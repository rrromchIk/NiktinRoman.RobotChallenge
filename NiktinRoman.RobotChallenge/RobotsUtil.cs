using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NiktinRoman.RobotChallenge {
    public class RobotsUtil {
        private EnergyStationsUtil energyStationsUtil;
        private readonly int MAX_AMOUNT_OF_CREATED_ROBOTS;
        private readonly int MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS;
        private readonly int MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT;

        public RobotsUtil(EnergyStationsUtil energyStationsUtil, int maxAmountOfCreatedRobots,
                                        int maxRoundWHenCreatingRobots, int minAmountOfEnergyToCreateTobot) {
            MAX_AMOUNT_OF_CREATED_ROBOTS = maxAmountOfCreatedRobots;
            MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS = maxAmountOfCreatedRobots;
            MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT = minAmountOfEnergyToCreateTobot;
            this.energyStationsUtil = energyStationsUtil;
        }

        public bool CheckConditionsToCreateNewRobot(Map map,
                                                    IList<Robot.Common.Robot> robots,
                                                    Robot.Common.Robot movingRobot,
                                                    int createdRobots,
                                                    int roundNumber) {
            int robotEnergy = movingRobot.Energy;
            int radius = 80; //default

            if (robotEnergy > MIN_AMOUNT_OF_ENERGY_TO_CREATE_ROBOT && 
                createdRobots < MAX_AMOUNT_OF_CREATED_ROBOTS && 
                roundNumber <= MAX_ROUND_NUMBER_WHEN_CREATING_ROBOTS) {

                IList<EnergyStation> energyStations = 
                    energyStationsUtil.FindAllReachableStationsInGivenRadius(map, radius, movingRobot, robots);

                IList<EnergyStation> sortedStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(energyStations, robots, movingRobot);

                IList<EnergyStation> notOccupied = 
                    energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(sortedStations, movingRobot, robots);

                if(notOccupied.Count > 0) {
                    return true;
                }
            }

            return false;
        }
    }
}
