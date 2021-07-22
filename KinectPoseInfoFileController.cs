using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Kinect;

namespace KinectModule
{
    class KinectPoseInfoFileController
    {
        public static string PoseInfoFileName { get; set; }

        public static Dictionary<int, KinectPoseInfo> ReadKinectPoseFile()
        {
            if (PoseInfoFileName == "")
                return null;

            Dictionary<int, KinectPoseInfo> _poseList = new Dictionary<int, KinectPoseInfo>();

            try
            {
                XmlDocument _reader = new XmlDocument();
                _reader.Load(PoseInfoFileName);

                XmlElement _root = _reader.DocumentElement;
                if (_root.Name != "KinectPoseInfo") 
                    return null;

                foreach (XmlNode _poseInfoNode in _root.ChildNodes)
                {
                    KinectPoseInfo _poseInfo = new KinectPoseInfo();
                    foreach (XmlAttribute _poseAtt in _poseInfoNode.Attributes)
                    {
                        if (_poseAtt.Name == "Name")
                        {
                            _poseInfo.PoseName = (KinectPoseName)Enum.Parse(typeof(KinectPoseName), _poseAtt.Value);
                        }
                    }

                    List<int> _poseFeatureIndexList = new List<int>();
                    List<double> _poseFeatureMinList = new List<double>();
                    List<double> _poseFeatureMaxList = new List<double>();
                    KinectPoseFeature _poseFeatureIndex = 0;
                    double min = 0.0;
                    double max = 0.0;
                    foreach (XmlNode _poseFeature in _poseInfoNode.ChildNodes)
                    {
                        foreach (XmlAttribute _featureAtt in _poseFeature.Attributes)
                        {
                            switch (_featureAtt.Name)
                            {
                                case "Name":
                                    _poseFeatureIndex = (KinectPoseFeature)Enum.Parse(typeof(KinectPoseFeature), _featureAtt.Value);
                                    break;
                                case "MinValue":
                                    min = double.Parse(_featureAtt.Value);
                                    break;
                                case "MaxValue":
                                    max = double.Parse(_featureAtt.Value);
                                    break;
                            }
                        }

                        _poseFeatureIndexList.Add((int)_poseFeatureIndex);
                        _poseFeatureMinList.Add(min);
                        _poseFeatureMaxList.Add(max);
                    }

                    _poseInfo.IndexList = _poseFeatureIndexList;
                    _poseInfo.MinList = _poseFeatureMinList;
                    _poseInfo.MaxList = _poseFeatureMaxList;
                    _poseList.Add((int)_poseInfo.PoseName, _poseInfo);

                    //_poseInfo.DebugPrint(); // debug
                }
            }
            catch (System.IO.IOException _e)
            {
                System.Diagnostics.Trace.WriteLine("XML에러 : " + _e.Message);
            }
            return _poseList;
        }

        // Write은 제대로 체크해야 함!!
        public static bool WriteKinectPoseFile(Dictionary<int, KinectPoseInfo> poseList)
        {
            if (PoseInfoFileName == null || PoseInfoFileName == "")
                return false;

            XmlTextWriter _writer = new XmlTextWriter(PoseInfoFileName, Encoding.UTF8);
            _writer.Formatting = Formatting.Indented;

            _writer.WriteStartDocument();

            _writer.WriteStartElement("KinectPoseInfo");

            foreach (KeyValuePair<int, KinectPoseInfo> _pose in poseList)
            {
                _writer.WriteStartElement("PoseInfo");
                //PoseInfo Attribute
                _writer.WriteStartAttribute("Name");
                _writer.WriteString(_pose.Value.PoseName.ToString());
                _writer.WriteEndAttribute();

                for (int i = 0; i < _pose.Value.IndexList.Count; i++ )
                {
                    _writer.WriteStartElement("PoseFeature"); //<PoseFeature>
                    //Name
                    _writer.WriteStartAttribute("Name");
                    _writer.WriteString(((KinectPoseFeature)_pose.Value.IndexList[i]).ToString());
                    _writer.WriteEndAttribute();
                    //MinValue
                    _writer.WriteStartAttribute("MinValue");
                    _writer.WriteValue(_pose.Value.MinList[i].ToString());
                    _writer.WriteEndAttribute();
                    //MaxValue
                    _writer.WriteStartAttribute("MaxValue");
                    _writer.WriteValue(_pose.Value.MaxList[i].ToString());
                    _writer.WriteEndAttribute();
                    _writer.WriteEndElement(); //</PoseFeature>
                }

                _writer.WriteEndElement();                  //</PoseInfo>
            }

            _writer.WriteEndElement();

            _writer.WriteEndDocument();

            _writer.Close();
            return true;
        }
    }
}
