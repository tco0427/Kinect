using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Kinect;
using System.Diagnostics;

namespace KinectModule
{
    class KinectManager
    {
        // KinectSensor
        private KinectSensor _sensor;
        MultiSourceFrameReader _reader;
        IList<Body> _bodies; // skeletons

        public KinectModule.ImageViewer Viewer
        {
            get;
            set;
        }
        /*public System.Windows.Media.ImageSource RGB
        {
            get;
            set;
        }
        public System.Windows.Controls.Canvas Body
        {
            get;
            set;
        }*/

        public bool IsRunning
        {
            get
            {
                if (_sensor != null && _sensor.IsOpen)
                    return true;
                else return false;
            }
        }
        public bool Start
        {
            get
            {
                if (_sensor == null)
                    return false;

                if (_sensor.IsOpen)
                    return true;

                try
                {
                    _sensor = KinectSensor.GetDefault();
                    _sensor.Open(); 
                }
                catch (IOException)
                {
                    _sensor = null;
                    return false;
                }

                return true;
            }
        }
        public bool Stop
        {
            get
            {
                if (!IsRunning)
                    return false;

                _sensor.Close();
                return true;
            }
        }

        public int Angle
        {
            get
            {
                // Kinect 2.0 SDK 지원안함
                return 0;
            }
        }

        public SkeletonSelectFunc Selector;

        public SkeletonCorrectFunc Corrector;

        public SkeletonSendFunc Pooler;

        public SkeletonAnalyzeFunc Analyzer;

        public TriggerFunc StateUpdater;

        public Dictionary<int, XSkeleton> CurrentData;

        public KinectManager(KinectModule.ImageViewer viewer)
        {
            Viewer = viewer; // assign
            System.Diagnostics.Trace.WriteLine("Viewer");

            _sensor = KinectSensor.GetDefault();

            if (_sensor == null)
            {
                return;
            }

            _sensor.Open();
            _reader = _sensor.OpenMultiSourceFrameReader(FrameSourceTypes.Color | FrameSourceTypes.Body);
            _reader.MultiSourceFrameArrived += Reader_MultiSourceFrameArrived;

            Selector = null;
            Corrector = null;
            Pooler = null;
            StateUpdater = null;

            CurrentData = null;
        }

        ~KinectManager()
        {
            try
            {
                if (_sensor != null/* && _sensor.IsOpen*/)
                {
                    //_sensor.Close();
                }
            }
            catch (System.NullReferenceException)
            {
                Trace.WriteLine("test");
            }
            if (_reader != null)
            {
                _reader.Dispose();
            }
        }

        void Reader_MultiSourceFrameArrived(object sender, MultiSourceFrameArrivedEventArgs e)
        {
            var reference = e.FrameReference.AcquireFrame();

            // Color
            using (var frame = reference.ColorFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (Viewer == null) System.Diagnostics.Trace.WriteLine("Viewer NULL~~~~");
                    if (Viewer != null) Viewer.CameraImage.Source = frame.ToBitmap();
                }
            }


            // Body
            using (var frame = reference.BodyFrameReference.AcquireFrame())
            {
                if (frame != null)
                {
                    if (Viewer != null) Viewer.SkeletonCanvas.Children.Clear();

                    _bodies = new Body[frame.BodyFrameSource.BodyCount];
                    frame.GetAndRefreshBodyData(_bodies);

                    //유효한 스켈레톤만을 선택
                    if (Selector != null)
                        _bodies = Selector(_bodies.ToArray());
                    if (_bodies == null)
                        return;

                    //스켈레톤 정보를 교정
                    Dictionary<int, XSkeleton> _correctData = null;
                    if (Corrector != null)
                        _correctData = Corrector(_bodies.ToArray());
                    CurrentData = _correctData;

                    //스켈레톤 정보 저장
                    if (Pooler != null)
                        Pooler(_correctData);

                    //foreach (var body in _bodies)
                    //{
                    if (_bodies[0] != null)
                    {
                        if (_bodies[0].IsTracked)
                        {
                            // Draw skeleton.
                            if (Viewer != null) Viewer.SkeletonCanvas.DrawSkeleton(_bodies[0]);
                        }
                    }
                    //}
                }
            }
        }
/*
        private void UpdateSkeleton(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] _skeletons = new Skeleton[0];
            //스켈레톤 프레임을 읽어들임
            using (SkeletonFrame _sf = e.OpenSkeletonFrame())
            {
                if (_sf != null)
                {
                    _skeletons = new Skeleton[_sf.SkeletonArrayLength];
                    _sf.CopySkeletonDataTo(_skeletons);
                }
            }
            if (_skeletons.Length == 0)
                return;

            //유효한 스켈레톤만을 선택
            if(Selector != null)
                _skeletons = Selector(_skeletons);

            if (_skeletons == null)
                return;
            
            //스켈레톤 정보를 교정
            Dictionary<int, XSkeleton> _correctData = null;
            if (Corrector != null)
                _correctData = Corrector(_skeletons);
            
            CurrentData = _correctData;

            //스켈레톤 정보 저장
            if (Pooler != null)
                Pooler(_correctData);
             
        }
*/
    }
}
