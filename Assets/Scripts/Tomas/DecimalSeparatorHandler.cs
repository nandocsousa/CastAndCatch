using System.Globalization;
using System.Threading;
using UnityEngine;

public class DecimalSeparatorHandler : MonoBehaviour
{
    void Start()
    {
        //set the culture to "en-US" which uses a period (.) as the decimal separator
        CultureInfo culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
