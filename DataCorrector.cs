using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Microsoft.Kinect;

namespace KinectModule
{
    class KinectDataCorrector
    {
        private int userID;
        private Dictionary<int, Body> matchSkeleton;

        private string configurationPath = "./Configuration/Kinect.conf";
        private double side;
        private double altitude;

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
                manager.Corrector = SkeletonCorrect;
            }
        }

        public KinectDataCorrector()
        {
            userID = 0;
            matchSkeleton = new Dictionary<int, Body>();

            manager = null;

            if (!File.Exists(configurationPath))
                _createConf();
            _readConf();
        }

        public Dictionary<int, XSkeleton> SkeletonCorrect(Body[] skeletons)
        {
            if (skeletons == null || skeletons.Length <= 0)
                return null;

            Dictionary<int, Body> _newInput = new Dictionary<int, Body>();

            //Part 1. Skeleton ID 확인
                //기존의 것과 비교하여 id를 계승시키거나 새로운 ID를 할당한다.
            foreach (Body _skel in skeletons)
            {
                int _id = -1;
                foreach (KeyValuePair<int, Body> _idSkel in matchSkeleton)
                {
                    double _dis = Distance(_skel.Joints[JointType.Head], _idSkel.Value.Joints[JointType.Head]);
                    if (_dis < 0.3)
                    {
                        _id = _idSkel.Key;
                        break;
                    }
                }

                if (_id >= 0)
                    _newInput.Add(_id, _skel);
                else
                {
                    _newInput.Add(userID++, _skel);
                }
            }
            matchSkeleton = _newInput;

            //Part 2. 값 보정. 
                //여기서는, 키넥트 장치의 각도를 보정한다. 
            double _angle = manager.Angle;
            _angle = _angle * Math.PI / 180.0; //각도는 라디안 값으로
            Matrix _rotMat = new Matrix();
            _rotMat.Rotate(_angle, 1.0, 0, 0);
            Dictionary<int, XSkeleton> _result1 = new Dictionary<int,XSkeleton>();
            foreach (KeyValuePair<int, Body> _pair in _newInput)
                _result1.Add(_pair.Key, _rotMat * _pair.Value);

            //Part 3. 값 보정2.
                //여기서, 키넥트의 위치에 대한 정보를 보정한다.

            Dictionary<int, XSkeleton> _result2 = new Dictionary<int, XSkeleton>();
            foreach (KeyValuePair<int, XSkeleton> _pair in _result1)
            {
                XJoint _col = new XJoint(side, altitude, 0);
                XSkeleton _skel = _pair.Value + _col;
                _result2.Add(_pair.Key, _skel);
            }

            return _result2;
        }

        double Distance(Joint a, Joint b)
        {
            double x = a.Position.X - b.Position.X;
            double y = a.Position.Y - b.Position.Y;
            double z = a.Position.Z - b.Position.Z;

            return Math.Sqrt(x * x + y * y + z * z);
        }

        void _createConf()
        {
            FileInfo _fi = new FileInfo(configurationPath);
            if (!Directory.Exists(_fi.DirectoryName))
                Directory.CreateDirectory(_fi.DirectoryName);
            XmlTextWriter _writer = new XmlTextWriter(configurationPath, Encoding.UTF8);
            _writer.Formatting = Formatting.Indented;

            _writer.WriteStartDocument();

            _writer.WriteStartElement("KinectTerminal");

            _writer.WriteStartElement("Network");

            _writer.WriteStartElement("ServerIP");
            _writer.WriteString("127.0.0.1");
            _writer.WriteEndElement();

            _writer.WriteStartElement("PortNumber");
            _writer.WriteString("25001");
            _writer.WriteEndElement();

            _writer.WriteStartElement("Interval");
            _writer.WriteString("0.05");
            _writer.WriteEndElement();

            _writer.WriteEndElement();

            _writer.WriteStartElement("Terminal");

            _writer.WriteStartElement("Side");
            _writer.WriteString("0.6");
            _writer.WriteEndElement();

            _writer.WriteStartElement("Altitude");
            _writer.WriteString("0.8");
            _writer.WriteEndElement();

            _writer.WriteEndElement();

            _writer.WriteEndElement();

            _writer.WriteEndDocument();

            _writer.Close();
        }

        void _readConf()
        {
            try
            {
                XmlDocument _reader = new XmlDocument();
                _reader.Load(configurationPath);

                XmlElement _root = _reader.DocumentElement;

                if (_root.ChildNodes.Count < 2)
                {
                    _createConf();
                    _reader.Load(configurationPath);
                    _root = _reader.DocumentElement;
                }

                foreach (XmlNode _node in _root.ChildNodes[1].ChildNodes)
                {
                    if (_node.Name == "Side")
                        side = double.Parse(_node.InnerText);
                    else if (_node.Name == "Altitude")
                        altitude = double.Parse(_node.InnerText);
                }
            }
            catch (Exception)
            {
            }
        }

    }
}
