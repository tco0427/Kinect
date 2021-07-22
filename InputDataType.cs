using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Kinect;
using System.Runtime.Serialization;

///Kinect 2.0 SDK (25 JointType https://msdn.microsoft.com/en-us/library/microsoft.kinect.jointtype.aspx)
///Kinect 1.8 SDK (20 JointType https://msdn.microsoft.com/en-us/library/jj131025.aspx)

namespace KinectModule
{

    /// <summary>
    /// 스켈레톤 선별 함수
    /// <param name="s">스켈레톤 데이터 배열</param>
    /// <returns>선별된 스켈레톤 데이터</returns>
    /// </summary>
    public delegate Body[] SkeletonSelectFunc(Body[] s);

    /// <summary>
    /// 스켈레톤 교정 함수
    /// <param name="s">스켈레톤 데이터 배열</param>
    /// <returns>선별된 스켈레톤 데이터</returns>
    /// </summary>
    public delegate Dictionary<int, XSkeleton> SkeletonCorrectFunc(Body[] s);

    /// <summary>
    /// 스켈레톤 전송 함수
    /// <param name="s">스켈레톤 데이터 배열</param>
    /// </summary>
    public delegate void SkeletonSendFunc(Dictionary<int, XSkeleton> s);


    /// <summary>
    /// 데이터를 전달받을 저장소의 데이터 저장 함수의 유형
    /// <param name="key">클라이언트 호스트네임 & 호스트의 개체식별코드 쌍</param>
    /// <param name="s">스켈레톤 데이터</param>
    /// <returns>저장소에서 지정한 데이터의 일련번호(ID)</returns>
    /// </summary>
    public delegate int PoolingFunc(SkeletonKey key, XSkeleton s);

    /// <summary>
    /// 스켈레톤 교정 함수
    /// <param name="s">스켈레톤 데이터 배열</param>
    /// <returns>선별된 스켈레톤 데이터</returns>
    /// </summary>
    //public delegate Dictionary<int, InputMessage> SkeletonAnalyzeFunc(Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> input);
    public delegate Dictionary<int, KinectPoseInputMessage> SkeletonAnalyzeFunc(Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> input);

    /// <summary>
    /// 이벤트의 시작이나 종료시, 다른 컨트롤간의 중계를 위한 함수
    /// </summary>
    public delegate void TriggerFunc();

    /// <summary>
    /// 분류기의 일반적인 모양. 키넥트에서 쓰이는 것.
    /// </summary>
    abstract class Classifier
    {
        protected Dictionary<int, List<DataPair<DateTime, XSkeleton>>> skeletonStream;
        //protected Dictionary<string, ActionInformation> actions;

        private Mutex skeletonSyncMutex;

        private double streamRate;
        public double StreamRate
        {
            get
            {
                return streamRate;
            }
        }

        public Classifier()
        {
            skeletonSyncMutex = new Mutex(false, "ClassifierSync");
            skeletonStream = new Dictionary<int, List<DataPair<DateTime, XSkeleton>>>();
            //actions = new Dictionary<string, ActionInformation>();
            streamRate = 0;
        }

        ~Classifier()
        {
            skeletonSyncMutex.ReleaseMutex();
        }

        public void InputStream(int id, XSkeleton skeleton)
        {
            skeletonSyncMutex.WaitOne();

            if (!skeletonStream.ContainsKey(id))
                skeletonStream.Add(id, new List<DataPair<DateTime, XSkeleton>>());
            skeletonStream[id].Add(new DataPair<DateTime, XSkeleton>(DateTime.Now, skeleton));

            skeletonSyncMutex.ReleaseMutex();
        }

        public void Update()
        {
            Mutex _syncMutex = new Mutex(false, "ClassifierSync");
            _syncMutex.WaitOne();

            DateTime _time = DateTime.Now;
            //List<DataPair<DateTime, XSkeleton>> _removes = new List<DataPair<DateTime, XSkeleton>>();
            int _removeNum;
            List<int> _rId = new List<int>();
            foreach (int _id in skeletonStream.Keys)
            {
                _removeNum = 0;
                foreach (DataPair<DateTime, XSkeleton> _dsPair in skeletonStream[_id])
                {
                    double _delta = _time.TimeOfDay.TotalSeconds - _dsPair.First.TimeOfDay.TotalSeconds;
                    if (_delta > streamRate)
                        _removeNum++;
                    else
                        break;
                }
                skeletonStream[_id].RemoveRange(0, _removeNum);

                if (skeletonStream[_id].Count <= 0)
                    _rId.Add(_id);
            }
            foreach (int _id in _rId)
                skeletonStream.Remove(_id);

            _syncMutex.ReleaseMutex();
        }

        /*public void SetupPoseInformation(ActionInformation pose)
        {
            if (actions.ContainsKey(pose.Name))
                actions[pose.Name] = pose;
            else
                actions.Add(pose.Name, pose);

            if (pose.SensitivityRate > streamRate)
                streamRate = pose.SensitivityRate;
        }*/

        public virtual void ClearAll()
        {
            //actions.Clear();
            skeletonStream.Clear();
            streamRate = 0;
        }

        public abstract Dictionary<int, Dictionary<string, ActionValue>> Recognition();
    }

    /// <summary>
    /// 수정된 라이브러리에서 사용되는 Joint Type
    /// <para>X, Y, Z의 3차원 데이터로, 합과 차, 벡터의 길이 등의 간단한 연산이 수행가능하고, bitstream으로 변환 가능</para>
    /// </summary>
    public class XJoint : ICloneable
    {
        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
        }
        public double Z
        {
            get;
            set;
        }

        public XJoint()
        {
            this.X = 0;
            this.Y = 0;
            this.Z = 0;
        }

        public XJoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public static XJoint Forward
        {
            get
            {
                return new XJoint(0, 0, 1);
            }
        }

        public static XJoint Back
        {
            get
            {
                return new XJoint(0, 0, -1);
            }
        }

