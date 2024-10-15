﻿using CarinaStudio.Windows.Input;
using System;
using System.Threading;
using System.Windows.Input;

namespace CarinaStudio
{
    /// <summary>
    /// Predefined <see cref="ICommand"/>s to using functions provided by <see cref="Platform"/>.
    /// </summary>
    public static class PlatformCommands
    {
        // Command to open link.
        class OpenLinkCommandImpl : ICommand
        {
            /// <inheritdoc/>
            public bool CanExecute(object? parameter) =>
                Platform.IsOpeningLinkSupported && (parameter is Uri || parameter is string);

            /// <inheritdoc/>
            public void Execute(object? parameter)
            {
                if (parameter is Uri uri)
                    Platform.OpenLink(uri);
                else if (parameter is string uriString)
                    Platform.OpenLink(uriString);
            }

#pragma warning disable CS0067
            /// <inheritdoc/>
            public event EventHandler? CanExecuteChanged;
#pragma warning restore CS0067
        }
        
        
        // Fields.
        static ICommand? openFileManagerCommand;
        static ICommand? openLinkCommand;
        static readonly Lock syncLock = new();


        /// <summary>
        /// Command to use <see cref="Platform.OpenFileManager(String)"/>.
        /// </summary>
        /// <remarks>The type of parameter is <see cref="string"/>.</remarks>
        public static ICommand OpenFileManagerCommand
        {
            get
            {
                if (openFileManagerCommand is not null)
                    return openFileManagerCommand;
                lock (syncLock)
                    openFileManagerCommand ??= new Command<string>(Platform.OpenFileManager, new FixedObservableValue<bool>(Platform.IsOpeningFileManagerSupported));
                return openFileManagerCommand;
            }
        }
        
        
        /// <summary>
        /// Command to use <see cref="Platform.OpenLink(String)"/> or <see cref="Platform.OpenLink(Uri)"/>.
        /// </summary>
        /// <remarks>The type of parameter is <see cref="string"/> or <see cref="Uri"/>.</remarks>
        public static ICommand OpenLinkCommand
        {
            get
            {
                if (openLinkCommand is not null)
                    return openLinkCommand;
                lock (syncLock)
                    openLinkCommand ??= new OpenLinkCommandImpl();
                return openLinkCommand;
            }
        }
    }
}