using NikitinRoman.RobotChallenge;
using NiktinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices;

namespace rom4ik.RobotChallange {
    public class Rom4ikAlgorithm : IRobotAlgorithm {
        public static int RoundNumber = 0;
        private static int CreatedRobots = 0;
        private EnergyStationsUtil energyStationsUtil;
        private MapUtil mapUtil;
        private DistanceHelper distanceHelper;
        private RobotsUtil robotsUtil;

        
        public string Author {
            get { return "Roman Nikitin"; }
        }

        public Rom4ikAlgorithm() {
            //Calculating round number
            Logger.OnLogRound += (object sender, LogRoundEventArgs e) => { Rom4ikAlgorithm.RoundNumber += 1; };

            mapUtil = new MapUtil();
            distanceHelper = new DistanceHelper();
            energyStationsUtil = new EnergyStationsUtil(distanceHelper, mapUtil, 50);
            robotsUtil = new RobotsUtil(energyStationsUtil, 60, 40, 700);
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map) {
            var myRobot = robots[robotToMoveIndex];
            Position topOneEnergyStationPosition = null;

            if (energyStationsUtil.IsRobotOnTheStation(map.Stations, myRobot)) {
                //If robot is on the station

                //If new robot can reach energy station with maximum amount of possible(profit) energy 
                if(robotsUtil.CheckConditionsToCreateNewRobot(map, robots, myRobot, CreatedRobots, RoundNumber)) {
                    CreatedRobots++;
                    return new CreateNewRobotCommand();
                }

                //By default just collecting
                return new CollectEnergyCommand();
            } else {
                //If robot is not on the station
                //Searching all reachable stations in given radius 
                int radius = myRobot.Energy < 300 ? 100 : 300;
                IList<EnergyStation> stationsInRadius = energyStationsUtil.FindAllReachableStationsInGivenRadius(
                                                                                map,
                                                                                radius,
                                                                                myRobot,
                                                                                robots);

                //Sorting by energy profit
                IList<EnergyStation> topStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(
                                                                                stationsInRadius,
                                                                                robots,
                                                                                myRobot);

                //Filter to not kick our robot
                IList<EnergyStation> filteredStations = energyStationsUtil.FilterStationsOccupiedByMyOwnRobots(
                                                                                topStations,
                                                                                myRobot,
                                                                                robots);
                
                if (filteredStations.Count == 0) {
                    //Console.WriteLine("No stations in reachable radius");
                    //return new MoveCommand() {
                    //    NewPosition = new Position(myRobot.Position.X + 1, myRobot.Position.Y + 1)
                    //};

                    //хуйово буде
                }

                topOneEnergyStationPosition = filteredStations[0].Position;
            }

            return new MoveCommand() { NewPosition = topOneEnergyStationPosition }; 
        }
    }
}
