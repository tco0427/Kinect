Kinect2 SDK 사용

KinectPose.cs 안에 12종 자세인식 정보 

KinectDataAnalyzer.cs 안에 FindPose 에서 키넥트 스켈레톤으로부터 정보를 추출해서 자세인식에 사용

        /// <summary>
        /// 12종 운동자세 (BOTH_SIDE_OUT, BOTH_SIDE_UP, BOTH_SIDE_FRONT, BOTH SIDE DOWN, LEFT_HAND_UP, RIGHT_HAND_UP, LEFT_KNEE_UP, RIGHT_KNEE_UP, LEFT_LEG_AB, RIGHT_LEG_AB, SQUAT, CLAP)
        /// </summary>
        /// <param name="skelList"></param>
        /// <returns></returns>
        private int FindPose(ValuePair<List<DateTime>, List<XSkeleton>> skelList)