using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NikitinRoman.RobotChallenge;
using Robot.Common;
using rom4ik.RobotChallange;

namespace RomanNikitin.RobotChallange.Test {
    public class Rom4ikAlgorithmTests {
        [Fact]
        public void DoStepMethodTest() {
            
            Robot.Common.Robot movingRobot = new Robot.Common.Robot() { 
                OwnerName = "roma",
                Energy = 100,
                Position = new Position(5, 5) 
            };

            List<EnergyStation> energyStations = new List<EnergyStation>();
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(5, 3), RecoveryRate = 1 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(3, 4), RecoveryRate = 4 });
            energyStations.Add(new EnergyStation() { Energy = 1, Position = new Position(3, 5), RecoveryRate = 3 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(4, 7), RecoveryRate = 5 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(7, 7), RecoveryRate = 2 });
            energyStations.Add(new EnergyStation() { Energy = 0, Position = new Position(6, 4), RecoveryRate = 2 });

            IList<Robot.Common.Robot> robots = new List<Robot.Common.Robot>();
            robots.Add(movingRobot);
            
            int robotToMoveIndex = robots.IndexOf(movingRobot);

            Map map = new Map() { Stations = energyStations};

            Rom4ikAlgorithm rom4IkAlgorithm = new Rom4ikAlgorithm();

            RobotCommand robotCommand = rom4IkAlgorithm.DoStep(robots, robotToMoveIndex, map);
            Assert.True(robotCommand is MoveCommand);
            MoveCommand moveCommand = robotCommand as MoveCommand;
            Assert.Equal(moveCommand.NewPosition, new Position(6, 4));

            robots.Add(new Robot.Common.Robot() { OwnerName = "roma", Position = new Position(6, 4) });
            robotCommand = rom4IkAlgorithm.DoStep(robots, robotToMoveIndex, map);
            Assert.True(robotCommand is MoveCommand);
            moveCommand = robotCommand as MoveCommand;
            Assert.Equal(new Position(3, 5), moveCommand.NewPosition);

            movingRobot.Position = energyStations[1].Position;
            robotCommand = rom4IkAlgorithm.DoStep(robots, robotToMoveIndex, map);
            Assert.True(robotCommand is CollectEnergyCommand);

        }
    }
}
