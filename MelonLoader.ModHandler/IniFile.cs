using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MelonLoader
{
    public class IniFile
    {
        [DllImport("KERNEL32.DLL", EntryPoint = "GetPrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int GetPrivateProfileString(string lpSection, string lpKey, string lpDefault, StringBuilder lpReturnString, int nSize, string lpFileName);
        [DllImport("KERNEL32.DLL", EntryPoint = "WritePrivateProfileStringW", SetLastError = true, CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int WritePrivateProfileString(string lpSection, string lpKey, string lpValue, string lpFileName);
        private string _path = "";
        public string Path { get { return _path; } set { if (!File.Exists(value)) File.WriteAllText(value, "", Encoding.Unicode); _path = value; } }
        public IniFile(string INIPath) { this.Path = INIPath; }
        public void IniWriteValue(string Section, string Key, string Value) { WritePrivateProfileString(Section, Key, Value, this.Path); }
        public string IniReadValue(string Section, string Key)
        {
            const int MAX_CHARS = 1023;
            StringBuilder result = new StringBuilder(MAX_CHARS);
            GetPrivateProfileString(Section, Key, " _", result, MAX_CHARS, this.Path);
            if (result.ToString().Equals(" _")) return null;
            return result.ToString();
        }
    }
}
