using Microsoft.Win32;
using System;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;
using System.Collections.Generic;
using System.Windows;

namespace VideoSearch.ViewModel
{
    public class MovieViewModel : HostViewModel
    {
        public MovieViewModel(DataItemBase owner)
            : base(owner)
        {
            Contents = new MovieViewListModel(owner, this);
        }

        #region utility function
        protected void updateList()
        {
            if (Contents == null || 
                Contents.GetType() == typeof(MovieViewPlayModel) ||
                Contents.GetType() == typeof(MovieViewAnalysisModel))
            {
                Contents = new MovieViewListModel(Owner);
            }
        }

        public void ImportMovie()
        {
            if (Owner == null)
                return;

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.FileName = "Export";
            dlg.DefaultExt = ".avi";
            dlg.Filter = "AVI files (*.avi)|*.AVI|DAV files (*.dav)|*.DAV|MKV files (*.mkv)|*.MKV|MOV files (*.mov)|*.MOV|MP4 files (*.mp4)|*.MP4|MPEG files (*.mpg)|*.MPG|MPEG TS files (*.ts)|*.TS|WMP files (*.wmv)|*.WMV|All Video Files(*.avi;*.dav;*.mkv;*.mov;*.mp4;*.mpg;*.ts;*.wmv)|*.AVI;*.DAV;*.MKV;*.MOV;*.MP4;*.MPG;*.TS;*.WMV";
            dlg.FilterIndex = 9;
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                updateList();

                String[] filenames = dlg.FileNames;
                
                foreach (String filePath in filenames)
                {
                    Owner.AddItem(new MovieItem(filePath, Owner));
                }
            }
        }

        public void DeleteSelectedMovies()
        {
            if (Owner == null)
                return;

            if (Owner == null || !Owner.HasCheckedItem)
                return;

            ConfirmDeleteWindow deleteDlg = new ConfirmDeleteWindow();

            Nullable<bool> result = deleteDlg.ShowDialog();

            if (result == true)
            {
                updateList();
                Owner.DeleteSelectedItem();
            }
        }

        public void PlaySelectedMovies()
        {
            List<String> movieList = new List<String>();
            List<String> nameList = new List<String>();

            foreach (MovieItem item in Owner.Children)
            {
                if (item.IsMultiPlayable)
                {
                    movieList.Add(item.PlayPath);
                    nameList.Add(item.Name);
                }
            }

            if(movieList.Count == 0)
            {
                MessageBox.Show("Please check the movie now, you want to play!");
                return;
            }

            Contents = new MovieViewPlayModel(nameList, movieList);
        }

        public void ShowMovieList()
        {
            Contents = new MovieViewListModel(Owner, this);
        }

        public void ShowMovieAnalysis()
        {
            Contents = new MovieViewAnalysisModel();
        }
        #endregion
    }
}