        public static XJoint Left
        {
            get
            {
                return new XJoint(-1, 0, 0);
            }
        }

        public static XJoint Right
        {
            get
            {
                return new XJoint(1, 0, 0);
            }
        }

        public static XJoint Up
        {
            get
            {
                return new XJoint(0, 1, 0);
            }
        }

        public static XJoint Down
        {
            get
            {
                return new XJoint(0, -1, 0);
            }
        }

        #region ICloneable Members
        public object Clone()
        {
            return new XJoint() { X = this.X, Y = this.Y, Z = this.Z };
        }
        #endregion

        public static implicit operator XJoint(Microsoft.Kinect.Joint joint)
        {
            XJoint xJoint = new XJoint();
            xJoint.X = joint.Position.X;
            xJoint.Y = joint.Position.Y;
            xJoint.Z = joint.Position.Z;

            return xJoint;
        }

        public static implicit operator XJoint(byte[] data)
        {
            XJoint xJoint = new XJoint();
            xJoint.X = System.BitConverter.ToDouble(data, 0);
            xJoint.Y = System.BitConverter.ToDouble(data, sizeof(double));
            xJoint.Z = System.BitConverter.ToDouble(data, sizeof(double) * 2);

            return xJoint;
        }

        public static implicit operator byte[](XJoint data)
        {
            byte[] x = System.BitConverter.GetBytes(data.X);
            byte[] y = System.BitConverter.GetBytes(data.Y);
            byte[] z = System.BitConverter.GetBytes(data.Z);
            byte[] result = new byte[y.Length * 3];

            Array.Copy(x, 0, result, 0, x.Length);
            Array.Copy(y, 0, result, x.Length, y.Length);
            Array.Copy(z, 0, result, x.Length + y.Length, z.Length);

            return result;
        }

        public static implicit operator Point(XJoint data)
        {
            Point point = new Point();
            point.X = data.X;
            point.Y = data.Y;
            point.Z = data.Z;

            return point;
        }

        public static XJoint operator +(XJoint l, XJoint r)
        {
            XJoint result = new XJoint();
            result.X = l.X + r.X;
            result.Y = l.Y + r.Y;
            result.Z = l.Z + r.Z;
            return result;
        }

        public static XJoint operator -(XJoint l, XJoint r)
        {
            XJoint result = new XJoint();
            result.X = l.X - r.X;
            result.Y = l.Y - r.Y;
            result.Z = l.Z - r.Z;
            return result;
        }

        public static XJoint operator /(XJoint l, double scalar)
        {
            XJoint result = new XJoint();
            if (scalar != 0)
            {
                result.X = l.X / scalar;
                result.Y = l.Y / scalar;
                result.Z = l.Z / scalar;
            }

            return result;
        }

        public static XJoint operator *(XJoint l, double scalar)
        {
            XJoint result = new XJoint();
            result.X = l.X * scalar;
            result.Y = l.Y * scalar;
            result.Z = l.Z * scalar;

            return result;
        }

