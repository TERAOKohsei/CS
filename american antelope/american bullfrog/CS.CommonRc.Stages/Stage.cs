using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CS.CommonRc.Inspections;
using System.IO;

namespace CS.CommonRc.Stages {
    public enum StageType {
        Linear = 0,
        Virtical = 1,
        Rotary = 2,
        UnknownType = -1,
    }

    struct TextConsts {
        public const string StageInformation = "StageInformation";
        public const string StageType = "StageType";
        public const string ProductName = "ProductName";
        public const string ProductType = "ProductType";
        public const string HaveOriginSensor = "HaveOriginSensor";
        public const string HaveLimitSensors = "HaveLimitSensors";
        public const string Resolution = "Resolution";
        public const string TravelRangeLower = "TravelRangeLower";
        public const string TravelRangeUpper = "TravelRangeUpper";
        public const string OriginPosition = "OriginPosition";
        public const string PositioningAccuracy = "PositioningAccuracy";
        public const string RepeatabilityOfPositioning = "RepeatabilityOfPositioning";
        public const string LostMotion = "LostMotion";
        public const string MotionAccuracyHorizontal = "MotionAccuracyHorizontal";
        public const string MotionAccuracyVertical = "MotionAccuracyVertical";
        public const string MotionAccuracyYaw = "MotionAccuracyYaw";
        public const string MotionAccuracyPitch = "MotionAccuracyPitch";
        public const string ParallelismOfMotion = "ParallelismOfMotion";
        public const string InspectTravelRange = "InspectTravelRange";
        public const string InspectPositioningAccuracy = "InspectPositioningAccuracy";
        public const string InspectRepeatabilityOfPositioning = "InspectRepeatabilityOfPositioning";
        public const string InspectLostMotion = "InspectLostMotion";
        public const string InspectMotionAccuracyHorizontal = "InspectMotionAccuracyHorizontal";
        public const string InspectMotionAccuracyVertical = "InspectMotionAccuracyVertical";
        public const string InspectMotionAccuracyYaw = "InspectMotionAccuracyYaw";
        public const string InspectMotionAccuracyPitch = "InspectMotionAccuracyPitch";
        public const string InspectParallelismOfMotion = "InspectParallelismOfMotion";
        public const string LowerSpeedPps = "LowerSpeedPps";
        public const string UpperSpeedPps = "UpperSpeedPps";
        public const string AccelerationTimeMs = "AccelerationTimeMs";
        public const string LostMotionCorrectPps = "LostMotionCorrectPps";
        public const string WaitTimeMsAfterStopped = "WaitTimeMsAfterStopped";
        public const string MeasuringPointCount = "MeasuringPointCount";
        public const string RepeatCount = "RepeatCount";
        public const string MeasuringPositions = "MeasuringPositions";
    }

