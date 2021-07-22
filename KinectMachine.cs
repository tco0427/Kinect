using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Net;


///Kinect 2.0 SDK (25 JointType https://msdn.microsoft.com/en-us/library/microsoft.kinect.jointtype.aspx)
///Kinect 1.8 SDK (20 JointType https://msdn.microsoft.com/en-us/library/jj131025.aspx)


namespace KinectModule
{
    class KinectMachine
    {
        //키넥트 장치
        private KinectManager kinect;

        //정보 선택자
        private KinectDataSelector selector;

        //정보 교정자
        private KinectDataCorrector corrector;

        //키넥트 정보
        //private string deviceName = "Kinect";

        //정보 보관
        private KinectDataPool pooler;
        
        //정보 분석
        private KinectDataAnalyzer analyzer;

        //정보 전달
        private KinectDataSender sender;

        //서버
        private string configurationPath = "./Configuration/Kinect.conf";
        private string serverIP;
        private int kinectInputPort;
        private double sendInterval;

        public string PoseRateInformation
        {
            get;
            set;
        }

        public string Information
        {
            get;
            set;
        }

        public double[] PoseRate
        {
            get;
            set;
        }

        public PoseFeature FeaturePoint
        {
            get;
            set;
        }

        public XSkeleton Skeleton
        {
            get;
            set;
        }

        public PoseDetailRate PoseDetailRates
        {
            get;
            set;
        }

        Mutex kinectSyncMutex;

        Thread processThread;
        bool IsAlive;

        public KinectMachine(KinectModule.ImageViewer viewer)
        {
            kinect = new KinectManager(viewer);            

            selector = new KinectDataSelector();
            selector.Kinect = kinect;

            corrector = new KinectDataCorrector();
            corrector.Kinect = kinect;

            pooler = new KinectDataPool();
            pooler.Kinect = kinect;

            _readConf();

            kinectSyncMutex = new Mutex(false, "KinectPoolSync");

            analyzer = new KinectDataAnalyzer();
            analyzer.Kinect = kinect;

            sender = new KinectDataSender(serverIP, kinectInputPort);

            processThread = new Thread(new ThreadStart(_processFunc));
            IsAlive = true;

            PoseRateInformation =
                "<포즈 상태>\r\n" +
                "BOTH_SIDE_OUT = " + 0 + "\r\n" +
                "BOTH_SIDE_UP = " + 0 + "\r\n" +
                "BOTH_SIDE_FRONT = " + 0 + "\r\n" +
                "BOTH_SIDE_DOWN = " + 0 + "\r\n" +
                "ONLY_LEFT_HAND_UP = " + 0 + "\r\n" +
                "ONLY_RIGHT_HAND_UP = " + 0 + "\r\n" +
                "ONLY_LEFT_KNEE_UP = " + 0 + "\r\n" +
                "ONLY_RIGHT_KNEE_UP = " + 0 + "\r\n" +
                "ONLY_LEFT_LEG_AB = " + 0 + "\r\n" +
                "ONLY_RIGHT_LEG_AB = " + 0 + "\r\n" +
                "BOTH_HAND_TOUCH = " + 0 + "\r\n" +
                "SQUAT = " + 0 + "\r\n";
        }

        ~KinectMachine()
        {
            if (processThread != null && processThread.IsAlive)
                processThread.Abort();
            processThread = null;

            kinectSyncMutex.Close();
        }

        public string PoseClassification()
        {
            if (kinect.StateUpdater != null)
                return "포즈 상태";
            return PoseRateInformation;
        }

        public bool IsRunning()
        {
            if (kinect.IsRunning)
                return true;
            return false;
        }

        public string Start()
        {
            processThread.Start();
            if (kinect.Start)
                return "<키넥트>\r\n동작 중";//IP : " + serverIP + "\r\nPort : " + kinectInputPort + "\r\nInterval : " + sendInterval;
            return "<키넥트>\r\n키넥트 장치를 시작할 수 없습니다.";
        }

        public void Close()
        {
            bool _result = kinect.Stop;

            IsAlive = false;

            if (processThread.IsAlive)
                processThread.Abort();
            processThread = null;
        }

