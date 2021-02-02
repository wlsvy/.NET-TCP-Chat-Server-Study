﻿using CodeGenerator.Protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator.Helper
{
    internal static class CodeGenUtil
    {
        public enum Directories : byte
        {
            Root,
            Shared,
            Shared_Protocol,
        }

        public const string ROOT_DIRECTORY_NAME = "Chat";

        private static DirectoryInfo s_RootDirectoryPath;
        public static string ROOT_DIRECTORY_PATH => s_RootDirectoryPath.FullName;

        public static Dictionary<Directories, string> s_DirectoryDic;
        public static IReadOnlyDictionary<Directories, string> DIRECTORY_DIC => s_DirectoryDic;

        public static void Initialize()
        {
            s_RootDirectoryPath = FindRootDirectory() ?? throw new DirectoryNotFoundException("루트 디렉토리를 찾지 못했습니다");
            s_DirectoryDic = new Dictionary<Directories, string>()
            {
                { Directories.Root, ROOT_DIRECTORY_PATH },
                { Directories.Shared, Path.Combine(ROOT_DIRECTORY_PATH, "Shared") },
                { Directories.Shared_Protocol, Path.Combine(ROOT_DIRECTORY_PATH, "Shared", "Protocol") },
            };
        }

        public static string GetNamespaceFromDirectory(string directoryPath)
        {
            var pos = directoryPath.LastIndexOf(ROOT_DIRECTORY_NAME);
            if(pos == -1)
            {
                return null;
            }

            pos += ROOT_DIRECTORY_NAME.Length;
            pos += 1; //구분자 포함.
            if(pos >= directoryPath.Length)
            {
                return null;
            }

            return directoryPath.Substring(pos).Replace("\\", ".");
        }

        private static DirectoryInfo FindRootDirectory()
        {
            var directory = Directory.GetParent(Directory.GetCurrentDirectory());
            while (directory != null &&
                directory.Name != ROOT_DIRECTORY_NAME)
            {
                directory = directory.Parent;
            }

            if (directory == null ||
                directory.Name != ROOT_DIRECTORY_NAME)
            {
                return null;
            }

            return directory;
        }
    }
}
