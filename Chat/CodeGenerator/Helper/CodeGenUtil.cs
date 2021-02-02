using CodeGenerator.Protocol;
using System;
using System.Collections.Generic;
using System.IO;

namespace CodeGenerator.Helper
{
    internal static class CodeGenUtil
    {
        public const string ROOT_DIRECTORY_NAME = "Chat";

        private static string s_RootDirPath;
        private static string s_SharedDirPath;
        private static string s_SharedProtocolDirPath;
        public static string ROOT_DIR_PATH => s_RootDirPath;
        public static string SHARED_DIR_PATH => s_SharedDirPath;
        public static string SHARED_PROTOCOL_DIR_PATH => s_SharedProtocolDirPath;

        public static void Initialize()
        {
            var root = FindRootDirectory() ?? throw new DirectoryNotFoundException("루트 디렉토리를 찾지 못했습니다");
            s_RootDirPath = root.FullName;
            s_SharedDirPath = $"{root.FullName}\\Shared";
            s_SharedProtocolDirPath = $"{root.FullName}\\Shared\\Protocol";
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
