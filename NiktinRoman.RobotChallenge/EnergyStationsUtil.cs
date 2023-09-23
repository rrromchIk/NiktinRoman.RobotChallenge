using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NikitinRoman.RobotChallenge {
    public class EnergyStationsUtil {
        private DistanceHelper distanceHelper;
        private MapUtil mapUtil;

        public EnergyStationsUtil(DistanceHelper distanceHelper, MapUtil mapUtil) {
            this.distanceHelper = distanceHelper;
            this.mapUtil = mapUtil;
        }
        
        public List<EnergyStation> FindAllReachableStationsInGivenRadius(Map map, int radius,
                        Robot.Common.Robot movingRobot, List<Robot.Common.Robot> robots) {

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

            return energyStationsInGivenRadius.ToList();
        }


        //public List<EnergyStations> FilterStationsOccupiedByMyOwnRobots() {

        //}

        public List<EnergyStation> SortEnergyStationsByEnergyProfit(List<EnergyStation> energyStations,
                                                                     List<Robot.Common.Robot> robots,
                                                                     Robot.Common.Robot movingRobot,
                                                                    Position currentPosition) {

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

        public Position FindNearestFreeStation(Robot.Common.Robot movingRobot, Map map,
                                                    List<Robot.Common.Robot> robots) {
            EnergyStation nearest = null;
            int minDistance = int.MaxValue;
            foreach (var station in map.Stations) {
                if (IsStationFree(station, movingRobot, robots)) {
                    int d = distanceHelper.FindDistance(station.Position, movingRobot.Position);
                    if (d < minDistance) {
                        minDistance = d;
                        nearest = station;
                    }
                }
            }
            return nearest == null ? null : nearest.Position;
        }

        public bool IsStationOccupiedByOwnRobot(EnergyStation energyStation, Robot.Common.Robot movingRobot,
            List<Robot.Common.Robot> allRobots) {

            foreach(Robot.Common.Robot robot in allRobots) {
                if(movingRobot != robot 
                    && robot.Position == energyStation.Position 
                    && robot.OwnerName.Equals(movingRobot.OwnerName)) {
                    return true;
                }
            }

            return false;
         }

        public bool IsRobotOnTheStation(List<EnergyStation> stations, Robot.Common.Robot movingRobot) {
            foreach (EnergyStation energyStation in stations) {
                if (energyStation.Position == movingRobot.Position)
                    return true;
            }

            return false;
        }

        public bool IsStationFree(EnergyStation station, Robot.Common.Robot movingRobot, List<Robot.Common.Robot> robots) {
            return mapUtil.IsCellFree(station.Position, movingRobot, robots);
        }
    }
}
