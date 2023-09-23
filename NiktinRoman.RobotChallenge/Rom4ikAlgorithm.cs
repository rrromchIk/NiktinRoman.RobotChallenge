using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace rom4ik.RobotChallange {
    public class Rom4ikAlgorithm : IRobotAlgorithm {
        public static int RoundNumber = 0;
        private EnergyStationsUtil energyStationsUtil;
        private MapUtil mapUtil;
        private DistanceHelper distanceHelper;

        public string Author {
            get { return "Roman Nikitin"; }
        }

        public Rom4ikAlgorithm() {
            Logger.OnLogRound += (object sender, LogRoundEventArgs e) => { Rom4ikAlgorithm.RoundNumber += 1; };
            mapUtil = new MapUtil();
            distanceHelper = new DistanceHelper();
            energyStationsUtil = new EnergyStationsUtil(distanceHelper, mapUtil);
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map) {
            var myRobot = robots[robotToMoveIndex];
            Position topOneEnergyStationPosition = null;

            if (energyStationsUtil.IsRobotOnTheStation(map.Stations, myRobot)) {
                //If robot is on station


                //By default just collecting
                return new CollectEnergyCommand();
            } else {
                //If robot is not on station
                //Searching all reachable stations
                IList<EnergyStation> stationsInRadius = energyStationsUtil.FindAllReachableStationsInGivenRadius(
                                                                                map,
                                                                                myRobot.Energy,
                                                                                myRobot,
                                                                                robots);

                //Sorting by energy profit
                IList<EnergyStation> topStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(
                                                                                stationsInRadius,
                                                                                robots,
                                                                                myRobot,
                                                                                myRobot.Position);

                //Filter to not kick our robot
                IList<EnergyStation> filteredStations = energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(topStations, myRobot, robots);
                
                if (filteredStations.Count == 0) {
                    Console.WriteLine("No stations in reachable radius");
                    return new MoveCommand() {
                        NewPosition = new Position(myRobot.Position.X + 1, myRobot.Position.Y + 1)
                    };
                }

             
                topOneEnergyStationPosition = filteredStations[0].Position;
            }

            return new MoveCommand() { NewPosition = topOneEnergyStationPosition }; 
        }
    }
}
