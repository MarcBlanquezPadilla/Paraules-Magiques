using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;

public static class NetworkUtils
{
    public static string GetLocalIPAddress()
    {
        string candidateIP = "";

        try
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    string name = ni.Name.ToLower();

                    if (name.Contains("p2p") || name.Contains("tun") || name.Contains("dummy") || name.Contains("vbox"))
                        continue;

                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            string ipStr = ip.Address.ToString();

                            if (ipStr.StartsWith("192.168."))
                            {
                                return ipStr;
                            }

                            candidateIP = ipStr;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[NetworkUtils] Error leyendo IP: {e.Message}");
        }

        if (!string.IsNullOrEmpty(candidateIP))
            return candidateIP;

        return "127.0.0.1";
    }
}