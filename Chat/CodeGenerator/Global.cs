using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeGenerator
{
    internal static class Global
    {
        public enum Directories : byte
        {
            Root,
            Shared_Protocol
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
                { Directories.Shared_Protocol, Path.Combine(ROOT_DIRECTORY_PATH, "Shared", "Protocol") },
            };
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
