// -----------------------------------------------------------------------
// <copyright file="DbTypes.cs" company="SMS">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess
{
    public class DbTypes
    {
        private static string _main = "main";

        private static string _production = "production";

        private static string _archive = "archive";
        private static string _archive5 = "archive05";
        private static string _archive52 = "archive052";
        private static string _archive53 = "archive053";
        private static string _archive6 = "archive06";

        private static string _onUser = "on_user";
        private static string _wareHouse = "warehouse";
        private static string _message = "MessageDB";
        private static string _maintenance = "maintenance";
        private static string _dblog = "dblog";
        private static string _db03 = "db03";
        private static string _bingo_archive = "BG_ArchiveDB";

        public static string MESSAGE
        {
            get
            {
                return _message;
            }
        }

        public static string MAIN
        {
            get
            {
                return _main;
            }
        }

        public static string PRODUCTION
        {
            get
            {
                return _production;
            }
        }

        public static string ARCHIVE
        {
            get
            {
                return _archive;
            }
        }

        public static string ARCHIVE5
        {
            get
            {
                return _archive5;
            }
        }

        public static string ARCHIVE52
        {
            get
            {
                return _archive52;
            }
        }

        public static string ARCHIVE53
        {
            get
            {
                return _archive53;
            }
        }

        public static string ARCHIVE6
        {
            get
            {
                return _archive6;
            }
        }

        public static string ON_USER
        {
            get
            {
                return _onUser;
            }
        }

        public static string WAREHOUSE
        {
            get
            {
                return _wareHouse;
            }
        }

        public static string DBLOG
        {
            get
            {
                return _dblog;
            }
        }

        public static string DB03
        {
            get
            {
                return _db03;
            }
        }

        public static string Maintenance
        {
            get
            {
                return _maintenance;
            }
        }

        public static string BINGO_ARCHIVE
        {
            get
            {
                return _bingo_archive;
            }
        }
    }
}
