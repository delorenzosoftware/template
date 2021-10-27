using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using UnityEngine.SceneManagement;
//using FluentAssertions;

public class SpinnerTest
{
    GameObject detector;
    ElementDestroy destroyer;
    SpinnerGroup spinnerGroup;

    [UnitySetUp]
    public IEnumerator SpinnerSetup()
    {
        SceneManager.LoadScene("SampleScene");
        yield return null;

        Time.timeScale = 25f;

        detector = Utils.CreatePrimitive(PrimitiveType.Cube);
        spinnerGroup = GameObject.Find("spinner_group").GetComponent<SpinnerGroup>();

        detector.GetComponent<Collider>().isTrigger = true;
        detector.transform.position = new Vector3(4.4f, 0f, 0f);
        detector.transform.parent = spinnerGroup.transform;
        destroyer = detector.AddComponent<ElementDestroy>();


    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator SpinnerGroupCarriesObject([ValueSource(nameof(Speeder))] float speed,
                                                 [ValueSource(nameof(Rotationer))] float rotation,
                                                 [ValueSource(nameof(Directioner))] bool direction)
    {

        spinnerGroup.ForceOnStatus = -1;
        spinnerGroup.ForceReverseStatus = -1;
        spinnerGroup.ReleaseSpeedStatus = true;
        spinnerGroup.IsOn = true;

        detector.transform.position = new Vector3(direction ? 4.4f : -4.4f, 0f, 0f);
        spinnerGroup.transform.rotation = Quaternion.Euler(0, rotation, 0);
        spinnerGroup.Reverse = direction;
        spinnerGroup.Speed = speed;
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        //yield return new WaitForSeconds(15f / speed);
        yield return new WaitForTestEnd(20f / speed);
        Assert.AreEqual(destroyer.NumDestroyed, 1);
        //destroyer.NumDestroyed.Should().Be(1);

    }

    [UnityTearDown]
    public IEnumerator SpinnerTeardown()
    {
        GameObject.Destroy(detector);
        yield return null;
    }

    public static float[] Speeder = { 5, 10 };
    public static float[] Rotationer = { 0, 90, 180, 270 };
    public static bool[] Directioner = { true, false };
}

public class WaitForTestEnd : CustomYieldInstruction
{
    public override bool keepWaiting => !(TestIsOver || (Time.time - _startTime) > _timeout);

    public static bool TestIsOver; // Change to non-static if feasible

    private float _timeout;

    private float _startTime;

    public WaitForTestEnd(float timeout)
    {
        _timeout = timeout;
        _startTime = Time.time;
        TestIsOver = false;
    }
}