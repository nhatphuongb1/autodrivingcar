using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDrivingCarSimulation.UnitTests
{
    [TestFixture]
    public class CarProgramTest
    {
        private CarData carData;
        [SetUp]
        public void SetUp()
        {
            carData = new CarData("Test", new CurrentPosition(new Position(0, 0), Direction.N), "F");
        }
        [Test]
        public void TurnLeft_Should_Work()
        {
            var newPosition = CarProgram.TurnLeft(new CurrentPosition(new Position(0, 0), Direction.N));

        }
    }
}
