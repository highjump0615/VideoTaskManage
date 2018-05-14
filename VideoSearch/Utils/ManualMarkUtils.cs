using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Xml;
using vlcPlayerLib;

namespace VideoSearch.Utils
{
    public class ManualMarkUtils
    {
        vlcPlayer _player = null;
        int _videoId = 0;
        String _path = null;
        int _lastIndex = 0;
        List<ManualMark> _markList = new List<ManualMark>();

        #region Constructor
        
        public ManualMarkUtils(vlcPlayer player, int videoId)
        {
            _player = player;
            _videoId = videoId;

            if (videoId > 0)
                _path = $"D:\\VideoInvestigationDataDB\\AnalysisFile\\VideoId_{_videoId}_Articlue";

            if (Directory.Exists(_path))
            {
                String[] files = Directory.GetFiles(_path, "*.xml");

                _lastIndex += files.Length;
            }
            else
            {
                Directory.CreateDirectory(_path);
            }
        }
        #endregion

        #region Public functions...

        public bool AddManualMark(Rectangle manualMarkRect, long frame)
        {
            ManualMark mark = new ManualMark();
            mark.timeStamp = frame / 40;
            mark.markRect = manualMarkRect;

            if (_markList.Count > 0 && mark.timeStamp <= _markList[_markList.Count - 1].timeStamp)
            {
                return false;
            }
            _markList.Add(mark);

            return true;
        }

        /// <summary>
        /// 保存标注
        /// </summary>
        public void SaveManualMark()
        {
            if (_player == null || _videoId == 0 || _markList.Count == 0)
                return;

            List<ManualMark> newList = new List<ManualMark>();
            if (_markList.Count == 1)
            {
                newList.Add(_markList[0]);
            }
            else
            {
                for (int i = 0; i < _markList.Count - 1; i++)
                {
                    if (_markList[i].timeStamp < _markList[i + 1].timeStamp)
                    {
                        newList.AddRange(GetAutoMark(_markList[i], _markList[i + 1]));
                    }
                }
            }

            _lastIndex++;

            List<List<ManualMark>> manualMarksList = new List<List<ManualMark>>();
            manualMarksList.Add(newList);
            List<Mark> markList = ConvertMark(manualMarksList);
            _player.AddManualMarks(_lastIndex, newList);
            //_player.DelManualMarks(_lastIndex);

            String path = String.Format("{0}\\{1}.xml", _path, _lastIndex);
            WriteMarkToXML(markList, path);

            _markList.Clear();
        }

        public void ResetManualMark()
        {
            _markList.Clear();
        }
        #endregion

        #region Internal functions...

        /// <summary>
        /// 已知起始点坐标，求连线上与起点距离disToStartPoint的点坐标
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <param name="disToStartPoint"></param>
        /// <returns></returns>
        private Point GetMiddlePoint(Point startPoint, Point endPoint, double disToStartPoint)
        {
            Point p = new Point();
            double disBetStartEnd = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
            p.X = startPoint.X + (int)(disToStartPoint * (endPoint.X - startPoint.X) / disBetStartEnd);
            p.Y = startPoint.Y + (int)(disToStartPoint * (endPoint.Y - startPoint.Y) / disBetStartEnd);
            return p;
        }

        /// <summary>
        /// 获取Rectangle结构的中心点位置
        /// </summary>
        /// <param name="startManualMark"></param>
        /// <param name="endManualMark"></param>
        /// <returns></returns>
        private Point GetCenterPointFromRectangle(Rectangle rect)
        {
            Point p = new Point();
            p.X = rect.X + rect.Width / 2;
            p.Y = rect.Y + rect.Height / 2;
            return p;
        }

        /// <summary>
        /// 根据中心点、长宽生成Rectangle
        /// </summary>
        /// <param name="startManualMark"></param>
        /// <param name="endManualMark"></param>
        /// <returns></returns>
        private Rectangle GetRectangleFromCenterPoint(Point p, int width, int height)
        {
            Rectangle rect = new Rectangle();
            rect.X = p.X - width / 2;
            rect.Y = p.Y - height / 2;
            rect.Width = width;
            rect.Height = height;
            return rect;
        }

