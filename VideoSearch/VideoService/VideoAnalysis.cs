using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using System.Text;
using VideoSearch.Utils;

namespace VideoSearch.VideoService
{
    public class VideoAnalysis
    {

        #region QueryVideo & QueryVideoList

        public static QueryVideoResponse QueryVideo(String videoId)
        {
            QueryVideoResponse videoResponse = null;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/QueryVideo?VideoId=" + videoId;
            HttpWebRequest convertRequest = (HttpWebRequest)WebRequest.Create(strReq);

            if (convertRequest != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)convertRequest.GetResponse();

                    videoResponse = VideoResponsor.QueryVideoResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("QueryVideo fail...", exception);
                }
                finally
                {
                    convertRequest = null;
                }
            }
            return videoResponse;
        }

        public static List<VideoElement> QueryVideoList()
        {
            List<VideoElement> videoList = null;
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/QueryVideoList";
            HttpWebRequest convertRequest = (HttpWebRequest)WebRequest.Create(strReq);

            if (convertRequest != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)convertRequest.GetResponse();

                    videoList = VideoResponsor.QueryVideoListResponsor(response.GetResponseStream());

                    response.Close();
                 }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("QueryVideoList fail...", exception);
                }
                finally
                {
                    convertRequest = null;
                }
            }

            return videoList;
        }

        public static async Task<List<VideoElement>> QueryVideoListAsync()
        {
            List<VideoElement> videoList = null;
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/QueryVideoList";
            HttpWebRequest convertRequest = (HttpWebRequest)WebRequest.Create(strReq);

            if (convertRequest != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)await convertRequest.GetResponseAsync();

                    Stream ReceiveStream = response.GetResponseStream();
                    videoList = await VideoResponsor.QueryVideoListResponsorAsync(ReceiveStream);

                    response.Close();
                 }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("QueryVideoListAsync fail...", exception);
                }
                finally
                {
                    convertRequest = null;
                }
            }

            return videoList;
        }
        #endregion

        #region DeleteVideo
        public static bool DeleteVideo(String videoId)
        {
            bool ret = false;
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/DeleteVideo?VideoId=" + videoId;
            HttpWebRequest deleteRequest = (HttpWebRequest)WebRequest.Create(strReq);

            if (deleteRequest != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)deleteRequest.GetResponse();

                    response.Close();

                    ret = true;
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("DeleteVideo fail...", exception);
                }
                finally
                {
                    deleteRequest = null;
                }
            }

            return ret;
        }
        
        #endregion

        #region SubmitVideo & Transcode

        public static SubmitVideoResponse SubmitVideo(MovieItem item)
        {
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/SubmitVideo?OrgPath=file://" +
                               item.Parent.DisplayID + "/" +
                               item.ID + "/" +
                               item.ID;

            return SubmitVideoAndTransCode(strReq);
        }

        public static SubmitVideoResponse TranscodeVideo(MovieItem item)
        {
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/Transcode?VideoId=" +
                               item.VideoId;

            return SubmitVideoAndTransCode(strReq);
        }
        
        public static SubmitVideoResponse SubmitVideoAndTransCode(MovieItem item)
        {
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/SubmitVideo?OrgPath=file://" +
                               item.Parent.DisplayID + "/" +
                               item.ID + "/" +
                               item.ID + "&Transcode=1";

            return SubmitVideoAndTransCode(strReq);
        }

        public static SubmitVideoResponse SubmitVideoAndTransCode(String strReq)
        {
            HttpWebRequest submitRequest = (HttpWebRequest)WebRequest.Create(strReq);

            SubmitVideoResponse submitResponse = null;

            if (submitRequest != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)submitRequest.GetResponse();

                    submitResponse = VideoResponsor.SubmitVideoAndTransCodeResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("SubmitVideoAndTransCode fail...", exception);
                }
                finally
                {
                    submitRequest = null;
                }
            }

            return submitResponse;
        }

        #endregion

        #region QueryTask (QueryTask & QueryListDetail)
        public static QueryTaskResponse QueryTask(String taskId)
        {
            QueryTaskResponse taskResponse = null;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/QueryTask?TaskId=" + taskId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    taskResponse = VideoResponsor.QueryTaskResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("QueryTask fail...", exception);
                }
                finally
                {
                    request = null;
                }
            }
            return taskResponse;
        }

        public static QueryListDetailResponse QueryListDetail()
        {
            QueryListDetailResponse detailResponse = null;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/QueryVideoList";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    detailResponse = VideoResponsor.QueryListDetailResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("QueryListDetail fail...", exception);
                }
                finally
                {
                    request = null;
                }

            }
            return detailResponse;
        }
        #endregion

        #region DeleteTask
        public static bool DeleteTask(String taskId)
        {
            bool ret = false;
            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/DeleteTask?TaskId=" + taskId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    response.Close();

                    ret = true;
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("DeleteTask fail...", exception);
                }
                finally
                {
                    request = null;
                }
            }

            return ret;
        }

        #endregion

        #region Submit Task (Search & Summary & Compress)

        public static SubmitTaskResponse CreateSummaryTask(String videoId, String sensitivity, int nRegionType, Rect region)
        {
            return SubmitTask("1", videoId, sensitivity, nRegionType, region);
        }

        public static SubmitTaskResponse CreateCompressTask(String videoId, int thickness, String sensitivity, int nRegionType, Rect region)
        {
            return SubmitTask("2", videoId, sensitivity, nRegionType, region, thickness);
        }

        public static SubmitTaskResponse SubmitTask(String taskType, String videoId, String sensitivity, int nRegionType, Rect region, int thickness = -1)
        {
            int left, top, right, bottom;

            left = (int)region.Left;
            top = (int)region.Top;
            right = (int)region.Right;
            bottom = (int)region.Bottom;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/SubmitTask";

            String postData = "VideoId=" + videoId +
                                "&TaskType=" + taskType +
                                "&Sensitivity=" + sensitivity;
            if(nRegionType > 0)
                postData += (nRegionType == 1 ? "&RcgArea=" : "&UnrcgArea=") +
                                String.Format("{0},{1},{2},{3}", left, top, right, bottom);

            if(thickness >= 0)
                postData += String.Format("&Thickness={0}", thickness);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);
            request.Method = "POST";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteEncoded = encoding.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteEncoded.Length;

            Stream newStream = request.GetRequestStream();

            newStream.Write(byteEncoded, 0, byteEncoded.Length);
            newStream.Close();


            SubmitTaskResponse createOutlineResponse = null;

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    createOutlineResponse = VideoResponsor.CreateSummaryTaskResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("SubmitTask fail...", exception);
                }
                finally
                {
                    request = null;
                }
            }

            return createOutlineResponse;

        }
        #endregion

        #region Submit Task Result (GetTaskSnapshot & GetVideoSummary)

        public static TaskSnapShotResponse GetTaskSnapshot(String taskId)
        {
            TaskSnapShotResponse snapshotResponse = null;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/GetTaskSnapshot?TaskId=" + taskId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    snapshotResponse = VideoResponsor.GetTaskSnapshotResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("GetTaskSnapshot fail...", exception);
                }
                finally
                {
                    request = null;
                }
            }
            return snapshotResponse;
        }

        public static SummaryResponse GetVideoSummary(String taskId)
        {
            SummaryResponse summaryResponse = null;

            String strReq = "http://localhost:8080/VideoAnalysisService/VideoService/GetVideoSummary?TaskId=" + taskId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strReq);

            if (request != null)
            {
                try
                {
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                    summaryResponse = VideoResponsor.GetVideoSummaryResponsor(response.GetResponseStream());

                    response.Close();
                }
                catch (Exception exception)
                {
                    ExceptionUtils.ShowMessage("GetVideoSummary fail...", exception);
                }
                finally
                {
                    request = null;
                }
            }
            return summaryResponse;
        }

        #endregion

    }
}
