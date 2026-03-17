using UnityEngine;
using UnityEngine.UI;
public class MyJump : MonoBehaviour
{
    public Rigidbody rb;
    public float power = 200f;
    public Text TextUi;
    public float timer = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        TextUi.text = timer.ToString();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            power += Random.Range(-100, 200);
            rb.AddForce(transform.up * power);
        }
        if(this.gameObject.transform.position.y > 5 || this.gameObject.transform.position.y < -3)
        {
            Destroy(this.gameObject);
        }
    }
}
