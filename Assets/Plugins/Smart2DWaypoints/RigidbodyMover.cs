using UnityEngine;

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class RigidbodyMover : WaypointsMover
    {
        private Rigidbody2D _rigidbody2D;

		public float RotationSpeed = 10f;

        public override void Awake()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
			base.Awake();
        }

        public override void Update()
        {
			//Functional moved to FixedUpdate
        }

        public void FixedUpdate()
        {
			OnUpdate();
        }

        protected override void UpdatePoint(Vector3 point)
	    {
			float speed = (point - Transform.position).magnitude / Time.fixedDeltaTime;
			_rigidbody2D.velocity = Direction * speed;		    
	    }

        protected override void StartRunning()
        {
            base.StartRunning();
            _rigidbody2D.WakeUp();
        }

        protected override void StopRunning()
        {
            base.StopRunning();
            _rigidbody2D.Sleep();
        }

        protected override void SetRotationAngle(float targetAngle)
		{
            float dAngle = targetAngle - Transform.localEulerAngles.z;
            if (dAngle <= -180f)
                dAngle += 360f;
            _rigidbody2D.angularVelocity = Mathf.Abs(dAngle) < 0.1f ? 0f : dAngle * RotationSpeed;
		}

        protected override void OnPause()
        {
            _rigidbody2D.Sleep();
        }

        protected override void OnResume()
        {
            _rigidbody2D.WakeUp();
        }
    }
}
