using System;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.EventArguments;
using Titanium.Web.Proxy.Models;
using Microsoft.Win32;

namespace HkrpgProxy.Core;

public class ProxyService
{
    private readonly ProxyConfig _conf;
    private readonly ProxyServer _webProxyServer;
    private readonly string _targetRedirectHost;
    private readonly int _targetRedirectPort;

    public ProxyServer ProxyServer => _webProxyServer;

    public ProxyService(string targetRedirectHost, int targetRedirectPort, ProxyConfig conf)
    {
        _conf = conf;
        _webProxyServer = new ProxyServer();
        _webProxyServer.CertificateManager.EnsureRootCertificateAsync();

        _webProxyServer.BeforeRequest += BeforeRequest;
        _webProxyServer.ServerCertificateValidationCallback += OnCertValidation;

        _targetRedirectHost = targetRedirectHost;
        _targetRedirectPort = targetRedirectPort;

        int port = conf.ProxyBindPort == 0 ? Random.Shared.Next(10000, 60000) : conf.ProxyBindPort;
        SetEndPoint(new ExplicitProxyEndPoint(IPAddress.Any, port, true));
    }

    private void SetEndPoint(ExplicitProxyEndPoint explicitEP)
    {
        explicitEP.BeforeTunnelConnectRequest += BeforeTunnelConnectRequest;

        _webProxyServer.AddEndPoint(explicitEP);
        _webProxyServer.StartAsync();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                _webProxyServer.SetAsSystemHttpProxy(explicitEP);
                _webProxyServer.SetAsSystemHttpsProxy(explicitEP);
                Console.WriteLine($"System proxy set to 127.0.0.1:{explicitEP.Port}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to set system proxy: {ex.Message}");
            }
        }
    }

    public void Shutdown()
    {
        try
        {
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    // Always try to clear system proxy settings, even if proxy server is stopped
                    _webProxyServer.DisableSystemHttpProxy();
                    _webProxyServer.DisableSystemHttpsProxy();
                    Console.WriteLine("System proxy settings cleared");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to clear system proxy: {ex.Message}");
                    // Try alternative method to clear proxy settings
                    try
                    {
                        using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", true))
                        {
                            if (key != null)
                            {
                                key.SetValue("ProxyEnable", 0);
                                key.SetValue("ProxyServer", "");
                                Console.WriteLine("System proxy settings cleared via registry");
                            }
                        }
                    }
                    catch (Exception regEx)
                    {
                        Console.WriteLine($"Failed to clear system proxy via registry: {regEx.Message}");
                    }
                }
            }

            try
            {
                _webProxyServer.Stop();
                _webProxyServer.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error stopping proxy server: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during proxy shutdown: {ex.Message}");
        }
    }

    private Task BeforeTunnelConnectRequest(object sender, TunnelConnectSessionEventArgs args)
    {
        string hostname = args.HttpClient.Request.RequestUri.Host;
        args.DecryptSsl = ShouldRedirect(hostname);
        return Task.CompletedTask;
    }

    private Task OnCertValidation(object sender, CertificateValidationEventArgs args)
    {
        if (args.SslPolicyErrors == SslPolicyErrors.None)
        {
            args.IsValid = true;
            return Task.CompletedTask;
        }

        // For development/testing, accept all certificates
        args.IsValid = true;
        return Task.CompletedTask;
    }
    
    private bool ShouldForceRedirect(string path)
    {
        foreach (var keyword in _conf.ForceRedirectOnUrlContains)
        {
            if (path.Contains(keyword)) return true;
        }
        return false;
    }

    private bool ShouldBlock(Uri uri)
    {
        var path = uri.AbsolutePath;
        return _conf.BlockUrls.Contains(path);
    }

    private Task BeforeRequest(object sender, SessionEventArgs args)
    {
        string hostname = args.HttpClient.Request.RequestUri.Host;
        string path = args.HttpClient.Request.RequestUri.AbsolutePath;
        bool shouldRedirect = ShouldRedirect(hostname);
        bool shouldForceRedirect = ShouldForceRedirect(path);

        if (shouldRedirect || shouldForceRedirect)
        {
            string requestUrl = args.HttpClient.Request.Url;
            Uri local = new Uri($"http://{_targetRedirectHost}:{_targetRedirectPort}/");

            Uri builtUrl = new UriBuilder(requestUrl)
            {
                Scheme = local.Scheme,
                Host = local.Host,
                Port = local.Port
            }.Uri;

            string replacedUrl = builtUrl.ToString();
            if (ShouldBlock(builtUrl))
            {
                Console.WriteLine($"[BLOCK] {path}");
                args.Respond(new Titanium.Web.Proxy.Http.Response(Encoding.UTF8.GetBytes("Fuck off"))
                    {
                        StatusCode = 404,
                        StatusDescription = "Oh no!!!",
                    }, true);
                return Task.CompletedTask;
            }

            Console.WriteLine($"[REDIRECT] {hostname}{path} -> {_targetRedirectHost}:{_targetRedirectPort}{path}");
            args.HttpClient.Request.Url = replacedUrl;
        }
        else
        {
            Console.WriteLine($"[PASS] {hostname}{path}");
        }

        return Task.CompletedTask;
    }

    private bool ShouldRedirect(string hostname)
    {
        if (hostname.Contains(':'))
            hostname = hostname[0..hostname.IndexOf(':')];
        foreach (string domain in _conf.AlwaysIgnoreDomains)
        {
            if (hostname.EndsWith(domain))
            {
                return false;
            }
        }
        foreach (string domain in _conf.RedirectDomains)
        {
            if (hostname.EndsWith(domain))
                return true;
        }

        return false;
    }
} 