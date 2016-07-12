using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BKup {
    public static class Common {
        public const int MAX_ENTRIES = 100;
        public const int MAIN_WINDOW_WIDTH = 350;
        public const int BUTTON_HEIGHT = 50;

        public const string PATH_LOG = "\\Log";
        public const string PATH_CONFIG = "\\Config.txt";
        public const string PATH_CONFIG_TMP = "\\Config.tmp";
        public const string PATH_HISTORY = "\\History";

        public const string FONT = "宋体";

        public enum Status { Normal, Deleted, Paused, Ignored, SrcNotExists, Error, Unknown };

        public struct Entry {
            public string src_path,tar_path;
            public Status status;
        };
    }
}
