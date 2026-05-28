using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Reflection;

namespace LapBoxCamera.Desktop;

internal sealed class MainForm : Form
{
    private readonly WebView2 webView;
    private FormBorderStyle restoreBorderStyle;
    private Rectangle restoreBounds;
    private FormWindowState restoreWindowState;
    private bool isHostFullscreen;

    public MainForm()
    {
        Text = "LapBoxCamera";
        MinimumSize = new Size(960, 540);
        WindowState = FormWindowState.Maximized;

        webView = new WebView2
        {
            Dock = DockStyle.Fill,
        };

        Controls.Add(webView);
        Shown += async (_, _) => await InitializeWebViewAsync();
    }

    private async Task InitializeWebViewAsync()
    {
        string webRoot;

        try
        {
            webRoot = EnsureWebAssets();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Unable to prepare embedded web assets.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "LapBoxCamera",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            Close();
            return;
        }

        await webView.EnsureCoreWebView2Async();
        webView.CoreWebView2.PermissionRequested += HandlePermissionRequested;
        webView.CoreWebView2.ContainsFullScreenElementChanged += HandleContainsFullScreenElementChanged;
        webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
            "appassets.lapboxcamera",
            webRoot,
            CoreWebView2HostResourceAccessKind.Allow);
        webView.Source = new Uri("https://appassets.lapboxcamera/index.html");
    }

    private static string EnsureWebAssets()
    {
        var webRoot = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "LapBoxCamera",
            "www");

        Directory.CreateDirectory(webRoot);

        WriteEmbeddedAsset("WebAssets.index.html", Path.Combine(webRoot, "index.html"));
        WriteEmbeddedAsset("WebAssets.Thumb.png", Path.Combine(webRoot, "Thumb.png"));

        return webRoot;
    }

    private static void WriteEmbeddedAsset(string resourceName, string destinationPath)
    {
        using var resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Missing embedded resource '{resourceName}'.");
        using var fileStream = File.Create(destinationPath);
        resourceStream.CopyTo(fileStream);
    }

    private static void HandlePermissionRequested(object? sender, CoreWebView2PermissionRequestedEventArgs args)
    {
        if (args.PermissionKind == CoreWebView2PermissionKind.Camera ||
            args.PermissionKind == CoreWebView2PermissionKind.Microphone)
        {
            args.State = CoreWebView2PermissionState.Allow;
        }
    }

    private void HandleContainsFullScreenElementChanged(object? sender, object e)
    {
        if (webView.CoreWebView2 is null)
        {
            return;
        }

        var shouldEnterFullscreen = webView.CoreWebView2.ContainsFullScreenElement;

        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyHostFullscreen(shouldEnterFullscreen));
            return;
        }

        ApplyHostFullscreen(shouldEnterFullscreen);
    }

    private void ApplyHostFullscreen(bool shouldEnterFullscreen)
    {
        if (shouldEnterFullscreen == isHostFullscreen)
        {
            return;
        }

        if (shouldEnterFullscreen)
        {
            restoreBorderStyle = FormBorderStyle;
            restoreBounds = Bounds;
            restoreWindowState = WindowState;

            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Normal;
            Bounds = Screen.FromControl(this).Bounds;
            WindowState = FormWindowState.Maximized;
            TopMost = true;
            isHostFullscreen = true;
            return;
        }

        TopMost = false;
        FormBorderStyle = restoreBorderStyle;
        WindowState = FormWindowState.Normal;
        Bounds = restoreBounds;
        WindowState = restoreWindowState;
        isHostFullscreen = false;
    }
}