using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace VideoSearch.Model
{
    public class TaskSnapshot : INotifyPropertyChanged
    {
        public TaskSnapshot(int itemSizeIndex = 0)
        {
            _itemSizeCollection = new List<System.Windows.Size>();
            _itemSizeCollection.Add(new System.Windows.Size(217, 257));
            _itemSizeCollection.Add(new System.Windows.Size(270, 320));
            _itemSizeCollection.Add(new System.Windows.Size(350, 415));

            Margin = new Thickness(0);
            BlackBGVisible = Visibility.Visible;
            BorderThickness = 0;
            ViewPort = new Rect(0, 0, 1, 1);

            ItemSizeIndex = itemSizeIndex;
        }

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(String propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        #endregion

        #region Property

        private List<System.Windows.Size> _itemSizeCollection = null;

        private int _itemSizeIndex = -1;
        public int ItemSizeIndex
        {
            get { return _itemSizeIndex; }
            set
            {
                if(_itemSizeIndex != value)
                {
                    _itemSizeIndex = value;

                    System.Windows.Size size = _itemSizeCollection[_itemSizeIndex];
                    Width = size.Width;
                    Height = size.Height;

                    OnPropertyChanged("Height");
                    OnPropertyChanged("Width");
                }
            }
        }
        public String PicTitle
        {
            get;
            set;
        }
        public String PicPath
        {
            get;
            set;
        }

        public Visibility BlackBGVisible
        {
            get;
            set;
        }

        public int BorderThickness
        {
            get;
            set;
        }

        public SolidBrush BorderBrush
        {
            get;
            set;
        }

        private Rect _viewPort = new Rect(0, 0, 1, 1);
        public Rect ViewPort
        {
            get;
            set;
        }

        private int _displayType = -1;
        public int DisplayType
        {
            set
            {
                if (_displayType != value)
                {
                    _displayType = value;

                    if (_displayType == 0)
                    {
                        Margin = _normalMargin;
                        BlackBGVisible = Visibility.Visible;
                        BorderThickness = 1;
                        BorderBrush = new SolidBrush(Color.White);
                        ViewPort = new Rect(0, 0, 1, 1);
                    }
                    else if (_displayType == 1)
                    {
                        Margin = _clipMargin;
                        BlackBGVisible = Visibility.Hidden;
                        BorderThickness = 1;
                        BorderBrush = new SolidBrush(Color.DarkGray);
                        ViewPort = _viewPort;
                    }

                    OnPropertyChanged("BlackBGVisible");
                    OnPropertyChanged("BorderThickness");
                    OnPropertyChanged("BorderBrush");
                    OnPropertyChanged("Margin");
                    OnPropertyChanged("ViewPort");
                }
            }
        }

        public int StartFrame
        {
            get;
            set;
        }

        public int EndFrame
        {
            get;
            set;
        }

        public int FrameCount
        {
            get;
            set;
        }

        public int ObjType
        {
            get;
            set;
        }

        private Thickness _normalMargin = new Thickness(0);
        private Thickness _clipMargin = new Thickness(0);

        private Thickness _margin = new Thickness(0);
        public Thickness Margin
        {
            get { return _margin; }

            set
            {
                _margin = value;
            }
        }

        private Rect _objPath = new Rect();

        public Rect ObjPath
        {
            set
            {
                if (_objPath != value)
                {
                    _objPath = value;

                    BitmapImage thumbnail = new BitmapImage(new Uri(PicPath, UriKind.RelativeOrAbsolute));

                    if (thumbnail != null)
                    {
                        Rect rtContainer = new Rect(0, 0, 217, 180);
                        System.Windows.Size imgSize = new System.Windows.Size(thumbnail.Width, thumbnail.Height);

                        Rect bounds = calculateBounds(rtContainer, imgSize);

                        _normalMargin = new Thickness(bounds.Left, bounds.Top,
                            rtContainer.Right - bounds.Right, rtContainer.Bottom - bounds.Bottom);

                        // calc for clip
                        _viewPort = new Rect(_objPath.Left / imgSize.Width,
                            _objPath.Top / imgSize.Height,
                            _objPath.Right / imgSize.Width,
                            _objPath.Bottom / imgSize.Height);

                        imgSize = new System.Windows.Size(_objPath.Width, _objPath.Height);

                        bounds = calculateBounds(Rect.Inflate(rtContainer, -8, -8), imgSize);

                        _clipMargin = new Thickness(bounds.Left,
                            bounds.Top,
                            rtContainer.Right - bounds.Right,
                            rtContainer.Bottom - bounds.Bottom);

                    }
                }
            }
        }

        public double Width
        {
            get;
            set;
        }

        public double Height
        {
            get;
            set;
        }
        #endregion

        #region Utility
        protected Rect calculateBounds(Rect rt, System.Windows.Size imgSize)
        {
            Rect bounds = rt;

            double r1 = rt.Width / rt.Height;
            double r2 = imgSize.Width / imgSize.Height;
            double iw, ih;

            if (r1 > r2)
            {
                ih = rt.Height;
                iw = ih * r2;
            }
            else
            {
                iw = rt.Width;
                ih = iw / r2;
            }

            bounds = new Rect(bounds.Left + (bounds.Width - iw) / 2,
                              bounds.Top + (bounds.Height - ih) / 2,
                                iw, ih);

            return bounds;
        }
        #endregion
    };
}
