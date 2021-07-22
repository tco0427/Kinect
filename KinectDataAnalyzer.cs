using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Microsoft.Kinect;

namespace KinectModule
{
    class KinectDataAnalyzer
    {
        public double[] Rate
        {
            get;
            set;
        }

        public PoseFeature FeaturePoint
        {
            get;
            set;
        }

        public XSkeleton TrackedSkeleton
        {
            get;
            set;
        }

        public PoseDetailRate PoseDetailRates
        {
            get;
            set;
        }

        private Dictionary<int, KinectPoseInputMessage> prevMessages;
        //private Dictionary<int, DateTime> lastGesture;
        //private Dictionary<int, Point> lastGesturePoint;
        private Dictionary<int, DateTime> lastSend;

        private KinectManager manager;
        public KinectManager Kinect
        {
            set
            {
                if (value == null)
                {
                    manager = null;
                    return;
                }

                manager = value;
                manager.Analyzer = Analyze;
            }
        }

        public KinectDataAnalyzer()
        {
            PoseDetailRates = new PoseDetailRate(); // pose detail rate recording
            TrackedSkeleton = new XSkeleton(); // skeleton tracking recording
            FeaturePoint = new PoseFeature(); // (i.e., new double[21])
            prevMessages = new Dictionary<int, KinectPoseInputMessage>(); // inputmessage
            lastSend = new Dictionary<int, DateTime>();

            Rate = new double[12]; // (pose 개수만큼 생성)
        }

        public Dictionary<int, KinectPoseInputMessage> Analyze(Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> input)
        {
            Dictionary<int, KinectPoseInputMessage> _result = new Dictionary<int, KinectPoseInputMessage>();

            //ip찾기
            string _ipAddress = "";
            foreach (IPAddress _ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ip.AddressFamily.ToString() == "InterNetwork")
                {
                    _ipAddress = _ip.ToString();
                    break;
                }
            }

