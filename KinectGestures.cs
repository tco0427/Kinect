using System;
using System.Collections.Generic;
using Microsoft.Kinect;

/// <summary>
/// Vector3 is utility structure
/// </summary>
public struct Vector3
{
    public const float kEpsilon = 1E-05F;
    public float x;
    public float y;
    public float z;

    public Vector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public float this[int index]
    {
        get
        {
            if (index == 0)
                return x;
            if (index == 1)
                return y;
            if (index == 2)
                return z;
            return 0;
        }
        set
        {
            if (index == 0)
                x = value;
            if (index == 1)
                y = value;
            if (index == 2)
                z = value;
        }
    }

    public static Vector3 back
    {
        get
        {
            return new Vector3(0, 0, -1);
        }
    }

    public static Vector3 down
    {
        get
        {
            return new Vector3(0, -1, 0);
        }
    }

    public static Vector3 forward
    {
        get
        {
            return new Vector3(0, 0, 1);
        }
    }

    public static Vector3 left
    {
        get
        {
            return new Vector3(-1, 0, 0);
        }
    }

    public static Vector3 one
    {
        get
        {
            return new Vector3(1, 1, 1);
        }
    }

    public static Vector3 right
    {
        get
        {
            return new Vector3(1, 0, 0);
        }
    }

    public static Vector3 up
    {
        get
        {
            return new Vector3(0, 1, 0);
        }
    }

    public static Vector3 zero
    {
        get
        {
            return new Vector3(0, 0, 0);
        }
    }

    public float magnitude
    {
        get { return (float)System.Math.Sqrt(x * x + y * y + z * z); }
    }

    public void normalized()
    {
        float _len = magnitude;
        if (_len <= 0.0f)
            return;
        x /= _len;
        y /= _len;
        z /= _len;
    }
    public static float Angle(Vector3 from, Vector3 to)
    {
        from.normalized();
        to.normalized();
        float dot = Dot(from, to);
        float radian = (float)System.Math.Acos(dot);
        return (float)(radian * 180.0f / System.Math.PI); // RadianToDegree
    }

    public static float Magnitude(Vector3 a)
    {
        return (float)System.Math.Sqrt(a.x * a.x + a.y * a.y + a.z * a.z);
    }

    public static Vector3 Cross(Vector3 l, Vector3 r)
    {
        Vector3 result = new Vector3();
        result.x = l.y * r.z - r.y * l.z;
        result.y = -l.x * r.z + r.x * l.z;
        result.z = l.x * r.y - r.x * l.y;
        return result;
    }

    public static float Dot(Vector3 l, Vector3 r)
    {
        return (l.x * r.x + l.y * r.y + l.z * r.z);
    }

    public static Vector3 operator +(Vector3 l, Vector3 r)
    {
        Vector3 result = new Vector3();
        result.x = l.x + r.x;
        result.y = l.y + r.y;
        result.z = l.z + r.z;
        return result;
    }

    public static Vector3 operator -(Vector3 l, Vector3 r)
    {
        Vector3 result = new Vector3();
        result.x = l.x - r.x;
        result.y = l.y - r.y;
        result.z = l.z - r.z;
        return result;
    }

    public static Vector3 operator /(Vector3 l, float scalar)
    {
        Vector3 result = new Vector3();
        if (scalar > kEpsilon)
        {
            result.x = l.x / scalar;
            result.y = l.y / scalar;
            result.z = l.z / scalar;
        }

        return result;
    }

    public static Vector3 operator *(Vector3 l, float scalar)
    {
        Vector3 result = new Vector3();
        result.x = l.x * scalar;
        result.y = l.y * scalar;
        result.z = l.z * scalar;

        return result;
    }

    public static implicit operator byte[] (Vector3 v)
    {
        byte[] _x = BitConverter.GetBytes(v.x);
        byte[] _y = BitConverter.GetBytes(v.y);
        byte[] _z = BitConverter.GetBytes(v.z);

        byte[] _result = new byte[_x.Length * 3];
        Array.Copy(_x, 0, _result, 0, _x.Length);
        Array.Copy(_y, 0, _result, _x.Length, _y.Length);
        Array.Copy(_z, 0, _result, _x.Length * 2, _z.Length);

        return _result;
    }

    public static implicit operator Vector3(byte[] buf)
    {
        Vector3 _result = new Vector3();

        _result.x = BitConverter.ToInt32(buf, 0);
        _result.y = BitConverter.ToInt32(buf, sizeof(int));
        _result.z = BitConverter.ToInt32(buf, sizeof(int) * 2);

        return _result;
    }

}

/// <summary>
/// KinectGestures is utility class that processes programmatic Kinect gestures
/// </summary>
public class KinectGestures
{
    public const float PoseCompleteDuration = 0.1f;
    /// <summary>
    /// The gesture types.
    /// </summary>
    public enum Gestures
    {
        None = 0,
        RaiseRightHand,
        RaiseLeftHand,
        Psi,
        Tpose,
        Stop,
        Wave,
        //		Click,
        SwipeLeft,
        SwipeRight,
        SwipeUp,
        SwipeDown,
        //		RightHandCursor,
        //		LeftHandCursor,
        ZoomIn,
        ZoomOut,
        Wheel,
        Jump,
        Squat,
        Push,
        Pull,
        ShoulderLeftFront,
        ShoulderRightFront,
        LeanLeft,
        LeanRight,
        KickLeft,
        KickRight,
        Run,

        UserGesture1 = 101,
        UserGesture2 = 102,
        UserGesture3 = 103,
        UserGesture4 = 104,
        UserGesture5 = 105,
        UserGesture6 = 106,
        UserGesture7 = 107,
        UserGesture8 = 108,
        UserGesture9 = 109,
        UserGesture10 = 110,

        LHFo,
        RHFo,
        RHSu,
        LHSu,
        BoxerR,
        BoxerL,
        RKnee,
        LKnee,
        RHPu,
        LHPu,
        Squat2,
        RHSu100,
        RHSu70,
        RHFo100,
        RHFo70,
        RHPu100,
        RHPu70,
        Mansae,
        LRnaranhee,
        Charyeot
    }

    /// <summary>
    /// Gesture data structure.
    /// </summary>
    public struct GestureData
    {
        public long userId;
        public Gestures gesture;
        public int state;
        public float timestamp;
        public int joint;
        public Vector3 jointPos;
        public Vector3 screenPos;
        public float tagFloat;
        public Vector3 tagVector;
        public Vector3 tagVector2;
        public float progress;
        public bool complete;
        public bool cancelled;
        public List<Gestures> checkForGestures;
        public float startTrackingAtTime;
    }

    float a, b, c, d;

    // Gesture related constants, variables and functions
    protected int leftHandIndex;
    protected int rightHandIndex;

    protected int leftElbowIndex;
    protected int rightElbowIndex;

    protected int leftShoulderIndex;
    protected int rightShoulderIndex;

    protected int hipCenterIndex;
    protected int shoulderCenterIndex;

    protected int leftHipIndex;
    protected int rightHipIndex;

    protected int leftKneeIndex;
    protected int rightKneeIndex;

