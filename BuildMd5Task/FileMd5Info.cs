﻿namespace dotnetCampus.BuildMd5Task
{
    public class FileMd5Info
    {
        public string File { set; get; } = null!;
        public long FileSize { set; get; }
        public string Md5 { set; get; } = null!;
    }
}