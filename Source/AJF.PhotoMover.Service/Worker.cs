using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using Serilog;

namespace AJF.PhotoMover.Service
{
    public class Worker
    {
        private const string ConfigFileName = "photomover.config";
        private BackgroundWorker _backgroundWorker;
        private List<IPhotoMoverConfig> _config;
        private AppSettings _appSettings;
        public bool WorkDone { get; set; }

        public void Start()
        {
            try
            {
                var pathList = ConfigurationManager.AppSettings["PathList"].Split(';');
                _config = new List<IPhotoMoverConfig>();

                foreach (var path in pathList)
                {
                    Log.Logger.Information("Watching: " + path);
                    var photoMoverConfig = new PhotoMoverConfig
                    {
                        Path = path
                    };
                    _config.Add(photoMoverConfig);
                }

                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += _backgroundWorker_DoWork;
                _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
                _backgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "During Start", new object[0]);
                throw;
            }
        }

        private void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            WorkDone = true;
        }

        private void _backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Log.Logger.Information("Doing work");

            try
            {
                DoWorkInternal(sender);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "During do work", new object[0]);
                throw;
            }
        }

        private void DoWorkInternal(object sender)
        {
            WorkDone = false;

            _appSettings = new AppSettings();

            while (true)
            {
                var backgroundWorker = sender as BackgroundWorker;
                if (backgroundWorker==null||backgroundWorker.CancellationPending)
                {
                    Log.Information("backgroundworker.CancellationPending: {@backgroundWorker}",backgroundWorker);
                    return;
                }

                Thread.Sleep(_appSettings.TickSleep);


                foreach (var c in _config)
                {
                    //Log.Logger.Information("Looking in " + c.Path);

                    SettingsWrapper settings;
                    try
                    {
                        var xmlSettings = new XmlDocument();
                        xmlSettings.Load(c.Path + ConfigFileName);
                        settings = new SettingsWrapper(xmlSettings);
                    }
                    catch (DirectoryNotFoundException)
                    {
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Information("Could not read settings from " + c.Path + " : " + ex.Message);
                        continue;
                    }

                    var pathToOldestFile = GetPathToOldestFile(settings, c);
                    if (pathToOldestFile == null)
                    {
                        Log.Logger.Debug("Found no file to move in " + c.Path);
                        continue;
                    }

                    var lastWriteTime = File.GetLastWriteTime(pathToOldestFile);
                    var inAgeHours = settings.MinAgeHours;
                    var threshold = DateTime.Now.AddHours(-inAgeHours);
                    if (lastWriteTime > threshold)
                    {
                        Log.Logger.Information("File found but not old enough: " + lastWriteTime);
                        continue;
                    }

                    var hasConnectionToNas = GetHasConnectionToNas(settings);
                    if (!hasConnectionToNas)
                    {
                        //Log.Logger.Information("File found that should be moved but no connection to destination: " +
                        //                       settings.Destination);
                        continue;
                    }

                    try
                    {
                        Log.Logger.Information("Moving file: " + pathToOldestFile);
                        MoveOldestToNas(pathToOldestFile, settings);
                        if(settings.RemoveEmptySourceDir)
                        {
                            var directoryName = Path.GetDirectoryName(pathToOldestFile);
                            if (!Directory.GetFiles(directoryName).Any())
                            {
                                Log.Information(directoryName+" is empty -- trying to delete.");
                                Directory.Delete(directoryName);
                                Log.Information(directoryName + " deleted");
                            }
                            else
                            {
                                Log.Debug(directoryName + " is NOT empty.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "Trying to move " + pathToOldestFile);
                    }
                }
            }
        }

        private void MoveOldestToNas(string pathToOldestFile, SettingsWrapper settings)
        {
            var lastWriteTime = File.GetLastWriteTime(pathToOldestFile);
            Log.Logger.Information("Oldest file : " + pathToOldestFile);

            var newPath = string.Format(settings.Destination,
                string.Format("{0:yyyy}", lastWriteTime),
                string.Format("{0:MM}", lastWriteTime),
                string.Format("{0:dd}", lastWriteTime)
                );

            var filename = Path.GetFileName(pathToOldestFile);

            var prefix = "";
            string destination;
            do
            {
                destination = newPath + prefix + filename;
                Log.Logger.Information(newPath);
                Log.Logger.Information(destination);
                prefix += "_";
            } while (File.Exists(destination));

            var destinationDir = Path.GetDirectoryName(destination);
            Directory.CreateDirectory(destinationDir);
            if(_appSettings.PerformMove)
            File.Move(pathToOldestFile, destination);
            else
            {
                Log.Logger.Information("Skipped moving; setting does not allow.");
            }
        }

        private string GetPathToOldestFile(SettingsWrapper settings, IPhotoMoverConfig photoMoverConfig)
        {
            var list = new List<string>();

            foreach (var searchPattern in settings.SearchPatterns)
            {
                var enumerateFiles1 = GetFilesRecursively(photoMoverConfig.Path, searchPattern);
                list.AddRange(enumerateFiles1);
            }

            var sortedList = new SortedList<DateTime, string>();

            foreach (var enumerateFile in list)
            {
                try
                {
                    var creationTime = File.GetLastWriteTime(enumerateFile);
                    sortedList.Add(creationTime, enumerateFile);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Trying to get last writetime on " + enumerateFile);
                }
            }

            var firstOrDefault = sortedList.FirstOrDefault();
            return firstOrDefault.Value;
        }

        private static IEnumerable<string> GetFilesRecursively(string path, string searchPattern)
        {
            var result = new List<string>();

            var files = Directory.EnumerateFiles(path, searchPattern);
            result.AddRange(files);

            var subDirs = Directory.EnumerateDirectories(path);
            foreach (var subDir in subDirs)
            {
                var filesRecursively = GetFilesRecursively(subDir, searchPattern);
                result.AddRange(filesRecursively);
            }

            return result;
        }

        private bool GetHasConnectionToNas(SettingsWrapper settings)
        {
            try
            {
                var path = settings.TestFile;
                using (var sw = File.CreateText(path))
                {
                }
                File.Delete(path);
            }
            catch (Exception)
            {
                Log.Logger.Warning("No connection to "+ settings.TestFile);
                return false;
            }
            return true;
        }

        public void Stop()
        {
            if (_backgroundWorker != null)
            {
                _backgroundWorker.CancelAsync();

                while (!WorkDone)
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}