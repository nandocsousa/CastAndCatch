using System.Globalization;
using System.Threading;
using UnityEngine;

public class DecimalSeparatorExample : MonoBehaviour
{
    void Start()
    {
        //set the culture to "en-US" which uses a period (.) as the decimal separator
        CultureInfo culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        //now, Unity will use the period (.) as the decimal separator
        Debug.Log("Current culture set to: " + culture.Name);
        Debug.Log("Formatted number: " + 1234.56f); //this will print "1234.56"
    }
}