        private void _processFunc()
        {
            while (IsAlive)
            {
                PoseRate = analyzer.Rate; // update PoseRate
                FeaturePoint = analyzer.FeaturePoint; // update PoseFeature
                Skeleton = analyzer.TrackedSkeleton; // update Skeleton
                PoseDetailRates = analyzer.PoseDetailRates; // update PoseDetailRates
                //System.Diagnostics.Trace.WriteLine("_processFunc(): " + PoseDetailRates);

                //전송할 메시지 추출
                //Dictionary<int, InputMessage> _message = analyzer.Analyze(pooler.Output()); // 정보 분석
                Dictionary<int, KinectPoseInputMessage> _message = analyzer.Analyze(pooler.Output()); // 정보 분석
                PoseRateInformation = 
                    "<포즈 상태>\r\n" +
                    "BOTH_SIDE_OUT = " + analyzer.Rate[0].ToString() + "\r\n" +
                    "BOTH_SIDE_UP = " + analyzer.Rate[1].ToString() + "\r\n" +
                    "BOTH_SIDE_FRONT = " + analyzer.Rate[2].ToString() + "\r\n" +
                    "BOTH_SIDE_DOWN = " + analyzer.Rate[3].ToString() + "\r\n" +
                    "ONLY_LEFT_HAND_UP = " + analyzer.Rate[4].ToString() + "\r\n" +
                    "ONLY_RIGHT_HAND_UP = " + analyzer.Rate[5].ToString() + "\r\n" +
                    "ONLY_LEFT_KNEE_UP = " + analyzer.Rate[6].ToString() + "\r\n" +
                    "ONLY_RIGHT_KNEE_UP = " + analyzer.Rate[7].ToString() + "\r\n" +
                    "ONLY_LEFT_LEG_AB = " + analyzer.Rate[8].ToString() + "\r\n" +
                    "ONLY_RIGHT_LEG_AB = " + analyzer.Rate[9].ToString() + "\r\n" +
                    "BOTH_HAND_TOUCH = " + analyzer.Rate[10].ToString() + "\r\n" +
                    "SQUAT = " + analyzer.Rate[11].ToString() + "\r\n";

                // 메시지 테스트
                /*foreach (KeyValuePair<int, KinectPoseInputMessage> _pair in _message)
                {
                    Byte[] _data = _pair.Value; // KinectPoseInputMessage를 byte[]로
                    KinectPoseInputMessage _im = _data; // byte[]를 KinectPoseInputMessage로

                    System.Diagnostics.Trace.WriteLine("DeviceName:" + _im.DeviceName);
                    System.Diagnostics.Trace.WriteLine("HostName:" + _im.HostName);
                    for (int i = 0; i < 25; i++)
                        System.Diagnostics.Trace.WriteLine("UserBody:" + _im.UserBody[i].ToString());
                    System.Diagnostics.Trace.WriteLine("UserFeature:\n" + _im.UserFeature.ToString());
                }*/

                //메시지 전송
                /*if (_message.Count > 0)
                    sender.SendMessage(deviceName, _message);
                Thread.Sleep((int)(sendInterval * 1000));
                */
            }
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

                Information = "";

                foreach (XmlNode _node in _root.ChildNodes[0].ChildNodes)
                {
                    if (_node.Name == "ServerIP")
                        serverIP = _node.InnerText;
                    else if (_node.Name == "PortNumber")
                        kinectInputPort = int.Parse(_node.InnerText);
                    else if (_node.Name == "Interval")
                        sendInterval = double.Parse(_node.InnerText);
                }

                Information += "<Network>\n\r";
                Information += "ServerIP : " + serverIP + "\n\r";
                Information += "PortNumber : " + kinectInputPort + "\n\r";


                string _side = "";
                string _altitude = "";
                foreach (XmlNode _node in _root.ChildNodes[1].ChildNodes)
                {
                    if (_node.Name == "Side")
                        _side = _node.InnerText;
                    else if (_node.Name == "Altitude")
                        _altitude = _node.InnerText;
                }

                Information += "\n\r<Arrange>\n\r";
                Information += "Side : " + _side + "\n\r";
                Information += "Altitude : " + _altitude + "\n\r";
            }
            catch (Exception)
            {
            }
        }
    }
}
