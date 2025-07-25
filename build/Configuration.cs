using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Nuke.Common.Tooling;

[TypeConverter(typeof(TypeConverter<Configuration>))]
public class Configuration : Enumeration
{
    public static Configuration Debug = new Configuration { Value = nameof(Debug) };
    public static Configuration Release = new Configuration { Value = nameof(Release) };

    public static implicit operator string(Configuration configuration)
    {
        return configuration.Value;
    }
}

public class ChangelogEntry
{
    public string Version { get; set; }
    public DateTime Date { get; set; }
    public List<string> Added { get; set; } = new List<string>();
    public List<string> Changed { get; set; } = new List<string>();
    public List<string> Fixed { get; set; } = new List<string>();
}


public static class ChangelogParser
{
    public static ChangelogEntry ParseVersion(string changelogContent, string version)
    {
        var entry = new ChangelogEntry { Version = version };

        // Нормализуем переносы строк
        changelogContent = changelogContent.Replace("\r\n", "\n");
        Console.WriteLine($"Changelog content:\n[{changelogContent}]");

        // Паттерн для поиска заголовка версии
        string versionPattern = $@"## \[v{Regex.Escape(version)}\] - (\d{{4}}-\d{{2}}-\d{{2}})";
        Console.WriteLine($"Version pattern: {versionPattern}");

        var versionMatch = Regex.Match(changelogContent, versionPattern);

        if (!versionMatch.Success)
        {
            Console.WriteLine($"Version '{version}' not found in changelog.");
            return null;
        }

        entry.Date = DateTime.Parse(versionMatch.Groups[1].Value);
        Console.WriteLine($"Found version: {version}, Date: {entry.Date}");

        // Паттерн для секции от заголовка до ---
        string sectionPattern = $@"## \[v{Regex.Escape(version)}\] - \d{{4}}-\d{{2}}-\d{{2}}\n(.*?)\n---";
        var sectionMatch = Regex.Match(changelogContent, sectionPattern, RegexOptions.Singleline);

        if (!sectionMatch.Success)
        {
            Console.WriteLine($"Section for version {version} not found. Pattern: {sectionPattern}");
            return entry;
        }

        string sectionContent = sectionMatch.Groups[1].Value;
        Console.WriteLine($"Section content for {version}:\n[{sectionContent}]");

        ParseSection(sectionContent, "Added", entry.Added);
        ParseSection(sectionContent, "Changed", entry.Changed);
        ParseSection(sectionContent, "Fixed", entry.Fixed);

        Console.WriteLine($"Added: {string.Join(", ", entry.Added)}");
        Console.WriteLine($"Changed: {string.Join(", ", entry.Changed)}");
        Console.WriteLine($"Fixed: {string.Join(", ", entry.Fixed)}");

        return entry;
    }

    private static void ParseSection(string content, string sectionName, List<string> items)
    {
        string sectionPattern = $@"### {sectionName}\n(.*?)(?=\n### |\n---|\Z)";
        var match = Regex.Match(content, sectionPattern, RegexOptions.Singleline);

        if (!match.Success)
        {
            Console.WriteLine($"Section '{sectionName}' not found in content:\n[{content}]");
            return;
        }

        string sectionContent = match.Groups[1].Value;
        Console.WriteLine($"Found {sectionName} content:\n[{sectionContent}]");

        var lines = sectionContent.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("- "))
            {
                items.Add(line.Substring(2).Trim());
            }
        }
    }
}