            //메시지 분석!
            Dictionary<int, KinectPoseInputMessage> _newMessages = new Dictionary<int, KinectPoseInputMessage>();
            foreach (KeyValuePair<int, ValuePair<List<DateTime>, List<XSkeleton>>> _ill in input)
            {
                KinectPoseInputMessage _newMessage = new KinectPoseInputMessage();
                _newMessage.DeviceName = "Kinect1";
                _newMessage.HostName = _ipAddress;

                // 끝부분 스켈레톤
                XSkeleton _lastSkel = _ill.Value.Second[_ill.Value.Second.Count - 1];
                if (_ill.Value.Second.Count > 2)
                {
                    int _sumCount = 1;
                    for (int _i = _ill.Value.Second.Count - 2; _i >= 0 && _i >= _ill.Value.Second.Count - 15; _i--)
                    {
                        _lastSkel += _ill.Value.Second[_i];
                        _sumCount++;
                    }
                    _lastSkel /= _sumCount;
                }

                _newMessage.UserBody = _lastSkel; // update skeleton into KinectPoseInputMessage
                TrackedSkeleton = _lastSkel; // update trackedskeleton
                TrackedSkeleton.CurrentTime = DateTime.Now; // must store currenttime into trackedskeleton, otherwise crash

                //제스처는 적어도, 하나의 메시지가 생성된 후라야 설정된다.
                if (prevMessages.ContainsKey(_ill.Key))
                {
                    double _difSecond = (DateTime.Now - _ill.Value.First[0]).TotalSeconds;
                    if (_difSecond > 0.5) // 0.5초 쌓인후
                    {
                        XSkeleton _lastS = _ill.Value.Second[_ill.Value.Second.Count - 1];
                        DateTime _lastT = _ill.Value.First[_ill.Value.Second.Count - 1];

                        double elbowLeftAngle = XJoint.Angle(_lastS[JointType.ElbowLeft] - _lastS[JointType.ShoulderLeft], _lastS[JointType.HandLeft] - _lastS[JointType.ElbowLeft]);
                        double elbowRightAngle = XJoint.Angle(_lastS[JointType.ElbowRight] - _lastS[JointType.ShoulderRight], _lastS[JointType.HandRight] - _lastS[JointType.ElbowRight]);
                        double shoulderLeftAngle = XJoint.Angle(_lastS[JointType.ShoulderLeft] - _lastS[JointType.ShoulderRight], _lastS[JointType.HandLeft] - _lastS[JointType.ShoulderLeft]);
                        double shoulderRightAngle = XJoint.Angle(_lastS[JointType.ShoulderRight] - _lastS[JointType.ShoulderLeft], _lastS[JointType.HandRight] - _lastS[JointType.ShoulderRight]);
                        double armLeftAngleUp = XJoint.Angle(_lastS[JointType.HandLeft] - _lastS[JointType.ShoulderLeft], XJoint.Up);
                        double armRightAngleUp = XJoint.Angle(_lastS[JointType.HandRight] - _lastS[JointType.ShoulderRight], XJoint.Up);
                        double armLeftAngleDown = XJoint.Angle(_lastS[JointType.HandLeft] - _lastS[JointType.ShoulderLeft], XJoint.Down);
                        double armRightAngleDown = XJoint.Angle(_lastS[JointType.HandRight] - _lastS[JointType.ShoulderRight], XJoint.Down);
                        double armLeftAngleFront = XJoint.Angle(_lastS[JointType.ShoulderLeft] - _lastS[JointType.HandLeft], XJoint.Forward);
                        double armRightAngleFront = XJoint.Angle(_lastS[JointType.ShoulderRight] - _lastS[JointType.HandRight], XJoint.Forward);
                        double kneeLeftAngle = XJoint.Angle(_lastS[JointType.KneeLeft] - _lastS[JointType.HipLeft], _lastS[JointType.AnkleLeft] - _lastS[JointType.KneeLeft]);
                        double kneeRightAngle = XJoint.Angle(_lastS[JointType.KneeRight] - _lastS[JointType.HipRight], _lastS[JointType.AnkleRight] - _lastS[JointType.KneeRight]);
                        double kneeLeftAngle90 = Math.Abs(90 - kneeLeftAngle);
                        double kneeRightAngle90 = Math.Abs(90 - kneeRightAngle);
                        //double kneeLeftAngleDown = XJoint.Angle(_lastS[JointType.AnkleLeft] - _lastS[JointType.KneeLeft], XJoint.Down);
                        //double kneeRightAngleDown = XJoint.Angle(_lastS[JointType.AnkleRight] - _lastS[JointType.KneeRight], XJoint.Down);
                        double legLeftAngleDown = XJoint.Angle(_lastS[JointType.AnkleLeft] - _lastS[JointType.HipLeft], XJoint.Down);
                        double legRightAngleDown = XJoint.Angle(XJoint.Down, _lastS[JointType.AnkleRight] - _lastS[JointType.HipRight]);
                        double legLeftAngleDown45 = Math.Abs(45 - legLeftAngleDown);
                        double legRightAngleDown45 = Math.Abs(45 - legRightAngleDown);
                        double spineAngleUp = XJoint.Angle(_lastS[JointType.SpineShoulder] - _lastS[JointType.SpineBase], XJoint.Up);
                        //double kneeDistance = _lastS[JointType.KneeLeft].Distance(_lastS[JointType.KneeRight]);
                        double ankleDistance = _lastS[JointType.AnkleLeft].Distance(_lastS[JointType.AnkleRight]);
                        double handDistance = _lastS[JointType.HandLeft].Distance(_lastS[JointType.HandRight]);

                        //실제 키넥트에서 측정된 값으로 구한 각도 및 거리 값 Feature 리스트 UPDATE
                        FeaturePoint.CurrentTime = DateTime.Now; // need currenttime
                        FeaturePoint[KinectPoseFeature.ELA] = elbowLeftAngle; // 0
                        FeaturePoint[KinectPoseFeature.ERA] = elbowRightAngle; // 1
                        FeaturePoint[KinectPoseFeature.SLA] = shoulderLeftAngle; // 2
                        FeaturePoint[KinectPoseFeature.SRA] = shoulderRightAngle; // 3
                        FeaturePoint[KinectPoseFeature.ULA] = armLeftAngleUp; // 4
                        FeaturePoint[KinectPoseFeature.URA] = armRightAngleUp; // 5
                        FeaturePoint[KinectPoseFeature.DLA] = armLeftAngleDown; // 6
                        FeaturePoint[KinectPoseFeature.DRA] = armRightAngleDown; // 7
                        FeaturePoint[KinectPoseFeature.FLA] = armLeftAngleFront; // 8
                        FeaturePoint[KinectPoseFeature.FRA] = armRightAngleFront; // 9
                        FeaturePoint[KinectPoseFeature.KLA] = kneeLeftAngle; // 10
                        FeaturePoint[KinectPoseFeature.KRA] = kneeRightAngle; // 11
                        FeaturePoint[KinectPoseFeature.KLA90] = kneeLeftAngle90; // 12
                        FeaturePoint[KinectPoseFeature.KRA90] = kneeRightAngle90; // 13
                        FeaturePoint[KinectPoseFeature.LLA] = legLeftAngleDown; // 14
                        FeaturePoint[KinectPoseFeature.LRA] = legRightAngleDown; // 15
                        FeaturePoint[KinectPoseFeature.LLA45] = legLeftAngleDown45; // 16
                        FeaturePoint[KinectPoseFeature.LRA45] = legRightAngleDown45; // 17
                        FeaturePoint[KinectPoseFeature.SSA] = spineAngleUp; // 18
                        FeaturePoint[KinectPoseFeature.AD] = ankleDistance; // 19
                        FeaturePoint[KinectPoseFeature.HD] = handDistance; // 20
                    }

                    int _pose = FindPose(FeaturePoint);
                    /*switch (_pose)
                    {
                        case 0: System.Diagnostics.Trace.WriteLine("BOTH SIDE OUT!! RATE=" + Rate[0] + "\n"); break;
                        case 1: System.Diagnostics.Trace.WriteLine("BOTH SIDE UPPPPPPPPPPPPPPPPPPPPP!! RATE=" + Rate[1] + "\n"); break;
                        case 2: System.Diagnostics.Trace.WriteLine("BOTH SIDE FRONT 팔이 앞으로 RATE=" + Rate[2] + "\n"); break;
                        case 3: System.Diagnostics.Trace.WriteLine("BOTH SIDE DOWN  차렷 RATE=" + Rate[3] + "\n"); break;
                        case 4: System.Diagnostics.Trace.WriteLine("Only LEFT HandUp 왼팔만 위로 RATE=" + Rate[4] + "\n"); break;
                        case 5: System.Diagnostics.Trace.WriteLine("Only RIGHT HandUp 오른팔만 위로 RATE=" + Rate[5] + "\n"); break;
                        case 6: System.Diagnostics.Trace.WriteLine("Only LEFT KneeUp 왼무릎 위로 RATE=" + Rate[6] + "\n"); break;
                        case 7: System.Diagnostics.Trace.WriteLine("Only RIGHT KneeUp 오른무릎 위로 RATE=" + Rate[7] + "\n"); break;
                        case 8: System.Diagnostics.Trace.WriteLine("LEFT LEG AB RATE=" + Rate[8] + "\n"); break;
                        case 9: System.Diagnostics.Trace.WriteLine("RIGHT LEG AB RATE=" + Rate[9] + "\n"); break;
                        case 10: System.Diagnostics.Trace.WriteLine("손 모으기 RATE=" + Rate[10] + "\n"); break;
                        case 11: System.Diagnostics.Trace.WriteLine("스쿼트 RATE=" + Rate[11] + "\n"); break;
                        default: System.Diagnostics.Trace.WriteLine(""); break;
                    }*/
                }

                _newMessage.UserFeature = FeaturePoint;
                _newMessages.Add(_ill.Key, _newMessage);

                //메시지 전송 여부 설정.
                if (!prevMessages.ContainsKey(_ill.Key))
                {
                    _result.Add(_ill.Key, _newMessage);
                    if (lastSend.ContainsKey(_ill.Key))
                        lastSend[_ill.Key] = DateTime.Now;
                    else
                        lastSend.Add(_ill.Key, DateTime.Now);
                }
                else
                {
                    bool _isNowSend = false;
                    if ((DateTime.Now - lastSend[_ill.Key]).TotalSeconds > 0.05)
                        _isNowSend = true;

                    if (_isNowSend)
                    {
                        _result.Add(_ill.Key, _newMessage);
                        lastSend[_ill.Key] = DateTime.Now;
                    }
                }
            }

