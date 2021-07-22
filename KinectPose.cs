using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace KinectModule
{
    public enum KinectPoseFeature
    {
        ELA, //0
        ERA, //1
        SLA, //2
        SRA, //3
        ULA, //4
        URA, //5
        DLA, //6
        DRA, //7
        FLA, //8
        FRA, //9
        KLA, //10
        KRA, //11
        KLA90, //12
        KRA90, //13
        LLA, //14
        LRA, //15
        LLA45, //16
        LRA45, //17
        SSA, //18
        AD, //19
        HD //20
    }

    public enum KinectPoseName
    {
        BOTH_SIDE_OUT,
        BOTH_SIDE_UP,
        BOTH_SIDE_FRONT,
        BOTH_SIDE_DOWN,
        ONLY_LEFT_HAND_UP,
        ONLY_RIGHT_HAND_UP,
        ONLY_LEFT_KNEE_UP,
        ONLY_RIGHT_KNEE_UP,
        ONLY_LEFT_LEG_AB,
        ONLY_RIGHT_LEG_AB,
        BOTH_HAND_TOUCH,
        SQUAT
    }

    public class PoseFeature : ISerializable
    {
        public DateTime CurrentTime
        {
            get;
            set;
        }
        static readonly int count = Enum.GetNames(typeof(KinectPoseFeature)).Length;
        double[] featurePoints; // = new double[21];

        public double[] getFeaturePoints(){
            return this.featurePoints;
        }

        public PoseFeature()
        {
            CurrentTime = DateTime.Now;
            featurePoints = new double[21];
        }

        public PoseFeature(PoseFeature other)
        {
            CurrentTime = other.CurrentTime;
            if (featurePoints == null)
                featurePoints = new double[21];
            for (int i = 0; i < 21; i++)
                featurePoints[i] = other.featurePoints[i];
        }

        // ISerializable
        protected PoseFeature(SerializationInfo info, StreamingContext context)
        {
            CurrentTime = info.GetDateTime("CurrentTime");
            for (int i = 0; i < 21; i++)
            {
                featurePoints[i] = info.GetDouble(((KinectPoseFeature)i).ToString());
            }
        }

        // ISerializable
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CurrentTime", CurrentTime);
            for (int i = 0; i < 21; i++)
            {
                info.AddValue(((KinectPoseFeature)i).ToString(), featurePoints[i]);
            }
        }

        public int Length
        {
            get
            {
                return featurePoints.Length;
            }
        }

        public double this[int key]
        {
            get
            {
                return this.featurePoints[key];
            }
            set
            {
                if (key < 0 || key >= featurePoints.Length)
                    return;
                this.featurePoints[key] = value;
            }
        }

        public double this[KinectPoseFeature key]
        {
            get
            {
                return this.featurePoints[(int)key];
            }
            set
            {
                this.featurePoints[(int)key] = value;
            }
        }

        public static implicit operator List<double>(PoseFeature source)
        {
            List<double> value = new List<double>();
            for (int i = 0; i < 21; i++)
            {
                value.Add(source.featurePoints[i]);
            }

            return value;
        }

        public static implicit operator byte[] (PoseFeature source)
        {
            byte[] value = new byte[21 * sizeof(double)];
            for (int i = 0; i < 21; i++)
            {
                byte[] feature = BitConverter.GetBytes(source.featurePoints[i]);
                Array.Copy(feature, 0, value, i * sizeof(double), sizeof(double));
            }

            return value;
        }

        public static implicit operator PoseFeature(byte[] source)
        {
            PoseFeature value = new PoseFeature();
            value.CurrentTime = DateTime.Now; // check!

            if (source.Length < 21 * sizeof(double))
                return value;

            for (int i = 0; i < 21; i++)
            {
                value[i] = System.BitConverter.ToDouble(source, sizeof(double) * i);
            }

            return value;
        }

        public override string ToString()
        {
            string str = CurrentTime.ToString();
            for (int i = 0; i < 21; i++)
            {
                //str += string.Format("{0}\t{1:0.###}\n", (KinectPoseFeature)i, featurePoints[i]);
                str += string.Format(",{0:0.###}", featurePoints[i]);
            }
            return str;
        }
    }

    public class PoseDetailRate : ISerializable
    {
        public Dictionary<KinectPoseName, int> PoseNameCount = new Dictionary<KinectPoseName, int>()
        {
            {KinectPoseName.BOTH_SIDE_OUT, 6},
            {KinectPoseName.BOTH_SIDE_UP, 6},
            {KinectPoseName.BOTH_SIDE_FRONT, 6},
            {KinectPoseName.BOTH_SIDE_DOWN, 8},
            {KinectPoseName.ONLY_LEFT_HAND_UP, 5},
            {KinectPoseName.ONLY_RIGHT_HAND_UP, 5},
            {KinectPoseName.ONLY_LEFT_KNEE_UP, 5},
            {KinectPoseName.ONLY_RIGHT_KNEE_UP, 5},
            {KinectPoseName.ONLY_LEFT_LEG_AB, 4},
            {KinectPoseName.ONLY_RIGHT_LEG_AB, 4},
            {KinectPoseName.BOTH_HAND_TOUCH, 3},
            {KinectPoseName.SQUAT, 3}
        };

        public DateTime CurrentTime
        {
            get;
            set;
        }

        public Dictionary<KinectPoseName, ValuePair<double, List<double>>> PoseMatchingRates;
        double[] rates;

        public PoseDetailRate()
        {
            CurrentTime = DateTime.Now;
            PoseMatchingRates = new Dictionary<KinectPoseName, ValuePair<double, List<double>>>();
            rates = new double[72];
        }

        public PoseDetailRate(Dictionary<KinectPoseName, ValuePair<double, List<double>>> poseMatchingRates)
        {
            CurrentTime = DateTime.Now;
            PoseMatchingRates = poseMatchingRates;
            rates = new double[72];
            int index = 0;
            foreach (var pair in PoseMatchingRates)
            {
                rates[index] = pair.Value.First;
                for (int i = 0; i < pair.Value.Second.Count; i++)
                {
                    rates[index] = pair.Value.Second[i];
                }
                index++;
            }
        }
        
        public PoseDetailRate(PoseDetailRate other)
        {
            //CurrentTime
            CurrentTime = other.CurrentTime;
            //PoseMatchingRates
            PoseMatchingRates = new Dictionary<KinectPoseName, ValuePair<double, List<double>>>();
            foreach (var pair in other.PoseMatchingRates)
            {
                KinectPoseName poseName = pair.Key;
                ValuePair<double, List<double>> detailRates = new ValuePair<double, List<double>>();
                detailRates.First = pair.Value.First; // totalRate by pose
                detailRates.Second = new List<double>(); // detailRate by pose
                foreach (var item in pair.Value.Second) 
                {
                    detailRates.Second.Add(item); 
                }
                PoseMatchingRates.Add(poseName, detailRates);
            }
            //rates
            if (rates == null)
                rates = new double[72];
            int index = 0;
            foreach (var pair in PoseMatchingRates)
            {
                rates[index] = pair.Value.First;
                for (int i = 0; i < pair.Value.Second.Count; i++)
                {
                    rates[index] = pair.Value.Second[i];
                }
                index++;
            }
        }

        // ISerializable
        protected PoseDetailRate(SerializationInfo info, StreamingContext context)
        {
            CurrentTime = info.GetDateTime("CurrentTime");
            for (int i = 0; i < PoseNameCount.Count; i++)
            {
                KinectPoseName poseName = (KinectPoseName)i;
                ValuePair<double, List<double>> detailRates = new ValuePair<double, List<double>>();
                detailRates.First = info.GetDouble(poseName.ToString()); // totalRate by KinectPoseName
                detailRates.Second = new List<double>();
                for (int j = 0; j < PoseNameCount[poseName]; j++)
                {
                    detailRates.Second.Add(info.GetDouble(poseName.ToString()+j.ToString()));
                }
                PoseMatchingRates.Add(poseName, detailRates);
            }
        }

        // ISerializable
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CurrentTime", CurrentTime);
            foreach (var pair in PoseMatchingRates)
            {
                info.AddValue(pair.Key.ToString(), pair.Value.First);
                for (int i = 0; i < pair.Value.Second.Count; i++)
                {
                    info.AddValue(pair.Key.ToString() + i.ToString(), pair.Value.Second[i]);
                }
            }
        }

        /*public static implicit operator byte[] (PoseDetailRate source)
        {
            byte[] value = new byte[72 * sizeof(double)];
            for (int i = 0; i < 72; i++)
            {
                byte[] rates = BitConverter.GetBytes(source.rates[i]);
                Array.Copy(rates, 0, value, i * sizeof(double), sizeof(double));
            }

            return value;
        }

        public static implicit operator PoseDetailRate(byte[] source)
        {
            PoseDetailRate value = new PoseDetailRate();
            if (source.Length < 72 * sizeof(double))
                return value;

            for (int i = 0; i < 72; i++)
            {
                value = System.BitConverter.ToDouble(source, sizeof(double) * i);
            }

            return value;
        }*/

        public override string ToString()
        {
            string str = CurrentTime.ToString();
            foreach (var pair in PoseMatchingRates)
            {
                str += "," + pair.Key.ToString() + "=" + pair.Value.First.ToString();
                for (int i = 0; i < pair.Value.Second.Count; i++)
                {
                    str += "," + pair.Key.ToString() + i.ToString() + "=" + pair.Value.Second[i];
                }
            }
            return str;
        }
    }

    public struct KinectPoseInfo // 자세별 인식 판별 구간 정보
    {
        public KinectPoseName PoseName;
        public List<int> IndexList;
        public List<double> MinList;
        public List<double> MaxList;

        public KinectPoseInfo(KinectPoseName name, List<int> indexList, List<double> minList, List<double> maxList)
        {
            PoseName = name;
            IndexList = indexList;
            MinList = minList;
            MaxList = maxList;
        }

        public int KinectPoseInfoCount
        {
            get
            {
                return IndexList.Count;
            }
        }

        public override string ToString()
        {
            return PoseName.ToString() + " Index:" + string.Join(",", IndexList.Select(i => i.ToString()).ToArray()) + " Min:" + string.Join(",", MinList.Select(i => i.ToString()).ToArray()) + " Max:" + string.Join(",", MaxList.Select(i => i.ToString()).ToArray());
        }

        public void Print()
        {
            System.Diagnostics.Trace.WriteLine(this.ToString());
        }
    }

    public class KinectPose
    {
        public static Dictionary<int, KinectPoseInfo> poseList = new Dictionary<int, KinectPoseInfo>()
        {
            {0, new KinectPoseInfo(KinectPoseName.BOTH_SIDE_OUT, 
                            new List<int>() {0, 1, 2, 3, 10, 11}, 
                            new List<double>() {10, 10, 10, 10, 15, 15}, 
                            new List<double>() {20, 20, 20, 20, 25, 25})},
            {1, new KinectPoseInfo(KinectPoseName.BOTH_SIDE_UP,
                            new List<int>() {0, 1, 4, 5, 10, 11}, 
                            new List<double>() {15, 15, 20, 20, 15, 15}, 
                            new List<double>() {25, 25, 40, 40, 25, 25})},
            {2, new KinectPoseInfo(KinectPoseName.BOTH_SIDE_FRONT,
                            new List<int>() {0, 1, 8, 9, 10, 11},
                            new List<double>() {15, 15, 20, 20, 15, 15},
                            new List<double>() {25, 25, 40, 40, 25, 25})},
            {3, new KinectPoseInfo(KinectPoseName.BOTH_SIDE_DOWN, // 차렷
                            new List<int>() {0, 1, 6, 7, 10, 11, 14, 15},
                            new List<double>() {20, 20, 15, 15, 15, 15, 10, 10},
                            new List<double>() {35, 35, 25, 25, 25, 25, 20, 20})},
            {4, new KinectPoseInfo(KinectPoseName.ONLY_LEFT_HAND_UP,
                            new List<int>() {0, 4, 7, 10, 11},
                            new List<double>() {15, 20, 20, 15, 15},
                            new List<double>() {25, 40, 40, 25, 25})},
            {5, new KinectPoseInfo(KinectPoseName.ONLY_RIGHT_HAND_UP,
                            new List<int>() {1, 5, 6, 10, 11},
                            new List<double>() {15, 20, 20, 15, 15},
                            new List<double>() {25, 40, 40, 25, 25})},
            {6, new KinectPoseInfo(KinectPoseName.ONLY_LEFT_KNEE_UP, // 왼쪽무릎들기 (왼무릎각90 12:10-25)
                            new List<int>() {6, 7, 11, 12, 15},
                            new List<double>() {15, 15, 10, 10, 10},
                            new List<double>() {25, 25, 20, 25, 25})},
            {7, new KinectPoseInfo(KinectPoseName.ONLY_RIGHT_KNEE_UP, // 오른쪽무릎들기 (오른무릎각90 13:10-25)
                            new List<int>() {6, 7, 10, 13, 14},
                            new List<double>() {15, 15, 10, 10, 10},
                            new List<double>() {25, 25, 20, 25, 25})},
            {8, new KinectPoseInfo(KinectPoseName.ONLY_LEFT_LEG_AB, //  왼발 옆으로 들기
                            new List<int>() {10, 11, 15, 16},
                            new List<double>() {15, 15, 10, 15},
                            new List<double>() {25, 25, 20, 25})},
            {9, new KinectPoseInfo(KinectPoseName.ONLY_RIGHT_LEG_AB, // 오른발 옆으로 들기
                            new List<int>() {10, 11, 14, 17},
                            new List<double>() {15, 15, 10, 15},
                            new List<double>() {25, 25, 20, 25})},
            {10, new KinectPoseInfo(KinectPoseName.BOTH_HAND_TOUCH, //  박수 (손모으기) 무릎편 상태에서 손거리만 가까이
                            new List<int>() {10, 11, 20},
                            new List<double>() {15, 15, 0.05},
                            new List<double>() {25, 25, 0.15})},
            {11, new KinectPoseInfo(KinectPoseName.SQUAT, // Squat 양쪽 무릎각이 90이고 허리굽힘정도 40미만
                            new List<int>() {12, 13, 18},
                            new List<double>() {12, 12, 20},
                            new List<double>() {22, 22, 40})}
        };
        public static Dictionary<int, KinectPoseInfo> PoseList = poseList; // 다른 postList도 사용가능

        public static ValuePair<double, List<double>> ComputeMatchingRate(List<double> min, List<double> max, List<double> value)
        {
            double result = 0.0; // 인식판별값 총평균
            List<double> rateList = new List<double>(); // 자세별 주요각도에 대한 인식판별값 저장

            /*for (int i = 0; i < value.Count; i++)
            {
                if (value[i] > max[i]) return new ValuePair<double, List<double>>(result, rateList); // no need to compute if value is out of max, rate is 0
            }*/

            for (int i = 0; i < value.Count; i++)
            {
                double y = 0.0;
                if (value[i] > max[i])
                {
                    y = 0.0;
                }
                else
                {
                    y = 100 * Math.Pow(0.7, ((value[i] - min[i]) / (max[i] - min[i]))); // e.g. exponential interpolation x: min ~ max => y: 100 ~ 70
                    if (y > 100) y = 100.0; // clamp to 100 if y>100
                    else if (y < 70) y = 0.0; // set to 0 if y<70
                }
                rateList.Add(y);
                result += y; // sum
            }
            result /= value.Count; // average

            return new ValuePair<double, List<double>>(result, rateList);
        }
        /*
        public static double ComputeMatchingRate2(List<double> min, List<double> max, List<double> value)
        {
            double result = 0.0;
            System.Diagnostics.Trace.WriteLine("value[2] = " + value[2]);

            for (int i = 0; i < value.Count; i++)
            {
                if (value[i] > max[i]) return 0.0; // no need to compute if value is out of max, rate is 0
            }

            for (int i = 0; i < value.Count; i++)
            {
                double y = 100 - 30 * ((value[i] - min[i]) / (max[i] - min[i])); // e.g. linear interpolation x: min ~ max => y: 100 ~ 70
                if (y > 100) y = 100.0; // clamp to 100 if y>100
                else if (y < 70) y = 0.0; // set to 0 if y<70
                result += y; // sum
            }
            result /= value.Count; // average

            return result;
        }
        */
        public static ValuePair<double, List<double>> PoseMatchingRate(KinectPoseName poseName, List<double> poseData)
        {
            int poseIndex = (int)poseName;
            if (PoseList.ContainsKey(poseIndex))
            {
                List<int> indexList = PoseList[poseIndex].IndexList;
                List<double> minList = PoseList[poseIndex].MinList;
                List<double> maxList = PoseList[poseIndex].MaxList;
                List<double> valueList = new List<double>(); // 키넥트 각도 데이터 21종 중에서 자세별 주요각도만 선별
                foreach (int i in indexList) valueList.Add(poseData[i]); // 키넥트 각도 데이터 21종 중에서 자세별 주요각도만 선별

                return ComputeMatchingRate(minList, maxList, valueList); // compute matching rate within the min-max range
            }
            return new ValuePair<double, List<double>>(0, null);
        }
        /*
        public static double PoseMatchingRate2(int poseIndex, List<double> poseData) // linear interpolation
        {
            double rate = 0.0;
            if (PoseList.ContainsKey(poseIndex))
            {
                List<double> minList = PoseList[poseIndex].MinList;
                List<double> maxList = PoseList[poseIndex].MaxList;
                List<int> indexList = PoseList[poseIndex].IndexList;
                List<double> valueList = new List<double>();
                foreach (int i in indexList)
                {
                    valueList.Add(poseData[i]);
                }
                rate = ComputeMatchingRate2(minList, maxList, valueList); // compute matching rate within the min-max range
                System.Diagnostics.Trace.WriteLine(PoseList[poseIndex].PoseName + " Rate=" + rate);
            }
            return rate;
        }
        */
    }
}
