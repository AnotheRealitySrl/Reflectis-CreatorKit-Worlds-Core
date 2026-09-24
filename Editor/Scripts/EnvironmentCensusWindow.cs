using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Virtuademy.SDK.TenantConfiguration.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Virtuademy.SDK.Environments.Editor
{
    /// <summary>
    /// Where does each scene of this project already exist as a published environment? For every
    /// scene listed in the project's <see cref="SceneListScriptableObject"/> the window asks the
    /// Application API which worlds hold an environment with the same name (and whether one exists at
    /// tenant level), and says so per scene: not published, one world, several worlds (a duplicate
    /// that may belong at tenant level), tenant. The reverse view lists the environments the API knows
    /// that no scene of this project produces.
    /// </summary>
    /// <remarks>
    /// The match key is the environment <c>fileName</c>, which the publish pipeline derives from the
    /// scene name the same way <see cref="SceneListScriptableObject.SceneConfiguration.SceneNameFiltered"/>
    /// does (lower-case, alphanumeric only): the bundle is <c>…_scenes_&lt;name&gt;.bundle</c> and the
    /// API records that name in <c>env_filename</c>. Read-only: nothing here publishes or deletes.
    /// Written for the 2026.6 republish campaign (ADR 0021: every world built with the Reflectis SDK
    /// is rebuilt with the Virtuademy one), whose first step is deciding which environments survive,
    /// which move to tenant level and which go.
    /// </remarks>
    public class EnvironmentCensusWindow : EditorWindow
    {
        private const string api_version = "2";
        private static readonly HttpClient httpClient = new();
        private static readonly Regex filter = new(@"[^a-z0-9]", RegexOptions.Compiled);

        private class WorldDto { public int Id; public string Label; public string Status; }
        private class EnvironmentDto
        {
            public int Id; public string Label; public string FileName; public string Catalog; public string Status;
            public int? WorldId; public bool? IsTenant; public DateTime LastUpdate; public List<string> Platforms;
        }
        private class Hit { public WorldDto World; public EnvironmentDto Env; }
        private class SceneRow { public string Scene; public string Key; public List<Hit> Hits = new(); }

        private readonly List<SceneRow> rows = new();
        private readonly List<Hit> orphans = new();
        private readonly List<string> notes = new();
        private ScrollView list;
        private Label status;
        private Button refreshButton, exportButton;

        [MenuItem("Virtuademy/Environment census")]
        public static void Open()
        {
            EnvironmentCensusWindow window = GetWindow<EnvironmentCensusWindow>();
            window.titleContent = new GUIContent("Environment census");
            window.minSize = new Vector2(720, 400);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            root.style.paddingLeft = root.style.paddingRight = root.style.paddingTop = 8;

            Label intro = new("For every scene in this project's AddressablesSceneList: the worlds where an environment with the same name is already published, and whether one exists at tenant level. Read-only.");
            intro.style.whiteSpace = WhiteSpace.Normal;
            intro.style.marginBottom = 6;
            root.Add(intro);

            VisualElement bar = new() { style = { flexDirection = FlexDirection.Row, marginBottom = 6 } };
            refreshButton = new Button(() => _ = RunAsync()) { text = "Refresh" };
            exportButton = new Button(ExportCsv) { text = "Export CSV" };
            exportButton.SetEnabled(false);
            status = new Label("") { style = { marginLeft = 8, unityTextAlign = TextAnchor.MiddleLeft } };
            bar.Add(refreshButton); bar.Add(exportButton); bar.Add(status);
            root.Add(bar);

            list = new ScrollView();
            list.style.flexGrow = 1;
            root.Add(list);

            _ = RunAsync();
        }

        private async Task RunAsync()
        {
            rows.Clear(); orphans.Clear(); notes.Clear();
            list.Clear();
            refreshButton.SetEnabled(false); exportButton.SetEnabled(false);
            try
            {
                SceneListScriptableObject scenes = LoadSceneList();
                if (scenes == null)
                {
                    status.text = "No AddressablesSceneList asset in this project.";
                    return;
                }
                foreach (SceneListScriptableObject.SceneConfiguration cfg in scenes.SceneConfigurations.Where(c => c.Scene != null))
                {
                    rows.Add(new SceneRow { Scene = cfg.Scene.name, Key = cfg.SceneNameFiltered });
                }

                string apiUrl = EditorApiEndpoint.ApplicationApiUrl;
                if (string.IsNullOrEmpty(apiUrl))
                {
                    status.text = "No Application API URL in the tenant configuration.";
                    Render();
                    return;
                }

                status.text = "Loading worlds…";
                List<WorldDto> worlds = await GetAsync<List<WorldDto>>($"{apiUrl}/worlds?api-version={api_version}") ?? new();
                if (worlds.Count == 0)
                {
                    notes.Add("No world visible to this account (or the session expired: log in again).");
                }

                Dictionary<string, List<Hit>> index = new(StringComparer.OrdinalIgnoreCase);
                int done = 0;
                foreach (WorldDto world in worlds)
                {
                    status.text = $"Loading environments… {++done}/{worlds.Count} ({world.Label})";
                    List<EnvironmentDto> envs = await GetAsync<List<EnvironmentDto>>($"{apiUrl}/worlds/{world.Id}/environments?api-version={api_version}");
                    if (envs == null) { notes.Add($"World {world.Id} '{world.Label}': environments not readable with this account."); continue; }
                    foreach (EnvironmentDto env in envs)
                    {
                        // A tenant environment shared to the world comes back with isTenant = true and no
                        // world id: it is counted once, under "tenant", not once per world.
                        if (env.IsTenant == true) { Add(index, env, null); continue; }
                        Add(index, env, world);
                    }
                }

                status.text = "Loading tenant environments…";
                List<EnvironmentDto> tenantEnvs = await GetAsync<List<EnvironmentDto>>($"{apiUrl}/tenants/environments?api-version={api_version}", quiet: true);
                if (tenantEnvs != null)
                {
                    foreach (EnvironmentDto env in tenantEnvs) { env.IsTenant = true; Add(index, env, null); }
                }
                else
                {
                    notes.Add("Tenant-level environments: not readable with this account (TenantManager role needed); only the ones shared to a visible world are listed.");
                }

                HashSet<string> sceneKeys = new(rows.Select(r => r.Key), StringComparer.OrdinalIgnoreCase);
                foreach (SceneRow row in rows)
                {
                    if (index.TryGetValue(row.Key, out List<Hit> hits)) row.Hits = hits;
                }
                orphans.AddRange(index.Where(kv => !sceneKeys.Contains(kv.Key)).SelectMany(kv => kv.Value)
                                      .OrderBy(h => h.Env.IsTenant == true ? -1 : h.World?.Id ?? 0).ThenBy(h => h.Env.FileName));

                status.text = $"{rows.Count} scenes · {worlds.Count} worlds · {index.Values.Sum(v => v.Count)} environments known to the API";
                exportButton.SetEnabled(true);
            }
            catch (Exception ex)
            {
                status.text = "Failed: " + ex.Message;
                Debug.LogError($"[EnvironmentCensus] {ex}");
            }
            finally
            {
                refreshButton.SetEnabled(true);
                Render();
            }
        }

        private static void Add(Dictionary<string, List<Hit>> index, EnvironmentDto env, WorldDto world)
        {
            string key = Key(env.FileName ?? env.Label);
            if (string.IsNullOrEmpty(key)) return;
            if (!index.TryGetValue(key, out List<Hit> hits)) index[key] = hits = new List<Hit>();
            // the same tenant environment is returned by every world it is shared to: keep one
            if (env.IsTenant == true && hits.Any(h => h.Env.IsTenant == true && h.Env.Id == env.Id)) return;
            hits.Add(new Hit { World = world, Env = env });
        }

        private static string Key(string name) => name == null ? null : filter.Replace(name.ToLowerInvariant(), string.Empty);

        private async Task<T> GetAsync<T>(string url, bool quiet = false) where T : class
        {
            HttpResponseMessage response = await EditorSessionManager.SendAuthorizedAsync(
                () => new HttpRequestMessage(HttpMethod.Get, url), httpClient, allowInteractive: false);
            if (response == null) return null;
            if (!response.IsSuccessStatusCode)
            {
                if (!quiet) Debug.LogWarning($"[EnvironmentCensus] GET {url} -> {(int)response.StatusCode}");
                return null;
            }
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        private static SceneListScriptableObject LoadSceneList()
        {
            string guid = AssetDatabase.FindAssets("t:" + nameof(SceneListScriptableObject)).FirstOrDefault();
            return guid == null ? null : AssetDatabase.LoadAssetAtPath<SceneListScriptableObject>(AssetDatabase.GUIDToAssetPath(guid));
        }

        private static string Verdict(SceneRow row)
        {
            bool tenant = row.Hits.Any(h => h.Env.IsTenant == true);
            int worlds = row.Hits.Count(h => h.Env.IsTenant != true);
            if (row.Hits.Count == 0) return "not published";
            if (tenant && worlds == 0) return "tenant level";
            if (tenant) return $"tenant level AND {worlds} world(s) — duplicate";
            if (worlds == 1) return "1 world";
            return $"{worlds} worlds — duplicate, tenant-level candidate";
        }

        private static string Describe(Hit h)
        {
            string where = h.Env.IsTenant == true ? "tenant" : $"world {h.World?.Id} '{h.World?.Label}'";
            string platforms = h.Env.Platforms != null && h.Env.Platforms.Count > 0 ? string.Join("/", h.Env.Platforms) : "-";
            return $"{where} · catalog {h.Env.Catalog} · {h.Env.Status} · {platforms} · updated {h.Env.LastUpdate:yyyy-MM-dd}";
        }

        private void Render()
        {
            list.Clear();
            foreach (string note in notes)
            {
                list.Add(new Label("⚠ " + note) { style = { whiteSpace = WhiteSpace.Normal, marginBottom = 4 } });
            }

            list.Add(Header("Scenes of this project"));
            foreach (SceneRow row in rows.OrderByDescending(r => r.Hits.Count).ThenBy(r => r.Scene))
            {
                Foldout fold = new() { text = $"{row.Scene}  —  {Verdict(row)}", value = row.Hits.Count > 1 };
                fold.style.marginBottom = 2;
                foreach (Hit h in row.Hits.OrderBy(h => h.Env.IsTenant == true ? -1 : h.World?.Id ?? 0))
                {
                    fold.Add(new Label("• " + Describe(h)) { style = { marginLeft = 12, whiteSpace = WhiteSpace.Normal } });
                }
                list.Add(fold);
            }

            if (orphans.Count > 0)
            {
                list.Add(Header($"Environments known to the API with no scene in this project ({orphans.Count})"));
                Label hint = new("Published from another project, renamed since, or no longer wanted — the census decides which. Not deletable from here.")
                { style = { whiteSpace = WhiteSpace.Normal, marginBottom = 4 } };
                list.Add(hint);
                foreach (Hit h in orphans)
                {
                    list.Add(new Label($"• {h.Env.FileName}  —  {Describe(h)}") { style = { marginLeft = 12, whiteSpace = WhiteSpace.Normal } });
                }
            }
        }

        private static Label Header(string text)
        {
            return new Label(text) { style = { unityFontStyleAndWeight = FontStyle.Bold, marginTop = 8, marginBottom = 4 } };
        }

        private void ExportCsv()
        {
            string path = EditorUtility.SaveFilePanel("Export environment census", Path.GetDirectoryName(Application.dataPath),
                                                      $"EnvironmentCensus_{DateTime.Now:yyyyMMdd-HHmm}.csv", "csv");
            if (string.IsNullOrEmpty(path)) return;

            StringBuilder sb = new();
            sb.AppendLine("scene;key;verdict;where;worldId;worldLabel;catalog;status;platforms;lastUpdate;environmentId");
            foreach (SceneRow row in rows)
            {
                if (row.Hits.Count == 0) { sb.AppendLine($"{Csv(row.Scene)};{row.Key};{Verdict(row)};;;;;;;;"); continue; }
                foreach (Hit h in row.Hits)
                {
                    sb.AppendLine(string.Join(";", Csv(row.Scene), row.Key, Csv(Verdict(row)), h.Env.IsTenant == true ? "tenant" : "world",
                                              h.World?.Id.ToString() ?? "", Csv(h.World?.Label ?? ""), Csv(h.Env.Catalog), h.Env.Status,
                                              h.Env.Platforms == null ? "" : string.Join("/", h.Env.Platforms), h.Env.LastUpdate.ToString("yyyy-MM-dd"), h.Env.Id.ToString()));
                }
            }
            foreach (Hit h in orphans)
            {
                sb.AppendLine(string.Join(";", "", Csv(h.Env.FileName), "no scene in this project", h.Env.IsTenant == true ? "tenant" : "world",
                                          h.World?.Id.ToString() ?? "", Csv(h.World?.Label ?? ""), Csv(h.Env.Catalog), h.Env.Status,
                                          h.Env.Platforms == null ? "" : string.Join("/", h.Env.Platforms), h.Env.LastUpdate.ToString("yyyy-MM-dd"), h.Env.Id.ToString()));
            }
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
            status.text = "Exported to " + path;
        }

        private static string Csv(string s) => s == null ? "" : (s.Contains(';') || s.Contains('"') ? "\"" + s.Replace("\"", "\"\"") + "\"" : s);
    }
}
