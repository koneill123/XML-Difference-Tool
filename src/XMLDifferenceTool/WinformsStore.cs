using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XMLDifferenceTool.Core;

namespace XMLDifferenceTool
{
    //internal class WinformsStore
    //{
    //}
    public sealed class FileSettingsStore : ISettingsStore
    {
        private readonly string path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "XMLDifferenceTool", "settings.json");

        public async Task<AppSettings> LoadAsync() =>
            File.Exists(path) ? SettingsJson.Deserialize(await File.ReadAllTextAsync(path)) : new AppSettings();

        public async Task SaveAsync(AppSettings s)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            await File.WriteAllTextAsync(path, SettingsJson.Serialize(s));
        }
    }


}
