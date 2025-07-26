using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;

public class ChangelogEntry
{
    public string Version { get; set; }
    public DateTime Date { get; set; }
    public List<string> Added { get; set; } = new List<string>();
    public List<string> Changed { get; set; } = new List<string>();
    public List<string> Fixed { get; set; } = new List<string>();

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.AppendLine($"## Version: v{Version} - Date: {Date.ToString("yyyy-MM-dd")}\n");

        if (Added.Any())
        {
            stringBuilder.AppendLine($"### Added:\n");
            foreach (var item in Added)
            {
                stringBuilder.AppendLine($"- {item}\n");
            }
        }

        if (Changed.Any())
        {
            stringBuilder.AppendLine($"### Changed:\n");
            foreach (var item in Changed)
            {
                stringBuilder.AppendLine($"- {item}\n");
            }
        }

        if (Fixed.Any())
        {
            stringBuilder.AppendLine($"### Fixed:\n");
            foreach (var item in Fixed)
            {
                stringBuilder.AppendLine($"- {item}\n");
            }
        }

        return stringBuilder.ToString();
    }
}