            prevMessages = _newMessages;

            return _result;
        }

        /*
         * 키넥트 입력정보를 입력메시지로 변환
         * 기본메시지는, 양 손의 정보만을 사용하며, 기타의 경우는 나중에 생각하기로 한다.
         */
        /*
        public Dictionary<int, InputMessage> Analyze(Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> input)
        {
            Dictionary<int, InputMessage> _result = new Dictionary<int, InputMessage>();


            //ip찾기
            string _ipAddress = "";
            foreach (IPAddress _ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_ip.AddressFamily.ToString() == "InterNetwork")
                {
                    _ipAddress = _ip.ToString();
                    break;
                }
            }

            //메시지 분석!
            Dictionary<int, InputMessage> _newMessages = new Dictionary<int,InputMessage>();
            foreach (KeyValuePair<int, ValuePair<List<DateTime>, List<XSkeleton>>> _ill in input)
            {
                InputMessage _newMessage = new InputMessage();
                _newMessage.DeviceName = "Kinect1";
                _newMessage.HostName = _ipAddress;
                _newMessage.EventType = 0;
                _newMessage.EventValue = 0;
                _newMessage.UIID = 0;

                //끝부분 스켈레톤
                XSkeleton _lastSkel = _ill.Value.Second[_ill.Value.Second.Count - 1];
                if (_ill.Value.Second.Count > 2)
                {
                    int _sumCount = 1;
                    for (int _i = _ill.Value.Second.Count - 2; _i >= 0 && _i >= _ill.Value.Second.Count - 15; _i--)
                    {
                        _lastSkel += _ill.Value.Second[_i];
                        _sumCount++;
                    }
                    _lastSkel /= _sumCount;
                }

                //사용자 위치 지정 - 키넥트 사용자 위치는, 목지점 사용
                _newMessage.UsePosition = 1;
                _newMessage.Position.X = _lastSkel[JointType.SpineBase].X;
                _newMessage.Position.Y = _lastSkel[JointType.SpineShoulder].Y;
                _newMessage.Position.Z = _lastSkel[JointType.SpineBase].Z;

                //포인트 지정 - 손 위치 지정
                _newMessage.PointCount = 1;
                _newMessage.Points = new ValuePair<int, Point>[1];

                Point _rightHand = _lastSkel[JointType.HandRight];
                
                //제스처는 적어도, 하나의 메시지가 생성된 후라야 설정된다.
                if (preMessages.ContainsKey(_ill.Key))
                {
                    int _gesture = 0;
                    //제스처 확인. 제스처는 0.5초 미만의 간격으로 확인될 수 없다(중복&오류 방지)
                    if (!lastGesture.ContainsKey(_ill.Key))
                        _gesture = FindGesture(_ill.Value);
                    else
                    {
                         double _difTime = (DateTime.Now - lastGesture[_ill.Key]).TotalSeconds;
                         if (_difTime > 0.5)
                             _gesture = FindGesture(_ill.Value);
                    }
                    int _pose = FindPose(_ill.Value);

                    //만일 제스처가 확인되지 않았으면 이전 제스처 적용
                    if (_gesture == 0)
                    {
                        _newMessage.Points[0].First = preMessages[_ill.Key].Points[0].First;
                        _newMessage.EventType = preMessages[_ill.Key].EventType;
                    }
                    //제스처가 확인되었으면 신규 제스처 적용
                    else
                    {   //1. 푸쉬 제스처. 이것은 선택/해제를 토글하는 것.
                        if (_gesture == 1)
                        {
                            System.Diagnostics.Trace.WriteLine("PUSH~~~~~~~~~~~~~");
                            //이전에 선택이면 해제, 해제면 선택상태로 설정.
                            if (preMessages[_ill.Key].Points[0].First == 0)
                                _newMessage.Points[0].First = 2;
                            else
                                _newMessage.Points[0].First = 0;
                            if (lastGesture.ContainsKey(_ill.Key))
                                lastGesture[_ill.Key] = DateTime.Now;
                            else
                                lastGesture.Add(_ill.Key, DateTime.Now);
                        }
                        //2. Swipein 제스처.
                        else if (_gesture == 2)
                        {
                            System.Diagnostics.Trace.WriteLine("Swipe IN");
                            _newMessage.Points[0].First = preMessages[_ill.Key].Points[0].First;
                            if (_newMessage.Points[0].First != 0)
                            {
                                if (preMessages[_ill.Key].EventType != 1)
                                    _newMessage.EventType = 1;
                                else
                                    _newMessage.EventType = 0;

                                if (lastGesture.ContainsKey(_ill.Key))
                                {
                                    lastGesture[_ill.Key] = DateTime.Now;
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint[_ill.Key] = _rightHand;
                                }
                                else
                                {
                                    lastGesture.Add(_ill.Key, DateTime.Now);
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint.Add(_ill.Key, _rightHand);
                                }
                            }
                        }
                        //3. Swipeout 제스처.
                        else if (_gesture == 3)
                        {
                            System.Diagnostics.Trace.WriteLine("Swipe OUTTTTTTTTTTTTTTTTTTTTTTTT");
                            _newMessage.Points[0].First = preMessages[_ill.Key].Points[0].First;

                            if (_newMessage.Points[0].First != 0)
                            {
                                if (preMessages[_ill.Key].EventType != 2)
                                    _newMessage.EventType = 2;
                                else
                                    _newMessage.EventType = 0;

                                if (lastGesture.ContainsKey(_ill.Key))
                                {
                                    lastGesture[_ill.Key] = DateTime.Now;
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint[_ill.Key] = _rightHand;
                                }
                                else
                                {
                                    lastGesture.Add(_ill.Key, DateTime.Now);
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint.Add(_ill.Key, _rightHand);
                                }
                            }
                        }
                        else if (_gesture == 4)
                        {
                            System.Diagnostics.Trace.WriteLine("RAISE---------------------");
                            _newMessage.Points[0].First = preMessages[_ill.Key].Points[0].First;

                            if (_newMessage.Points[0].First != 0)
                            {
                                if (preMessages[_ill.Key].EventType != 4)
                                    _newMessage.EventType = 4;
                                else
                                    _newMessage.EventType = 0;

                                if (lastGesture.ContainsKey(_ill.Key))
                                {
                                    lastGesture[_ill.Key] = DateTime.Now;
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint[_ill.Key] = _rightHand;
                                }
                                else
                                {
                                    lastGesture.Add(_ill.Key, DateTime.Now);
                                    if (_newMessage.EventType != 0)
                                        lastGesturePoint.Add(_ill.Key, _rightHand);
                                }
                            }
                        }
                    }
                }
                
                //제스처가 없을 때. 이때는 포인트를 정상적으로 적용한다.
                if (_newMessage.EventType == 0)
                {
                    _newMessage.Points[0].Second = _rightHand;
                }
                else  //제스처가 있다면, 포인트는 제스처 직후의 포인트를 적용하고, 또한 그 값을 활용하여 제스처 값을 설정한다.
                {
                    _newMessage.Points[0].Second = lastGesturePoint[_ill.Key];

                    Point _value = _rightHand - lastGesturePoint[_ill.Key];
                    double _eValue = Math.Abs(_value.X) > Math.Abs(_value.Y) ? _value.X : _value.Y;
                    if (_newMessage.EventType == 1)
                        _newMessage.EventValue = _eValue / 0.5 * 180;
                    else if(_newMessage.EventType == 2)
                        _newMessage.EventValue = 1.0 + _eValue;
                }

                _newMessages.Add(_ill.Key, _newMessage);

                //메시지 전송 여부 설정.
                if (!preMessages.ContainsKey(_ill.Key))
                {
                    _result.Add(_ill.Key, _newMessage);
                    if (lastSend.ContainsKey(_ill.Key))
                        lastSend[_ill.Key] = DateTime.Now;
                    else
                        lastSend.Add(_ill.Key, DateTime.Now);
                }
                else
                {
                    bool _isNowSend = false;
                    if (preMessages[_ill.Key].Points[0].First != _newMessage.Points[0].First)
                        _isNowSend = true;
                    else if (preMessages[_ill.Key].EventType != _newMessage.EventType)
                        _isNowSend = true;
                    else if ((DateTime.Now - lastSend[_ill.Key]).TotalSeconds > 0.05)
                        _isNowSend = true;

                    if(_isNowSend)
                    {
                        _result.Add(_ill.Key, _newMessage);
                        lastSend[_ill.Key] = DateTime.Now;
                    }
                }
            }

            preMessages = _newMessages;

            return _result;
        }
        */

