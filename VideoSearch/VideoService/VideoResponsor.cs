using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;


namespace VideoSearch.VideoService
{
    public class VideoResponsor
    {
        #region Utility
        public static byte String2Byte(String value)
        {
            return (value == null) ? (byte)0 : Decimal.ToByte(Decimal.Parse(value));
        }

        public static short String2Short(String value)
        {
            return (value == null) ? (short)0 : Decimal.ToInt16(Decimal.Parse(value));
        }

        public static int String2Int(String value)
        {
            return (value == null) ? 0 : Decimal.ToInt32(Decimal.Parse(value));
        }

        public static Int64 String2Int64(String value)
        {
            return (value == null) ? 0 : Decimal.ToInt64(Decimal.Parse(value));
        }

        public static UInt64 String2UInt64(String value)
        {
            return (value == null) ? 0 : Decimal.ToUInt64(Decimal.Parse(value));
        }

        public static double String2Double(String value)
        {
            return (value == null) ? 0 : Decimal.ToDouble(Decimal.Parse(value));
        }

        public static bool String2Bool(String value)
        {
            int intVal = (value == null) ? 0 : Decimal.ToInt32(Decimal.Parse(value));

            return (intVal == 0) ? false : true;
        }

        public static String String2String(String value)
        {
            return (value == null) ? "" : (String)value.Clone();
        }

        public static Rect String2Rect(String value)
        {
            Rect rt = new Rect();

            if (value == null || value.Length == 0)
                return rt;

            string[] separators = { ","};
            string[] words = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<double> list = new List<double>();
            foreach (var word in words)
                list.Add(String2Double(word));

            if (list.Count == 4)
                return new Rect(list[0], list[1], list[2] - list[0], list[3] - list[1]);

            return rt;
        }
        #endregion

        #region QueryVideo & QueryVideoList
        public static QueryVideoResponse QueryVideoResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;
            QueryVideoResponse response = new QueryVideoResponse();
            
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
 
                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:

                            if (curVal == "VideoId")
                                response.VideoId = String2UInt64(reader.Value);
                            else if (curVal == "VideoName")
                                response.VideoName = String2String(reader.Value);
                            else if (curVal == "FilePath")
                                response.FilePath = String2String(reader.Value);
                            else if (curVal == "FileSize")
                                response.FileSize = String2UInt64(reader.Value);
                            else if (curVal == "TranscodeStatus")
                                response.TranscodeStatus = String2Int(reader.Value);
                            else if (curVal == "FrameCount")
                                response.FrameCount = String2Int(reader.Value);
                            else if (curVal == "Width")
                                response.Width = String2Int(reader.Value);
                            else if (curVal == "Height")
                                response.Height = String2Int(reader.Value);
                            else if (curVal == "SubmitTime")
                                response.SubmitTime = String2Int64(reader.Value);
                            else if (curVal == "RelatedTaskCount")
                                response.RelatedTaskCount = String2Int(reader.Value);
                            else if (curVal == "Progress")
                                response.Progress = String2Double(reader.Value);
                            else if (curVal == "CvtPath")
                                response.CvtPath = String2String(reader.Value);
                            else if (curVal == "FirstFrameBmp")
                                response.FirstFrameBmp = String2String(reader.Value);

                            break;
                        case XmlNodeType.EndElement:

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }

        public static List<VideoElement> QueryVideoListResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            VideoList listInfo = new VideoList();
            List<VideoElement> videoList = new List<VideoElement>();