    protected int leftAnkleIndex;
    protected int rightAnkleIndex;


    /// <summary>
    /// Gets the list of gesture joint indexes.
    /// </summary>
    /// <returns>The needed joint indexes.</returns>
    public virtual int[] GetNeededJointIndexes()
    {
        leftHandIndex = (int)(JointType.HandLeft);
        rightHandIndex = (int)(JointType.HandRight);

        leftElbowIndex = (int)(JointType.ElbowLeft);
        rightElbowIndex = (int)(JointType.ElbowRight);

        leftShoulderIndex = (int)(JointType.ShoulderLeft);
        rightShoulderIndex = (int)(JointType.ShoulderRight);

        hipCenterIndex = (int)(JointType.SpineBase);
        shoulderCenterIndex = (int)(JointType.SpineShoulder);

        leftHipIndex = (int)(JointType.HipLeft);
        rightHipIndex = (int)(JointType.HipRight);

        leftKneeIndex = (int)(JointType.KneeLeft);
        rightKneeIndex = (int)(JointType.KneeRight);

        leftAnkleIndex = (int)(JointType.AnkleLeft);
        rightAnkleIndex = (int)(JointType.AnkleRight);

        int[] neededJointIndexes = {
            leftHandIndex, rightHandIndex, leftElbowIndex, rightElbowIndex, leftShoulderIndex, rightShoulderIndex,
            hipCenterIndex, shoulderCenterIndex, leftHipIndex, rightHipIndex, leftKneeIndex, rightKneeIndex,
            leftAnkleIndex, rightAnkleIndex
        };

        return neededJointIndexes;
    }

    protected void SetGestureJoint(ref GestureData gestureData, float timestamp, int joint, Vector3 jointPos)
    {
        gestureData.joint = joint;
        gestureData.jointPos = jointPos;
        gestureData.timestamp = timestamp;
        gestureData.state++;
    }

    protected void SetGestureCancelled(ref GestureData gestureData)
    {
        gestureData.state = 0;
        gestureData.progress = 0f;
        gestureData.cancelled = true;
    }

    protected float Clamp(float val, float floor, float ceil)
    {
        return val <= ceil ? (val >= floor ? val : floor) : ceil;
    }
    protected float Clamp01(float val)
    {
        return Clamp(val, 0.0f, 1.0f);
    }

    protected void CheckPoseComplete(ref GestureData gestureData, float timestamp, Vector3 jointPos, bool isInPose, float durationToComplete)
    {
        if (isInPose)
        {
            float timeLeft = timestamp - gestureData.timestamp;
            gestureData.progress = durationToComplete > 0f ? Clamp01(timeLeft / durationToComplete) : 1.0f;

            if (timeLeft >= durationToComplete)
            {
                gestureData.timestamp = timestamp;
                gestureData.jointPos = jointPos;
                gestureData.state++;
                gestureData.complete = true;
            }
        }
        else
        {
            SetGestureCancelled(ref gestureData);
        }
    }
    /*
    protected void SetScreenPos(long userId, ref GestureData gestureData, ref Vector3[] jointsPos, ref bool[] jointsTracked)
    {
        Vector3 handPos = jointsPos[rightHandIndex];
        bool calculateCoords = false;

        if (gestureData.joint == rightHandIndex)
        {
            if (jointsTracked[rightHandIndex]) // && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex]
            {
                calculateCoords = true;
            }
        }
        else if (gestureData.joint == leftHandIndex)
        {
            if (jointsTracked[leftHandIndex]) // && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex]
            {
                handPos = jointsPos[leftHandIndex];
                calculateCoords = true;
            }
        }

        if (calculateCoords)
        {
            if (jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] &&
                jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex])
            {
                Vector3 shoulderToHips = jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex];
                Vector3 rightToLeft = jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex];

                gestureData.tagVector2.x = rightToLeft.x; // * 1.2f;
                gestureData.tagVector2.y = shoulderToHips.y; // * 1.2f;

                if (gestureData.joint == rightHandIndex)
                {
                    gestureData.tagVector.x = jointsPos[rightShoulderIndex].x - gestureData.tagVector2.x / 2;
                    gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
                }
                else
                {
                    gestureData.tagVector.x = jointsPos[leftShoulderIndex].x - gestureData.tagVector2.x / 2;
                    gestureData.tagVector.y = jointsPos[hipCenterIndex].y;
                }
            }

            if (gestureData.tagVector2.x != 0 && gestureData.tagVector2.y != 0)
            {
                Vector3 relHandPos = handPos - gestureData.tagVector;
                gestureData.screenPos.x = Mathf.Clamp01(relHandPos.x / gestureData.tagVector2.x);
                gestureData.screenPos.y = Mathf.Clamp01(relHandPos.y / gestureData.tagVector2.y);
            }

        }
    }

    protected void SetZoomFactor(long userId, ref GestureData gestureData, float initialZoom, ref Vector3[] jointsPos, ref bool[] jointsTracked)
    {
        Vector3 vectorZooming = jointsPos[rightHandIndex] - jointsPos[leftHandIndex];

        if (gestureData.tagFloat == 0f || gestureData.userId != userId)
        {
            gestureData.tagFloat = 0.5f; // this is 100%
        }

        float distZooming = vectorZooming.magnitude;
        gestureData.screenPos.z = initialZoom + (distZooming / gestureData.tagFloat);
    }

    protected void SetWheelRotation(long userId, ref GestureData gestureData, Vector3 initialPos, Vector3 currentPos)
    {
        float angle = Vector3.Angle(initialPos, currentPos) * Mathf.Sign(currentPos.y - initialPos.y);
        gestureData.screenPos.z = angle;
    }
    */