        public double Length
        {
            get { return System.Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public double this[int key]
        {
            get
            {
                if (key == 0)
                    return X;
                if (key == 1)
                    return Y;
                if (key == 2)
                    return Z;
                return 0;
            }
            set
            {
                if (key == 0)
                    X = value;
                if (key == 1)
                    Y = value;
                if (key == 2)
                    Z = value;
            }
        }

        public double Distance(XJoint pos)
        {
            if (pos == this)
                return 0;

            XJoint dist = this - pos;
            return dist.Length;
        }

        public void Normalize()
        {
            double _len = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (_len <= 0)
                return;
            X /= _len;
            Y /= _len;
            Z /= _len;
        }

        public double Difference(XJoint pos)
        {
            double result = 1.0;
            double temp = 0;
            temp = this.X - pos.X;
            result -= temp * temp;
            temp = this.Y - pos.Y;
            result -= temp * temp;
            temp = this.Z - pos.Z;
            result -= temp * temp;

            return result;
        }

        public static double Angle(XJoint from, XJoint to)
        {
            from.Normalize();
            to.Normalize();
            double dot = Dot(from, to);
            double radian = System.Math.Acos(dot);
            return (radian * 180.0f / System.Math.PI); // RadianToDegree
        }
        public static double Theta(XJoint v) // polar angle (0 < theta < 180)
        {
            double u = v.Z / v.Length;
            double radian = System.Math.Acos(u);
            return (radian * 180.0f / System.Math.PI); // RadianToDegree
        }
        public static double Phi(XJoint v) // azimuth angle (0 < phi < 360)
        {
            double radian = System.Math.Atan2(v.Y, v.X);
            return (radian * 180.0f / System.Math.PI); // RadianToDegree
        }

        public static XJoint Cross(XJoint l, XJoint r)
        {
            XJoint result = new XJoint();
            result.X = l.Y * r.Z - r.Y * l.Z;
            result.Y = -l.X * r.Z + r.X * l.Z;
            result.Z = l.X * r.Y - r.X * l.Y;
            return result;
        }

        public static double Dot(XJoint l, XJoint r)
        {
            return (l.X * r.X + l.Y * r.Y + l.Z * r.Z);
        }

        public override string ToString()
        {
            //return string.Format("X={0:0.##} Y={1:0.##} Z={2:0.##}", this.X, this.Y, this.Z);
            return "X=" + this.X + " Y=" + this.Y + " Z=" + this.Z;
        }
    }

    /// <summary>
    /// 새롭게 간소화되어 정의된 스켈레톤 데이터. 
    /// <para>index 및 XJointName으로 개별 관절에 접근이 가능하고 bitstream으로 변환 가능</para>
    /// </summary>
    public class XSkeleton : ISerializable
    {
        public DateTime CurrentTime
        {
            get;
            set;
        }

        XJoint[] joints;
        public XJoint[] xJoints{
            get {return joints;}
        }
        public XSkeleton()
        {
            CurrentTime = DateTime.Now;

            joints = new XJoint[25];
            for (int i = 0; i < 25; i++)
                joints[i] = new XJoint();
        }

        public XSkeleton(XSkeleton other)
        {
            CurrentTime = other.CurrentTime;
            if (joints == null)
                joints = new XJoint[25];
            for (int i = 0; i < 25; i++)
                joints[i] = other.joints[i];
        }

        // ISerializable
        protected XSkeleton(SerializationInfo info, StreamingContext context)
        {
            CurrentTime = info.GetDateTime("CurrentTime");
            for (int i = 0; i < 25; i++)
            {
                joints[i].X = info.GetDouble("X" + i.ToString());
                joints[i].Y = info.GetDouble("Y" + i.ToString());
                joints[i].Z = info.GetDouble("Z" + i.ToString());
            }
        }

        // ISerializable
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CurrentTime", CurrentTime);
            for (int i = 0; i < 25; i++)
            {
                info.AddValue("X" + i.ToString(), joints[i].X);
                info.AddValue("Y" + i.ToString(), joints[i].Y);
                info.AddValue("Z" + i.ToString(), joints[i].Z);
            }
        }

        public int Length
        {
            get
            {
                return joints.Length;
            }
        }

        public XJoint this[int key]
        {
            get
            {
                if (key < 0 || key >= Length)
                    return null;
                return this.joints[key];
            }
            set
            {
                if (key < 0 || key >= Length)
                    return;
                this.joints[key] = value;
            }
        }

        public XJoint this[Microsoft.Kinect.JointType key]
        {
            get
            {
                return this.joints[(int)key];
            }
            set
            {
                this.joints[(int)key] = value;
            }
        }

        public void WidthPosCorrection(double xCorr)
        {
            for (int num = 0; num < 25; num++)
                joints[num].X += xCorr;
        }

        public static implicit operator XSkeleton(byte[] source)
        {
            XSkeleton value = new XSkeleton();
            if (source.Length < 25 * 3 * sizeof(double))
                return value;

            for (int partNum = 0; partNum < 25; partNum++)
            {
                byte[] parts = new byte[3 * sizeof(double)];
                Array.Copy(source, partNum * 3 * sizeof(double), parts, 0, sizeof(double) * 3);

                value[partNum] = parts;
            }

            return value;
        }

        public static implicit operator XSkeleton(Body source)
        {
            XSkeleton value = new XSkeleton();

            for (int i = 0; i < source.Joints.Count; i++)
                value[i] = source.Joints[(JointType)i];

            return value;
        }

        public static implicit operator byte[](XSkeleton source)
        {
            byte[] value = new byte[25 * 3 * sizeof(double)];
            for (int partNum = 0; partNum < 25; partNum++)
            {
                byte[] parts = source[partNum];
                Array.Copy(parts, 0, value, partNum * 3 * sizeof(double), sizeof(double) * 3);
            }

            return value;
        }

        public static XSkeleton operator +(XSkeleton data1, XSkeleton data2)
        {
            XSkeleton result = new XSkeleton();
            for (int partNum = 0; partNum < 25; partNum++)
                result[partNum] = data1[partNum] + data2[partNum];

            return result;
        }

        public static XSkeleton operator +(XSkeleton data1, XJoint data2)
        {
            XSkeleton result = new XSkeleton();
            for (int partNum = 0; partNum < 25; partNum++)
                result[partNum] = data1[partNum] + data2;

            return result;
        }

        public static XSkeleton operator -(XSkeleton data1, XJoint data2)
        {
            XSkeleton result = new XSkeleton();
            for (int partNum = 0; partNum < 25; partNum++)
                result[partNum] = data1[partNum] - data2;

            return result;
        }

        public static XSkeleton operator /(XSkeleton data1, double data2)
        {
            XSkeleton result = new XSkeleton();

            for (int partNum = 0; partNum < 25; partNum++)
                result[partNum] = data1[partNum] / data2;

            return result;
        }

        public static XSkeleton Mean(List<XSkeleton> list)
        {
            XSkeleton _result = new XSkeleton();
            foreach (XSkeleton _item in list)
                _result += _item;
            _result /= list.Count;

            return _result;
        }

        public double Commotion(XSkeleton data)
        {
            double _result = 0;

            for (int _i = 0; _i < 25; _i++)
                _result += (this[_i] - data[_i]).Length;

            return _result / 25;
        }

        public PoseStructure CreateMatchPose(PoseStructure source)
        {
            PoseStructure result = new PoseStructure();

            for (int _i = 0; _i < source.Length; _i++)
                result[source[_i]] = this[source[_i]];
            result.Normalize();
            return result;
        }

        public override string ToString()
        {
            string str = CurrentTime.ToString();
            for (int i = 0; i < 25; i++)
            {
                str += string.Format(",{0:0.###},{1:0.###},{2:0.###}", joints[i].X, joints[i].Y, joints[i].Z);
            }
            return str;
        }
    }

    /// <summary>
    /// 두개의 서로 다른, 혹은 동일한 타입의 데이터를 하나로 묶기 위한 데이터쌍
    /// </summary>
    /// <typeparam name="T">두 데이터 중 첫번째 데이터</typeparam>
    /// <typeparam name="U">두 데이터 중 두번째 데이터</typeparam>
    class DataPair<T, U>
    {
        public T First { get; set; }
        public U Second { get; set; }

        public DataPair()
        {
        }

