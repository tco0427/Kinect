using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Kinect;

namespace KinectModule
{
    class KinectDataSelector
    {
        public KinectManager Kinect
        {
            set
            {
                if (value == null)
                    return;

                value.Selector = SkeletonSelect;
            }
        }

        public Body[] SkeletonSelect(Body[] input)
        {
            ArrayList _select = new ArrayList();

            foreach (Body _skel in input)
            {   //1. 잡음 제거
                //if (_skel.TrackingState != SkeletonTrackingState.Tracked)
                if (!_skel.IsTracked)
                    continue;
                //2. 못쓰는 데이터 제거
                    //머리, 목, 허리, 엉덩이를 조사해, 순서대로가 아님 제거.
                double _y = _skel.Joints[JointType.Head].Position.Y;
                if (_y <= _skel.Joints[JointType.SpineShoulder].Position.Y)
                    continue;
                _y = _skel.Joints[JointType.SpineShoulder].Position.Y;
                if (_y <= _skel.Joints[JointType.SpineMid].Position.Y)
                    continue;
                _y = _skel.Joints[JointType.SpineMid].Position.Y;
                if (_y <= _skel.Joints[JointType.SpineBase].Position.Y)
                    continue;

                //3. 유효하지 않은 데이터 제거
                    //너무 가까이 있는 데이터
                double _z = _skel.Joints[JointType.Head].Position.Z;
                if (_z < 1.2)
                    continue;
                    //너무 외곽에 있는 데이터
                double _maxWidth = (_z - 0.6) * Math.Tan((Math.PI * 28.0) / 180.0);
                double _x = _skel.Joints[JointType.Head].Position.X;
                if (_x > _maxWidth || -_x > _maxWidth)
                    continue;

                _select.Add(_skel);
            }

            if(_select.Count <= 0)
                return null;

            Body[] _return = new Body[_select.Count];
            for (int i = 0; i < _select.Count; i++)
                _return[i] = (Body)_select[i];

            return _return;
        }
    }
}
