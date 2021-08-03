using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KinectModule;
using System.IO;

namespace KinectTerminal
{
    public partial class KinectTerminal : Form
    {
        // kinect machine
        KinectMachine kinect;
        // kinect image viewer
        KinectModule.ImageViewer ImageViewer;
        // pose tracking
        Dictionary<int, KinectPoseInfo> readPoseList = null;
        // pose recording & stop toggle button
        //private ToggleButton button1;

        private List<XSkeleton> skeletonList = new List<XSkeleton>();
        private List<PoseFeature> featureList = new List<PoseFeature>();
        private List<double[]> rateList = new List<double[]>();
        private List<PoseDetailRate> detailRateList = new List<PoseDetailRate>();
        private float startTime;
        private float finishTime;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public KinectTerminal()
        {
            InitializeComponent();
            // kinect pose recording toggle button
            /*button1 = new ToggleButton(null);
            button1.Location = new System.Drawing.Point(15, 650);
            button1.Size = new System.Drawing.Size(150, 50);
            button1.Text = "KinectPoseRecord";
            button1.CheckedText = "Recording";
            button1.UncheckedText = "Stop";
            button1.Click += new EventHandler(button1_Click);
            this.Controls.Add(button1);*/

            // kinect image viewer
            this.ImageViewer = new KinectModule.ImageViewer();
            SceneViewer.Child = this.ImageViewer;
            // kinect machine
            kinect = new KinectMachine(this.ImageViewer);

            // kinect info
            KinectInformationLabel.Text = kinect.Information + "\n\r" + kinect.Start(); // kinect info
            // pose info
            PoseInformationLabel.Text = kinect.PoseClassification(); // pose info
        }

        /*private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Checked)   // start recording when checked
            {
                // start recording
                timer1.Start(); // start timer
                timer1.Interval = 1000;
                System.Diagnostics.Trace.WriteLine("Timer start " + DateTime.Now.ToString());
            }
            else                // stop recording when unchecked
            {
                // stop recording & save into the file
                timer1.Stop(); // stop timer
                System.Diagnostics.Trace.WriteLine("Timer stop");

                DateTime date = DateTime.Now;
                string path = string.Format("c:\\KinectPose\\kinect_rate_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var rate in rateList)
                    {
                        for (int i = 0; i < 11; i++)
                        {
                            sw.Write("{0},", rate[i]);
                        }
                        sw.WriteLine(rate[11]);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\KinectPose\\kinect_feature_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var feature in featureList)
                    {
                        sw.WriteLine(feature);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\KinectPose\\kinect_skeleton_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var skeleton in skeletonList)
                    {
                        //System.Diagnostics.Trace.WriteLine("SAVE: " + skeleton);
                        sw.WriteLine(skeleton);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\KinectPose\\kinect_detailrate_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var detailrate in detailRateList)
                    {
                        sw.WriteLine(detailrate);
                    }
                    sw.Close();
                }

                rateList.Clear();
                featureList.Clear();
                skeletonList.Clear();
                detailRateList.Clear();
                MessageBox.Show("포즈데이터를 저장했습니다.\n" + DateTime.Now.ToString(), path);
            }
        }*/


        // kinect pose recognition text update
        private void timer1_Tick(object sender, EventArgs e)
        {
            PoseInformationLabel.Text = kinect.PoseRateInformation;
            //System.Diagnostics.Trace.WriteLine("timer1_Tick(): " + kinect.PoseRateInformation);
            if (timer1.Enabled)
            {
                //System.Diagnostics.Trace.WriteLine("TIMER : " + kinect.FeaturePoint);
                detailRateList.Add(new PoseDetailRate(kinect.PoseDetailRates));
                skeletonList.Add(new XSkeleton(kinect.Skeleton));
                featureList.Add(new PoseFeature(kinect.FeaturePoint));
                double[] rate = new double[12]; // rateList
                for (int i = 0; i < 12; i++)
                {
                    rate[i] = kinect.PoseRate[i];
                }
                rateList.Add(rate); // end of rateList
            }
        }
        private void KinectIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            KinectIcon.Visible = false;
            this.Focus();
        }

