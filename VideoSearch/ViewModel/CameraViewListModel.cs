﻿using System;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class CameraViewListModel : ListViewModel
    {
        public CameraViewListModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
        }

        public override DataItemBase EmptyItem()
        {
            CameraItem item = new CameraItem();
            item.Visibility = Visibility.Hidden;

            return item;
        }
    }
}