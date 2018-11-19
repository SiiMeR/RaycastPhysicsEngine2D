
using RaycastEngine2D;
using UnityEngine;


[RequireComponent(typeof(BoxController2D))]
public class Player : MonoBehaviour
{
    public Vector3 Velocity;

    [SerializeField] private float _accelerationTimeAirborne = .2f;
    [SerializeField] private float _accelerationTimeGrounded = .1f;
    [SerializeField] private float _maxJumpHeight = 4f;
    [SerializeField] private float _moveSpeed = 10;
    [SerializeField] private float _minJumpHeight = 1f;
    [SerializeField] private float _timeToJumpApex = .4f;

    private BoxController2D _controller;

    private float _maxJumpVelocity;
    private float _minJumpVelocity;
    private float _velocityXSmoothing;


    // Use this for initialization
    private void Start()
    {
        _controller = GetComponent<BoxController2D>();

        var gravity = -(2 * _maxJumpHeight) / Mathf.Pow(_timeToJumpApex, 2);

        Constants.GRAVITY = gravity;

        _maxJumpVelocity = Mathf.Abs(gravity * _timeToJumpApex);
        _minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * _minJumpHeight);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Time.timeScale > 0.01f)
        {
            UpdateMovement();
        }
    }

    private void UpdateMovement()
    {
        if (_controller.Collisions.Above || _controller.Collisions.Below)
        {
            Velocity.y = 0;
        }

        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        Debug.DrawRay(transform.position, input, Color.yellow);

        if (Input.GetButtonDown("Jump") && _controller.Collisions.Below)
        {
            Velocity.y = _maxJumpVelocity;
        }

        if (Input.GetButtonUp("Jump"))
        {
            if (Velocity.y > _minJumpVelocity)
            {
                Velocity.y = _minJumpVelocity;
            }
        }

        var targetVelocityX = Mathf.Round(input.x) * _moveSpeed;

        Velocity.x = Mathf.SmoothDamp(Velocity.x, targetVelocityX, ref _velocityXSmoothing,
            _controller.Collisions.Below ? _accelerationTimeGrounded : _accelerationTimeAirborne);

        Velocity.y += Constants.GRAVITY * Time.deltaTime;

        _controller.Move(Velocity * Time.deltaTime);
    }
}