using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
//using FluentAssertions;
using NSubstitute;

public class SpinnerLogicTests
{
	ISpinnerControlAdapter spinnerControl;
    ISpinnerUnitAdapter spinnerUnit;
    SpinnerSimulation spinnerSimulation;
    SpinnerLogic spinnerLogic;

    [SetUp]
    public void SpinnerSetup()
    {
	    spinnerControl = Substitute.For<ISpinnerControlAdapter>();
        spinnerUnit = Substitute.For<ISpinnerUnitAdapter>();
        spinnerControl.Integration.Returns(new Integration());

        spinnerSimulation = new SpinnerSimulation(spinnerUnit);
        spinnerLogic = new SpinnerLogic(spinnerControl, spinnerSimulation);
    }
    // A Test behaves as an ordinary method
	[Test]
    [TestCase(true, -1, true)]
    [TestCase(true, 0, false)]
    [TestCase(false, 1, true)]
    public void SpinnerTurnsOnAndOff(bool On, int ForceOn, bool ExpectedOnStatus)
    {
        //Arrange
        spinnerControl.IsOn = On;
        spinnerControl.ForceOnStatus = ForceOn;

        //Act
        bool IsOn = spinnerLogic.OnStatus;

        //Assert
        Assert.AreEqual(IsOn, ExpectedOnStatus);
        //IsOn.Should().Be(ExpectedOnStatus);
    }

    [Test]
    [TestCase(true, -1, true)]
    [TestCase(true, 0, false)]
    [TestCase(false, 1, true)]
    public void SpinnerReverts(bool Reverse, int ForceReverse, bool ExpectedReverseStatus)
    {
        //Arrange
        spinnerControl.Reverse = Reverse;
        spinnerControl.ForceReverseStatus = ForceReverse;

        //Act
        bool IsReverse = spinnerLogic.ReverseStatus;

        //Assert
        Assert.AreEqual(IsReverse, ExpectedReverseStatus);
        //IsReverse.Should().Be(ExpectedReverseStatus);
    }

    [Test]
    [TestCase( 2f, true, 0f,  2f)]
    [TestCase(12f, true, 0f, 10f)]
    [TestCase(-1f, true, 0f,  0f)]
    [TestCase(2f, false, 5f,  5f)]
    public void SpinnerChangesSpeed(float InputSpeed, bool ReleaseSpeed, float ForcedSpeed, float ExpectedSpeed)
    {
        //Arrange
        spinnerControl.Speed = InputSpeed;
        spinnerControl.ForcedSpeedValue = ForcedSpeed;
        spinnerControl.ReleaseSpeedStatus = ReleaseSpeed;

        //Act
        float Speed = spinnerLogic.SpeedValue;

        //Assert
        Assert.AreEqual(Speed, ExpectedSpeed, 0.01);
        //Speed.Should().BeApproximately(ExpectedSpeed, 0.01f);
    }
}