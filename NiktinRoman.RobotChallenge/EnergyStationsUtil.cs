using Robot.Common;
using System.Collections.Generic;
using System.Linq;


namespace NikitinRoman.RobotChallenge {
    public class EnergyStationsUtil {
        private DistanceHelper distanceHelper;
        private MapUtil mapUtil;
        public readonly int AMOUNT_OF_ENERGY_TO_KICK_OUT_ENEMY_ROBOT;

        public EnergyStationsUtil(DistanceHelper distanceHelper, MapUtil mapUtil, int amountOfEnergyToKickOutEnemyRobot) {
            this.distanceHelper = distanceHelper;
            this.mapUtil = mapUtil;
            this.AMOUNT_OF_ENERGY_TO_KICK_OUT_ENEMY_ROBOT = amountOfEnergyToKickOutEnemyRobot;
        }
        
        public IList<EnergyStation> FindAllReachableStationsInGivenRadius(Map map, int radius,
                        Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots) {

            Position currentPosition = movingRobot.Position;
            IList<EnergyStation> allEnergyStations = map.Stations;
            IList<EnergyStation> energyStationsInGivenRadius = new List<EnergyStation>();

            foreach(EnergyStation energyStation in allEnergyStations) {
                Position positionOfStation = energyStation.Position;

                int distance = distanceHelper.FindDistance(currentPosition, positionOfStation);

                int robotEnergy = movingRobot.Energy;
                if(!IsStationFree(energyStation, movingRobot, robots)) {
                    robotEnergy -= 50;
                }

                if(distance <= radius && distance <= robotEnergy) {
                    energyStationsInGivenRadius.Add(energyStation);
                }
            }

            return energyStationsInGivenRadius;
        }


        public IList<EnergyStation> FilterStationsOccupiedByMyOwnRobots(IList<EnergyStation> energyStations,
                                                                        Robot.Common.Robot movingRobot,
                                                                        IList<Robot.Common.Robot> robots) {
            return energyStations
                .Select(s => s)
                .Where(s => !IsStationOccupiedByOwnRobot(s, movingRobot, robots))
                .ToList();
        }

        public IList<EnergyStation> SortEnergyStationsByEnergyProfit(IList<EnergyStation> energyStations,
                                                                     IList<Robot.Common.Robot> robots,
                                                                     Robot.Common.Robot movingRobot) {
            Position currentPosition = movingRobot.Position;
            var energyStationProfit = energyStations.Select(energyStation => {
                int amountOfEnergyOnStation = energyStation.Energy;
                int distanceToStation = distanceHelper.FindDistance(currentPosition, energyStation.Position);

                int energyToKickOutEnemyRobot = 0;
                if (!IsStationFree(energyStation, movingRobot, robots)) {
                    energyToKickOutEnemyRobot = 50;
                }

                int profit = amountOfEnergyOnStation - distanceToStation - energyToKickOutEnemyRobot;

                return (Profit: profit, Station: energyStation);
            })
                //.Where(profitAndStation => profitAndStation.Profit >= 0)
                .ToLookup(profitAndStation => profitAndStation.Profit, profitAndStation => profitAndStation.Station);

            return energyStationProfit
                .OrderByDescending(kv => kv.Key)
                .SelectMany(kv => kv)
                .ToList();
        }

        public bool IsStationOccupiedByOwnRobot(EnergyStation energyStation, Robot.Common.Robot movingRobot,
            IList<Robot.Common.Robot> allRobots) {

            foreach(Robot.Common.Robot robot in allRobots) {
                if(movingRobot != robot 
                    && robot.Position == energyStation.Position 
                    && robot.OwnerName.Equals(movingRobot.OwnerName)) {
                    return true;
                }
            }

            return false;
         }

        public bool IsRobotOnTheStation(IList<EnergyStation> stations, Robot.Common.Robot movingRobot) {
            foreach (EnergyStation energyStation in stations) {
                if (energyStation.Position == movingRobot.Position)
                    return true;
            }

            return false;
        }

        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot, IList<Robot.Common.Robot> robots) {
            return mapUtil.IsCellFree(station.Position, movingRobot, robots);
        }
    }
}
