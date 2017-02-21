using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;
using AJF.PhotoMover.Selector.Annotations;

namespace AJF.PhotoMover.Selector
{
    public class Vm : INotifyPropertyChanged
    {
        private int _index;
        private readonly string path = @"D:\Pixum\Pixum2015\_bruges\_bruges\";

        public BitmapImage Source
        {
            get
            {
                var first = CurrentFullPath;
                if (first == null)
                {
                    return null;
                }

                var src = new BitmapImage();
                src.BeginInit();
                src.UriSource = new Uri(first, UriKind.Relative);
                src.CacheOption = BitmapCacheOption.OnLoad;
                src.EndInit();


                return src;
            }
        }

        public string CurrentFullPath
        {
            get
            {
                var enumerateFiles = Directory.EnumerateFiles(path, "*.jpg").OrderBy(x => x);
                var first = enumerateFiles.Skip(Index).FirstOrDefault();
                return first;
            }
        }

        public int Index
        {
            get { return _index; }
            set
            {
                if (value < 0)
                    return;
                _index = value;
                OnPropertyChanged(nameof(Source));
                OnPropertyChanged(nameof(CurrentFullPath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void MoveToKeep()
        {
            try
            {
                var currentFullPath = CurrentFullPath;
                var fileName = Path.GetFileName(currentFullPath);
                var newPath = path + @"\_bruges\";
                var newFullPath = newPath + fileName;
                Directory.CreateDirectory(newPath);

                File.Move(currentFullPath, newFullPath);
            }
            catch (Exception)
            {
            }
            Index = 0;
        }

        public void MoveToDiscard()
        {
            try
            {
                var currentFullPath = CurrentFullPath;
                var fileName = Path.GetFileName(currentFullPath);
                var newPath = path + @"\_bruges_ikke\";
                Directory.CreateDirectory(newPath);
                var newFullPath = newPath + fileName;

                File.Move(currentFullPath, newFullPath);
            }
            catch (Exception)
            {
            }
            Index = 0;
        }

        public void Reset()
        {
            Index = 0;
        }

        public void UndoOneEach()
        {
            try
            {
                var notUsedPath = path + @"\_bruges_ikke\";
                var usedPath = path + @"\_bruges\";

                var lastNotUsed = Directory.EnumerateFiles(notUsedPath).LastOrDefault();
                var lastUsed = Directory.EnumerateFiles(usedPath).LastOrDefault();

                MoveFileOneUp(lastNotUsed);
                MoveFileOneUp(lastUsed);
            }
            catch (Exception)
            {
                Index = 0;
            }
        }

        private void MoveFileOneUp(string filePath)
        {
            if (filePath == null)
                return;

            var fileName = Path.GetFileName(filePath);

            var newPath = path + @"\" + fileName;

            File.Move(filePath, newPath);
        }
    }
}