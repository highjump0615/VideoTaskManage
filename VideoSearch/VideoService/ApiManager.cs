﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using VideoSearch.Utils;

namespace VideoSearch.VideoService
{
    public class ApiManager
    {
        public static string API_PATH = ConfigurationManager.AppSettings["webApiUrlBase"];

        private static ApiManager _Instance;
        public static ApiManager Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new ApiManager();

                return _Instance;
            }
        }

        #region Video Management
        /// <summary>
        /// Query video info
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public async Task<XElement> GetQueryVideo(int videoId)
        {
            return await sendToServiceByGet($"{API_PATH}/QueryVideo?VideoId={videoId}");
        }

        /// <summary>
        /// Submit video
        /// </summary>
        /// <returns></returns>
        public Task<XElement> SubmitVideo(String videoPath)
        {
            return sendToServiceByGet(API_PATH + "/SubmitVideo?OrgPath=" + videoPath + "&Transcode=0");
        }

        /// <summary>
        /// Delete video
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteVideo(int videoId)
        {
            var response = await sendToServiceByGet(API_PATH + "/DeleteVideo?VideoId=" + videoId);

            if (response != null)
                return true;

            return false;
        }
        #endregion

        #region Task Management
        /// <summary>
        /// Query task info
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<XElement> GetQueryTask(String taskId)
        {
            return await sendToServiceByGet(API_PATH + "/QueryTask?TaskId=" + taskId);
        }

        /// <summary>
        /// Create summary task
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="videoId"></param>
        /// <param name="sensitivity"></param>
        /// <param name="nRegionType"></param>
        /// <param name="region"></param>
        public async Task<XElement> CreateSummaryTask(int videoId, String sensitivity, int nRegionType, Rect region)
        {
            return await SubmitTask("1", videoId, sensitivity, nRegionType, region, -1);
        }

        /// <summary>
        /// Create compress task
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="videoId"></param>
        /// <param name="sensitivity"></param>
        /// <param name="nRegionType"></param>
        /// <param name="region"></param>
        /// <param name="thickness"></param>
        public async Task<XElement> CreateCompressTask(int videoId, int thickness, String sensitivity, int nRegionType, Rect region)
        {
            return await SubmitTask("2", videoId, sensitivity, nRegionType, region, thickness);
        }

        /// <summary>
        /// Create Search task
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="videoId"></param>
        /// <param name="sensitivity"></param>
        /// <param name="nRegionType"></param>
        /// <param name="region"></param>
        /// <param name="objType"></param>
        /// <param name="colors"></param>
        /// <param name="alarmInfo"></param>
        /// <param name="renxingPic"></param>
        /// <param name="renxingMaskPic"></param>
        /// <param name="renxingWaijieRect"></param>
        public async Task<XElement> CreateSearchTask(int videoId, String sensitivity, int nRegionType, Rect region, String objType, String colors, String alarmInfo, char[] renxingPic, char[] renxingMaskPic, Rect renxingWaijieRect)
        {
            return await SubmitTask("3", videoId, sensitivity, nRegionType, region, -1, objType, colors, alarmInfo, renxingPic, renxingMaskPic, renxingWaijieRect);
        }

        /// <summary>
        /// Submit task
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="videoId"></param>
        /// <param name="sensitivity"></param>
        /// <param name="nRegionType"></param>
        /// <param name="region"></param>
        /// <param name="thickness"></param>
        /// <param name="objType"></param>
        /// <param name="colors"></param>
        /// <param name="alarmInfo"></param>
        /// <param name="renxingPic"></param>
        /// <param name="renxingMaskPic"></param>
        /// <param name="renxingWaijieRect"></param>
        /// <returns></returns>
        public async Task<XElement> SubmitTask(String taskType, int videoId, String sensitivity, int nRegionType, Rect region, int thickness, String objType = "", String colors = "", String alarmInfo = "", char[] renxingPic = null, char[] renxingMaskPic = null, Rect renxingWaijieRect = new Rect())
        {
            int left, top, right, bottom;

            left = (int)region.Left;
            top = (int)region.Top;
            right = (int)region.Right;
            bottom = (int)region.Bottom;

            String postData = "VideoId=" + videoId +
                                "&TaskType=" + taskType +
                                "&Sensitivity=" + sensitivity;
            if (nRegionType > 0)
                postData += (nRegionType == 1 ? "&RcgArea=" : "&UnrcgArea=") +
                                String.Format("{0},{1},{2},{3}", left, top, right, bottom);

            if (thickness >= 0)
                postData += String.Format("&Thickness={0}", thickness);

            // for only SearchTask
            if(taskType == "3" && objType.Length > 0)
            {
                postData += String.Format("&ObjType={0}&Color={1}&AlarmLine={2}", objType, colors, alarmInfo);

                if(objType == "5" && 
                   renxingPic != null &&
                   renxingMaskPic != null &&
                   renxingWaijieRect.Width > 0 &&
                   renxingWaijieRect.Height > 0)
                {
                    left = (int)renxingWaijieRect.Left;
                    top = (int)renxingWaijieRect.Top;
                    right = (int)renxingWaijieRect.Right; 
                    bottom = (int)renxingWaijieRect.Bottom;

                    String strRenXingWaiJieRect = String.Format("{0},{1},{2},{3}", left, top, right, bottom);
                    String strRenxingPic = new string(renxingPic);
                    String strRenxingMaskPic = new string(renxingMaskPic);
                    postData += String.Format("&RenXingPic={0}&RenXingMaskPic={1}&RenXingWaiJieRect={2}", strRenxingPic, strRenxingMaskPic, strRenXingWaiJieRect);
                }
            }

            return await sendToServiceByPost(API_PATH + "/SubmitTask", postData);
        }

        /// <summary>
        /// Delete task
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<bool> DeleteTask(String taskId)
        {
            var response = await sendToServiceByGet(API_PATH + "/DeleteTask?TaskId=" + taskId);

            if (response != null)
                return true;

            return false;
        }

        /// <summary>
        /// Get task snapshot
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<XElement> GetTaskSnapshot(String taskId)
        {
            return await sendToServiceByGet(API_PATH + "/GetTaskSnapshot?TaskId=" + taskId);
        }

        /// <summary>
        /// Get video summary
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public async Task<XElement> GetVideoSummary(String taskId)
        {
            return await sendToServiceByGet(API_PATH + "/GetVideoSummary?TaskId=" + taskId);
        }
        #endregion

        #region Get & Post
        /// <summary>
        /// Send Http Get Request
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<XElement> sendToServiceByGet(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            try
            {
                var response = await request.GetResponseAsync();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    XDocument xmlDoc = new XDocument();
                    try
                    {
                        xmlDoc = XDocument.Parse(sr.ReadToEnd());
                        return xmlDoc.Root;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ExceptionUtils.ShowMessage("sendToServiceByGet::GetResponseStream fail...", e);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                ExceptionUtils.ShowMessage("sendToServiceByGet fail...", e);
            }

            return null;
        }

        /// <summary>
        /// Send Http Post Request
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        protected async Task<XElement> sendToServiceByPost(string url, string postData)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            UTF8Encoding encoding = new UTF8Encoding();
            byte[] byteEncoded = encoding.GetBytes(postData);

            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteEncoded.Length;

            Stream newStream = request.GetRequestStream();

            newStream.Write(byteEncoded, 0, byteEncoded.Length);
            newStream.Close();

            try
            {
                var response = await request.GetResponseAsync();
                using (var sr = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    XDocument xmlDoc = new XDocument();
                    try
                    {
                        xmlDoc = XDocument.Parse(sr.ReadToEnd());
                        return xmlDoc.Root;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ExceptionUtils.ShowMessage("sendToServiceByGet::GetResponseStream fail...", e);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
                ExceptionUtils.ShowMessage("sendToServiceByGet fail...", e);
            }

            return null;
        }
        #endregion
    }
}
