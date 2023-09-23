using NikitinRoman.RobotChallenge;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;

namespace rom4ik.RobotChallange {
    public class Rom4ikAlgorithm : IRobotAlgorithm {
        private int _roundNumber = 0;
        private EnergyStationsUtil energyStationsUtil;
        private MapUtil mapUtil;
        private DistanceHelper distanceHelper;

        public string Author {
            get { return "Roman Nikitin"; }
        }

        public Rom4ikAlgorithm() {
            Logger.OnLogRound += (object sender, LogRoundEventArgs e) => { _roundNumber += 1; };
            mapUtil = new MapUtil();
            distanceHelper = new DistanceHelper();
            energyStationsUtil = new EnergyStationsUtil(distanceHelper, mapUtil);
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map) {
            var myRobot = robots[robotToMoveIndex];
            Position topOneEnergyStationPosition = null;

            if (energyStationsUtil.IsRobotOnTheStation(map.Stations.ToList(), myRobot)) {
                Console.WriteLine("Robot is already on station");
                return new CollectEnergyCommand();
            } else {
                Console.WriteLine("Robot is not on station");

                List<EnergyStation> stationsInRadius = energyStationsUtil.FindAllReachableStationsInGivenRadius(
                                                                                map,
                                                                                myRobot.Energy,
                                                                                myRobot,
                                                                                robots.ToList());
                List<EnergyStation> topStations = energyStationsUtil.SortEnergyStationsByEnergyProfit(
                                                                                stationsInRadius,
                                                                                robots.ToList(),
                                                                                myRobot,
                                                                                myRobot.Position);

                if (topStations.Count == 0) {
                    Console.WriteLine("No stations in reachable radius");
                    return new MoveCommand() {
                        NewPosition = new Position(myRobot.Position.X + 1, myRobot.Position.Y + 1)
                    };
                }


                foreach (EnergyStation station in topStations) {
                    if (!energyStationsUtil.IsStationOccupiedByOwnRobot(station, myRobot, robots.ToList())) {
                        topOneEnergyStationPosition = station.Position;
                        break;
                    }
                }
            }

            return new MoveCommand() { NewPosition = topOneEnergyStationPosition }; 
        }
    }
}
