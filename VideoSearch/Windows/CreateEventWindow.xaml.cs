using System;
using System.Windows;
using System.Windows.Controls;
using VideoSearch.Model;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for CreateEventWindow.xaml
    /// </summary>
    public partial class CreateEventWindow : Window
    {
        public CreateEventWindow(EventItem item = null)
        {
            InitializeComponent();
            EventCalendar.Visibility = Visibility.Hidden;

            if(item == null)
            {
                _item = new EventItem();

                DateTime today = DateTime.Now;
                EventCalendar.SelectedDate = today;

                m_eventDate.Text = String.Format("{0}", today.ToString("yyyy-MM-dd"));
            }
            else
            {
                Title = "编辑案件信息";

                _item = new EventItem(item);

                EventID = item.ID;
                EventDisplayID = item.DisplayID;
                EventName = item.Name;
                EventDate = item.Date;
                EventRemark = item.Remark;

                if(item.Date.Length == 0)
                {
                    DateTime today = DateTime.Now;
                    EventCalendar.SelectedDate = today;

                    EventDate = String.Format("{0}", today.ToString("yyyy-MM-dd"));
                }
                else
                {
                    EventCalendar.SelectedDate = DateTime.Parse(item.Date);
                }
            }
        }

        #region Property
        private EventItem _item = null;
        public EventItem NewEvent
        {
            get
            {
                _item.ID = EventID;
                _item.DisplayID = EventDisplayID;
                _item.Name = EventName;
                _item.Date = EventDate;
                _item.Remark = EventRemark;

                return _item;
            }
        }

        public String EventID
        {
            get;
            set;
        }

        public String EventDisplayID
        {
            get { return m_eventID.Text; }
            set { m_eventID.Text = value; }
        }

        public String EventName
        {
            get { return m_eventName.Text; }
            set { m_eventName.Text = value; }
        }

        public String EventDate
        {
            get{ return m_eventDate.Text; }
            set { m_eventDate.Text = value; }
        }

        public String EventRemark
        {
            get { return m_eventRemark.Text; }
            set { m_eventRemark.Text = value; }
        }
        #endregion

        #region Handler
        private void btnCalendar_Click(object sender, RoutedEventArgs e)
        {
            if (EventCalendar.IsVisible)
                EventCalendar.Visibility = Visibility.Hidden;
            else
                EventCalendar.Visibility = Visibility.Visible;
        }

        private void EventCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            EventCalendar.Visibility = Visibility.Hidden;
            DateTime? date = EventCalendar.SelectedDate;

            if (date != null)
            {
                DateTime dt = (DateTime)date;

                //               DateTimeFormatInfo myDTFI = new CultureInfo("en-US", false).DateTimeFormat;
                m_eventDate.Text = String.Format("{0}", dt.ToString("yyyy-MM-dd"));
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (m_eventID.Text.Length == 0)
            {
                MessageBox.Show("Please fill the Event ID!");
                return;
            }

            if (m_eventName.Text.Length == 0)
            {
                MessageBox.Show("Please fill the Event Name!");
                return;
            }

            if (m_eventDate.Text.Length == 0)
            {
                MessageBox.Show("Please fill the Event Date!");
                return;
            }

            DialogResult = true;
            Close();
        }
        #endregion
    }
}