    // estimate the next state and completeness of the gesture
    /// <summary>
    /// estimate the state and progress of the given gesture.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="gestureData">Gesture-data structure</param>
    /// <param name="timestamp">Current time</param>
    /// <param name="jointsPos">Joints-position array</param>
    /// <param name="jointsTracked">Joints-tracked array</param>
    public virtual void CheckForGesture(long userId, ref GestureData gestureData, float timestamp, ref Vector3[] jointsPos, ref bool[] jointsTracked)
    {
        if (gestureData.complete)
            return;

        float bandSize = (jointsPos[shoulderCenterIndex].y - jointsPos[hipCenterIndex].y);
        float gestureTop = jointsPos[shoulderCenterIndex].y + bandSize * 1.2f / 3f;
        float gestureBottom = jointsPos[shoulderCenterIndex].y - bandSize * 1.8f / 3f;
        float gestureRight = jointsPos[rightHipIndex].x;
        float gestureLeft = jointsPos[leftHipIndex].x;

        switch (gestureData.gesture)
        {
            // check for RaiseRightHand
            case Gestures.RaiseRightHand:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                            (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f &&
                               (jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                            (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.1f &&
                            (jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            // check for RaiseLeftHand
            case Gestures.RaiseLeftHand:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                            (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f &&
                               (jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                            (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.1f &&
                            (jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            // check for Psi
            case Gestures.Psi:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[shoulderCenterIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[shoulderCenterIndex].y) > 0.1f &&
                           (jointsPos[leftHandIndex].y - jointsPos[shoulderCenterIndex].y) > 0.1f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[shoulderCenterIndex] &&
                            (jointsPos[rightHandIndex].y - jointsPos[shoulderCenterIndex].y) > 0.1f &&
                            (jointsPos[leftHandIndex].y - jointsPos[shoulderCenterIndex].y) > 0.1f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            // check for Tpose
            case Gestures.Tpose:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
                           Math.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.07f
                           Math.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                           jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
                             Math.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
                           Math.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[rightShoulderIndex] &&
                                Math.Abs(jointsPos[rightElbowIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                                Math.Abs(jointsPos[rightHandIndex].y - jointsPos[rightShoulderIndex].y) < 0.1f &&  // 0.7f
                                jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                Math.Abs(jointsPos[leftElbowIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f &&
                                Math.Abs(jointsPos[leftHandIndex].y - jointsPos[leftShoulderIndex].y) < 0.1f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            // check for Stop
            case Gestures.Stop:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) < 0.2f &&
                              (jointsPos[rightHandIndex].x - jointsPos[rightHipIndex].x) >= 0.4f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
                           (jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) < 0.2f &&
                           (jointsPos[leftHandIndex].x - jointsPos[leftHipIndex].x) <= -0.4f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = (gestureData.joint == rightHandIndex) ?
                            (jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
                            (jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) < 0.2f &&
                             (jointsPos[rightHandIndex].x - jointsPos[rightHipIndex].x) >= 0.4f) :
                            (jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
                            (jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) < 0.2f &&
                             (jointsPos[leftHandIndex].x - jointsPos[leftHipIndex].x) <= -0.4f);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            // check for Wave
            case Gestures.Wave:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f &&
                           (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.3f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
                                (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < -0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture - phase 2
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f &&
                                (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) < -0.05f :
                                jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
                                (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) > 0.05f;

                            if (isInPose)
                            {
                                gestureData.timestamp = timestamp;
                                gestureData.state++;
                                gestureData.progress = 0.7f;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;

                    case 2:  // gesture phase 3 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > 0.1f &&
                                (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0.05f :
                                jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > 0.1f &&
                                (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < -0.05f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            //			// check for Click
            //			case Gestures.Click:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1
            //						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            //					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f)
            //						{
            //							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
            //							gestureData.progress = 0.3f;
            //
            //							// set screen position at the start, because this is the most accurate click position
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //						}
            //						else if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            //					            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f)
            //						{
            //							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
            //							gestureData.progress = 0.3f;
            //
            //							// set screen position at the start, because this is the most accurate click position
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //						}
            //						break;
            //				
            //					case 1:  // gesture - phase 2
            ////						if((timestamp - gestureData.timestamp) < 1.0f)
            ////						{
            ////							bool isInPose = gestureData.joint == rightHandIndex ?
            ////								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            ////								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f && 
            ////								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) < -0.05f :
            ////								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            ////								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
            ////								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) < -0.05f;
            ////				
            ////							if(isInPose)
            ////							{
            ////								gestureData.timestamp = timestamp;
            ////								gestureData.jointPos = jointsPos[gestureData.joint];
            ////								gestureData.state++;
            ////								gestureData.progress = 0.7f;
            ////							}
            ////							else
            ////							{
            ////								// check for stay-in-place
            ////								Vector3 distVector = jointsPos[gestureData.joint] - gestureData.jointPos;
            ////								isInPose = distVector.magnitude < 0.05f;
            ////
            ////								Vector3 jointPos = jointsPos[gestureData.joint];
            ////								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, Constants.ClickStayDuration);
            ////							}
            ////						}
            ////						else
            //						{
            //							// check for stay-in-place
            //							Vector3 distVector = jointsPos[gestureData.joint] - gestureData.jointPos;
            //							bool isInPose = distVector.magnitude < 0.05f;
            //
            //							Vector3 jointPos = jointsPos[gestureData.joint];
            //							CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.ClickStayDuration);
            ////							SetGestureCancelled(gestureData);
            //						}
            //						break;
            //									
            ////					case 2:  // gesture phase 3 = complete
            ////						if((timestamp - gestureData.timestamp) < 1.0f)
            ////						{
            ////							bool isInPose = gestureData.joint == rightHandIndex ?
            ////								jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
            ////								//(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f && 
            ////								Mathf.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[rightHandIndex].z - gestureData.jointPos.z) > 0.05f :
            ////								jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
            ////								//(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
            ////								Mathf.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.08f &&
            ////								(jointsPos[leftHandIndex].z - gestureData.jointPos.z) > 0.05f;
            ////
            ////							if(isInPose)
            ////							{
            ////								Vector3 jointPos = jointsPos[gestureData.joint];
            ////								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
            ////							}
            ////						}
            ////						else
            ////						{
            ////							// cancel the gesture
            ////							SetGestureCancelled(ref gestureData);
            ////						}
            ////						break;
            //				}
            //				break;

            // check for SwipeLeft
            case Gestures.SwipeLeft:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                             //						if(jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
                             //					       (jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) > -0.05f &&
                             //					       (jointsPos[rightHandIndex].x - jointsPos[rightElbowIndex].x) > 0f)
                             //						{
                             //							SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                             //							gestureData.progress = 0.5f;
                             //						}
                        if (jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                               jointsPos[rightHandIndex].x <= gestureRight && jointsPos[rightHandIndex].x > gestureLeft)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.1f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            //							bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] &&
                            //								Mathf.Abs(jointsPos[rightHandIndex].y - jointsPos[rightElbowIndex].y) < 0.1f && 
                            //								Mathf.Abs(jointsPos[rightHandIndex].y - gestureData.jointPos.y) < 0.08f && 
                            //								(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < -0.15f;
                            //
                            //							if(isInPose)
                            //							{
                            //								Vector3 jointPos = jointsPos[gestureData.joint];
                            //								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            //							}

                            bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                    jointsPos[rightHandIndex].x <= gestureLeft;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[rightHandIndex].x <= gestureRight)
                            {
                                float gestureSize = gestureRight - gestureLeft;
                                gestureData.progress = gestureSize > 0.01f ? (gestureRight - jointsPos[rightHandIndex].x) / gestureSize : 0f;
                            }

                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeRight
            case Gestures.SwipeRight:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                             //						if(jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
                             //				            (jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) > -0.05f &&
                             //				            (jointsPos[leftHandIndex].x - jointsPos[leftElbowIndex].x) < 0f)
                             //						{
                             //							SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                             //							gestureData.progress = 0.5f;
                             //						}

                        if (jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                               jointsPos[leftHandIndex].x >= gestureLeft && jointsPos[leftHandIndex].x < gestureRight)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.1f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            //							bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftElbowIndex] &&
                            //								Mathf.Abs(jointsPos[leftHandIndex].y - jointsPos[leftElbowIndex].y) < 0.1f &&
                            //								Mathf.Abs(jointsPos[leftHandIndex].y - gestureData.jointPos.y) < 0.08f && 
                            //								(jointsPos[leftHandIndex].x - gestureData.jointPos.x) > 0.15f;
                            //
                            //							if(isInPose)
                            //							{
                            //								Vector3 jointPos = jointsPos[gestureData.joint];
                            //								CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            //							}

                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                    jointsPos[leftHandIndex].x >= gestureRight;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                            else if (jointsPos[leftHandIndex].x >= gestureLeft)
                            {
                                float gestureSize = gestureRight - gestureLeft;
                                gestureData.progress = gestureSize > 0.01f ? (jointsPos[leftHandIndex].x - gestureLeft) / gestureSize : 0f;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeUp
            case Gestures.SwipeUp:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) < -0.0f &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.15f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) < -0.0f &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.15f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) > 0.05f &&
                                Math.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) <= 0.1f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) > 0.05f &&
                                Math.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) <= 0.1f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for SwipeDown
            case Gestures.SwipeDown:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftShoulderIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftShoulderIndex].y) >= 0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightShoulderIndex].y) >= 0.05f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) < -0.15f &&
                                Math.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) <= 0.1f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) < -0.15f &&
                                Math.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) <= 0.1f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            //			// check for RightHandCursor
            //			case Gestures.RightHandCursor:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1 (perpetual)
            //						if(jointsTracked[rightHandIndex] && jointsTracked[rightHipIndex] &&
            //							//(jointsPos[rightHandIndex].y - jointsPos[rightHipIndex].y) > -0.1f)
            //				   			(jointsPos[rightHandIndex].y - jointsPos[hipCenterIndex].y) >= 0f)
            //						{
            //							gestureData.joint = rightHandIndex;
            //							gestureData.timestamp = timestamp;
            //							gestureData.jointPos = jointsPos[rightHandIndex];
            //
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //							gestureData.progress = 0.7f;
            //						}
            //						else
            //						{
            //							// cancel the gesture
            //							//SetGestureCancelled(ref gestureData);
            //							gestureData.progress = 0f;
            //						}
            //						break;
            //				
            //				}
            //				break;
            //
            //			// check for LeftHandCursor
            //			case Gestures.LeftHandCursor:
            //				switch(gestureData.state)
            //				{
            //					case 0:  // gesture detection - phase 1 (perpetual)
            //						if(jointsTracked[leftHandIndex] && jointsTracked[leftHipIndex] &&
            //							//(jointsPos[leftHandIndex].y - jointsPos[leftHipIndex].y) > -0.1f)
            //							(jointsPos[leftHandIndex].y - jointsPos[hipCenterIndex].y) >= 0f)
            //						{
            //							gestureData.joint = leftHandIndex;
            //							gestureData.timestamp = timestamp;
            //							gestureData.jointPos = jointsPos[leftHandIndex];
            //
            //							SetScreenPos(userId, ref gestureData, ref jointsPos, ref jointsTracked);
            //							gestureData.progress = 0.7f;
            //						}
            //						else
            //						{
            //							// cancel the gesture
            //							//SetGestureCancelled(ref gestureData);
            //							gestureData.progress = 0f;
            //						}
            //						break;
            //				
            //				}
            //				break;

            // check for ZoomIn
            case Gestures.ZoomIn:
                Vector3 vectorZoomOut = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
                float distZoomOut = vectorZoomOut.magnitude;

                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                               jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                               jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distZoomOut < 0.3f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.tagVector = Vector3.right;
                            gestureData.tagFloat = 0f;
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = zooming
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            float angleZoomOut = Vector3.Angle(gestureData.tagVector, vectorZoomOut) * Math.Sign(vectorZoomOut.y - gestureData.tagVector.y);
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                    jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                distZoomOut < 1.5f && Math.Abs(angleZoomOut) < 20f;

                            if (isInPose)
                            {
                                //SetZoomFactor(userId, ref gestureData, 1.0f, ref jointsPos, ref jointsTracked);
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                            }
                            //							else
                            //							{
                            //								// cancel the gesture
                            //								SetGestureCancelled(ref gestureData);
                            //							}
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for ZoomOut
            case Gestures.ZoomOut:
                Vector3 vectorZoomIn = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
                float distZoomIn = vectorZoomIn.magnitude;

                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distZoomIn >= 0.7f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.tagVector = Vector3.right;
                            gestureData.tagFloat = distZoomIn;
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = zooming
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            float angleZoomIn = Vector3.Angle(gestureData.tagVector, vectorZoomIn) * Math.Sign(vectorZoomIn.y - gestureData.tagVector.y);
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                    jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                    jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                distZoomIn >= 0.2f && Math.Abs(angleZoomIn) < 20f;

                            if (isInPose)
                            {
                                //SetZoomFactor(userId, ref gestureData, 0.0f, ref jointsPos, ref jointsTracked);
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                            }
                            //							else
                            //							{
                            //								// cancel the gesture
                            //								SetGestureCancelled(ref gestureData);
                            //							}
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Wheel
            case Gestures.Wheel:
                Vector3 vectorWheel = (Vector3)jointsPos[rightHandIndex] - jointsPos[leftHandIndex];
                float distWheel = vectorWheel.magnitude;

                //				Debug.Log(string.Format("{0}. Dist: {1:F1}, Tag: {2:F1}, Diff: {3:F1}", gestureData.state,
                //				                        distWheel, gestureData.tagFloat, Mathf.Abs(distWheel - gestureData.tagFloat)));

                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                           jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                           jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                           distWheel >= 0.3f && distWheel < 0.7f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.tagVector = Vector3.right;
                            gestureData.tagFloat = distWheel;
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = zooming
                        if ((timestamp - gestureData.timestamp) < 0.5f)
                        {
                            float angle = Vector3.Angle(gestureData.tagVector, vectorWheel) * Math.Sign(vectorWheel.y - gestureData.tagVector.y);
                            bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[rightHandIndex] && jointsTracked[hipCenterIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] &&
                                jointsPos[leftHandIndex].y >= gestureBottom && jointsPos[leftHandIndex].y <= gestureTop &&
                                jointsPos[rightHandIndex].y >= gestureBottom && jointsPos[rightHandIndex].y <= gestureTop &&
                                distWheel >= 0.3f && distWheel < 0.7f &&
                                Math.Abs(distWheel - gestureData.tagFloat) < 0.1f;

                            if (isInPose)
                            {
                                //SetWheelRotation(userId, ref gestureData, gestureData.tagVector, vectorWheel);
                                gestureData.screenPos.z = angle;  // wheel angle
                                gestureData.timestamp = timestamp;
                                gestureData.tagFloat = distWheel;
                                gestureData.progress = 0.7f;
                            }
                            //							else
                            //							{
                            //								// cancel the gesture
                            //								SetGestureCancelled(ref gestureData);
                            //							}
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Jump
            case Gestures.Jump:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[hipCenterIndex] &&
                            (jointsPos[hipCenterIndex].y > 0.6f) && (jointsPos[hipCenterIndex].y < 1.2f))
                        {
                            SetGestureJoint(ref gestureData, timestamp, hipCenterIndex, jointsPos[hipCenterIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[hipCenterIndex] &&
                                (jointsPos[hipCenterIndex].y - gestureData.jointPos.y) > 0.15f &&
                                Math.Abs(jointsPos[hipCenterIndex].x - gestureData.jointPos.x) < 0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Squat
            case Gestures.Squat:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[hipCenterIndex] &&
                            (jointsPos[hipCenterIndex].y <= 0.7f))
                        {
                            SetGestureJoint(ref gestureData, timestamp, hipCenterIndex, jointsPos[hipCenterIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[hipCenterIndex] &&
                                (jointsPos[hipCenterIndex].y - gestureData.jointPos.y) < -0.15f &&
                                Math.Abs(jointsPos[hipCenterIndex].x - gestureData.jointPos.x) < 0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Push
            case Gestures.Push:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                               (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                               Math.Abs(jointsPos[rightHandIndex].x - jointsPos[rightShoulderIndex].x) < 0.2f &&
                               (jointsPos[rightHandIndex].z - jointsPos[leftElbowIndex].z) < -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[leftHandIndex].x - jointsPos[leftShoulderIndex].x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - jointsPos[rightElbowIndex].z) < -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[rightHandIndex].z - gestureData.jointPos.z) < -0.2f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - gestureData.jointPos.z) < -0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for Pull
            case Gestures.Pull:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                           (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                           Math.Abs(jointsPos[rightHandIndex].x - jointsPos[rightShoulderIndex].x) < 0.2f &&
                           (jointsPos[rightHandIndex].z - jointsPos[leftElbowIndex].z) < -0.3f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        else if (jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[leftHandIndex].x - jointsPos[leftShoulderIndex].x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - jointsPos[rightElbowIndex].z) < -0.3f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = gestureData.joint == rightHandIndex ?
                                jointsTracked[rightHandIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                                (jointsPos[rightHandIndex].y - jointsPos[leftElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[rightHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[rightHandIndex].z - gestureData.jointPos.z) > 0.25f :
                                jointsTracked[leftHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                                (jointsPos[leftHandIndex].y - jointsPos[rightElbowIndex].y) > -0.1f &&
                                Math.Abs(jointsPos[leftHandIndex].x - gestureData.jointPos.x) < 0.2f &&
                                (jointsPos[leftHandIndex].z - gestureData.jointPos.z) > 0.25f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for ShoulderLeftFron
            case Gestures.ShoulderLeftFront:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[leftHipIndex] &&
                              (jointsPos[rightShoulderIndex].z - jointsPos[leftHipIndex].z) < 0f &&
                           (jointsPos[rightShoulderIndex].z - jointsPos[leftShoulderIndex].z) > -0.15f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightShoulderIndex, jointsPos[rightShoulderIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[leftHipIndex] &&
                                    (jointsPos[rightShoulderIndex].z - jointsPos[leftShoulderIndex].z) < -0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for ShoulderRightFront
            case Gestures.ShoulderRightFront:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightHipIndex] &&
                           (jointsPos[leftShoulderIndex].z - jointsPos[rightHipIndex].z) < 0f &&
                           (jointsPos[leftShoulderIndex].z - jointsPos[rightShoulderIndex].z) > -0.15f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftShoulderIndex, jointsPos[leftShoulderIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightHipIndex] &&
                                    (jointsPos[leftShoulderIndex].z - jointsPos[rightShoulderIndex].z) < -0.2f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for LeanLeft
            case Gestures.LeanLeft:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1  (right shoulder is left of the right hip, means leaning left)
                        if (jointsTracked[rightShoulderIndex] && jointsTracked[rightHipIndex] &&
                           (jointsPos[rightShoulderIndex].x - jointsPos[rightHipIndex].x) < 0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightShoulderIndex, jointsPos[rightShoulderIndex]);
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 0.5f)
                        {
                            // check if right shoulder is still left of the right hip (leaning left)
                            bool isInPose = jointsTracked[rightShoulderIndex] && jointsTracked[rightHipIndex] &&
                                (jointsPos[rightShoulderIndex].x - jointsPos[rightHipIndex].x) < 0f;

                            if (isInPose)
                            {
                                // calculate lean angle
                                Vector3 vSpineLL = jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex];
                                gestureData.screenPos.z = Vector3.Angle(Vector3.up, vSpineLL);

                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for LeanRight
            case Gestures.LeanRight:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1 (left shoulder is right of the left hip, means leaning right)
                        if (jointsTracked[leftShoulderIndex] && jointsTracked[leftHipIndex] &&
                           (jointsPos[leftShoulderIndex].x - jointsPos[leftHipIndex].x) > 0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftShoulderIndex, jointsPos[leftShoulderIndex]);
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 0.5f)
                        {
                            // check if left shoulder is still right of the left hip (leaning right)
                            bool isInPose = jointsTracked[leftShoulderIndex] && jointsTracked[leftHipIndex] &&
                                (jointsPos[leftShoulderIndex].x - jointsPos[leftHipIndex].x) > 0f;

                            if (isInPose)
                            {
                                // calculate lean angle
                                Vector3 vSpineLR = jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex];
                                gestureData.screenPos.z = Vector3.Angle(Vector3.up, vSpineLR);

                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for KickLeft
            case Gestures.KickLeft:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftAnkleIndex] && jointsTracked[rightAnkleIndex] && jointsTracked[leftHipIndex] &&
                           (jointsPos[leftAnkleIndex].z - jointsPos[leftHipIndex].z) < 0f &&
                           (jointsPos[leftAnkleIndex].z - jointsPos[rightAnkleIndex].z) > -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftAnkleIndex, jointsPos[leftAnkleIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[leftAnkleIndex] && jointsTracked[rightAnkleIndex] && jointsTracked[leftHipIndex] &&
                                (jointsPos[leftAnkleIndex].z - jointsPos[rightAnkleIndex].z) < -0.4f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // check for KickRight
            case Gestures.KickRight:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                        if (jointsTracked[leftAnkleIndex] && jointsTracked[rightAnkleIndex] && jointsTracked[rightHipIndex] &&
                           (jointsPos[rightAnkleIndex].z - jointsPos[rightHipIndex].z) < 0f &&
                           (jointsPos[rightAnkleIndex].z - jointsPos[leftAnkleIndex].z) > -0.2f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightAnkleIndex, jointsPos[rightAnkleIndex]);
                            gestureData.progress = 0.5f;
                        }
                        break;

                    case 1:  // gesture phase 2 = complete
                        if ((timestamp - gestureData.timestamp) < 1.5f)
                        {
                            bool isInPose = jointsTracked[leftAnkleIndex] && jointsTracked[rightAnkleIndex] && jointsTracked[rightHipIndex] &&
                                (jointsPos[rightAnkleIndex].z - jointsPos[leftAnkleIndex].z) < -0.4f;

                            if (isInPose)
                            {
                                Vector3 jointPos = jointsPos[gestureData.joint];
                                CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, 0f);
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            case Gestures.Run:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection - phase 1
                             // check if the left knee is up
                        if (jointsTracked[leftKneeIndex] && jointsTracked[rightKneeIndex] &&
                           (jointsPos[leftKneeIndex].y - jointsPos[rightKneeIndex].y) > 0.1f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftKneeIndex, jointsPos[leftKneeIndex]);
                            gestureData.progress = 0.3f;
                        }
                        break;

                    case 1:  // gesture complete
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            // check if the right knee is up
                            bool isInPose = jointsTracked[rightKneeIndex] && jointsTracked[leftKneeIndex] &&
                                (jointsPos[rightKneeIndex].y - jointsPos[leftKneeIndex].y) > 0.1f;

                            if (isInPose)
                            {
                                // go to state 2
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.7f;
                                gestureData.state = 2;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;

                    case 2:  // gesture complete
                        if ((timestamp - gestureData.timestamp) < 1.0f)
                        {
                            // check if the left knee is up again
                            bool isInPose = jointsTracked[leftKneeIndex] && jointsTracked[rightKneeIndex] &&
                                (jointsPos[leftKneeIndex].y - jointsPos[rightKneeIndex].y) > 0.1f;

                            if (isInPose)
                            {
                                // go back to state 1
                                gestureData.timestamp = timestamp;
                                gestureData.progress = 0.8f;
                                gestureData.state = 1;
                            }
                        }
                        else
                        {
                            // cancel the gesture
                            SetGestureCancelled(ref gestureData);
                        }
                        break;
                }
                break;

            // here come more gesture-cases



            case Gestures.RHFo:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                             //if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             // (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f)))
                        Vector3 leftshoulderpos = Vector3.zero;
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }

                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                            (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                            Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 30.0f) &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 150.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                             // bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             //    (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f));
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }
                        a = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0))));
                        b = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]));

                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                        a < 30.0f && b > 150.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b);
                        List<float> std_angles = new List<float>(); std_angles.Add(0.0f); std_angles.Add(180.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(30.0f); range_angles.Add(30.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;
                /*
            case Gestures.RHFo100:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                             //if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             // (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f)))
                        Vector3 leftshoulderpos = Vector3.zero;
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }

                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                            (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                            Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 25.0f) &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 160.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                             // bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             //    (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f));
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                            (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                            Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 25.0f) &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 160.0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
                        break;
                }
                break;
            case Gestures.RHFo70:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                             //if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             // (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f)))
                        Vector3 leftshoulderpos = Vector3.zero;
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }

                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                            (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                            Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 45.0f) &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 140.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                             // bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                             //    (jointsPos[rightHandIndex].z < (jointsPos[rightShoulderIndex].z - 0.15f));
                        if (jointsTracked[leftShoulderIndex])
                        {
                            leftshoulderpos = jointsPos[leftShoulderIndex];
                        }
                        else
                        {
                            leftshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[rightShoulderIndex];
                            leftshoulderpos.y = 0.0f;
                            leftshoulderpos = jointsPos[rightShoulderIndex] + leftshoulderpos * 2;
                        }
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] &&
                            (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                            Vector3.Cross((leftshoulderpos - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 45.0f) &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 140.0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
                        break;
                }
                break;
                */
            case Gestures.LHFo:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                             //if (jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                             //(jointsPos[leftHandIndex].z < (jointsPos[leftShoulderIndex].z - 0.15f)))
                        Vector3 rightshoulderpos = Vector3.zero;
                        if (jointsTracked[rightShoulderIndex])
                        {
                            rightshoulderpos = jointsPos[rightShoulderIndex];
                        }
                        else
                        {
                            rightshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[leftShoulderIndex];
                            rightshoulderpos.y = 0.0f;
                            rightshoulderpos = jointsPos[leftShoulderIndex] + rightshoulderpos * 2;
                        }

                        if (jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[leftElbowIndex] &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]),
                            Vector3.Cross((jointsPos[leftShoulderIndex] - rightshoulderpos), (Vector3.up))) < 30.0f &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex])) > 150.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                             //bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                             //(jointsPos[leftHandIndex].z < (jointsPos[leftShoulderIndex].z - 0.15f));
                        if (jointsTracked[rightShoulderIndex])
                        {
                            rightshoulderpos = jointsPos[rightShoulderIndex];
                        }
                        else
                        {
                            rightshoulderpos = jointsPos[shoulderCenterIndex] - jointsPos[leftShoulderIndex];
                            rightshoulderpos.y = 0.0f;
                            rightshoulderpos = jointsPos[leftShoulderIndex] + rightshoulderpos * 2;
                        }
                        bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[leftElbowIndex] &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]),
                            Vector3.Cross((jointsPos[leftShoulderIndex] - rightshoulderpos), (Vector3.up))) < 30.0f &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex])) > 150.0f;


                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;


            case Gestures.BoxerR:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] &&
                          (Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]),
                          Vector3.Cross((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0)))) < 30.0f) &&
                          Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) < 60.0f &&
                          (Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightElbowIndex] - jointsPos[rightShoulderIndex])) < 110.0f))
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        a = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.Cross((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (new Vector3(0, 1, 0))));
                        b = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]));
                        c = Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightElbowIndex] - jointsPos[rightShoulderIndex]));
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[rightShoulderIndex] &&
                        a < 30.0f && b < 60.0f && c < 110.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c);
                        List<float> std_angles = new List<float>(); std_angles.Add(0.0f); std_angles.Add(30.0f); std_angles.Add(90.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(30.0f); range_angles.Add(30.0f); range_angles.Add(20);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.RHPu:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                           Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.up) < 20.0f &&
                           Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 160.0f)

                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:
                        a = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.up);
                        b = Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]));
                        bool isInPose = jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                        a < 40.0f && b > 130.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b);
                        List<float> std_angles = new List<float>(); std_angles.Add(0.0f); std_angles.Add(180.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(40.0f); range_angles.Add(50.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.Mansae:       // 두 팔 위로 드는 자세. RHPU LHPU 활용함
                switch (gestureData.state)
                {
                    case 0:
                        if ((jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                           Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.up) < 20.0f &&
                           Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 160.0f)&&
                           (jointsTracked[leftElbowIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                             (Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]), Vector3.up) < 30.0f) &&
                           (Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftHandIndex] - jointsPos[leftElbowIndex])) > 160.0f))
                           )

                        {
                            SetGestureJoint(ref gestureData, timestamp, shoulderCenterIndex, jointsPos[shoulderCenterIndex]); //기존의 왼쪽 혹은 오른쪽 어꺠 조인트인덱스 대신 어깨 중심을 기준으로함. 이부분은 동작에 미치는 영향이 없음.
                        }
                        break;

                    case 1:
                        a = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector3.up);
                        b = Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]));
                        c = Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]), Vector3.up);
                        d = Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]));      //손이 뻗는 방향각, 팔꿈치 관절각을 양팔 전부 구함.

                        bool isInPose = jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                        a < 40.0f && b > 130.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c); angles.Add(d);
                        List<float> std_angles = new List<float>(); std_angles.Add(0.0f); std_angles.Add(180.0f); std_angles.Add(0.0f); std_angles.Add(180.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(40.0f); range_angles.Add(50.0f); range_angles.Add(40.0f); range_angles.Add(50.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.LRnaranhee:       //좌우로 나란히
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftElbowIndex] && 
                            jointsTracked[leftShoulderIndex] && jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
                            Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 130.0f &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 150.0f &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex])) > 130.0f &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex])) > 150.0f &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f)

                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        a = Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]));
                        b = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]));
                        c = Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]));
                        d = Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex]));
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                            jointsTracked[rightElbowIndex] && jointsTracked[leftElbowIndex] && jointsTracked[leftShoulderIndex] &&
                            jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] && a > 130.0f && b > 150.0f && c > 130.0f && d > 150.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c); angles.Add(d);
                        List<float> std_angles = new List<float>(); std_angles.Add(180.0f); std_angles.Add(180.0f); std_angles.Add(180.0f); std_angles.Add(180.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(50.0f); range_angles.Add(30.0f); range_angles.Add(50.0f); range_angles.Add(30.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.Charyeot:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                            jointsTracked[leftShoulderIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[hipCenterIndex] &&
                            Vector3.Angle(Vector3.up, (jointsPos[rightShoulderIndex] - jointsPos[rightHandIndex])) < 30.0f &&
                            Vector3.Angle(Vector3.up, (jointsPos[leftShoulderIndex] - jointsPos[leftHandIndex])) < 30.0f &&
                            Vector3.Angle(Vector3.up, (jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex])) < 30.0f)

                        {
                            SetGestureJoint(ref gestureData, timestamp, shoulderCenterIndex, jointsPos[shoulderCenterIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        a = Vector3.Angle(Vector3.up, (jointsPos[rightShoulderIndex] - jointsPos[rightHandIndex]));
                        b = Vector3.Angle(Vector3.up, (jointsPos[leftShoulderIndex] - jointsPos[leftHandIndex]));
                        c = Vector3.Angle(Vector3.up, (jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex]));
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[leftHandIndex] && jointsTracked[rightShoulderIndex] &&
                            jointsTracked[leftShoulderIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[hipCenterIndex];

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c);
                        List<float> std_angles = new List<float>(); std_angles.Add(0.0f); std_angles.Add(0.0f); std_angles.Add(0.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(30.0f); range_angles.Add(30.0f); range_angles.Add(30.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;
            /*
                        case Gestures.RHPu100:
                            switch (gestureData.state)
                            {
                                case 0:
                                    if (jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                                       Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector2.up) < 20.0f && 
                                       Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 160.0f)

                                    {
                                        SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                                    }
                                    break;

                                case 1:
                                    bool isInPose = jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                                       Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector2.up) < 20.0f &&
                                       Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 160.0f;

                                    Vector3 jointPos = jointsPos[gestureData.joint];
                                    CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
                                    break;
                            }
                            break;

                        case Gestures.RHPu70:
                            switch (gestureData.state)
                            {
                                case 0:
                                    if (jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                                       Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector2.up) < 40.0f &&
                                       Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 130.0f)

                                    {
                                        SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                                    }
                                    break;

                                case 1:
                                    bool isInPose = jointsTracked[rightElbowIndex] && jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] &&
                                       Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]), Vector2.up) < 40.0f &&
                                       Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightHandIndex] - jointsPos[rightElbowIndex])) > 130.0f;

                                    Vector3 jointPos = jointsPos[gestureData.joint];
                                    CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
                                    break;
                            }
                            break;*/
            case Gestures.LHPu:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[leftElbowIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                             (Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]), Vector3.up) < 30.0f) &&
                           (Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftHandIndex] - jointsPos[leftElbowIndex])) > 150.0f))
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        }
                        break;

                    case 1:
                        bool isInPose = jointsTracked[leftElbowIndex] && jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] &&
                             (Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex]), Vector3.up) < 30.0f) &&
                           (Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftHandIndex] - jointsPos[leftElbowIndex])) > 150.0f);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;
            //case Gestures.RHSu100:
            //    switch (gestureData.state)
            //    {
            //        case 0:  // gesture detection
            //            if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
            //                jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
            //                Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 165.0f &&
            //                Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 170.0f &&
            //                Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 15.0f)

            //            {
            //                SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
            //            }
            //            break;

            //        case 1:  // gesture complete
            //            bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
            //                jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
            //                Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 165.0f &&
            //                Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 170.0f &&
            //                Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 15.0f;

            //            Vector3 jointPos = jointsPos[gestureData.joint];
            //            CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
            //            break;
            //    }
            //    break;

            //case Gestures.RHSu70:
            //    switch (gestureData.state)
            //    {
            //        case 0:  // gesture detection
            //            if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
            //                jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
            //                Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 150.0f &&
            //                Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 150.0f &&
            //                Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f)

            //            {
            //                SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
            //            }
            //            break;

            //        case 1:  // gesture complete
            //            bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
            //                jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
            //                Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 150.0f &&
            //                Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 150.0f &&
            //                Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f;

            //            Vector3 jointPos = jointsPos[gestureData.joint];
            //            CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, KinectInterop.Constants.PoseCompleteDuration);
            //            break;
            //    }
            //    break;

            case Gestures.RHSu:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                            jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
                            Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex])) > 130.0f &&
                            Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex])) > 150.0f &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f)

                        {
                            SetGestureJoint(ref gestureData, timestamp, rightHandIndex, jointsPos[rightHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        a = Vector3.Angle((jointsPos[leftShoulderIndex] - jointsPos[rightShoulderIndex]), (jointsPos[rightHandIndex] - jointsPos[rightShoulderIndex]));
                        b = Vector3.Angle((jointsPos[rightHandIndex] - jointsPos[rightElbowIndex]), (jointsPos[rightShoulderIndex] - jointsPos[rightElbowIndex]));
                        c = Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex]));
                        bool isInPose = jointsTracked[rightHandIndex] && jointsTracked[rightShoulderIndex] && jointsTracked[rightElbowIndex] && jointsTracked[leftShoulderIndex] &&
                            jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] && a > 130.0f && b > 150.0f && c < 30.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c);
                        List<float> std_angles = new List<float>(); std_angles.Add(180.0f); std_angles.Add(180.0f); std_angles.Add(0.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(50.0f); range_angles.Add(30.0f); range_angles.Add(30.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.LHSu:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        if (jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                            jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex])) > 130.0f &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex])) > 150.0f &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftHandIndex, jointsPos[leftHandIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        bool isInPose = jointsTracked[leftHandIndex] && jointsTracked[leftShoulderIndex] && jointsTracked[leftElbowIndex] && jointsTracked[rightShoulderIndex] &&
                            jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex] &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[leftHandIndex] - jointsPos[leftShoulderIndex])) > 130.0f &&
                            Vector3.Angle((jointsPos[leftHandIndex] - jointsPos[leftElbowIndex]), (jointsPos[leftShoulderIndex] - jointsPos[leftElbowIndex])) > 150.0f &&
                            Vector3.Angle((jointsPos[rightShoulderIndex] - jointsPos[leftShoulderIndex]), (jointsPos[rightHipIndex] - jointsPos[leftHipIndex])) < 30.0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.RKnee:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] && jointsTracked[rightKneeIndex] &&
                            jointsTracked[rightAnkleIndex] && jointsTracked[leftKneeIndex] && jointsTracked[leftAnkleIndex] &&
                            Vector3.Angle(jointsPos[leftHipIndex] - jointsPos[leftKneeIndex], jointsPos[leftAnkleIndex] - jointsPos[leftKneeIndex]) > 150.0f &&
                            Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[rightKneeIndex], new Vector3(0, 1, 0)) > 60.0f && Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[leftHipIndex],
                            Vector3.Cross(new Vector3(0, 1, 0), jointsPos[rightHipIndex] - jointsPos[rightKneeIndex])) < 30.0f &&
                            Vector3.Angle(jointsPos[rightKneeIndex] - jointsPos[rightAnkleIndex], new Vector3(0, 1, 0)) < 30.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, rightKneeIndex, jointsPos[rightKneeIndex]);
                        }
                        break;

                    case 1:
                        a = Vector3.Angle(jointsPos[leftHipIndex] - jointsPos[leftKneeIndex], jointsPos[leftAnkleIndex] - jointsPos[leftKneeIndex]);
                        b = Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[rightKneeIndex], new Vector3(0, 1, 0));
                        c = Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[leftHipIndex], Vector3.Cross(new Vector3(0, 1, 0), jointsPos[rightHipIndex] - jointsPos[rightKneeIndex]));
                        d = Vector3.Angle(jointsPos[rightKneeIndex] - jointsPos[rightAnkleIndex], new Vector3(0, 1, 0));
                        bool isInPose = jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] && jointsTracked[rightKneeIndex] &&
                            jointsTracked[rightAnkleIndex] && jointsTracked[leftKneeIndex] && jointsTracked[leftAnkleIndex] &&
                            a > 150.0f && b > 60.0f && c < 30.0f && d < 30.0f;

                        List<float> angles = new List<float>(); angles.Add(a); angles.Add(b); angles.Add(c); angles.Add(d);
                        List<float> std_angles = new List<float>(); std_angles.Add(180.0f); std_angles.Add(90.0f); std_angles.Add(0.0f); std_angles.Add(0.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(30.0f); range_angles.Add(30.0f); range_angles.Add(30.0f); range_angles.Add(30.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

            case Gestures.LKnee:
                switch (gestureData.state)
                {
                    case 0:
                        if (jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] && jointsTracked[leftKneeIndex] &&
                            jointsTracked[leftAnkleIndex] && jointsTracked[rightKneeIndex] && jointsTracked[rightAnkleIndex] &&
                            Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[rightKneeIndex], jointsPos[rightAnkleIndex] - jointsPos[rightKneeIndex]) > 150.0f &&
                            Vector3.Angle(jointsPos[leftHipIndex] - jointsPos[leftKneeIndex], new Vector3(0, 1, 0)) > 60.0f && Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[leftHipIndex],
                            Vector3.Cross(new Vector3(0, 1, 0), jointsPos[leftHipIndex] - jointsPos[leftKneeIndex])) < 30.0f &&
                            Vector3.Angle(jointsPos[leftKneeIndex] - jointsPos[leftAnkleIndex], new Vector3(0, 1, 0)) < 30.0f)
                        {
                            SetGestureJoint(ref gestureData, timestamp, leftKneeIndex, jointsPos[leftKneeIndex]);
                        }
                        break;

                    case 1:
                        bool isInPose = jointsTracked[leftHipIndex] && jointsTracked[rightHipIndex] && jointsTracked[leftKneeIndex] &&
                            jointsTracked[leftAnkleIndex] && jointsTracked[rightKneeIndex] && jointsTracked[rightAnkleIndex] &&
                            Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[rightKneeIndex], jointsPos[rightAnkleIndex] - jointsPos[rightKneeIndex]) > 150.0f &&
                            Vector3.Angle(jointsPos[leftHipIndex] - jointsPos[leftKneeIndex], new Vector3(0, 1, 0)) > 60.0f && Vector3.Angle(jointsPos[rightHipIndex] - jointsPos[leftHipIndex],
                            Vector3.Cross(new Vector3(0, 1, 0), jointsPos[leftHipIndex] - jointsPos[leftKneeIndex])) < 30.0f &&
                            Vector3.Angle(jointsPos[leftKneeIndex] - jointsPos[leftAnkleIndex], new Vector3(0, 1, 0)) < 30.0f;

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;


            case Gestures.Squat2:
                switch (gestureData.state)
                {
                    case 0:  // gesture detection
                        float tempangle = Vector3.Angle((jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex]), new Vector3(0, 1, 0));
                        if (jointsTracked[rightKneeIndex] && jointsTracked[leftKneeIndex] && jointsTracked[rightHipIndex] && jointsTracked[leftHipIndex]
                            && jointsTracked[rightAnkleIndex] && jointsTracked[leftAnkleIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[hipCenterIndex] &&
                          (Vector3.Angle((jointsPos[rightHipIndex] - jointsPos[rightKneeIndex]), (jointsPos[rightAnkleIndex] - jointsPos[rightKneeIndex])) < 160.0f) &&
                          (Vector3.Angle((jointsPos[leftHipIndex] - jointsPos[leftKneeIndex]), (jointsPos[leftKneeIndex] - jointsPos[leftAnkleIndex])) < 160.0f) &&
                           tempangle > 20.0f && tempangle < 40.0f)

                        {
                            SetGestureJoint(ref gestureData, timestamp, hipCenterIndex, jointsPos[hipCenterIndex]);
                        }
                        break;

                    case 1:  // gesture complete
                        tempangle = Vector3.Angle((jointsPos[shoulderCenterIndex] - jointsPos[hipCenterIndex]), new Vector3(0, 1, 0));
                        a = Vector3.Angle((jointsPos[rightHipIndex] - jointsPos[rightKneeIndex]), (jointsPos[rightAnkleIndex] - jointsPos[rightKneeIndex]));
                        b = Vector3.Angle((jointsPos[leftHipIndex] - jointsPos[leftKneeIndex]), (jointsPos[leftKneeIndex] - jointsPos[leftAnkleIndex]));
                        bool isInPose = jointsTracked[rightHipIndex] && jointsTracked[rightAnkleIndex] && jointsTracked[rightKneeIndex] && jointsTracked[leftKneeIndex] &&
                             jointsTracked[leftHipIndex] && jointsTracked[leftAnkleIndex] && jointsTracked[shoulderCenterIndex] && jointsTracked[hipCenterIndex] &&
                          a < 160.0f && b < 160.0f && tempangle > 20.0f && tempangle < 40.0f;


                        List<float> angles = new List<float>(); angles.Add(tempangle); angles.Add(a); angles.Add(b);
                        List<float> std_angles = new List<float>(); std_angles.Add(40.0f); std_angles.Add(90.0f); std_angles.Add(90.0f);
                        List<float> range_angles = new List<float>(); range_angles.Add(20.0f); range_angles.Add(70.0f); range_angles.Add(70.0f);

                        //GestureScoringListner gestureScoreingListner = GetComponent<GestureScoringListner>();
                        //gestureScoreingListner.GestureScoring(angles, std_angles, range_angles);

                        Vector3 jointPos = jointsPos[gestureData.joint];
                        CheckPoseComplete(ref gestureData, timestamp, jointPos, isInPose, PoseCompleteDuration);
                        break;
                }
                break;

        }
    }

}
