using System.Text.Json.Serialization;

namespace hkrpg.proxy;

[JsonConverter(typeof(ProxyConfigJsonConverter))]
public class ProxyConfig
{
    // Hardcoded redirect domains
    public List<string> RedirectDomains { get; } = new()
    {
        ".hoyoverse.com",
        ".mihoyo.com",
        ".yuanshen.com",
        ".bhsr.com",
        ".starrails.com",
        ".juequling.com",
        ".zenlesszonezero.com",
        ".bh3.com",
        ".honkaiimpact3.com",
        ".mob.com",
        ".hyg.com"
    };

    // Hardcoded domains to ignore
    public List<string> AlwaysIgnoreDomains { get; } = new()
    {
        "autopatchcn.yuanshen.com",
        "autopatchhk.yuanshen.com",
        "autopatchcn.juequling.com",
        "autopatchos.zenlesszonezero.com",
        "autopatchcn.bhsr.com",
        "autopatchos.starrails.com"
    };

    // Hardcoded URLs to block
    public HashSet<string> BlockUrls { get; } = new()
    {
        "/sdk/upload",
        "/sdk/dataUpload",
        "/common/h5log/log/batch",
        "/crash/dataUpload",
        "/crashdump/dataUpload",
        "/client/event/dataUpload",
        "/log",
        "/asm/dataUpload",
        "/sophon/dataUpload",
        "/apm/dataUpload",
        "/2g/dataUpload",
        "/v1/firelog/legacy/log",
        "/h5/upload",
        "/_ts",
        "/perf/config/verify",
        "/ptolemaios_api/api/reportStrategyData"
    };

    // Hardcoded URLs to force redirect
    public List<string> ForceRedirectOnUrlContains { get; } = new()
    {
        "query_dispatch",
        "query_gateway",
        "query_region_list",
        "query_cur_region"
    };

    // User configurable settings
    public required string DestinationHost { get; set; }
    public required int DestinationPort { get; set; }
    public int ProxyBindPort { get; set; }
    public string? LastGamePath { get; set; }
}
