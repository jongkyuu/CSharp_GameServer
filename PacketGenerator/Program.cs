using System.Xml;

namespace PacketGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            using (XmlReader r = XmlReader.Create("PDL.xml", settings))  // using 범위 벗어나면 알아서 Dispose
            {
                r.MoveToContent();
                while (r.Read())
                {
                    if (r.Depth == 1 && r.NodeType == XmlNodeType.Element) 
                        ParsePacket(r);

                    //Console.WriteLine(r.Name + " " + r["name"]);
                }
            }
        }

        private static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
                return;

            if (r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid packet node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without Name");
            }

            ParseMembers(r);
        }

        private static void ParseMembers(XmlReader r)
        {
            string packetName = r["name"];

            int depth = r.Depth + 1;
            while (r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Memeber without name");
                    return;
                }

                string memberType  = r.Name.ToLower();
                switch (memberType)
                {
                    case "bool":
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                    case "string":
                    case "list":
                        break;
                    default:
                        break;
                }

            }
        }
    }
}