    public struct Stage {
        public StageType StageType { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        #region General Specs
        public bool HaveOriginSensor { get; set; }
        public bool HaveLimitSensors { get; set; }
        public double Resolution { get; set; }
        public Range<double> TravelRange { get; set; }
        #endregion // General Specs
        #region Accuracy Specs
        public double PositionAccuracy { get; set; }
        public double RepeatabilityOfPositioning { get; set; }
        public double LostMotion { get; set; }
        public double MotionAccuracyHorizontal { get; set; }
        public double MotionAccuracyVertical { get; set; }
        public double MotionAccuracyYaw { get; set; }
        public double MotionAccuracyPitch { get; set; }
        public double ParallelismOfMotion { get; set; }
        #endregion // Accuracy Specs
        public InspectionItems InspectionItems { get; set; }
        #region // Conditions
        public int LowerSpeedPps { get; set; }
        public int UpperSpeedPps { get; set; }
        public int AccelerationTimeMs { get; set; }
        public int LostMotionCorrectPps { get; set; }
        public int WaitTimeMsAfterStopped { get; set; }
        public int MeasuringPointCount { get; set; }
        public int RepeatCount { get; set; }
        public double[] MeasuringPositions { get; set; }
        #endregion // Conditons

        private void LoadInformationVersion0(StreamReader reader) {
            StageType = Stages.StageType.Linear;
            ProductName = "";
            HaveOriginSensor = true;
            HaveLimitSensors = true;
            Resolution = 0.002;
            PositionAccuracy = Double.NaN;
            RepeatabilityOfPositioning = Double.NaN;
            LostMotion = Double.NaN;
            LowerSpeedPps = 1000;
            UpperSpeedPps = 5000;
            AccelerationTimeMs = 100;
            LostMotionCorrectPps = 1000;
            WaitTimeMsAfterStopped = 1000;
            RepeatCount = 7;

            InspectionItems = Inspections.InspectionItems.Nothing;
            while ( !reader.EndOfStream ) {
                var words = reader.ReadLine();

                switch ( words ) {
                case "総移動量":
                    double travelRange = Double.Parse(reader.ReadLine());
                    TravelRange = new Range<double>(travelRange/2, -travelRange/2, 0);
                    break;
                case "測定点数":
                    MeasuringPointCount = Int32.Parse(reader.ReadLine());
                    break;
                case "ｽﾃｰｼﾞｻｲｽﾞ":
                case "HV判定":
                case "YP判定":
                case "走り判定":
                    reader.ReadLine();
                    break;
                case "HV測定":
                    if ( Int32.Parse(reader.ReadLine()) == 1 ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyHorizontal | Inspections.InspectionItems.MotionAccuracyVertical;
                    }
                    break;
                case "H精度":
                    MotionAccuracyHorizontal = Double.Parse(reader.ReadLine());
                    break;
                case "V精度":
                    MotionAccuracyVertical = Double.Parse(reader.ReadLine());
                    break;
                case "YP測定":
                    if ( Int32.Parse(reader.ReadLine()) == 1 ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyYaw | Inspections.InspectionItems.MotionAccuracyPitch;
                    }
                    break;
                case "Y精度":
                    MotionAccuracyYaw = Double.Parse(reader.ReadLine());
                    break;
                case "P精度":
                    MotionAccuracyPitch = Double.Parse(reader.ReadLine());
                    break;
                case "走り精度":
                    ParallelismOfMotion = Double.Parse(reader.ReadLine());
                    break;
                default:
                    break;
                }

                MeasuringPositions = new double[] {
                    TravelRange.Lower + TravelRange.Difference /10,
                    0,
                    TravelRange.Upper - TravelRange.Difference / 10
                };
            }
        }

        private void LoadInformationVersion1(StreamReader reader) {
            InspectionItems = Inspections.InspectionItems.Nothing;
            double upper = 0, lower = 0, offset = 0;
            while ( !reader.EndOfStream ) {
                var words = reader.ReadLine().Split(',');

                bool b;
                double d;
                switch ( words[0] ) {
                case TextConsts.StageType:
                    StageType = (StageType)Enum.Parse(typeof(StageType), words[1]);
                    break;
                case TextConsts.ProductName:
                    ProductName = words[1];
                    break;
                case TextConsts.ProductType:
                    ProductType = words[1];
                    break;
                case TextConsts.HaveOriginSensor:
                    Boolean.TryParse(words[1], out b);
                    HaveOriginSensor = b;
                    break;
                case TextConsts.HaveLimitSensors:
                    Boolean.TryParse(words[1], out b);
                    HaveLimitSensors = b;
                    break;
                case TextConsts.Resolution:
                    Double.TryParse(words[1], out d);
                    Resolution = d;
                    break;
                case TextConsts.TravelRangeLower:
                    lower = Double.Parse(words[1]);
                    break;
                case TextConsts.TravelRangeUpper:
                    upper = Double.Parse(words[1]);
                    break;
                case TextConsts.OriginPosition:
                    offset = Double.Parse(words[1]);
                    break;
                case TextConsts.PositioningAccuracy:
                    Double.TryParse(words[1], out d);
                    PositionAccuracy = d;
                    break;
                case TextConsts.RepeatabilityOfPositioning:
                    Double.TryParse(words[1], out d);
                    RepeatabilityOfPositioning = d;
                    break;
                case TextConsts.LostMotion:
                    Double.TryParse(words[1], out d);
                    LostMotion = d;
                    break;
                case TextConsts.MotionAccuracyHorizontal:
                    MotionAccuracyHorizontal = Double.Parse(words[1]);
                    break;
                case TextConsts.MotionAccuracyVertical:
                    MotionAccuracyVertical = Double.Parse(words[1]);
                    break;
                case TextConsts.MotionAccuracyYaw:
                    MotionAccuracyYaw = Double.Parse(words[1]);
                    break;
                case TextConsts.MotionAccuracyPitch:
                    MotionAccuracyPitch = Double.Parse(words[1]);
                    break;
                case TextConsts.ParallelismOfMotion:
                    ParallelismOfMotion = Double.Parse(words[1]);
                    break;
                case TextConsts.InspectTravelRange:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.TravelRange;
                    }
                    break;
                case TextConsts.InspectPositioningAccuracy:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.PositioningAccuracy;
                    }
                    break;
                case TextConsts.InspectRepeatabilityOfPositioning:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.RepeatabilityofPositioning;
                    }
                    break;
                case TextConsts.InspectLostMotion:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.LostMotion;
                    }
                    break;
                case TextConsts.InspectMotionAccuracyHorizontal:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyHorizontal;
                    }
                    break;
                case TextConsts.InspectMotionAccuracyVertical:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyVertical;
                    }
                    break;
                case TextConsts.InspectMotionAccuracyYaw:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyYaw;
                    }
                    break;
                case TextConsts.InspectMotionAccuracyPitch:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.MotionAccuracyPitch;
                    }
                    break;
                case TextConsts.InspectParallelismOfMotion:
                    if ( Boolean.Parse(words[1]) ) {
                        InspectionItems |= Inspections.InspectionItems.ParallelismOfMotion;
                    }
                    break;
                case TextConsts.LowerSpeedPps:
                    LowerSpeedPps = Int32.Parse(words[1]);
                    break;
                case TextConsts.UpperSpeedPps:
                    UpperSpeedPps = Int32.Parse(words[1]);
                    break;
                case TextConsts.AccelerationTimeMs:
                    AccelerationTimeMs = Int32.Parse(words[1]);
                    break;
                case TextConsts.LostMotionCorrectPps:
                    LostMotionCorrectPps = Int32.Parse(words[1]);
                    break;
                case TextConsts.WaitTimeMsAfterStopped:
                    WaitTimeMsAfterStopped = Int32.Parse(words[1]);
                    break;
                case TextConsts.MeasuringPointCount:
                    MeasuringPointCount = Int32.Parse(words[1]);
                    break;
                case TextConsts.RepeatCount:
                    RepeatCount = Int32.Parse(words[1]);
                    break;
                case TextConsts.MeasuringPositions:
                    MeasuringPositions = new double[words.Count() - 1];
                    for ( int i = 1; i < words.Count(); i++ ) {
                        MeasuringPositions[i - 1] = Double.Parse(words[i]);
                    }
                    break;
                default:
                    break;
                }

                TravelRange = new Range<double>(upper, lower, offset);
            }
        }

        public void LoadInformation(string filePath) {
            using ( var sr = new StreamReader(filePath, Encoding.GetEncoding("shift-jis")) ) {
                var words = sr.ReadLine().Split(',');
                if ( (words[0] == TextConsts.StageInformation) && words[1] == "1" ) {
                    LoadInformationVersion1(sr);
                } else {
                    ProductType = words[0];
                    LoadInformationVersion0(sr);
                }
            }
        }
    }
}
