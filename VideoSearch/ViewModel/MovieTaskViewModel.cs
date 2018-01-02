using System;
using VideoSearch.Model;
using VideoSearch.VideoService;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel
{
    public class MovieTaskViewModel : HostViewModel
    {
        public MovieTaskViewModel(DataItemBase owner) : base(owner)
        {
            Contents = new MovieTaskViewMainModel(Owner, this);

        }

        #region utility function

        protected void updateList()
        {
            if (Contents == null ||
                Contents.GetType() == typeof(MovieTaskViewListModel))
            {
                Contents = new MovieTaskViewMainModel(Owner, this);
            }
        }

        public void MovieSearch()
        {
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId == null || movieItem.VideoId.Length == 0)
                return;

            MovieSearchWindow searchDlg = new MovieSearchWindow(movieItem);

            Nullable<bool> result = searchDlg.ShowDialog();
            if (result == true)
            {
            }
        }

        public void MovieOutline()
        {
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId == null || movieItem.VideoId.Length == 0)
                return;

            MovieSummaryWindow outlineDlg = new MovieSummaryWindow(movieItem);

            Nullable<bool> result = outlineDlg.ShowDialog();
            if (result == true)
            {
                SubmitTaskResponse response = VideoAnalysis.CreateSummaryTask(movieItem.VideoId, outlineDlg.Sensitivity, outlineDlg.RegionType, outlineDlg.Region);

                if(response.IsOk)
                {
                    MovieTaskSummaryItem item = new MovieTaskSummaryItem(response.TaskId, outlineDlg.TaskName, MovieTaskType.OutlineTask, Owner);
                    Owner.AddItem(item);
                }
            }
        }

        public void MovieCompress()
        {
            updateList();

            if (Owner == null || Owner.GetType() != typeof(MovieItem))
                return;

            MovieItem movieItem = (MovieItem)Owner;

            if (movieItem.VideoId == null || movieItem.VideoId.Length == 0)
                return;

            MovieCompressWindow compressDlg = new MovieCompressWindow(movieItem);

            Nullable<bool> result = compressDlg.ShowDialog();
            if (result == true)
            {
                SubmitTaskResponse response = VideoAnalysis.CreateCompressTask(movieItem.VideoId, compressDlg.Thickness, compressDlg.Sensitivity, compressDlg.RegionType, compressDlg.Region);

                if (response.IsOk)
                {
                    MovieTaskCompressItem item = new MovieTaskCompressItem(response.TaskId, compressDlg.TaskName, MovieTaskType.CompressTask, Owner);
                    Owner.AddItem(item);
                }
            }
        }

        public void ShowMovieChargeList()
        {
            updateList();

            Contents = new MovieTaskViewListModel(Owner);
        }

        public void MovieFindAndPlay()
        {

        }

        #endregion
    }
}