            String curVal = null;
            VideoElement curItem = null;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // new element
                            if (reader.Name == "element")
                                curItem = new VideoElement();

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:
                            // VideoList
                            if (curVal == "Count")
                                listInfo.Count = String2Int(reader.Value);
                            else if (curVal == "TotalCount")
                                listInfo.TotalCount = String2Int(reader.Value);
                            else if (curVal == "State")
                                listInfo.State = String2Int(reader.Value);
                            // VideoElement
                            else if (curVal == "VideoId")
                                curItem.VideoId = String2UInt64(reader.Value);
                            else if (curVal == "VideoName")
                                curItem.VideoName = String2String(reader.Value);
                            else if (curVal == "FilePath")
                                curItem.FilePath = String2String(reader.Value);
                            else if (curVal == "FileSize")
                                curItem.FileSize = String2UInt64(reader.Value);
                            else if (curVal == "TranscodeStatus")
                                curItem.TranscodeStatus = String2Int(reader.Value);
                            else if (curVal == "FrameCount")
                                curItem.FrameCount = String2Int(reader.Value);
                            else if (curVal == "Width")
                                curItem.Width = String2Int(reader.Value);
                            else if (curVal == "Height")
                                curItem.Height = String2Int(reader.Value);
                            else if (curVal == "SubmitTime")
                                curItem.SubmitTime = String2Int64(reader.Value);
                            else if (curVal == "RelatedTaskCount")
                                curItem.RelatedTaskCount = String2Int(reader.Value);
                            else if (curVal == "Progress")
                                curItem.Progress = String2Double(reader.Value);
                            else if (curVal == "IsTranscode")
                                curItem.IsTranscode = String2Bool(reader.Value);

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "element" && curItem != null)
                                videoList.Add(curItem);

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return videoList;
        }

        public static async Task<List<VideoElement>> QueryVideoListResponsorAsync(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Async = true;

            VideoList listInfo = new VideoList();
            List<VideoElement> videoList = new List<VideoElement>();

            String curVal = null;
            VideoElement curItem = null;

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (await reader.ReadAsync())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // new element
                            if (reader.Name == "element")
                                curItem = new VideoElement();

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:
                            // VideoList
                            if (curVal == "Count")
                                listInfo.Count = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "TotalCount")
                                listInfo.TotalCount = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "State")
                                listInfo.State = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));

                            // VideoElement
                            else if (curVal == "VideoId")
                                curItem.VideoId = Decimal.ToUInt64(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "VideoName")
                                curItem.VideoName = String.Format("{0}", await reader.GetValueAsync());
                            else if (curVal == "FilePath")
                                curItem.FilePath = String.Format("{0}", await reader.GetValueAsync());
                            else if (curVal == "FileSize")
                                curItem.FileSize = Decimal.ToUInt64(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "TranscodeStatus")
                                curItem.TranscodeStatus = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "FrameCount")
                                curItem.FrameCount = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "Width")
                                curItem.Width = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "Height")
                                curItem.Height = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "SubmitTime")
                                curItem.SubmitTime = Decimal.ToInt64(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "RelatedTaskCount")
                                curItem.RelatedTaskCount = Decimal.ToInt32(Decimal.Parse(await reader.GetValueAsync()));
                            else if (curVal == "Progress")
                                curItem.Progress = String2Double(reader.Value);
                            else if (curVal == "IsTranscode")
                                curItem.IsTranscode = Boolean.Parse(await reader.GetValueAsync());

                            break;
                        case XmlNodeType.EndElement:
                            if (reader.Name == "element" && curItem != null)
                                videoList.Add(curItem);
 
                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return videoList;
        }
        #endregion

        #region Submit Video
        public static SubmitVideoResponse SubmitVideoAndTransCodeResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;

            SubmitVideoResponse response = new SubmitVideoResponse();

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
 
                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:
                            if (curVal == "State")
                                response.State = String2Int(reader.Value);
                            else if (curVal == "VideoId")
                                response.SubmitId = String2UInt64(reader.Value);
                            else if (curVal == "StrDesc")
                                response.StrDesc = String2String(reader.Value);

                            break;
                        case XmlNodeType.EndElement:
 
                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }

        #endregion

        #region QueryTask & QueryTaskList
        public static QueryTaskResponse QueryTaskResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;
            QueryTaskResponse response = new QueryTaskResponse();

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:

                            if (curVal == "TaskId")
                                response.TaskId = String2UInt64(reader.Value);
                            else if (curVal == "Status")
                                response.Status = String2Int(reader.Value);
                            else if (curVal == "SectionCount")
                                response.SectionCount = String2Int(reader.Value);
                            else if (curVal == "Progress")
                                response.Progress = String2Double(reader.Value);

                            break;
                        case XmlNodeType.EndElement:

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }

        public static QueryListDetailResponse QueryListDetailResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;
            QueryListDetailResponse response = new QueryListDetailResponse();
            
            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:

                            if (curVal == "Thickness")
                                response.Thickness = String2Int(reader.Value);
                            else if (curVal == "Sensitivity")
                                response.Sensitivity = String2Int(reader.Value);
                            else if (curVal == "RcgArea")
                                response.RcgArea = String2Rect(reader.Value);
                            else if (curVal == "UnrcgArea")
                                response.UnrcgArea = String2Rect(reader.Value);
                            else if (curVal == "ObjType")
                                response.ObjType = String2Int(reader.Value);
                            else if (curVal == "Color")
                                response.Color = String2Int(reader.Value);
                            else if (curVal == "AlarmLine")
                                response.AlarmLine = String2Int(reader.Value);
                            else if (curVal == "AlarmArea")
                                response.AlarmArea = String2Int(reader.Value);

                            break;
                        case XmlNodeType.EndElement:

                            curVal = null;

                            break;
                        default:
                            break;

                    }
                }
            }

            return response;
        }

        #endregion

        #region Submit Task
        public static SubmitTaskResponse CreateSummaryTaskResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;

            SubmitTaskResponse response = new SubmitTaskResponse();

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:
                            if (curVal == "State")
                                response.State = String2Int(reader.Value);
                            else if (curVal == "TaskId")
                                response.SubmitId = String2UInt64(reader.Value);
                            else if (curVal == "StrDesc")
                                response.StrDesc = String2String(reader.Value);

                            break;
                        case XmlNodeType.EndElement:

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }
        #endregion

        #region Submit Task Reslut (GetTaskSnapshot & GetVideoSummary)
        public static TaskSnapShotResponse GetTaskSnapshotResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            TaskSnapShot curItem = null;
            String curVal = null;

            TaskSnapShotResponse response = new TaskSnapShotResponse();

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            if (reader.Name == "Obj")
                                curItem = new TaskSnapShot();

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:

                            if (curVal == "FrameCount")
                                response.FrameCount = String2Int(reader.Value);
                            else if (curVal == "Progress")
                                response.Progress = String2Double(reader.Value);
                            else if (curVal == "CvtPath")
                                response.CvtPath = reader.Value;
                            else if (curVal == "ObjCount")
                            {
                                response.ObjCount = String2Int(reader.Value);
                                if (response.ObjCount > 0)
                                    response.ObjList = new List<TaskSnapShot>();
                            }

                            else if (curVal == "PicPath")
                                curItem.PicPath = reader.Value;
                            else if (curVal == "StartFrame")
                                curItem.StartFrame = String2Int(reader.Value);
                            else if (curVal == "EndFrame")
                                curItem.EndFrame = String2Int(reader.Value);
                            else if (curVal == "FrameCount")
                                curItem.FrameCount = String2Int(reader.Value);
                            else if (curVal == "ObjType")
                                curItem.ObjType = String2Int(reader.Value);
                            else if (curVal == "ObjPathLeft")
                                curItem.ObjPathLeft = String2Int(reader.Value);
                            else if (curVal == "ObjPathRight")
                                curItem.ObjPathRight = String2Int(reader.Value);
                            else if (curVal == "ObjPathTop")
                                curItem.ObjPathTop = String2Int(reader.Value);
                            else if (curVal == "ObjPathBottom")
                                curItem.ObjPathBottom = String2Int(reader.Value);

                            break;
                        case XmlNodeType.EndElement:

                            if (reader.Name == "Obj" && curItem != null)
                                response.ObjList.Add(curItem);

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }

        public static SummaryResponse GetVideoSummaryResponsor(Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.DtdProcessing = DtdProcessing.Parse;

            String curVal = null;
            SummaryResponse response = new SummaryResponse();

            using (XmlReader reader = XmlReader.Create(stream, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:

                            curVal = reader.Name;

                            break;
                        case XmlNodeType.Text:

                            if (curVal == "TbiPath")
                                response.TbiPath = reader.Value;
                            else if (curVal == "VideoSkimmingPath")
                                response.VideoSkimmingPath = reader.Value;

                            break;
                        case XmlNodeType.EndElement:

                            curVal = null;

                            break;
                        default:
                            break;
                    }
                }
            }

            return response;
        }
        #endregion
    }
}
