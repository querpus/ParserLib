#pragma warning disable CA1710 // Identifiers should have correct suffix
#pragma warning disable format // Formatting

namespace Parser.Nodes;
//            SAMPLE
/*{
      "area": "SERVICE / DT AGS",
      "cs_running": true,
      "cs_tag": "MTAG/MEI/AM/US/AIM/1231",
      "five_s_enabled": true,
      "hostname": "mes-OptiPlex-3060",
      "ip": "10.68.134.123",
      "kiosk_alive": true,
      "label": "DT Upper Test - Service",
      "last_seen": 14,
      "maintenance": false,
      "name": "DT Upper Test - Service",
      "note": "",
      "online": true,
      "scan_guard_enabled": true,
      "ssh_running": true,
      "station_id": "",
      "uptime": "11m",
      "uptime_sec": 689
    },*/

public enum StorageType
{
  None,
  JSON,
  XML,
  CSV,
  INI,
  Text,
  Binary,
}
