using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCampus.GitCommand
{
    public class Git : Lindexi.Src.GitCommand.Git
    {
        public Git(DirectoryInfo repo) : base(repo)
        {
        }
    }
}