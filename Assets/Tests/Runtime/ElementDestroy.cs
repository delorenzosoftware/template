using UnityEngine;

public class ElementDestroy : MonoBehaviour
{
	public int NumDestroyed;

    private void OnTriggerEnter(Collider other)
    {
        NumDestroyed++;
	    GameObject.Destroy(other.gameObject);
        WaitForTestEnd.TestIsOver = true;
    }
}