        public DataPair(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public override System.Boolean Equals(object obj)
        {
            DataPair<T, U> data = obj as DataPair<T, U>;
            if (data == null)
                return false;

            if (data.First.Equals(this.First) && data.Second.Equals(this.Second))
                return true;

            return false;
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }


    /// <summary>
    /// 여러 호스트가 서버에 전송하는 서로 다른 Skeleton 정보를 구분하기 위한 Key
    /// <para>호스트의 Address와 호스트에서 Skeleton에 부여한 식별번호로 구성되어 있으며, 값에 대한 비교연산을 수행하도록 함</para>
    /// </summary>
    public class SkeletonKey
    {
        private string address;
        private int index;

        public string Address
        {
            get { return address; }
        }
        public int Index
        {
            get { return index; }
        }

        public SkeletonKey(string s, int i)
        {
            address = s;
            index = i;
        }

        public static bool operator ==(SkeletonKey data1, SkeletonKey data2)
        {
            if (data1.index == data2.index && data1.address == data2.address)
                return true;
            return false;
        }

        public static bool operator !=(SkeletonKey data1, SkeletonKey data2)
        {
            if (data1.index == data2.index && data1.address == data2.address)
                return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is SkeletonKey)
            {
                SkeletonKey data = obj as SkeletonKey;
                if (this == data)
                    return true;
                else
                    return false;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return address.GetHashCode() ^ index.GetHashCode();
        }
    }

    /// <summary>
    /// 포즈를 적용할 신체의 부위
    /// <para>포즈의 설정 영역을 제어하기 위함</para>
    /// </summary>
    public enum PosePart
    {
        LeftHand = 0,
        RightHand = 1,
        Body = 2,
        LeftLeg = 3,
        RightLeg = 4,
        UpperBody = 5,
        LowerBody = 6,
        FullBody = 7
    }

    /// <summary>
    /// 정의된/학습된 동작의 정보.
    /// <para>
    /// <see cref="Name"/>
    /// : 동작의 이름을 의미합니다.
    /// </para>
    /// <para>
    /// <see cref="SensitivityTime"/>
    /// : 동작의 민감도를 의미합니다. 특히, 동작를 인식하기 위해 동작을 유지할 최소 시간을 의미합니다.
    /// </para>
    /// <para>
    /// <see cref="SensitivityRate"/>
    /// : 동작의 민감도를 의미합니다. 특히, 동작의 일치율을 의미합니다.
    /// </para>
    /// <para>
    /// <seealso cref="BodyPart"/>
    /// : 정의된 동작가 취해질 신체 부위를 의미합니다.
    /// </para>
    /// </summary>
    public abstract class ActionInformation
    {
        public string Name = "";
        public double SensitivityTime = 1.0;
        public double SensitivityRate = 1.0;
        public PosePart BodyPart = PosePart.LeftHand;

        public abstract void Normalize();

        public abstract ActionValue CheckIdentity(object obj);

        public override string ToString()
        {
            return String.Format("Name: {0}, SensitivityTime: {1}, Rate: {2}, BodyPart: {3}", Name, SensitivityTime, SensitivityRate, BodyPart);
        }
    }

    /// <summary>
    /// 정의된/학습된 포즈의 정보.
    /// <para>
    /// <see cref="PoseStruct"/>
    /// : 포즈에 대한 정보가 입력됩니다.
    /// </para>
    /// </summary>
    public class PoseInformation : ActionInformation
    {
        public PoseStructure PoseStruct = new PoseStructure();

        public override void Normalize()
        {
            PoseStruct.Normalize();
        }

        public override ActionValue CheckIdentity(object pose)
        {
            PoseStructure _pose = (PoseStructure)pose;
            PoseValue _pV = null;
            double _result = PoseStruct.Compare(_pose);
            if (_result > SensitivityRate)
            {
                _pV = new PoseValue();
                _pV.Name = this.Name;
                _pV.PoseRate = _result;
            }
            return _pV;
        }
    }

    public abstract class ActionStructure
    {
        public abstract void Normalize();
        public abstract bool IsMatching(ActionStructure actionS);
        public abstract double Compare(ActionStructure actionS);
    }

    public class PoseStructure : ActionStructure
    {
        private int jointNum = 0;
        public int Length
        {
            get
            {
                return jointNum;
            }
        }
        private Dictionary<JointType, XJoint> poseJoints = new Dictionary<JointType, XJoint>();
        private Dictionary<int, JointType> jointIndex = new Dictionary<int, JointType>();
        public XJoint this[JointType key]
        {
            get
            {
                if (poseJoints.ContainsKey(key))
                    return poseJoints[key];
                return null;
            }
            set
            {
                if (value == null)
                {
                    if (poseJoints.ContainsKey(key))
                    {
                        poseJoints.Remove(key);
                        foreach (KeyValuePair<int, JointType> _pair in jointIndex)
                        {
                            if (_pair.Value == key)
                            {
                                jointIndex.Remove(_pair.Key);
                                break;
                            }
                        }
                        jointNum--;
                    }
                    return;
                }
                if (poseJoints.ContainsKey(key))
                    poseJoints[key] = value;
                else
                {
                    poseJoints.Add(key, value);
                    jointIndex.Add(jointNum++, key);
                }
            }
        }

        public JointType this[int index]
        {
            get
            {
                if (index < 0 || index >= jointNum)
                    return 0;

                return jointIndex[index];
            }
        }

        public override void Normalize()
        {
            if (poseJoints.Count <= 0)
                return;
            XJoint _standard = this[this[0]];
            for (int _i = 0; _i < Length; _i++)
                this[this[_i]] -= _standard;
        }

        public override bool IsMatching(ActionStructure other)
        {
            PoseStructure _other = (PoseStructure)other;
            if (Length != _other.Length)
                return false;

            for (int i = 0; i < Length; i++)
            {
                if (this[i] != _other[i])
                    return false;
            }

            return true;
        }

        public override double Compare(ActionStructure other)
        {
            if (!IsMatching(other))
                return 0;

            PoseStructure _ps = (PoseStructure)other;
            double result = 1.0;

            for (int i = 0; i < Length; i++)
            {
                XJoint _this = (XJoint)this[this[i]].Clone();
                XJoint _other = (XJoint)_ps[_ps[i]].Clone();
                _this.Normalize();
                _other.Normalize();
                double _check = _this.Difference(_other);
                if (result > _check)
                    result = _check;
                if (result <= 0)
                {
                    result = 0;
                    break;
                }
            }

            return result;
        }
    }

    /// <summary>
    /// 행해 지고 있는 액션의 확인 결과입니다.
    /// </summary>
    public abstract class ActionValue
    {
        string name = "";
        int userID = 0;
       // XSkeleton skeleton = new XSkeleton();
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int UserID
        {
            get { return userID; }
            set { userID = value; }
        }/*
        public XSkeleton Skeleton
        {
            get { return skeleton; }
            set { skeleton = value; }
        }*/
    }

    /// <summary>
    /// 행해 지고 있는 포즈의 확인 결과입니다.
    /// </summary>
    public class PoseValue : ActionValue
    {
        private double poseRate = 0;
        public double PoseRate
        {
            get
            {
                return poseRate;
            }
            set
            {
                poseRate = value;
            }
        }

        public static implicit operator byte[](PoseValue pv)
        {
            //포즈이름
            byte[] _name = Encoding.UTF8.GetBytes(pv.Name);
            //이름 길이
            byte[] _nameLen = BitConverter.GetBytes(_name.Length);
            //아이디
            byte[] _id = BitConverter.GetBytes(pv.UserID);
            //데이터
            byte[] _value = BitConverter.GetBytes(pv.PoseRate);
            //스켈레톤
            //byte[] _skeleton = pv.Skeleton;

            //전체 데이터 길이
            int _totalLen = sizeof(Int32) + _id.Length + _name.Length + _nameLen.Length + _value.Length;// +_skeleton.Length;
            byte[] _total = BitConverter.GetBytes(_totalLen);

            //결과물
            byte[] _fullData = new byte[_totalLen];
            int _copyPos = 0;
            //결과물에 데이터 삽입
            //totalLen
            Array.Copy(_total, 0, _fullData, 0, _total.Length);
            _copyPos = _total.Length;
            //nameLen
            Array.Copy(_nameLen, 0, _fullData, _copyPos, _nameLen.Length);
            _copyPos += _nameLen.Length;
            //name
            Array.Copy(_name, 0, _fullData, _copyPos, _name.Length);
            _copyPos += _name.Length;
            //_id
            Array.Copy(_id, 0, _fullData, _copyPos, _id.Length);
            _copyPos += _id.Length;
            //_value
            Array.Copy(_value, 0, _fullData, _copyPos, _value.Length);
            _copyPos += _value.Length;/*
            //_skeleton
            Array.Copy(_skeleton, 0, _fullData, _copyPos, _skeleton.Length);
            _copyPos += _skeleton.Length;*/

            return _fullData;
        }

        public static implicit operator PoseValue(byte[] value)
        {
            int _startPos = sizeof(Int32);
            //포즈이름길이
            int _nameLen = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(Int32);
            //포즈이름
            string _name = Encoding.UTF8.GetString(value, sizeof(Int32) * 2, _nameLen);
            _startPos += _nameLen;
            //ID
            int _id = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(Int32);
            //Value
            double _value = BitConverter.ToDouble(value, _startPos);
            _startPos += sizeof(double);
            
            //Skeleton
            byte[] _skelBuf = new byte[25 * sizeof(double) * 3];
            Array.Copy(value, _startPos, _skelBuf, 0, _skelBuf.Length);
            
            //적용
            PoseValue result = new PoseValue();
            result.Name = _name;
            result.UserID = _id;
            result.PoseRate = _value;
            //result.Skeleton = _skelBuf;

            return result;
        }
    }

    public class UserInformation
    {
        private int id = 0;
        public int UserID
        {
            set { id = value; }
            get { return id; }
        }
        Dictionary<string, PoseValue> poseValues = new Dictionary<string, PoseValue>();
        int nextIndex = 0;
        Dictionary<int, string> poseKeys = new Dictionary<int, string>();
        public PoseValue this[string key]
        {
            set
            {
                if (poseValues.ContainsKey(key))
                    poseValues[key] = value;
                else
                {

                    poseValues.Add(key, value);
                    poseKeys.Add(nextIndex++, key);
                }
            }
            get
            {
                if (key != "" && poseValues.ContainsKey(key))
                    return poseValues[key];
                return null;
            }
        }
        public string this[int keyCount]
        {
            get
            {
                if (keyCount < 0 || keyCount >= nextIndex)
                    return "";

                return poseKeys[keyCount];
            }
        }

        public int PoseCount
        {
            get
            {
                return poseValues.Count;
            }
        }

        XSkeleton skeleton = new XSkeleton();
        public XSkeleton UserSkeleton
        {
            set { skeleton = value; }
            get { return skeleton; }
        }

        public static implicit operator byte[](UserInformation ui)
        {   //사용자 ID
            byte[] _id = BitConverter.GetBytes(ui.UserID);
            //사용자의 Poses
            List<byte[]> _poses = new List<byte[]>();
            int _poseMemLen = 0;
            for (int i = 0; i < ui.PoseCount; i++)
            {
                if (ui[ui[i]] != null)
                {
                    byte[] _poseBuf = ui[ui[i]];
                    _poses.Add(_poseBuf);
                    _poseMemLen += _poseBuf.Length;
                }
            }
            //사용자의 Pose 수
            byte[] _poseCount = BitConverter.GetBytes(_poses.Count);
            //사용자의 스켈레톤 정보
            byte[] _skeleton = ui.UserSkeleton;
            //총 길이
            int _totalLen = sizeof(int) + _id.Length + _poseCount.Length + _poseMemLen + _skeleton.Length;
            byte[] _total = BitConverter.GetBytes(_totalLen);

            //획득한 정보를 통합하여 반환
            byte[] _result = new byte[_totalLen];
            int _copyPose = 0;
            //총길이 삽입
            Array.Copy(_total, _result, _total.Length);
            _copyPose += _total.Length;
            //ID
            Array.Copy(_id, 0, _result, _copyPose, _id.Length);
            _copyPose += _id.Length;
            //Pose 수량
            Array.Copy(_poseCount, 0, _result, _copyPose, _poseCount.Length);
            _copyPose += _poseCount.Length;
            //Poses 입력
            foreach (byte[] _buf in _poses)
            {
                Array.Copy(_buf, 0, _result, _copyPose, _buf.Length);
                _copyPose += _buf.Length;
            }
            //Skeleton 입력
            Array.Copy(_skeleton, 0, _result, _copyPose, _skeleton.Length);

            return _result;
        }

        public static implicit operator UserInformation(byte[] value)
        {
            UserInformation _result = new UserInformation();

            int _startPos = sizeof(int);
            //ID
            int _id = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //포즈 수량
            int _poseCount = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //포즈 정보
            for (int i = 0; i < _poseCount; i++)
            {
                int _poseLen = BitConverter.ToInt32(value, _startPos);
                byte[] _poseBuf = new byte[_poseLen];
                Array.Copy(value, _startPos, _poseBuf, 0, _poseLen);
                _startPos += _poseLen;
                PoseValue _pose = _poseBuf;
                _result[_pose.Name] = _pose;
            }

            //골격정보
            byte[] _skelBuf = new byte[sizeof(double) * 3 * 25];
            Array.Copy(value, _startPos, _skelBuf, 0, _skelBuf.Length);
            _result.UserSkeleton = _skelBuf;

            _result.UserID = _id;

            return _result;
        }
    }

    class Quat
    {
        private double[] data;

        public double X
        {
            get{return data[0];}
        }
        public double Y
        {
            get { return data[1]; }
        }
        public double Z
        {
            get { return data[2]; }
        }
        public double W
        {
            get { return data[3]; }
        }
        public Quat()
        {
            data = new double[4];
            for (int i = 0; i < 4; i++)
                data[i] = 0;
        }

        public double Length
        {
            get
            {
                return Math.Sqrt(Length2);
            }
        }

        public double Length2
        {
            get
            {
                return X * X + Y * Y + Z * Z + W * W;
            }
        }

        public void Rotate(double radAngle, double x, double y, double z)
        {
            const double _epsilon = 0.0001;

            double _length = Math.Sqrt(x * x + y * y + z * z);
            if (_length < _epsilon)
            {
                for (int i = 0; i < 4; i++)
                    data[i] = 0;
                return;
            }

            double _invLen = 1.0 / _length;
            double _cosHalfAngle = Math.Cos(radAngle * 0.5);
            double _sinHalfAngle = Math.Sin(radAngle * 0.5);

            data[0] = x * _sinHalfAngle * _invLen;
            data[1] = y * _sinHalfAngle * _invLen;
            data[2] = z * _sinHalfAngle * _invLen;
            data[3] = _cosHalfAngle;
        }
    }

    class Matrix
    {
        double[] data;

        public Matrix()
        {
            data = new double[16];
            for (int i = 0; i < 16; ++i)
                data[i] = 0;
            data[0] = 1;
            data[5] = 1;
            data[10] = 1;
            data[15] = 1;
        }

        public void Identity()
        {
            for (int i = 0; i < 16; ++i)
                data[i] = 0;
            data[0] = 1;
            data[5] = 1;
            data[10] = 1;
            data[15] = 1;
        }

        public void Rotate(Quat rotation)
        {
            Identity();

            double _len2 = rotation.Length2;
            if (_len2 == 0 || _len2 == double.NaN)
            {
                for (int i = 0; i < 16; i++)
                    data[i] = 0;
                return;
            }

            double _rlen2 = _len2 == 1.0 ? 2.0 : 2 / _len2;

            double x2, y2, z2, xx, xy, xz, yy, yz, zz, wx, wy, wz;
            x2 = _rlen2 * rotation.X;
            y2 = _rlen2 * rotation.Y;
            z2 = _rlen2 * rotation.Z;

            xx = rotation.X * x2;
            xy = rotation.X * y2;
            xz = rotation.X * z2;

            yy = rotation.Y * y2;
            yz = rotation.Y * z2;
            zz = rotation.Z * z2;

            wx = rotation.W * x2;
            wy = rotation.W * y2;
            wz = rotation.W * z2;

            data[0] = 1.0 - (yy + zz);
            data[4] = xy - wz;
            data[8] = xz + wy;

            data[1] = xy + wz;
            data[5] = 1.0 - (xx + zz);
            data[9] = yz - wx;

            data[2] = xz - wy;
            data[6] = yz + wx;
            data[10] = 1.0 - (xx + yy);

            data[3] = 0;
            data[7] = 0;
            data[11] = 0;

            data[12] = 0;
            data[13] = 0;
            data[14] = 0;
            data[15] = 1.0;
        }

        public void Rotate(double radAngle, double x, double y, double z)
        {
            Quat quat = new Quat();
            quat.Rotate(radAngle, x, y, z);

            Rotate(quat);
        }

        public static XJoint operator *(Matrix mat, XJoint joint)
        {
            XJoint _joint = new XJoint();

            double _d = 1.0 / (mat.data[12] * joint.X + mat.data[13] * joint.Y + mat.data[14] * joint.Z + mat.data[15]);
            _joint.X = (mat.data[0] * joint.X + mat.data[1] * joint.Y + mat.data[2] * joint.Z + mat.data[3]) * _d;
            _joint.Y = (mat.data[4] * joint.X + mat.data[5] * joint.Y + mat.data[6] * joint.Z + mat.data[7]) * _d;
            _joint.Z = (mat.data[8] * joint.X + mat.data[9] * joint.Y + mat.data[10] * joint.Z + mat.data[11]) * _d;

            return _joint;
        }

        public static XSkeleton operator *(Matrix mat, XSkeleton skel)
        {
            XSkeleton _newSkel = new XSkeleton();

            for (int i = 0; i < skel.Length; i++)
            {
                _newSkel[i] = mat * skel[i];
            }

            return _newSkel;
        }
    }

    /// <summary>
    /// 서로 다른 두 개의 데이터 쌍을 생성
    /// <para>2개의 각각 다른 데이터 타입을 형성할 수 있고, 주소가 아닌 값에 의한 비교를 가능하게 함</para>
    /// </summary>
    public struct ValuePair<T1, T2>
    {
        private T1 data1;
        private T2 data2;

        public T1 First
        {
            get { return data1; }
            set { data1 = value; }
        }
        public T2 Second
        {
            get { return data2; }
            set { data2 = value; }
        }

        public ValuePair(T1 d1, T2 d2)
        {
            data1 = d1;
            data2 = d2;
        }

        public static bool operator ==(ValuePair<T1, T2> data1, ValuePair<T1, T2> data2)
        {
            if (data1.First.Equals(data2.First) && data1.Second.Equals(data2.Second))
                return true;
            return false;
        }

        public static bool operator !=(ValuePair<T1, T2> data1, ValuePair<T1, T2> data2)
        {
            if (data1.First.Equals(data2.First) && data1.Second.Equals(data2.Second))
                return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj is ValuePair<T1, T2>)
            {
                ValuePair<T1, T2> data = (ValuePair<T1, T2>)obj;
                if (this == data)
                    return true;
                else
                    return false;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return First.GetHashCode() ^ Second.GetHashCode();
        }
    }

    public class InputMessageType
    {
        //MessageName
        public const int UM_MULTIINPUT = 0x0400 + 100;
        //MessageType
        public const int NONE = 0;
        public const int BEGIN = 1;
        public const int CONTINUE = 2;
        public const int END = 3;
    }

    public struct Point
    {
        public double X
        {
            get;
            set;
        }

        public double Y
        {
            get;
            set;
        }

        public double Z
        {
            get;
            set;
        }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double this[int key]
        {
            get
            {
                if (key == 0)
                    return X;
                if (key == 1)
                    return Y;
                if (key == 2)
                    return Z;
                return 0;
            }
            set
            {
                if (key == 0)
                    X = value;
                if (key == 1)
                    Y = value;
                if (key == 2)
                    Z = value;
            }
        }

        public double Length
        {
            get { return Math.Sqrt(X * X + Y * Y + Z * Z); }
        }

        public void Normalize()
        {
            double _len = Math.Sqrt(X * X + Y * Y + Z * Z);
            if (_len <= 0)
                return;
            X /= _len;
            Y /= _len;
            Z /= _len;
        }

        public double Distance(Point p)
        {
            Point _gap = this - p;
            return _gap.Length;
        }

        public static double Angle(Point from, Point to)
        {
            from.Normalize();
            to.Normalize();
            double dot = Dot(from, to);
            double radian = System.Math.Acos(dot);
            return (radian * 180.0f / System.Math.PI); // RadianToDegree
        }

        public static Point Cross(Point l, Point r)
        {
            Point result = new Point();
            result.X = l.Y * r.X - r.Y * l.Z;
            result.Y = -l.X * r.Z + r.X * l.Z;
            result.Z = l.X * r.Y - r.X * l.Y;
            return result;
        }

        public static double Dot(Point l, Point r)
        {
            return (l.X * r.X + l.Y * r.Y + l.Z * r.Z);
        }

        public static Point operator -(Point p1, Point p2)
        {
            Point _result = new Point();
            _result.X = p1.X - p2.X;
            _result.Y = p1.Y - p2.Y;
            _result.Z = p1.Z - p2.Z;

            return _result;
        }

        public static Point operator +(Point p1, Point p2)
        {
            Point _result = new Point();
            _result.X = p1.X + p2.X;
            _result.Y = p1.Y + p2.Y;
            _result.Z = p1.Z + p2.Z;

            return _result;
        }

        public static Point operator /(Point l, double scalar)
        {
            Point result = new Point();
            if (scalar != 0)
            {
                result.X = l.X / scalar;
                result.Y = l.Y / scalar;
                result.Z = l.Z / scalar;
            }

            return result;
        }

        public static Point operator *(Point l, double scalar)
        {
            Point result = new Point();
            result.X = l.X * scalar;
            result.Y = l.Y * scalar;
            result.Z = l.Z * scalar;

            return result;
        }

        public static implicit operator byte[](Point point)
        {
            byte[] _x = BitConverter.GetBytes(point.X);
            byte[] _y = BitConverter.GetBytes(point.Y);
            byte[] _z = BitConverter.GetBytes(point.Z);

            byte[] _result = new byte[_x.Length * 3];
            Array.Copy(_x, 0, _result, 0, _x.Length);
            Array.Copy(_y, 0, _result, _x.Length, _y.Length);
            Array.Copy(_z, 0, _result, _x.Length * 2, _z.Length);

            return _result;
        }

        public static implicit operator Point(byte[] buf)
        {
            Point _result = new Point();

            _result.X = BitConverter.ToInt32(buf, 0);
            _result.Y = BitConverter.ToInt32(buf, sizeof(int));
            _result.Z = BitConverter.ToInt32(buf, sizeof(int) * 2);

            return _result;
        }
    }

    public struct InputMessage
    {
        public string DeviceName;
        public string HostName;
        public int UserID;
        public int UsePosition;
        public Point Position;
        public int PointCount;
        public ValuePair<int, Point>[] Points;
        public int EventType;
        public double EventValue;
        public int UIID;

        public InputMessage(string deviceName, string hostName, int userID, int usePosition, Point position, int pointCount, ValuePair<int, Point>[] points, int eventType, double eventValue, int uiID)
        {
            DeviceName = deviceName;
            HostName = hostName;
            UserID = userID;
            UsePosition = usePosition;
            Position = position;
            PointCount = pointCount;
            Points = points;
            EventType = eventType;
            EventValue = eventValue;
            UIID = uiID;
        }

        public static implicit operator byte[](InputMessage ui)
        {   //장치명
            byte[] _deviceName = Encoding.Unicode.GetBytes(ui.DeviceName);
            byte[] _nameLen = BitConverter.GetBytes(_deviceName.Length);
            //호스트명
            byte[] _hostName = Encoding.Unicode.GetBytes(ui.HostName);
            byte[] _hNameLen = BitConverter.GetBytes(_hostName.Length);
            //사용자 ID
            byte[] _id = BitConverter.GetBytes(ui.UserID);
            //포지션 여부
            byte[] _usePosition = BitConverter.GetBytes(ui.UsePosition);
            //포지션
            byte[] _position = ui.Position;
            //포인트 수
            byte[] _pointCount = BitConverter.GetBytes(ui.PointCount);
            //포인트들
            List<byte[]> _points = new List<byte[]>();
            int _pointsLen = 0;
            for (int _i = 0; _i < ui.PointCount; _i++)
            {
                byte[] _pointG = BitConverter.GetBytes(ui.Points[_i].First);
                byte[] _pointBuf = ui.Points[_i].Second;
                byte[] _buf = new byte[_pointG.Length + _pointBuf.Length];

                Array.Copy(_pointG, 0, _buf, 0, _pointG.Length);
                Array.Copy(_pointBuf, 0, _buf, _pointG.Length, _pointBuf.Length);

                _points.Add(_buf);
                _pointsLen += _buf.Length;
            }

            //이벤트
            byte[] _event = BitConverter.GetBytes(ui.EventType);
            //이벤트값
            byte[] _eventValue = BitConverter.GetBytes(ui.EventValue);
            //uiID
            byte[] _uiID = BitConverter.GetBytes(ui.UIID);

            //총 길이
            int _totalLen = sizeof(int) + _nameLen.Length + _deviceName.Length + _hNameLen.Length + _hostName.Length + _id.Length + _usePosition.Length + _position.Length + _pointCount.Length + _pointsLen + _event.Length + _eventValue.Length + _uiID.Length;
            byte[] _total = BitConverter.GetBytes(_totalLen);

            //획득한 정보를 통합하여 반환
            byte[] _result = new byte[_totalLen];
            int _copyPose = 0;
            //총길이 삽입
            Array.Copy(_total, _result, _total.Length);
            _copyPose += _total.Length;
            //장치 길이 삽입
            Array.Copy(_nameLen, 0, _result, _copyPose, _nameLen.Length);
            _copyPose += _nameLen.Length;
            //장치 명 삽입
            Array.Copy(_deviceName, 0, _result, _copyPose, _deviceName.Length);
            _copyPose += _deviceName.Length;
            //호스트 길이 삽입
            Array.Copy(_hNameLen, 0, _result, _copyPose, _hNameLen.Length);
            _copyPose += _hNameLen.Length;
            //호스트 명 삽입
            Array.Copy(_hostName, 0, _result, _copyPose, _hostName.Length);
            _copyPose += _hostName.Length;
            //ID
            Array.Copy(_id, 0, _result, _copyPose, _id.Length);
            _copyPose += _id.Length;
            //포지션여부
            Array.Copy(_usePosition, 0, _result, _copyPose, _usePosition.Length);
            _copyPose += _usePosition.Length;
            //포지션
            Array.Copy(_position, 0, _result, _copyPose, _position.Length);
            _copyPose += _position.Length;
            //포인트수
            Array.Copy(_pointCount, 0, _result, _copyPose, _pointCount.Length);
            _copyPose += _pointCount.Length;
            //포인트들
            foreach (byte[] _buf in _points)
            {
                Array.Copy(_buf, 0, _result, _copyPose, _buf.Length);
                _copyPose += _buf.Length;
            }
            //이벤트
            Array.Copy(_event, 0, _result, _copyPose, _event.Length);
            _copyPose += _event.Length;
            //이벤트값
            Array.Copy(_eventValue, 0, _result, _copyPose, _eventValue.Length);
            _copyPose += _eventValue.Length;
            //UI식별부호
            Array.Copy(_uiID, 0, _result, _copyPose, _uiID.Length);

            return _result;
        }

        public static implicit operator InputMessage(byte[] value)
        {
            InputMessage _result = new InputMessage();

            int _startPos = sizeof(int);
            //장치명
            int _deviceLen = System.BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            byte[] _deviceName = new byte[_deviceLen];
            Array.Copy(value, _startPos, _deviceName, 0, _deviceLen);
            _startPos += _deviceLen;
            _result.DeviceName = Encoding.Unicode.GetString(_deviceName);
            //호스트명
            int _hostLen = System.BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            byte[] _hostName = new byte[_hostLen];
            Array.Copy(value, _startPos, _hostName, 0, _hostLen);
            _startPos += _hostLen;
            _result.HostName = Encoding.Unicode.GetString(_hostName);
            //ID
            _result.UserID = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //포지션여부
            _result.UsePosition = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //포지션
            byte[] _position = new byte[sizeof(double) * 3];
            Array.Copy(value, _startPos, _position, 0, _position.Length);
            _result.Position = _position;
            _startPos += sizeof(double) * 3;
            //포인트수
            _result.PointCount = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //포인트들
            _result.Points = new ValuePair<int, Point>[_result.PointCount];
            for (int _i = 0; _i < _result.PointCount; _i++)
            {
                _result.Points[_i].First = BitConverter.ToInt32(value, _startPos);
                _startPos += sizeof(int);
                Array.Copy(value, _startPos, _position, 0, _position.Length);
                _result.Points[_i].Second = _position;
                _startPos += sizeof(double) * 3;
            }
            //이벤트
            _result.EventType = BitConverter.ToInt32(value, _startPos);
            _startPos += sizeof(int);
            //이벤트값
            _result.EventValue = BitConverter.ToDouble(value, _startPos);
            _startPos += sizeof(int);
            //UI식별부호
            _result.UIID = BitConverter.ToInt32(value, _startPos);

            return _result;
        }
    }
    
}
