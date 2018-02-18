using UnityEngine;
using System;
using System.Collections;
using System.IO.Ports;

public class ArduinoIO : MonoBehaviour {

    public string port = "COM3";
    public int baudrate = 9600;

    private SerialPort stream;

    public void Open() {
        stream = new SerialPort(port, baudrate);
        stream.ReadTimeout = 50;
        stream.Open();
    }

    public string ReadFromArduino(int timeout = 50)
    {
        stream.ReadTimeout = timeout;
        try
        {
            return stream.ReadLine();
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }

    public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity)
    {
        DateTime initialTime = DateTime.Now;
        DateTime nowTime;
        TimeSpan diff = default(TimeSpan);

        string dataString = null;
        do
        {
            try
            {
                dataString = stream.ReadLine();
            }
            catch (TimeoutException)
            {
                dataString = null;
            }

            if (dataString != null)
            {
                callback(dataString);
                yield return null;
            }
            else
                yield return new WaitForSeconds(0.05f);

            nowTime = DateTime.Now;
            diff = nowTime - initialTime;

        } while (diff.Milliseconds < timeout);

        if (fail != null)
            fail();
        yield return null;
    }

    public void Close() {
        stream.Close();
    }
}