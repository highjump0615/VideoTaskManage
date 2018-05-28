using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using VideoSearch.Utils;
using VideoSearch.VideoService;

namespace VideoSearch.Model
{
    public class MovieTaskCompressItem : MovieTaskItem
    {
        public class FRM_INFO_T
        {
            public Int32 version; //meta data recording format version 
            public Int32 objNum; //number of objects on "frmIdx" frame 
            public Int64 frmIdx; //frame Index in summary video. Begin from 0 and increases by 1 per each frame
            public List<OBJ_MARKER_INFO_T> objInfos = new List<OBJ_MARKER_INFO_T>();  //objs' info on "frmIdx" frame
        }

        public class OBJ_MARKER_INFO_T
        {
            public Int32 objId;     //object's ID
            public Int32 compntId;
            public Int64 orgVFrmIdx;    //object's frame index in original video
            public Int64 orgVStartFrmIdx;   //object's start frame index in original video
            public Int64 orgVideoTimeStamp;     //object'timestamp in orgnial video 
            public Int64 orgVideoStartTimeStamp;    //object's start timestamp in original video. 
            public Int64 orgVideoEndTimeStamp;      //object's end timestamp in original video
            public Int32 frmNum;
            public Int32 bbULXInSumV;   //object's bounding box coordinate
            public Int32 bbULYInSumV;   //object's bounding box coordinate
            public Int32 bbWidth;   //object's bounding box size 
            public Int32 bbHeight;  //object's bounding box size
            public Int32 objType;   //object type(pedestrain, vehicle, other)
            public Int32 reserve;   //reserved field
        }

        ////////////////////////////////////////////////////////////////////////////////
        //每个目标在浓缩视频中开始结束时间
        public class OBJ_INFO
        {
            public Int32 objId;             //四字段均与久凌DAT OBJ_MARKER_INFO_T中字段相对应
            public Int32 compntId;
            public Int64 orgVideoStartTimeStamp;
            public Int64 orgVideoEndTimeStamp;

            public Int64 sumVideoStartTimeStamp;
            public Int64 sumVideoEndTimeStamp;
        }
        List<OBJ_INFO> objInfos = new List<OBJ_INFO>();

        #region Constructor & Init

        public MovieTaskCompressItem(DataItemBase parent = null)
            : base(parent)
        {
        }

        public MovieTaskCompressItem(DataItemBase parent, String id, String displayID, String taskId, String name, String moviePos,
                        MovieTaskType taskType, MovieTaskState state) 
            : base(parent, id, displayID, taskId, name, moviePos, taskType, state)
        {
        }

        public MovieTaskCompressItem(DataItemBase parent, String taskId, String name, MovieTaskType taskType) 
            : base(parent, taskId, name, taskType)
        {
        }

        #endregion

        #region Property
        private String _compressedPath = "";
        public String CompressedPath
        {
            get { return _compressedPath; }
            set
            {
                if (_compressedPath != value)
                {
                    _compressedPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("CompressedPath"));
                }
            }
        }
        public String CompressedPlayPath
        {
            get
            {
                if (State != MovieTaskState.Created || CompressedPath == null || CompressedPath.Length == 0)
                    return null;

                return basePath + CompressedPath;
            }
        }

        public String CompresseInfoXmlPath
        {
            get
            {
                var strDirectory = Path.GetDirectoryName(basePath + _tbiPath);
                return $"{strDirectory}\\{_compressInfoPath}";
            }
        }
        #endregion

        private String _tbiPath = "";

        private readonly String _compressInfoPath = "compressInfo.xml";

        #region Override
        public override void UpdateProperty()
        {
        }
        #endregion

        /// <summary>
        /// 获取任务结果
        /// </summary>
        public override async Task FetchResult()
        {
            XElement response = await ApiManager.Instance.GetVideoSummary(TaskId);

            if (response != null)
            {
                _compressedPath = response.Element("VideoSkimmingPath").Value;
                _tbiPath = response.Element("TbiPath").Value;

                // 是否已解析tbi文件
                if (!File.Exists(CompresseInfoXmlPath))
                {
                    // 解析
                    JiuLingDatParse(basePath + _tbiPath, CompresseInfoXmlPath);
                }
            }
        }

        private Byte[] GetSubByte(Byte[] src, int offset, int length)
        {
            Byte[] dest = new Byte[length];
            Array.Copy(src, offset, dest, 0, length);
            return dest;
        }

