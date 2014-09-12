using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using System.Text.RegularExpressions;
using System.Net;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

public class INI
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "5"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "2"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1"), DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
    private static extern int GetPrivateProfileString(string lpAppName, string lpKeyName, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "2"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA2101:SpecifyMarshalingForPInvokeStringArguments", MessageId = "3"), DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
    private static extern bool WritePrivateProfileString(string lpAppName, string lpKeyName, string lpString, string lpFileName);

    /*static void Main(string[] args)
    {
        string val;
        val = GetIniValue("A", "Key1", "\\initest.ini");
        Console.WriteLine(val);
        WriteIniValue("B", "Key1", "New Value", "\\initest.ini");

        val = GetIniValue("C", "Key1", "\\initest.ini");
        Console.WriteLine(val);

        WriteIniValue("D", "Key1", "Value1", "\\initest.ini");
    }*/

    public static string GetIniValue(string section, string key, string filename)
    {
        int chars = 256;
        StringBuilder buffer = new StringBuilder(chars);
        string sDefault = "";
        if (GetPrivateProfileString(section, key, sDefault,
          buffer, chars, filename) != 0)
        {
            return buffer.ToString();
        }
        else
        {
            return null;
        }
    }

    public static bool WriteIniValue(string section, string key, string value, string filename)
    {
        return WritePrivateProfileString(section, key, value, filename);
    }
}