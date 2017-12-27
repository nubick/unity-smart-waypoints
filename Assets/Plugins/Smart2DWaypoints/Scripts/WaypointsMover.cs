using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Plugins.Smart2DWaypoints.Scripts
{
	public class WaypointsMover : MonoBehaviour
	{
	    protected Transform Transform;

		private int _previousTargetIndex;
	    protected int _targetIndex;
	    private bool _isRunning;
        private bool _isForwardDirection;
        private Vector2 _startScale;
		private int _passedWaypointsCount;
		private float _pathPartStartTime;
		private float _pathPartAllTime;
        private float _nextPartTime;
        private bool _isPaused;
        private float _pauseTime;

		public Path Path;
	    public Transform StartWaypoint;
        public LoopType LoopType = LoopType.Looped;
		public bool IsDestroyWhenOneWayFinished;
	    public bool IsAlignToDirection;
	    public float RotationOffset;
	    public bool IsXFlipEnabled;
	    public bool IsYFlipEnabled;

        public const float InitSpeed = 0.139139f;
        [FormerlySerializedAs("Speed")]
        [SerializeField]
	    private float _speed = InitSpeed;
	    public float Speed
	    {
	        get { return _speed; }
	        set
	        {
	            UpdateSpeed(_speed, value);
	            _speed = value;
	        }
	    }


        [NonSerialized]
	    public Vector3 Direction;

		public event EventHandler OneWayFinished;

		public virtual void Awake()
		{
			Transform = transform;
			_startScale = Transform.localScale;
			_isForwardDirection = true;

			if (Path == null)
			{
				_isRunning = false;
			}
			else
			{
				Run();
			}
		}

        #region Running

        private void Run()
		{
            StartRunning();
			_targetIndex = Path.GetIndex(StartWaypoint);
			Transform.position = Path.CapturePosition(this, _targetIndex);
			StartCoroutine(OnWaypointReached());
		}

	    public virtual void Update()
	    {
	        OnUpdate();
	    }

	    protected void OnUpdate()
	    {
            if (_isRunning && !_isPaused)
            {
				UpdateMovement();
                AlignToDirection();
                UpdateFlips();
            }
	    }

		private void InitializeStartDirection()
		{
			if (_passedWaypointsCount == 1 && IsAlignToDirection)
			{
				Vector3 point = Path.GetPoint(this, 0.01f);
				Direction = (point - Transform.position).normalized;
                AlignToDirection();
			}
		}

		private void AlignToDirection()
		{
			if (IsAlignToDirection && Direction != Vector3.zero)
			{
				float angle = Direction.y > 0
					? Vector2.Angle(new Vector2(1, 0), Direction)
					: -Vector2.Angle(new Vector2(1, 0), Direction);
                SetRotationAngle(angle + RotationOffset);
			}
		}

		protected virtual void SetRotationAngle(float angle)
		{
			Transform.rotation = Quaternion.identity;
			Transform.Rotate(Vector3.forward, angle);
		}

	    protected void TakeNextWaypoint()
	    {
	        switch (LoopType)
	        {
	            case LoopType.OneWay:
		        {
			        if ((Path.IsClosed && IsAtFirstWaypoint() && _passedWaypointsCount > 0) || IsAtLastWaypoint())
			        {
				        StopRunning();

						if (OneWayFinished != null)
							OneWayFinished(this, EventArgs.Empty);

				        if (IsDestroyWhenOneWayFinished)
					        Destroy(gameObject);
			        }
			        break;
		        }
	            case LoopType.Looped:
		        {
					if (IsAtLastWaypoint())//for not closed path jump immediately to first point
				        Transform.position = Path.Waypoints[0].position;
	                break;
	            }
	            case LoopType.PingPong:
		        {
			        if ((IsAtFirstWaypoint() && (!_isForwardDirection || (_isForwardDirection && _passedWaypointsCount > 0))) ||
			            (IsAtLastWaypoint() && _isForwardDirection))
				        _isForwardDirection = !_isForwardDirection;
			        break;
		        }
	        }
			MoveTargetIndexToNext();
	    }

		private bool IsAtFirstWaypoint()
		{
			return _targetIndex == 0;
		}

		private bool IsAtLastWaypoint()
		{
			return !Path.IsClosed && _targetIndex == Path.WaypointsCount - 1;
		}

		private void MoveTargetIndexToNext()
		{
			_previousTargetIndex = _targetIndex;
			int dInd = _isForwardDirection ? 1 : -1;
			_targetIndex = (_targetIndex + dInd + Path.WaypointsCount)%Path.WaypointsCount;
			_passedWaypointsCount++;
		}

	    protected virtual void StopRunning()
		{
			_isRunning = false;
		}

	    protected virtual void StartRunning()
	    {
	        _isRunning = true;
	    }

	    protected IEnumerator OnWaypointReached()
	    {
		    if (Path.HasDelay(_targetIndex))
		    {
                StopRunning();
			    yield return new WaitForSeconds(Path.GetDelay(_targetIndex));
		        StartRunning();
		    }

			TakeNextWaypoint();

			Path.StartNextPart(this, _previousTargetIndex, _targetIndex, _isForwardDirection);
			_pathPartAllTime = Path.GetLength(this, _previousTargetIndex, _targetIndex, _isForwardDirection)/Speed;
			_pathPartStartTime = Time.time - _nextPartTime;
	        _nextPartTime = 0;

            UpdateMovement();

		    if (!Path.IsClosed && LoopType == LoopType.Looped && _targetIndex == 0)
			    StartCoroutine(OnWaypointReached());

		    InitializeStartDirection();
	    }

        private void UpdateMovement()
		{
			float pathPartTime = (Time.time - _pathPartStartTime) / _pathPartAllTime;
			if (pathPartTime > 1f)
			{
				UpdatePoint(1f);
			    _nextPartTime = (pathPartTime - 1f)*_pathPartAllTime;
				StartCoroutine(OnWaypointReached());
			}
			else
			{
				UpdatePoint(pathPartTime);
			}
		}

		private void UpdatePoint(float pathPartTime)
		{
			Vector3 point = Path.GetPoint(this, pathPartTime);
			Direction = (point - Transform.position).normalized;
			UpdatePoint(point);
		}

	    protected virtual void UpdatePoint(Vector3 point)
	    {
            Transform.position = point;
	    }

        #endregion

        #region Flips

        private void UpdateFlips()
	    {
	        if (IsXFlipEnabled && Transform.localScale.x*Direction.x*Mathf.Sign(_startScale.x) < 0)
	            Transform.localScale = new Vector2(-Transform.localScale.x, Transform.localScale.y);

	        if (IsYFlipEnabled && Transform.localScale.y*Direction.y*Mathf.Sign(_startScale.y) < 0)
	            Transform.localScale = new Vector3(Transform.localScale.x, -Transform.localScale.y);
	    }

	    #endregion

		#region Gizmos

		public void OnDrawGizmos()
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(transform.position, transform.position + Direction*Camera.main.orthographicSize/2f);
		}

		#endregion

		#region Public API

		public void Go(Path path, int startWaypointIndex)
		{
			Path = path;
			StartWaypoint = path.Waypoints[startWaypointIndex];
			Run();
		}

		public void Go(Path path)
		{
			Go(path, 0);
		}

		public void Go(Path path, Transform startWaypoint)
		{
			int index = path.Waypoints.IndexOf(startWaypoint);
			Go(path, index == -1 ? 0 : index);
		}

		#endregion

        #region Pause

        public void Pause()
        {
            if (!_isPaused)
            {
                _pauseTime = Time.time;
                _isPaused = true;
                OnPause();
            }
        }

        protected virtual void OnPause() { }

        public void Resume()
        {
            if (_isPaused)
            {
                _isPaused = false;
                _pathPartStartTime += Time.time - _pauseTime;
                OnResume();
            }
        }

        protected virtual void OnResume() { }

	    public bool IsPaused()
	    {
	        return _isPaused;
        }

        #endregion

	    private void UpdateSpeed(float oldSpeed, float newSpeed)
	    {
            float st2 = _pathPartStartTime + (Time.time - _pathPartStartTime) * (1 - oldSpeed / newSpeed);
            _pathPartAllTime = (Time.time - st2) / ((Time.time - _pathPartStartTime) / _pathPartAllTime);
            _pathPartStartTime = st2;
	    }
    }
}
