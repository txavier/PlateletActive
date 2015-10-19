﻿using PlateletActive.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlateletActive.Infrastructure.Setters
{
    public class LogFileMover : ILogFileMover
    {
        public LogFileMover()
        {

        }

        public void MoveFiles(IEnumerable<string> namesOfFilesToMove, string outPath)
        {
            foreach(var oldFilePath in namesOfFilesToMove)
            {
                var fileName = oldFilePath.Split("\\".ToCharArray()).LastOrDefault();

                var newFilePath = outPath + "\\" + fileName;

                if (System.IO.File.Exists(oldFilePath) && !System.IO.File.Exists(newFilePath))
                {
                    System.IO.File.Move(oldFilePath, newFilePath);
                }
            }
        }
    }
}