        private void FindMinMaxSkeleton(ValuePair<List<DateTime>, List<XSkeleton>> skelList, ref XSkeleton minS, ref XSkeleton maxS, ref DateTime minT, ref DateTime maxT, JointType type, int axis)
        {
            for (int _i = skelList.Second.Count; _i > 0; --_i)
            {
                double _difSecond = (DateTime.Now - skelList.First[_i - 1]).TotalSeconds;
                if (_difSecond > 0.5)
                    break;

                if (axis == 0) // X-axis
                {
                    if (minS[type].X > skelList.Second[_i - 1][type].X)
                    {
                        minS = skelList.Second[_i - 1];
                        minT = skelList.First[_i - 1];
                    }

                    if (maxS[type].X < skelList.Second[_i - 1][type].X)
                    {
                        maxS = skelList.Second[_i - 1];
                        maxT = skelList.First[_i - 1];
                    }
                }
                else if (axis == 1) // Y-axis
                {
                    if (minS[type].Y > skelList.Second[_i - 1][type].Y)
                    {
                        minS = skelList.Second[_i - 1];
                        minT = skelList.First[_i - 1];
                    }

                    if (maxS[type].Y < skelList.Second[_i - 1][type].Y)
                    {
                        maxS = skelList.Second[_i - 1];
                        maxT = skelList.First[_i - 1];
                    }
                }
                else if (axis == 2) // Z-axis
                {
                    if (minS[type].Z > skelList.Second[_i - 1][type].Z)
                    {
                        minS = skelList.Second[_i - 1];
                        minT = skelList.First[_i - 1];
                    }

                    if (maxS[type].Z < skelList.Second[_i - 1][type].Z)
                    {
                        maxS = skelList.Second[_i - 1];
                        maxT = skelList.First[_i - 1];
                    }
                }
            }
        }
        /*private int FindGesture(ValuePair<List<DateTime>, List<XSkeleton>> skelList)
        {   
            //최소한 데이터는 0.5초는 쌓여야 한다.
            if (skelList.First.Count == 0)
                return 0;
            double _difSecond = (DateTime.Now - skelList.First[0]).TotalSeconds;
            if(_difSecond < 0.5)
                return 0;

            //1. push Event 검출.
            // step1. 최근 0.5초 사이 왼손의 z값이 가장 큰 정보와 가장 작은 정보 확인.
            XSkeleton _minS = skelList.Second[skelList.Second.Count - 1];
            XSkeleton _maxS = skelList.Second[skelList.Second.Count - 1];
            DateTime _minT = skelList.First[skelList.Second.Count - 1];
            DateTime _maxT = skelList.First[skelList.Second.Count - 1];
            FindMinMaxSkeleton(skelList, ref _minS, ref _maxS, ref _minT, ref _maxT, JointType.HandLeft, 2);

            _difSecond = (_minT - _maxT).TotalSeconds;
            if (_difSecond > 0 && (_maxS[JointType.HandLeft] - _minS[JointType.HandLeft]).Length > 0.3) // min, max 차이가 0.3보다 크면 push 제스쳐
            {
                XJoint _modXY = (_maxS[JointType.HandLeft] - _minS[JointType.HandLeft]) * 1.0 / _difSecond;
                double _modZ = _modXY.Z;
                _modXY.Z = 0;

                XJoint _sXY = _maxS[JointType.HandLeft] - _maxS[JointType.ShoulderLeft];
                _sXY.Z = 0;
                if (_modXY.Length < 0.3 && _sXY.Length < 0.3 && _modZ > 0.8)
                    return 1; // PUSH Gesture
            }

            //2. Swipe In/Out 제스처 검출
            // step1. 최근 0.5초 사이 왼손의 X값이 가장 큰 정보와 가장 작은 정보 확인.
            XSkeleton _minSX = skelList.Second[skelList.Second.Count - 1];
            XSkeleton _maxSX = skelList.Second[skelList.Second.Count - 1];
            DateTime _minTX = skelList.First[skelList.Second.Count - 1];
            DateTime _maxTX = skelList.First[skelList.Second.Count - 1];
            FindMinMaxSkeleton(skelList, ref _minSX, ref _maxSX, ref _minTX, ref _maxTX, JointType.HandLeft, 0);

            _difSecond = (_maxTX - _minTX).TotalSeconds;
            if (_difSecond > 0 && (_maxSX[JointType.HandLeft] - _minSX[JointType.HandLeft]).Length > 0.3)
            {
                XJoint _modYZ = (_maxSX[JointType.HandLeft] - _minSX[JointType.HandLeft]) * 1.0 / _difSecond;
                double _modX = _modYZ.X;
                _modYZ.X = 0;
                if (_modYZ.Length < 0.5 && _modX > 2.0)
                    return 2;
            }
            else
            {
                XJoint _modYZ = (_maxSX[JointType.HandLeft] - _minSX[JointType.HandLeft]) * 1.0 / _difSecond;
                double _modX = _modYZ.X;
                _modYZ.X = 0;
                if (_modYZ.Length < 0.5 && _modX < -1.5)
                    return 3;
            }

            //3. right hand raise Event 검출.
            // step1. 최근 0.5초 사이 오른손의 y값이 가장 큰 정보와 가장 작은 정보 확인.
            XSkeleton _minSY = skelList.Second[skelList.Second.Count - 1];
            XSkeleton _maxSY = skelList.Second[skelList.Second.Count - 1];
            DateTime _minTY = skelList.First[skelList.Second.Count - 1];
            DateTime _maxTY = skelList.First[skelList.Second.Count - 1];

            FindMinMaxSkeleton(skelList, ref _minSY, ref _maxSY, ref _minTY, ref _maxTY, JointType.HandRight, 1);

            _difSecond = (_minTY - _maxTY).TotalSeconds;
            if (_difSecond > 0 && (_maxSY[JointType.HandRight] - _minSY[JointType.HandRight]).Length > 0.3) // min, max 차이가 0.3보다 크면 push 제스쳐
            {
                XJoint _modXZ = (_maxSY[JointType.HandRight] - _minSY[JointType.HandRight]) * 1.0 / _difSecond;
                double _modY = _modXZ.Y;
                _modXZ.Y = 0;

                XJoint _sXZ = _maxS[JointType.HandRight] - _maxS[JointType.ShoulderRight];
                _sXZ.Y = 0;
                if (_modXZ.Length < 0.3 && _sXZ.Length < 0.3 && _modY > 0.8)
                    return 4;
            }

            // 검출안되면 0
            return 0;
        }*/

