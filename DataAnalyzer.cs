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
        private Dictionary<int, InputMessage> preMessages;
        private Dictionary<int, DateTime> lastSend;

        public KinectDataAnalyzer()
        {
            preMessages = new Dictionary<int, InputMessage>();
            lastSend = new Dictionary<int, DateTime>();
        }

        /*
         * 키넥트 입력정보를 입력메시지로 변환
         * 기본메시지는, 양 손의 정보만을 사용하며, 기타의 경우는 나중에 생각하기로 한다.
         */


        public Dictionary<int, InputMessage> Analize(Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> input)
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
                _newMessage.DeviceName = "Kinect";
                _newMessage.HostName = _ipAddress;
                _newMessage.EventType = 0;
                _newMessage.EventValue = 0;
                _newMessage.UIID = 0;

                //끝부분 스켈레톤
                XSkeleton _lastSkel = _ill.Value.Second[_ill.Value.Second.Count - 1];
                if (_ill.Value.Second.Count > 2)
                {
                    int _sumCount = 1;
                    for (int _i = _ill.Value.Second.Count - 2; _i >= 0 && _i >= _ill.Value.Second.Count - 10; _i--)
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

                Point _leftHand = _lastSkel[JointType.HandLeft];
                Point _rightHand = _lastSkel[JointType.HandRight];

                _newMessage.Points[0].Second = _rightHand;

                Point _leftShoulder = _lastSkel[JointType.ShoulderLeft];
                
                if(preMessages.ContainsKey(_ill.Key))
                {
                    _newMessage.Points[0].First = preMessages[_ill.Key].Points[0].First;
                    if(preMessages[_ill.Key].Points[0].First != 0 && (_leftShoulder.Z - _leftHand.Z) < 0.25)
                        _newMessage.Points[0].First = 0;
                    if(preMessages[_ill.Key].Points[0].First == 0 && (_leftShoulder.Z - _leftHand.Z) > 0.35)
                        _newMessage.Points[0].First = 2;
                }
                else
                    _newMessage.Points[0].First = 0;

                if (_newMessage.Points[0].First != 0)
                {
                    double _distX = _leftShoulder.X - _leftHand.X;
                    double _distY = _leftShoulder.Y - _leftHand.Y;

                    if (Math.Abs(_distY) > 0.1)
                    {
                        _newMessage.EventType = 2;
                        _newMessage.EventValue = -(_distY > 0 ? 1.0 + (_distY - 0.1) * 5 : 1.0 + (_distY + 0.1) * 5);
                    }
                    else if (Math.Abs(_distX) > 0.1)
                    {
                        _newMessage.EventType = 1;
                        _newMessage.EventValue = -(_distX > 0 ? (_distX - 0.1) * 1800 : (_distX + 0.1) * 1800);
                    }
                }
                
                _newMessages.Add(_ill.Key, _newMessage);

                _result.Add(_ill.Key, _newMessage);
                if (lastSend.ContainsKey(_ill.Key))
                    lastSend[_ill.Key] = DateTime.Now;
                else
                    lastSend.Add(_ill.Key, DateTime.Now);
                
                /* //이전 정보
                Point _leftHand = _lastSkel[JointType.HandLeft];
                Point _rightHand = _lastSkel[JointType.HandRight];

                //_leftHand.X += (_lastSkel[JointType.ShoulderCenter].X - _lastSkel[JointType.ShoulderLeft].X);
                _newMessage.Points[0].Second = _leftHand;
                //_rightHand.X -= (_lastSkel[JointType.ShoulderRight].X - _lastSkel[JointType.ShoulderCenter].X);
                _newMessage.Points[1].Second = _rightHand;

                _newMessage.Points[2].Second = _lastSkel[JointType.ShoulderCenter];


                //포인트 상태 지정 - 양 손에 대한 이벤트 정보 (NONE, BEGIN, CONTINUE, END)
                double _minDistance = _lastSkel[JointType.HipCenter].Distance(_lastSkel[JointType.Head]) * 0.4;
                bool _leftUse = false;
                bool _rightUse = false;
                if (preMessages.ContainsKey(_ill.Key))
                {
                    _leftUse = preMessages[_ill.Key].Points[0].First == 0 ? false : true;
                    _rightUse = preMessages[_ill.Key].Points[1].First == 0 ? false : true;
                }

                XJoint _neck = _lastSkel[JointType.ShoulderCenter];
                //leftHand
                XJoint _leftH = _lastSkel[JointType.HandLeft];
                XJoint _leftS = _lastSkel[JointType.ShoulderLeft];
                double _unit = _leftS.Distance(_neck);
                double _distance = _leftS.Distance(_leftH) / _unit;
                if (_leftUse)
                {
                    if (_distance < 1.6)
                        _leftUse = false;
                }
                else
                {
                    if (_distance > 1.9)
                        _leftUse = true;
                }
                if (_leftUse)
                {
                    XJoint _Sl = new XJoint(_leftS.X, 0, _leftS.Z);
                    XJoint _Hl = new XJoint(_leftH.X, 0, _leftH.Z);
                    if (_leftH.Y < _leftS.Y && _Sl.Distance(_Hl) < _unit * 1.5)
                        _leftUse = false;
                }

                //rightHand
                XJoint _rightH = _lastSkel[JointType.HandRight];
                XJoint _rightS = _lastSkel[JointType.ShoulderRight];
                _unit = _rightS.Distance(_neck);
                _distance = _rightS.Distance(_rightH) / _unit;
                if (_rightUse)
                {
                    if (_distance < 1.6)
                        _rightUse = false;
                }
                else
                {
                    if (_distance > 1.9)
                        _rightUse = true;
                }
                if (_rightUse)
                {
                    XJoint _Sl = new XJoint(_rightS.X, 0, _rightS.Z);
                    XJoint _Hl = new XJoint(_rightH.X, 0, _rightH.Z);
                    if (_rightH.Y < _rightS.Y && _Sl.Distance(_Hl) < _unit * 1.5)
                        _rightUse = false;
                }

                if (preMessages.ContainsKey(_ill.Key))
                {
                    switch (preMessages[_ill.Key].Points[0].First)
                    {
                        case InputMessageType.BEGIN:
                            _newMessage.Points[0].First = _leftUse ? InputMessageType.CONTINUE : InputMessageType.END;
                            break;
                        case InputMessageType.CONTINUE:
                            _newMessage.Points[0].First = _leftUse ? InputMessageType.CONTINUE : InputMessageType.END;
                            break;
                        case InputMessageType.END:
                            _newMessage.Points[0].First = _leftUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                            break;
                        default:
                            _newMessage.Points[0].First = _leftUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                            break;
                    }
                    switch (preMessages[_ill.Key].Points[1].First)
                    {
                        case InputMessageType.BEGIN:
                            _newMessage.Points[1].First = _rightUse ? InputMessageType.CONTINUE : InputMessageType.END;
                            break;
                        case InputMessageType.CONTINUE:
                            _newMessage.Points[1].First = _rightUse ? InputMessageType.CONTINUE : InputMessageType.END;
                            break;
                        case InputMessageType.END:
                            _newMessage.Points[1].First = _rightUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                            break;
                        default:
                            _newMessage.Points[1].First = _rightUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                            break;
                    }
                }
                else
                {
                    _newMessage.Points[0].First = _leftUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                    _newMessage.Points[1].First = _rightUse ? InputMessageType.BEGIN : InputMessageType.NONE;
                }

                //메시지 정리
                _newMessages.Add(_ill.Key, _newMessage);

                //새로운 메시지로 취급할 것인가를 결정.
                if (!preMessages.ContainsKey(_ill.Key) || !(_newMessage.Points[0].First == preMessages[_ill.Key].Points[0].First && _newMessage.Points[1].First == preMessages[_ill.Key].Points[1].First))
                {
                    _result.Add(_ill.Key, _newMessage);
                    if (lastSend.ContainsKey(_ill.Key))
                        lastSend[_ill.Key] = DateTime.Now;
                    else
                        lastSend.Add(_ill.Key, DateTime.Now);
                }
                else
                {
                    double _leftGap = _newMessage.Points[0].Second.Distance(preMessages[_ill.Key].Points[0].Second);
                    double _rightGap = _newMessage.Points[1].Second.Distance(preMessages[_ill.Key].Points[1].Second);

                    if (_leftGap > 0.02 || _rightGap > 0.02)
                    {
                        _result.Add(_ill.Key, _newMessage);
                        lastSend[_ill.Key] = DateTime.Now;
                    }
                    else
                    {
                        TimeSpan _span = DateTime.Now - lastSend[_ill.Key];
                        if (_span.TotalMilliseconds > 100.0)
                        {
                            _result.Add(_ill.Key, _newMessage);
                            lastSend[_ill.Key] = DateTime.Now;
                        }
                        else
                            _newMessages[_ill.Key] = preMessages[_ill.Key];
                    }
                }*/
            }

            preMessages = _newMessages;

            return _result;
        }
    }
}
