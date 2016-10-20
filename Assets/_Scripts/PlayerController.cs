using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
    public LayerMask drivableSurfaces;
    public float distanceToGround;
    public float torqueAir = 3;
    public float torqueGround = 100;
    public float driveAcceleratoin = 10;
    public float minSpeed = -10;
    public float maxSpeed = 100;
    Rigidbody body;
    bool isGrounded = false;

    float tireSpinAmount = 0;
	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody>();
	}

    // Update is called once per physicsframe
    void FixedUpdate() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float a = Input.GetAxis("Accelerate");
        bool j = Input.GetButton("Jump");
        bool b = Input.GetButton("HandBrake");

        CheckGround();

        if (isGrounded) {//if the player is on the ground
            

            //body.AddRelativeForce(new Vector3(0, 0, v) * driveAcceleratoin);
            tireSpinAmount += v * driveAcceleratoin * Time.fixedDeltaTime;
            tireSpinAmount = Mathf.Clamp(tireSpinAmount, minSpeed, maxSpeed);

            float speedPercent = tireSpinAmount / maxSpeed;

            if(v == 0) {
                if(tireSpinAmount > 0) {
                    tireSpinAmount -= driveAcceleratoin * Time.fixedDeltaTime*.1f;
                    if (tireSpinAmount < 0) {
                        tireSpinAmount = 0;
                    }
                }
                if (tireSpinAmount < 0) {
                    tireSpinAmount += driveAcceleratoin * Time.fixedDeltaTime*.1f;
                    if(tireSpinAmount > 0) {
                        tireSpinAmount = 0;
                    }
                }
            }
            body.velocity = tireSpinAmount * transform.forward;

            Quaternion newQ = Quaternion.AngleAxis(h * torqueGround * Time.deltaTime*speedPercent, transform.up) * transform.rotation;
            body.MoveRotation(newQ);
        }
        else {//if the player is in the air
            Vector3 torque = new Vector3();

            //calc torque here
            torque.x = v * torqueAir;
            if (b) {
                torque.z = h * torqueAir;
                //torque.y = 0;
            }else {
                torque.y = h * torqueAir;
               // torque.z = 0;
            }

            body.AddRelativeTorque(torque);
        }
	}//end FixedUpdate()

    void CheckGround() {
        Ray ray = new Ray(transform.position,transform.up*-1);
        RaycastHit hit;
        //Debug.DrawRay(ray.origin, ray.direction * distanceToGround);
        if (Physics.Raycast(ray, out hit, distanceToGround, drivableSurfaces)) {
            isGrounded = true;
        }
        else isGrounded = false;
    }
}
