using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;


namespace KinectModule
{
    class KinectDataPool
    {
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
                manager.Pooler = SkeletonPooling;
            }
        }

        private double StorageTime = 2.0;
        private Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> inputData;
        private Dictionary<int, List<DateTime>> inputTimes;

        private Mutex syncMutex;

        public KinectDataPool()
        {
            syncMutex = new Mutex(false, "KinectPoolSync");
            inputData = new Dictionary<int,ValuePair<List<DateTime>,List<XSkeleton>>>();
            inputTimes = new Dictionary<int, List<DateTime>>();
        }

        public void SkeletonPooling(Dictionary<int, XSkeleton> data)
        {
            if (data == null)
                return;

            foreach(KeyValuePair<int, XSkeleton> _pair in data)
                this.Input(_pair.Key, _pair.Value);
        }

        public void Input(int id, XSkeleton data)
        {
            syncMutex.WaitOne();
            
            if (!inputData.ContainsKey(id))
                inputData.Add(id, new ValuePair<List<DateTime>, List<XSkeleton>>(new List<DateTime>(), new List<XSkeleton>()));

            inputData[id].First.Add(DateTime.Now);
            inputData[id].Second.Add(data);
            
            syncMutex.ReleaseMutex();
        }

        public Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> Output()
        {
            syncMutex.WaitOne();
            //데이터 업데이트 - 오래된 데이터 제거
            List<int> _removeID = new List<int>();
            DateTime _now = DateTime.Now;
            foreach (KeyValuePair<int, ValuePair<List<DateTime>, List<XSkeleton>>> _idx in inputData)
            {
                TimeSpan _ts = _now - _idx.Value.First[_idx.Value.First.Count - 1];
                if (_ts.TotalSeconds > 1.0)
                {
                    _removeID.Add(_idx.Key);
                    continue;
                }

                int _removeCount = 0;
                foreach (DateTime _dt in _idx.Value.First)
                {
                    _ts = _now - _dt;
                    if (_ts.TotalSeconds > StorageTime)
                        _removeCount++;
                    else
                        break;
                }
                _idx.Value.First.RemoveRange(0, _removeCount);
                _idx.Value.Second.RemoveRange(0, _removeCount);
            }

            foreach (int _id in _removeID)
                inputData.Remove(_id);

            //현재 데이터 반환
            Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>> _result = new Dictionary<int, ValuePair<List<DateTime>, List<XSkeleton>>>();
            foreach (KeyValuePair<int, ValuePair<List<DateTime>, List<XSkeleton>>> _idx in inputData)
            {
                List<DateTime> _dl = new List<DateTime>();
                _dl.AddRange(_idx.Value.First.GetRange(0, _idx.Value.First.Count));
                List<XSkeleton> _skel = new List<XSkeleton>();
                _skel.AddRange(_idx.Value.Second.GetRange(0, _idx.Value.Second.Count));
                
                _result.Add(_idx.Key, new ValuePair<List<DateTime>, List<XSkeleton>>(_dl, _skel));
            }

            syncMutex.ReleaseMutex();

            return _result;
        }
    }
}
