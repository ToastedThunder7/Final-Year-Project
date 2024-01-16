using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CarController : MonoBehaviour
{
    // component declaration
    private Rigidbody rb;
    public WheelColliders colliders;
    public WheelMeshes wheelMeshes;
    
    // Input declaration
    public float throttleInput;
    public float brakeInput;
    public float clutchInput;
    public float steeringInput;

    // declaring engine and steering var
    public float BHP;
    public float brakePower;
    [SerializeField]
    private float speed;
    public AnimationCurve steeringCurve;
    public AnimationCurve BHPToRPMCurve;

    // declaring rpm floats
    public float currentRPM;
    public float maxRPM;
    public float idleRPM;
    private float wheelRPM;

    // declaring gears and ratios
    public int currentGear;
    public float[] gearRatio;
    public float differentialRatio;


    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // updates speed
        speed = rb.velocity.magnitude;
        CheckPlayerInputs();
        ApplyEnginePower();
        ApplySteering();
        ApplyBrakePower();
        UpdateWheelPos();
    }

    // declares inputs and checks them
    void CheckPlayerInputs()
    {
        throttleInput = Input.GetAxis("Vertical");
        steeringInput = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.LeftShift) == true)
        {
            clutchInput = 0;
        }
        else
        {
            clutchInput = Mathf.Lerp(clutchInput, 1, Time.deltaTime);
        }

        // Applies brakes if gas is not applied
        if (throttleInput < 0)
        {
            brakeInput = Mathf.Abs(throttleInput);
            throttleInput = 0;
        }
        else
        {
            brakeInput = 0;
        }
    }


    void ApplyEnginePower()
    {
        //calculates torque from engine
        float currentTorque = 0;
        //checks if clutch is engaged
        if (clutchInput < 0.1f)
        {
            //updates current rpm when clutch engaged
            currentRPM = Mathf.Lerp(currentRPM, Mathf.Max(idleRPM,maxRPM*throttleInput)+Random.Range(-50,50), Time.deltaTime);
        }
        else
        {
            // calculates average wheel rpm
            wheelRPM = Mathf.Abs((colliders.RRWheel.rpm + colliders.RLWheel.rpm) / 2) * gearRatio[currentGear] * differentialRatio;
            // calculates engine braking deceleration
            wheelRPM = Mathf.Lerp(wheelRPM,Mathf.Max(idleRPM-100,wheelRPM),Time.deltaTime * 3f);
            //updates current rpm
            currentRPM = wheelRPM;
            //converts engine power to torque, multiplied by 5252 to convert to Nm 
            currentTorque = (BHPToRPMCurve.Evaluate(currentRPM / maxRPM) * BHP / currentRPM) * gearRatio[currentGear] * differentialRatio * 5252f * clutchInput;
        }

        //applies torque to wheels
        colliders.RRWheel.motorTorque = currentTorque * throttleInput;
        colliders.RLWheel.motorTorque = currentTorque * throttleInput;

    }

    void ApplyBrakePower()
    {
        // applies brakes to wheels
        colliders.FRWheel.brakeTorque = brakeInput * brakePower;
        colliders.FLWheel.brakeTorque = brakeInput * brakePower;
        colliders.RRWheel.brakeTorque = brakeInput * brakePower;
        colliders.RLWheel.brakeTorque = brakeInput * brakePower;
    }

    void ApplySteering()
    {
        //calculates steering angle based on speed from curve
        float steeringAngle = steeringInput * steeringCurve.Evaluate(speed*10);
        //applies steering angle
        colliders.FRWheel.steerAngle = steeringAngle;
        colliders.FLWheel.steerAngle = steeringAngle;
    }

    //Wheel positioning and rotation
    void UpdateWheelPos()
    {
        // updates wheel meshes to colliders 
        UpdateWheel(colliders.FRWheel, wheelMeshes.FRWheel);
        UpdateWheel(colliders.FLWheel, wheelMeshes.FLWheel);
        UpdateWheel(colliders.RRWheel, wheelMeshes.RRWheel);
        UpdateWheel(colliders.RLWheel, wheelMeshes.RLWheel);
    }
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        // declares local variables
        Quaternion quat;
        Vector3 position;
        //gets position and rotation of object
        coll.GetWorldPose(out position, out quat);
        //set wheel position and rotation
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
}

[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}
