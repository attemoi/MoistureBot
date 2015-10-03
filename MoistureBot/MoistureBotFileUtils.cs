using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using SteamKit2.Internal;

namespace MoistureBot
{
    public class MoistureBotFileUtils
    {
        public static List<IPEndPoint> ReadServerListFromDisk(String filename)
        {
            var endPointList = new List<IPEndPoint>();
            if (File.Exists("servers.bin"))
            {              
                using (var fs = File.OpenRead(filename))
                using (var reader = new BinaryReader(fs))
                {
                    while (fs.Position < fs.Length)
                    {
                        var numAddressBytes = reader.ReadInt32();
                        var addressBytes = reader.ReadBytes(numAddressBytes);
                        var port = reader.ReadInt32();

                        var ipaddress = new IPAddress(addressBytes);
                        var endPoint = new IPEndPoint(ipaddress, port);

                        endPointList.Add(endPoint);
                    }
                }
            }
            return endPointList;

        }

        public static void WriteServerListToDisk(IEnumerable<IPEndPoint> servers, String filename)
        {

            using(var fs = File.OpenWrite(filename))
            using(var writer = new BinaryWriter(fs))
            {
                foreach (var endPoint in servers)
                {
                    var addressBytes = endPoint.Address.GetAddressBytes();
                    writer.Write(addressBytes.Length);
                    writer.Write(addressBytes);
                    writer.Write(endPoint.Port);
                }
            }

        }
    }
}

