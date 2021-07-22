using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectModule
{
    public struct KinectPoseInputMessage
    {
        public string DeviceName;
        public string HostName;
        public XSkeleton UserBody; // 25 skeleton joint data
        public PoseFeature UserFeature; // 21 pose feature data (angle & distance)
        //public List<ValuePair<double, List<double>>> UserMatchingRate; // user matching rates per pose

        public KinectPoseInputMessage(string deviceName, string hostName, XSkeleton userBody, PoseFeature userFeature)//, List<ValuePair<double, List<double>>> userMatchingRate)
        {
            DeviceName = deviceName;
            HostName = hostName;
            UserBody = userBody;
            UserFeature = userFeature;
           // UserMatchingRate = userMatchingRate;
        }

        // network packing
        public static implicit operator byte[] (KinectPoseInputMessage im)
        {   //장치명
            byte[] _deviceName = Encoding.Unicode.GetBytes(im.DeviceName);
            byte[] _deviceNameLen = BitConverter.GetBytes(_deviceName.Length);
            //호스트명
            byte[] _hostName = Encoding.Unicode.GetBytes(im.HostName);
            byte[] _hostNameLen = BitConverter.GetBytes(_hostName.Length);
            //사용자 스켈레톤
            byte[] _body = im.UserBody;
            byte[] _bodyLen = BitConverter.GetBytes(_body.Length);
            //사용자 특징점
            byte[] _feature = im.UserFeature;
            byte[] _featureLen = BitConverter.GetBytes(_feature.Length);

            //총 길이
            int _totalLen = sizeof(int) + _deviceNameLen.Length + _deviceName.Length + _hostNameLen.Length + _hostName.Length + _body.Length + _bodyLen.Length + _feature.Length + _featureLen.Length;
            byte[] _total = BitConverter.GetBytes(_totalLen);

            //획득한 정보를 통합하여 반환
            byte[] _result = new byte[_totalLen];
            int _copyPose = 0;
            //총길이 삽입
            Array.Copy(_total, _result, _total.Length);
            _copyPose += _total.Length;
            //장치 길이 삽입
            Array.Copy(_deviceNameLen, 0, _result, _copyPose, _deviceNameLen.Length);
            _copyPose += _deviceNameLen.Length;
            //장치 명 삽입
            Array.Copy(_deviceName, 0, _result, _copyPose, _deviceName.Length);
            _copyPose += _deviceName.Length;
            //호스트 길이 삽입
            Array.Copy(_hostNameLen, 0, _result, _copyPose, _hostNameLen.Length);
            _copyPose += _hostNameLen.Length;
            //호스트 명 삽입
            Array.Copy(_hostName, 0, _result, _copyPose, _hostName.Length);
            _copyPose += _hostName.Length;
            //사용자 스켈레톤 길이 삽입
            Array.Copy(_bodyLen, 0, _result, _copyPose, _bodyLen.Length);
            _copyPose += _bodyLen.Length;
            //사용자 스켈레톤
            Array.Copy(_body, 0, _result, _copyPose, _body.Length);
            _copyPose += _body.Length;
            //사용자 특징점 길이 삽입
            Array.Copy(_featureLen, 0, _result, _copyPose, _featureLen.Length);
            _copyPose += _featureLen.Length;
            //사용자 특징점
            Array.Copy(_feature, 0, _result, _copyPose, _feature.Length);
            _copyPose += _feature.Length;

            return _result;
        }

        // network unpacking
        public static implicit operator KinectPoseInputMessage(byte[] value)
        {
            KinectPoseInputMessage _result = new KinectPoseInputMessage();

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
            //사용자 스켈레톤
            int _bodyLen = 25 * 3 * sizeof(double);
            byte[] _body = new byte[_bodyLen];
            Array.Copy(value, _startPos, _body, 0, _bodyLen);
            _result.UserBody = _body;
            _startPos += _bodyLen;
            //사용자 특징점
            int _featureLen = 21 * sizeof(double);
            byte[] _feature = new byte[_featureLen];
            Array.Copy(value, _startPos, _feature, 0, _featureLen);
            _result.UserFeature = _feature;
            _startPos += _featureLen;

            return _result;
        }
    }
}
