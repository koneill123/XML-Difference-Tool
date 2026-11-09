using Microsoft.JSInterop;
using XMLDifferenceTool.Core;

namespace XMLDifferenceTool.Web;

/// <summary>Stores AppSettings as JSON in the browser's localStorage.</summary>
public sealed class LocalStorageSettingsStore : ISettingsStore
{
    private const string Key = "xmldiff.settings";
    private readonly IJSRuntime js;

    public LocalStorageSettingsStore(IJSRuntime js) => this.js = js;

    public async Task<AppSettings> LoadAsync()
    {
        try
        {
            return SettingsJson.Deserialize(await js.InvokeAsync<string?>("localStorage.getItem", Key));
        }
        catch
        {
            return new AppSettings();   // storage unavailable: use defaults
        }
    }

    public async Task SaveAsync(AppSettings settings)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", Key, SettingsJson.Serialize(settings));
        }
        catch
        {
            // Storage blocked or full (e.g. private browsing): losing a preference is not worth an error
        }
    }
}
