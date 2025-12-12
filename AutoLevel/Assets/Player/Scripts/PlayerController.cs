using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;


public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float movespeed;
    public float rotatespeed;
    public float jumpforce;
        void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.P))
        {
            this.transform.Rotate(Vector3.up * rotatespeed * Time.deltaTime);

        }

        if (Input.GetKey(KeyCode.O))
        {
            this.transform.Rotate(Vector3.down * rotatespeed * Time.deltaTime);

        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpforce * Time.deltaTime, ForceMode.Impulse);

        }
  


       
    }

    void FixedUpdate()
    {
        float horiz = Input.GetAxis("Horizontal");
        float verti = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horiz, 0, verti);
        rb.AddRelativeForce(movement * movespeed);
    }

   

}