        /// <summary>
        /// 根据两个人工标注，生成这两个标注之间的标注列表
        /// </summary>
        /// <param name="startManualMark"></param>
        /// <param name="endManualMark"></param>
        /// <returns></returns>
        private List<ManualMark> GetAutoMark(ManualMark startManualMark, ManualMark endManualMark)
        {
            List<ManualMark> list = new List<ManualMark>();
            if (startManualMark.timeStamp >= endManualMark.timeStamp)
                return list;

            Point startPoint = GetCenterPointFromRectangle(startManualMark.markRect);
            Point endPoint = GetCenterPointFromRectangle(endManualMark.markRect);

            long frameCount = endManualMark.timeStamp - startManualMark.timeStamp;

            double disBetStartEnd = Math.Sqrt(Math.Pow(endPoint.X - startPoint.X, 2) + Math.Pow(endPoint.Y - startPoint.Y, 2));
            double a = disBetStartEnd * 2 / frameCount / frameCount;

            //判断标注的物标运动方向
            bool vertical = true;
            if (Math.Abs(endPoint.Y - startPoint.Y) < 50)
            {
                vertical = false;
            }

            //list.Add(startManualMark);
            for (int t = 1; t < frameCount; t++)
            {
                //                 if (t % 4 != 0)
                //                 {
                //                     continue;
                //                 }
                ManualMark manualMark = new ManualMark();
                manualMark.timeStamp = startManualMark.timeStamp + t;

                Point newPoint = new Point();
                if (vertical)
                {
                    newPoint = GetMiddlePoint(startPoint, endPoint, a * t * t / 2);
                }
                else
                {
                    newPoint = GetMiddlePoint(startPoint, endPoint, disBetStartEnd / frameCount * t);
                }

                int width = (int)(startManualMark.markRect.Width + (endManualMark.markRect.Width - startManualMark.markRect.Width) * 1.0f / frameCount * t);
                int height = (int)(startManualMark.markRect.Height + (endManualMark.markRect.Height - startManualMark.markRect.Height) * 1.0f / frameCount * t);
                manualMark.markRect = GetRectangleFromCenterPoint(newPoint, width, height);
                list.Add(manualMark);
            }
            //list.Add(endManualMark);
            return list;
        }

        /// <summary>
        /// 将 List<Mark>标注列表数据写入到指定的XML中
        /// </summary>
        /// <param name="marks"></param>
        /// <param name="xmlFullPath"></param>
        private void WriteMarkToXML(List<Mark> marks, string xmlFullPath)
        {
            XmlTextWriter xmlWriter;
            xmlWriter = new XmlTextWriter(xmlFullPath, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.WriteStartElement("标注集合");

            foreach (Mark mark in marks)
            {
                xmlWriter.WriteStartElement("标注");
                xmlWriter.WriteAttributeString("时间戳", (mark.timeStamp * 40).ToString());

                foreach (ManualMark manualMark in mark.marks)
                {
                    xmlWriter.WriteStartElement("物标");
                    xmlWriter.WriteAttributeString("左上角横坐标", manualMark.markRect.Left.ToString());
                    xmlWriter.WriteAttributeString("左上角纵坐标", manualMark.markRect.Top.ToString());
                    xmlWriter.WriteAttributeString("右下角横坐标", manualMark.markRect.Right.ToString());
                    xmlWriter.WriteAttributeString("右下角纵坐标", manualMark.markRect.Bottom.ToString());
                    xmlWriter.WriteAttributeString("颜色", manualMark.color.ToString());
                    xmlWriter.WriteAttributeString("时标", (mark.timeStamp * 40).ToString());
                    xmlWriter.WriteAttributeString("原始开始时间", "0");
                    xmlWriter.WriteAttributeString("原始结束时间", "0");
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();

            }
            xmlWriter.WriteEndElement();
            xmlWriter.Close();
        }

        /// <summary>
        /// 将多个人工标注列表转换成一个可以转换成XML形式的标注列表
        /// </summary>
        /// <param name="m"></param>
        /// <param name="marks"></param>
        /// <returns></returns>
        private List<Mark> ConvertMark(List<List<ManualMark>> manualMarksList)
        {
            List<Mark> marks = new List<Mark>();

            if (manualMarksList.Count <= 0)
            {
                return marks;
            }

            //获取List<ManualMark>中最大的和最小的timeStamp
            long minTimeStamp = manualMarksList[0][0].timeStamp;
            long maxTimeStamp = manualMarksList[0][manualMarksList[0].Count - 1].timeStamp;

            for (int i = 1; i < manualMarksList.Count; i++)
            {
                if (manualMarksList[i][0].timeStamp < minTimeStamp)
                {
                    minTimeStamp = manualMarksList[i][0].timeStamp;
                }
                if (manualMarksList[i][manualMarksList[i].Count - 1].timeStamp > maxTimeStamp)
                {
                    maxTimeStamp = manualMarksList[i][manualMarksList[i].Count - 1].timeStamp;
                }
            }

            for (long i = 0; i <= maxTimeStamp; i++)
            {
                Mark mark = new Mark();
                mark.timeStamp = i;
                marks.Add(mark);
            }

            for (int i = 0; i < manualMarksList.Count; i++)
            {
                for (int j = 0; j < manualMarksList[i].Count; j++)
                {
                    marks[(int)(manualMarksList[i][j].timeStamp)].marks.Add(manualMarksList[i][j]);
                }
            }

            return marks;
        }

        #endregion
    }
}
