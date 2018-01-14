using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VideoSearch.VideoService
{
    #region Enum (ConvertStatus & MovieTaskState)
    public enum ConvertStatus
    {
        UnInited = -4,
        ImportReady = -3,
        Importing = -2,
        ImportingPaused = -1,
        Imported = 0,
        ConvertReady = Imported,
        Converting = 1,
        ConvertedOk = 2,
        ConvertedFail = 3,
        WaitInTaskQueue = 4,
        CancelWaitTask = 5,
        PlayReady = ConvertedOk
    }

    public enum MovieTaskState
    {
        UnInited = -1,
        CreateReady = 0,
        Creating = 1,
        Created = 2,
        CreateFail = 3,
        Merged = 4,
        WaitingDelete = 5,
        ErrorOccur = 6
    }
    #endregion    
}
