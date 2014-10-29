using System;

namespace BackupSharp
{
    public class BackupFolderEventArgs : EventArgs
    {
        public BackupFolderEventArgs (string forderFullName)
        {
            FolderFullName = forderFullName;
        }

        public string FolderFullName { get; private set; }
    }
}

