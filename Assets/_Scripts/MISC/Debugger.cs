using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides a controlled way to print debug messages based on the static PrintType configuration
/// The higher the printType, the more exclusive the messages printed
/// </summary>
public static class Debugger 
{
    public enum AlertType
    {
        Verbose = 1,
        Info = 2,
        Warning = 3,
        Error = 4,
    }

    public enum PrintType
    {
        All = 0,
        EssentialInfo = 1,
        WarningsUp = 2,
        ErrorOnly = 3,
        None = 99,
    }

    public static PrintType print = PrintType.All;



    public static void Log(AlertType alertType , string message)
    {
        if (ShouldPrint(alertType))
        {
            switch (alertType)
            {
                case AlertType.Verbose:
                    Debug.Log(message);
                    break;
                case AlertType.Info:
                    Debug.Log(message);
                    break;
                case AlertType.Warning: 
                    Debug.LogWarning(message);
                    break;
                case AlertType.Error:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
            
        }
    }


    private static bool ShouldPrint(AlertType alertType)
    {
        if((int)alertType > (int)print)
        {
            return true;
        }
        return false;
    }
}
