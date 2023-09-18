using UnityEngine;

public class PlaneController : MonoBehaviour
{
    // Plane object
    Rigidbody rb;
    float planeMass;
    public Transform propeller;

    Vector3 planeStart;
    Vector3 planeOrientation
    {
        get
        {
            return new Vector3(
                Vector3.Angle(Vector3.forward, transform.forward),
                Vector3.Angle(Vector3.up, transform.up),
                Vector3.Angle(Vector3.right, transform.right)
            ) / 180;
        }
    }

    // Config
    [SerializeField] float maxThrust = 200f;
    [SerializeField] float lift = 135f;
    [SerializeField] float responsiveness = 20f;

    // Motion
    float throttle = 100f;
    float roll = 0f;
    float pitch = 0f;
    float yaw = 0f;
    [SerializeField] float smoothStepScaling = 2f;

    private float responseModifier
    {
        get
        {
            return (planeMass / 10f) * responsiveness;
        }
    }

    // Neuroevolution
    [HideInInspector] public NeuralNetwork nn;
    int inputNodes;
    [HideInInspector] public bool isAlive = true;
    [HideInInspector] public bool isStopped = false;
    float cumulativeDistance = 0f;

    // Target
    Transform target;
    Vector3 startDifFromTarget;
    Vector3 currentDifFromTarget { get { return target.position - transform.position; } }
    float distanceToTarget { get { return Vector3.Magnitude(currentDifFromTarget); } }

    private void Awake()
    {
        target = GameObject.FindWithTag("Target").transform;
        rb = GetComponent<Rigidbody>();
        planeMass = rb.mass;
        planeStart = transform.position;
        startDifFromTarget = currentDifFromTarget;

        // Configuring neural network
        nn = GetComponent<NeuralNetwork>();
        inputNodes = nn.layerNodes[0];
    }

    float[] input
    {
        get
        {
            float[] _input = new float[inputNodes];
            _input[0] = currentDifFromTarget.x / startDifFromTarget.x / 2;
            _input[1] = currentDifFromTarget.y / startDifFromTarget.y / 2;
            _input[2] = currentDifFromTarget.z / startDifFromTarget.z / 2;
            _input[3] = planeOrientation.x;
            _input[4] = planeOrientation.y;
            _input[5] = planeOrientation.z;
            return _input;
        }
    }

    float[] output
    {
        get
        {
            return nn.feedforward(input);

        }
    }

    private void Move()
    {
        if (isStopped) return;

        throttle = Mathf.SmoothStep(throttle, output[0] * 100, Time.deltaTime / smoothStepScaling);
        throttle = Mathf.Clamp(throttle, 0f, 100f);
        roll = Mathf.SmoothStep(roll, (output[1] * 2) - 1, Time.deltaTime / smoothStepScaling);
        pitch = Mathf.SmoothStep(pitch, (output[2] * 2) - 1, Time.deltaTime / smoothStepScaling);
        yaw = Mathf.SmoothStep(yaw, (output[3] * 2) - 1, Time.deltaTime / smoothStepScaling);

        if (throttle < 5f)
        {
            StopEngine();
            return;
        }
    }

    // Keyboard controls
    private void HandleInputs()
    {
        float throttleIncrement = 0.4f;

        roll = Input.GetAxis("Roll");
        pitch = Input.GetAxis("Pitch");
        yaw = Input.GetAxis("Yaw");

        if (Input.GetKey(KeyCode.LeftShift)) throttle += throttleIncrement;
        else if (Input.GetKey(KeyCode.LeftControl)) throttle -= throttleIncrement;
        throttle = Mathf.Clamp(throttle, 0f, 100f);
    }

    public void Kill()
    {
        isAlive = false;
        StopEngine();
    }

    public void StopEngine()
    {
        isStopped = true;
        roll = 0f;
        pitch = 0f;
        yaw = 0f;
        throttle = 0f;
    }

    private void Update()
    {
        Move();
        cumulativeDistance += distanceToTarget;
    }

    private void FixedUpdate()
    {
        // Move plane forward
        rb.AddForce(-transform.right * maxThrust * throttle);
        propeller.Rotate(Vector3.forward * throttle);

        // Rotate plane
        rb.AddTorque(transform.right * roll * responseModifier);
        rb.AddTorque(transform.forward * pitch * responseModifier);
        rb.AddTorque(transform.up * yaw * responseModifier);

        // Generate lift
        rb.AddForce(transform.up * rb.velocity.magnitude * lift / (Vector3.Angle(Vector3.up, transform.up) + 1f));
    }

    public void ResetToStart()
    {
        // Set attributes
        isAlive = true;
        isStopped = false;
        cumulativeDistance = 0f;

        // Reset motion
        roll = 0f;
        pitch = 0f;
        yaw = 0f;
        throttle = 100f;
        rb.velocity = Vector3.zero;

        // Move to spawn position
        transform.position = planeStart;
        transform.rotation = Quaternion.identity;
        propeller.rotation = Quaternion.Euler(357.22699f, 270f, 0f);
    }

    public float fitness()
    {
        float _fitness = 1 / (Mathf.Pow(cumulativeDistance, 3) + 0.01f);
        float scaling = isAlive ? 1f : 0.01f;
        return _fitness * scaling;
    }
}