        /// <summary>
        /// 将久凌的浓缩视频DAT标注文件转换成自定义的XML文件
        /// </summary>
        /// <param name="srcFilePath"></param>
        /// <param name="destFilePath"></param>
        public void JiuLingDatParse(string srcFilePath, string destFilePath)
        {
            Stream flstr = new FileStream(srcFilePath, FileMode.Open);
            BinaryReader sr = new BinaryReader(flstr, Encoding.Unicode);

            //acknowledgement
            Byte[] buffer = sr.ReadBytes((int)flstr.Length);

            int bufferLen = buffer.Length;
            int readCount = 0;

            List<FRM_INFO_T> frm_infos = new List<FRM_INFO_T>();

            try
            {
                while (readCount < bufferLen)
                {
                    FRM_INFO_T frm_info = new FRM_INFO_T();
                    Byte[] frameBuffer = GetSubByte(buffer, readCount, 16);
                    readCount += 16;

                    frm_info.version = BitConvert.ToInt32(frameBuffer, 0);
                    frm_info.objNum = BitConvert.ToInt32(frameBuffer, 4);
                    frm_info.frmIdx = BitConvert.ToInt64(frameBuffer, 8);

                    for (int objCount = 0; objCount < frm_info.objNum; objCount++)
                    {
                        OBJ_MARKER_INFO_T objMarkerInfo = new OBJ_MARKER_INFO_T();
                        Byte[] objBuffer = GetSubByte(buffer, readCount, 80);
                        readCount += 80;
                        objMarkerInfo.objId = BitConvert.ToInt32(objBuffer, 0);
                        objMarkerInfo.compntId = BitConvert.ToInt32(objBuffer, 4);
                        objMarkerInfo.orgVFrmIdx = BitConvert.ToInt64(objBuffer, 8);
                        objMarkerInfo.orgVStartFrmIdx = BitConvert.ToInt64(objBuffer, 16);
                        objMarkerInfo.orgVideoTimeStamp = BitConvert.ToInt64(objBuffer, 24);
                        objMarkerInfo.orgVideoStartTimeStamp = BitConvert.ToInt64(objBuffer, 32);
                        objMarkerInfo.orgVideoEndTimeStamp = BitConvert.ToInt64(objBuffer, 40);
                        objMarkerInfo.frmNum = BitConvert.ToInt32(objBuffer, 48);
                        objMarkerInfo.bbULXInSumV = BitConvert.ToInt32(objBuffer, 52);
                        objMarkerInfo.bbULYInSumV = BitConvert.ToInt32(objBuffer, 56);
                        objMarkerInfo.bbWidth = BitConvert.ToInt32(objBuffer, 60);
                        objMarkerInfo.bbHeight = BitConvert.ToInt32(objBuffer, 64);
                        objMarkerInfo.objType = BitConvert.ToInt32(objBuffer, 68);
                        objMarkerInfo.reserve = BitConvert.ToInt32(objBuffer, 72);

                        frm_info.objInfos.Add(objMarkerInfo);
                    }
                    frm_infos.Add(frm_info);
                }
            }
            catch (System.Exception ex)
            {

            }
            finally
            {
                XmlTextWriter xmlWriter;
                xmlWriter = new XmlTextWriter(destFilePath, Encoding.UTF8);
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteStartElement("标注集合");
                //tbi.tubeInfos
                foreach (FRM_INFO_T frm_info in frm_infos)
                {
                    xmlWriter.WriteStartElement("标注");
                    xmlWriter.WriteAttributeString("时间戳", (frm_info.frmIdx * 40).ToString());

                    foreach (OBJ_MARKER_INFO_T objMarkerInfo in frm_info.objInfos)
                    {
                        xmlWriter.WriteStartElement("物标");
                        xmlWriter.WriteAttributeString("左上角横坐标", objMarkerInfo.bbULXInSumV.ToString());
                        xmlWriter.WriteAttributeString("左上角纵坐标", objMarkerInfo.bbULYInSumV.ToString());
                        xmlWriter.WriteAttributeString("右下角横坐标", (objMarkerInfo.bbULXInSumV + objMarkerInfo.bbWidth).ToString());
                        xmlWriter.WriteAttributeString("右下角纵坐标", (objMarkerInfo.bbULYInSumV + objMarkerInfo.bbHeight).ToString());
                        xmlWriter.WriteAttributeString("颜色", "0");
                        xmlWriter.WriteAttributeString("时标", (objMarkerInfo.orgVFrmIdx * 40).ToString());
                        xmlWriter.WriteAttributeString("原始开始时间", objMarkerInfo.orgVideoStartTimeStamp.ToString());
                        xmlWriter.WriteAttributeString("原始结束时间", objMarkerInfo.orgVideoEndTimeStamp.ToString());
                        xmlWriter.WriteEndElement();

                        OBJ_INFO objInfo = new OBJ_INFO();
                        objInfo.objId = objMarkerInfo.objId;
                        objInfo.compntId = objMarkerInfo.compntId;
                        objInfo.orgVideoStartTimeStamp = objMarkerInfo.orgVideoStartTimeStamp;
                        objInfo.orgVideoEndTimeStamp = objMarkerInfo.orgVideoEndTimeStamp;

                        int i = 0;
                        for (; i < objInfos.Count; i++)
                        {
                            if (objInfos[i].objId == objInfo.objId
                                && objInfos[i].compntId == objInfo.compntId)
                            {
                                objInfos[i].sumVideoEndTimeStamp = frm_info.frmIdx * 40;
                                break;
                            }
                        }
                        if (i >= objInfos.Count)
                        {
                            objInfo.sumVideoStartTimeStamp = objInfo.sumVideoEndTimeStamp = frm_info.frmIdx * 40;
                            objInfos.Add(objInfo);
                        }
                    }
                    xmlWriter.WriteEndElement();
                }
                xmlWriter.WriteEndElement();
                xmlWriter.Close();

                //////////
                //                 FileStream fs;
                //                 fs = new FileStream(@"D:\test.txt", FileMode.Create);
                //                 StreamWriter bw = new StreamWriter(fs);
                //                 for (int i = 0; i < objInfos.Count; i++)
                //                 {
                //                     bw.Write(objInfos[i].objId);
                //                     bw.Write(" ");
                //                     bw.Write(objInfos[i].compntId);
                //                     bw.Write(" ");
                //                     bw.Write(objInfos[i].sumVideoStartTimeStamp);
                //                     bw.Write(" ");
                //                     bw.Write(objInfos[i].sumVideoEndTimeStamp);
                //                     bw.Write("\r\n");
                //                 }
                //                 bw.Close();
                //                 fs.Close();
            }
        }

        public override bool IsFetched()
        {
            return !String.IsNullOrEmpty(_compressedPath);
        }
    }
}
