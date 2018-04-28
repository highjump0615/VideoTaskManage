﻿using System;
using System.Threading.Tasks;
using System.Xml.Linq;
using VideoSearch.Model;
using VideoSearch.Utils;
using VideoSearch.VideoService;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel
{
    public class MovieTaskViewModel : HostViewModel
    {
        public MovieTaskViewModel(DataItemBase owner) : base(owner)
        {
            Contents = new MovieTaskViewMainModel(Owner);
        }

        #region utility function

        protected void updateList()
        {
            if (Contents == null ||
                Contents.GetType() == typeof(MovieTaskViewMainModel))
            {
                Contents = new MovieTaskViewMainModel(Owner);
            }
        }

        /// <summary>
        /// 提交视频搜索
        /// </summary>
        /// <returns></returns>
        public async Task MovieSearch()
        {
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId <= 0)
                return;

            MovieSearchWindow searchDlg = new MovieSearchWindow(movieItem);

            Nullable<bool> result = searchDlg.ShowDialog();
            if (result == true)
            {
                Globals.Instance.ShowWaitCursor(true);

                var response = await ApiManager.Instance.CreateSearchTask(
                    movieItem.VideoId, 
                    searchDlg.Sensitivity, 
                    searchDlg.RegionType, 
                    searchDlg.Region, 
                    searchDlg.ObjectType, 
                    searchDlg.Colors, 
                    searchDlg.AlarmInfo,
                    searchDlg.RenXingPic, 
                    searchDlg.RenXingMaskPic,
                    searchDlg.RenXingWaiJieRect);

                if (response != null && StringUtils.String2Int(response.Element("State").Value) == 0)
                {
                    MovieTaskSearchItem item = new MovieTaskSearchItem(Owner, response.Element("TaskId").Value, searchDlg.TaskName, MovieTaskType.SearchTask);
                    Owner.AddItem(item);
                }

                Globals.Instance.ShowWaitCursor(false);

                // 跳转到任务列表
                ShowMovieChargeList();
            }
        }

        /// <summary>
        /// 提交视频摘要
        /// </summary>
        /// <returns></returns>
        public async Task MovieOutline()
        {           
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId <= 0)
                return;

            MovieSummaryWindow outlineDlg = new MovieSummaryWindow(movieItem);

            Nullable<bool> result = outlineDlg.ShowDialog();
            if (result == true)
            {
                Globals.Instance.ShowWaitCursor(true);

                var response = await ApiManager.Instance.CreateSummaryTask(
                    movieItem.VideoId, 
                    outlineDlg.Sensitivity, 
                    outlineDlg.RegionType, 
                    outlineDlg.Region);

                if(response != null && StringUtils.String2Int(response.Element("State").Value) == 0)
                {
                    MovieTaskSummaryItem item = new MovieTaskSummaryItem(Owner, response.Element("TaskId").Value, outlineDlg.TaskName, MovieTaskType.OutlineTask);
                    Owner.AddItem(item);
                }

                Globals.Instance.ShowWaitCursor(false);

                // 跳转到任务列表
                ShowMovieChargeList();
            }
        }

        /// <summary>
        /// 提交视频浓缩
        /// </summary>
        public async Task MovieCompress()
        {
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId <= 0)
                return;

            MovieCompressWindow compressDlg = new MovieCompressWindow(movieItem);

            Nullable<bool> result = compressDlg.ShowDialog();
            if (result == true)
            {
                var response = await ApiManager.Instance.CreateCompressTask(
                    movieItem.VideoId, 
                    compressDlg.Thickness, 
                    compressDlg.Sensitivity, 
                    compressDlg.RegionType, 
                    compressDlg.Region);

                if (response != null && StringUtils.String2Int(response.Element("State").Value) == 0)
                {
                    MovieTaskCompressItem item = new MovieTaskCompressItem(Owner, response.Element("TaskId").Value, compressDlg.TaskName, MovieTaskType.CompressTask);
                    Owner.AddItem(item);
                }
            }
        }

        /// <summary>
        /// 打开视频任务列表页面
        /// </summary>
        public void ShowMovieChargeList()
        {
            Contents = new MovieTaskViewListModel(Owner);
        }

        public void MovieFindAndPlay()
        {
            Contents = new MovieTaskViewMainModel(Owner, this);
        }

        #endregion
    }
}
