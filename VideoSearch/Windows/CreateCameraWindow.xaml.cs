using System;
using System.Collections.ObjectModel;
using System.Windows;
using VideoSearch.Database;
using VideoSearch.Model;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for CreateCameraWindow.xaml
    /// </summary>
    public partial class CreateCameraWindow : Window
    {
        ObservableCollection<String> EventIDs = null;

        public CreateCameraWindow(CameraItem item = null)
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;
            EventIDs = DBManager.EventIDList();
            cboEvent.ItemsSource = DBManager.EventDisplayIDList();

            if (item == null)
            {
                _item = new CameraItem();
            }
            else
            {
                Title = "编辑摄像头";

                _item = new CameraItem(item);
                cboEvent.IsEnabled = item.Children.Count == 0;
                CameraEventPos = item.EventPos;
                CameraID = item.ID;
                CameraDisplayID = item.DisplayID;
                CameraName = item.Name;
                CameraAddress = item.Address;
                CameraLongitude = item.Longitude;
                CameraLatitude = item.Latitude;
                CameraType = item.Type;
                CameraSource = item.Source;
                CameraPortCount = item.PortCount;
            }
        }

        #region Property
        private CameraItem _item = null;
        public CameraItem NewCamera
        {
            get
            {
                _item.EventPos = CameraEventPos;
                _item.ID = CameraID;
                _item.DisplayID = CameraDisplayID;
                _item.Name = CameraName;
                _item.Address = CameraAddress;
                _item.Longitude = CameraLongitude;
                _item.Latitude = CameraLatitude;
                _item.Type = CameraType;
                _item.Source = CameraSource;
                _item.PortCount = CameraPortCount;

                return _item;
            }
        }

        public String CameraEventPos
        {
            get
            {
                int index = cboEvent.SelectedIndex;
                if (index < 0 || index >= EventIDs.Count)
                    return null;

                return EventIDs[index];
            }
            set
            {
                int index = EventIDs.IndexOf(value);

                cboEvent.SelectedIndex = index;
            }
        }

        public String CameraID
        {
            get;
            set;
        }

        public String CameraDisplayID
        {
            get { return ID.Text; }
            set { ID.Text = value; }
        }

        public String CameraName
        {
            get { return Name.Text; }
            set { Name.Text = value; }
        }

        public String CameraAddress
        {
            get { return Address.Text; }
            set { Address.Text = value; }
        }

        public String CameraLongitude
        {
            get { return Longitude.Text; }
            set { Longitude.Text = value; }
        }

        public String CameraLatitude
        {
            get { return Latitude.Text; }
            set { Latitude.Text = value; }
        }

        public String CameraType
        {
            get { return cboType.Text; }
            set { cboType.Text = value; }
        }

        public String CameraSource
        {
            get { return cboSource.Text; }
            set { cboSource.Text = value; }
        }

        public String CameraPortCount
        {
            get { return PortCount.Text; }
            set { PortCount.Text = value; }
        }
        #endregion

        #region Handler
        private void OnSelectCoordinateFromMap(object sender, RoutedEventArgs e)
        {

        }

        private void OnCreateOrUpdateClick(object sender, RoutedEventArgs e)
        {
            if (cboEvent.Text.Length == 0)
            {
                MessageBox.Show("Please select the Event!");
                return;
            }

            if (ID.Text.Length == 0)
            {
                MessageBox.Show("Please fill the Camera ID!");
                return;
            }

            if (Name.Text.Length == 0)
            {
                MessageBox.Show("Please fill the Camera Name!");
                return;
            }

            DialogResult = true;
            Close();
        }
        #endregion

    }
}
