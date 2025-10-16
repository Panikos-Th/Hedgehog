using System.Collections;
using UnityEngine;

public class CoolDownTest : MonoBehaviour
{
    public bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        Test();
    }



    private void Test()
    {
        if (isGrounded == false)
        {
            Debug.Log("Cannot perform action while not grounded.");
        }
        else
        {
            StartCoroutine(TestCoroutine());
           Debug.Log("Action performed.");
        }
    }
    
    IEnumerator TestCoroutine()
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSeconds(2f);
        Debug.Log("Coroutine ended after 2 seconds");
    }
}