        /// <summary>
        /// 12종 운동자세 (BOTH_SIDE_OUT, BOTH_SIDE_UP, BOTH_SIDE_FRONT, BOTH SIDE DOWN, LEFT_HAND_UP, RIGHT_HAND_UP, LEFT_KNEE_UP, RIGHT_KNEE_UP, LEFT_LEG_AB, RIGHT_LEG_AB, SQUAT, CLAP)
        /// </summary>
        /// <param name="skelList"></param>
        /// <returns></returns>
        private int FindPose(PoseFeature featurePoint) // 좀더 자세히!!
        {
            int selectedIndex = -1;
            List<double> featureList = featurePoint;

            System.Diagnostics.Trace.WriteLine("----------------------");
            if (KinectPose.PoseList != null)
            {
                Dictionary<KinectPoseName, ValuePair<double, List<double>>> PoseMatchingRates = new Dictionary<KinectPoseName, ValuePair<double, List<double>>>();
                PoseDetailRates.CurrentTime = DateTime.Now; // must store currenttime into PoseDetailRates, otherwise crash
                ValuePair<double, List<double>> poseMatchingRate;
                for (int index=0; index < KinectPose.PoseList.Count; index++)
                {
                    KinectPoseName poseName = KinectPose.PoseList[index].PoseName;
                    //System.Diagnostics.Trace.Write("KinectPoseName = " + poseName.ToString());
                    poseMatchingRate = KinectPose.PoseMatchingRate(poseName, featureList);
                    // add PoseMatchingRates per pose
                    PoseMatchingRates.Add(poseName, poseMatchingRate);

                    Rate[index] = poseMatchingRate.First;
                    //System.Diagnostics.Trace.Write(" Rate = " + Rate[index].ToString());
                    //System.Diagnostics.Trace.WriteLine(" " + string.Join(",", poseMatchingRate.Second.Select(i => i.ToString()).ToArray()));

                    if (Rate[index] > 70.0) selectedIndex = index; // 해당 index(Pose)를 지정
                }
                PoseDetailRates.PoseMatchingRates = PoseMatchingRates; // Update PoseDetailRates
                return selectedIndex;
            }

            /*
            System.Diagnostics.Trace.WriteLine("HL = " + _lastS[JointType.HandLeft]);
            System.Diagnostics.Trace.WriteLine("HR = " + _lastS[JointType.HandRight]);
            System.Diagnostics.Trace.WriteLine("EL = " + _lastS[JointType.ElbowLeft]);
            System.Diagnostics.Trace.WriteLine("ER = " + _lastS[JointType.ElbowRight]);
            System.Diagnostics.Trace.WriteLine("SL = " + _lastS[JointType.ShoulderLeft]);
            System.Diagnostics.Trace.WriteLine("SR = " + _lastS[JointType.ShoulderRight]);
            System.Diagnostics.Trace.WriteLine("---------Left------------");
            System.Diagnostics.Trace.Write("HL = " + _lastS[JointType.HandLeft]);
            System.Diagnostics.Trace.WriteLine(" SL = " + _lastS[JointType.ShoulderLeft]);
            System.Diagnostics.Trace.WriteLine("HL-SL = " + (_lastS[JointType.HandLeft] - _lastS[JointType.ShoulderLeft]));
            System.Diagnostics.Trace.WriteLine("L ElbowAngle(EL-SL, HL-EL): " + elbowLeftAngle);
            System.Diagnostics.Trace.WriteLine("L ShoulderAngle(SL-SR, HL-SL): " + shoulderLeftAngle);
            System.Diagnostics.Trace.WriteLine("L armUp: " + armLeftAngleUp);
            System.Diagnostics.Trace.WriteLine("L armDown: " + armLeftAngleDown);
            System.Diagnostics.Trace.WriteLine("L armFront: " + armLeftAngleFront);
            System.Diagnostics.Trace.WriteLine("---------Right------------");
            System.Diagnostics.Trace.WriteLine("R ElbowAngle(ER-SR, HR-ER): " + elbowRightAngle);
            System.Diagnostics.Trace.WriteLine("R ShoulderAngle(SR-SL, HR-SR): " + shoulderRightAngle);
            System.Diagnostics.Trace.WriteLine("R armUp: " + armRightAngleUp);
            System.Diagnostics.Trace.WriteLine("R armDown: " + armRightAngleDown);
            System.Diagnostics.Trace.WriteLine("R armFront: " + armRightAngleFront);
            System.Diagnostics.Trace.WriteLine("/////////////////////////////////");
            */
            //System.Diagnostics.Trace.WriteLine("---------Knee------------");
            //System.Diagnostics.Trace.WriteLine("Knee Distance: " + kneeDistance); // 다리 모으기 판별??
            //System.Diagnostics.Trace.WriteLine("Ankle Distance: " + ankleDistance); // 다리 모으기 판별??
            //System.Diagnostics.Trace.WriteLine("L KneeAngle(KL-HL, AL-KL): " + kneeLeftAngle); // 왼쪽 무릎이 펴진 상태면 15미만 (90도 직각일 경우 75~105)
            //System.Diagnostics.Trace.WriteLine("R KneeAngle(KR-HR, AR-KR): " + kneeRightAngle); // 오른쪽 무릎이 펴진 상태면 15미만 (90도 직각일 경우 75~105)
            //System.Diagnostics.Trace.WriteLine("L KneeAngleDown: " + kneeLeftAngleDown); // 왼쪽 종아리가 일자면 12미만 적당히 20미만
            //System.Diagnostics.Trace.WriteLine("R KneeAngleDown: " + kneeRightAngleDown); // 오른쪽 종아리가 일자면 12미만 적당히 20미만
            //System.Diagnostics.Trace.WriteLine("/////////////////////////////////");
            return -1;           
        }
    }
}
