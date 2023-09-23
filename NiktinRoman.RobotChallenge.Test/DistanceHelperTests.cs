using System;
using Robot.Common;
using NikitinRoman.RobotChallenge;

namespace RomanNikitin.RobotChallange.Test {
    public class DistanceHelperTests {
        private DistanceHelper distanceHelper;

        public DistanceHelperTests() {
            this.distanceHelper = new DistanceHelper();
        }

        [Fact]
        public void FindDispanceMethodTest() {
            var p1 = new Position(5, 5);
            var p2 = new Position(5, 1);
            var p3 = new Position(1, 5);
            var p4 = new Position(9, 5);
            var p5 = new Position(5, 9);
            var p6 = new Position(7, 2);
            var p7 = new Position(8, 3);
            var p8 = new Position(2, 7);
            var p9 = new Position(3, 8);

            int expected = 16;
            
            var actual = distanceHelper.FindDistance(p1, p2);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p3);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p4);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p5);
            Assert.Equal(expected, actual);

            expected = 13;

            actual = distanceHelper.FindDistance(p1, p6);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p7);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p8);
            Assert.Equal(expected, actual);

            actual = distanceHelper.FindDistance(p1, p9);
            Assert.Equal(expected, actual);
        }
    }
}