        private void KinectTerminal_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.ShowInTaskbar = false;
                KinectIcon.Visible = true;
                KinectIcon.Text = "Kinect Terminal";
            }
        }

        private void KinectTerminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            kinect.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog _ofd = new OpenFileDialog();
            _ofd.InitialDirectory = "./";
            _ofd.Filter = "Kinect Pose Information File(*.kpi)|*.kpi";
            DialogResult _dr = _ofd.ShowDialog();

            if (_dr != DialogResult.OK)
                return;

            KinectPoseInfoFileController.PoseInfoFileName = _ofd.FileName;
            readPoseList = KinectPoseInfoFileController.ReadKinectPoseFile();
            KinectPose.PoseList = readPoseList; // set to readPoseList

            foreach (KeyValuePair<int, KinectPoseInfo> _poseInfo in KinectPose.PoseList)
            {
                System.Diagnostics.Trace.WriteLine("OPEN: " + _poseInfo.Key + " ");
                _poseInfo.Value.Print();

                ListViewItem item = new ListViewItem(_poseInfo.Value.PoseName.ToString());
                string range = "";
                for (int index = 0; index < _poseInfo.Value.IndexList.Count; index++) // (보류)
                {
                    range += ((KinectPoseFeature)_poseInfo.Value.IndexList[index]).ToString() + "[" + _poseInfo.Value.MinList[index].ToString() + "-" + _poseInfo.Value.MaxList[index].ToString() + "]\n";
                }
                item.SubItems.Add(new ListViewItem.ListViewSubItem(item, range));
                //poseInfoList.Items.Add(item);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //if (registeredPose.Count == 0)
            //{
            //    MessageBox.Show("저장할 항목이 없습니다.");
            //    return;
            //}

            if (KinectPoseInfoFileController.PoseInfoFileName == null || KinectPoseInfoFileController.PoseInfoFileName == "")
                SaveNewFile();

            bool result = KinectPoseInfoFileController.WriteKinectPoseFile(readPoseList);

            if (!result)
                MessageBox.Show("포즈를 저장하지 못했습니다.");
        }

        private void SaveNewFile()
        {
            SaveFileDialog _sfd = new SaveFileDialog();
            _sfd.InitialDirectory = "./";
            _sfd.Filter = "Kinect Pose Information File(*.kpi)|*.kpi";

            DialogResult _dr = _sfd.ShowDialog();
            if (_dr != DialogResult.OK)
                return;

            string _saveFileName = _sfd.FileName;
            if ("kpi" != FileExtension(_saveFileName))
                _saveFileName = FileExtensionChange(_saveFileName, "kpi");

            KinectPoseInfoFileController.PoseInfoFileName = _saveFileName;
            bool result = KinectPoseInfoFileController.WriteKinectPoseFile(readPoseList);
        }

        private string FileExtension(string fileName)
        {
            int _extensionStartIndex = fileName.LastIndexOf('.') + 1;
            return fileName.Substring(_extensionStartIndex, fileName.Length - _extensionStartIndex);
        }

        private string FileExtensionChange(string fileName, string newExtension)
        {
            int _extensionStartIndex = fileName.LastIndexOf('.') + 1;
            return fileName.Substring(0, _extensionStartIndex) + newExtension;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // look for the expected key
            if (keyData == Keys.Space)
            {
                // take some action
                System.Diagnostics.Trace.WriteLine("Space-Key Pressed");
                if (toggleButton1.Checked) {
                    toggleButton1.Checked = false;
                    toggleButton1.Text = "Stop";
                }
                else
                {
                    toggleButton1.Checked = true;
                    toggleButton1.Text = "Start Recording";
                }
                this.toggleButton1_Click(toggleButton1, new EventArgs());

                // eat the message to prevent it from being passed on
                return true;

                // (alternatively, return FALSE to allow the key event to be passed on)
            }

            // call the base class to handle other key events
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void toggleButton1_Click(object sender, EventArgs e)
        {
            if (toggleButton1.Checked)   // start recording when checked
            {
                // start recording
                timer1.Start(); // start timer
                timer1.Interval = 10;
                System.Diagnostics.Trace.WriteLine("Timer start " + DateTime.Now.ToString());
                stopwatch.Reset();
                stopwatch.Start();
            }
            else                // stop recording when unchecked
            {
                // stop recording & save into the file
                timer1.Stop(); // stop timer
                System.Diagnostics.Trace.WriteLine("Timer stop");
                stopwatch.Stop();

                float processTime = stopwatch.ElapsedMilliseconds/1000.0f;

                DateTime date = DateTime.Now;
                string path = string.Format("c:\\kinect_origin\\kinect_rate_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var rate in rateList)
                    {
                        for (int i = 0; i < 11; i++)
                        {
                            sw.Write("{0},", rate[i]);
                        }
                        sw.WriteLine(rate[11]);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\kinect_origin\\kinect_feature_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var feature in featureList)
                    {
                        sw.WriteLine(feature);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\kinect_origin\\kinect_skeleton_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var skeleton in skeletonList)
                    {
                        //System.Diagnostics.Trace.WriteLine("SAVE: " + skeleton);
                        sw.WriteLine(skeleton);
                    }
                    sw.Close();
                }
                path = string.Format("c:\\kinect_origin\\kinect_detailrate_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                    foreach (var detailrate in detailRateList)
                    {
                        sw.WriteLine(detailrate);
                    }
                    sw.Close();
                }

                //이동변화량, 속도
                path = string.Format("c:\\kinect_origin\\kinect_moveDifference_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
                var x=0.0;
                var y=0.0;
                var z=0.0;

                var sumX=0.0;
                var sumY=0.0;
                var sumZ=0.0;

                var euculidDistance = 0.0;

                foreach ( var skeleton in skeletonList){

                    if(skeletonList.IndexOf(skeleton)==0){
                         absX = skeleton.xJoints[1].X;
                         absY = skeleton.xJoints[1].Y;
                         absZ = skeleton.xJoints[1].Z;
                    }else{
                        var diffX = skeleton.xJoints[1].X;
                        var diffY = skeleton.xJoints[1].Y;
                        var diffZ = skeleton.xJoints[1].Z;

                        var absX = Math.Abs(diffX - x);
                        var absY = Math.Abs(diffY - y);
                        var absZ = Math.Abs(diffZ - z);

                        euculidDistance = (Math.Sqrt((absX * absX)+(absY * absY) + (absZ * absZ))) + euculidDistance;

                        sumX=absX+sumX;
                        sumY=absY+sumY;
                        sumZ=absZ+sumZ;
                  
                        x=diffX;
                        y=diffY;
                        z=diffZ;
                    }
                }

                sw.WriteLine(sumX);
                sw.WriteLine(sumY);
                sw.WriteLine(sumZ);
                sw.WriteLine(euculidDistance);
                sw.WriteLine(processTime);
                sw.WriteLine(euculidDistance/processTime);

                sw.Close();
                }

                //각 변화량, 각속도
                path = string.Format("c:\\kinect_origin\\kinect_angle_{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}.csv", date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                using (FileStream fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);

                var elaValue=0.0;
                var eraValue=0.0;
                var klaValue=0.0;
                var kraValue=0.0;
                
                var elaSum=0.0;
                var eraSum=0.0;
                var klaSum=0.0;
                var kraSum=0.0;

                foreach (var feature in featureList)
                {
                    double[] featurePoints=feature.getFeaturePoints();

                    if(featureList.IndexOf(feature)==0){
                        elaValue=featurePoints[0];
                        eraValue=featurePoints[1];
                        klaValue=featurePoints[10];
                        kraValue=featurePoints[11];
                    }
                    else{
                        double getEla=featurePoints[0];   //ela
                        double getEra=featurePoints[1];   //era
                        double getKla=featurePoints[10];  //kla
                        double getKra=featurePoints[11];  //kra

                        elaSum=Math.Abs(getEla-elaValue) + elaSum;
                        eraSum=Math.Abs(getEra-eraValue) + eraSum;
                        klaSum=Math.Abs(getKla-klaValue) + klaSum;
                        kraSum=Math.Abs(getKra-kraValue) + kraSum;

                        elaValue=getEla;
                        eraValue=getEra;
                        klaValue=getKla;
                        kraValue=getKra;
                    }
                }
                
                sw.WriteLine(elaSum);
                sw.WriteLine(elaSum/processTime);
                sw.WriteLine(eraSum);
                sw.WriteLine(eraSum/processTime);
                sw.WriteLine(klaSum);
                sw.WriteLine(klaSum/processTime);
                sw.WriteLine(kraSum);
                sw.WriteLine(kraSum/processTime);
                sw.Close();
                }

                rateList.Clear();
                featureList.Clear();
                skeletonList.Clear();
                detailRateList.Clear();
                stopwatch.Reset();
                processTime = 0.0f;
                MessageBox.Show("포즈데이터를 저장했습니다.\n" + DateTime.Now.ToString(), path);
            }
        }
    }
}
