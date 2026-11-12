using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;
using System.Text;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;

[BepInPlugin("br.codex.wardislove.traducao", "WardIsLove GUI Translator", "1.0.0")]
[BepInDependency("Azumatt.WardIsLove", BepInDependency.DependencyFlags.HardDependency)]
public class WardTraducao : BaseUnityPlugin
{
    internal static readonly Dictionary<string, string> Textos = new Dictionary<string, string>
    {
        {"Ward Config Menu", "Configuração da Ward"},
        {"View as owner", "Ver como dono"}, {"Back to admin view", "Voltar à visão de admin"},
        {"'s Ward", " - Ward"}, {"Ward owner controls", "Controles do dono da Ward"},
        {"Ward owner controls (admin preview)", "Controles do dono da Ward (prévia do admin)"},
        {"Administrator controls", "Controles de administrador"},
        {"General", "Geral"}, {"Access", "Acesso"}, {"Protection", "Proteção"},
        {"Combat", "Combate"}, {"Appearance", "Aparência"}, {"Players", "Jogadores"},
        {"Ward", "Ward"},
        {"These settings belong to this one ward. The server config only decides what a brand new ward starts out with.", "Estas configurações valem para esta Ward. A configuração do servidor define apenas os valores iniciais das novas Wards."},
        {"Radius in meters", "Raio em metros"},
        {"Show the ground circle while inside", "Mostrar o círculo no chão enquanto estiver dentro"},
        {"Flash the ward when something is damaged", "Fazer a Ward piscar quando algo sofrer dano"},
        {"Close doors behind people", "Fechar portas automaticamente"},
        {"Food does not tick down inside", "A duração da comida não diminui dentro da área"},
        {"No skill loss when you die inside", "Não perder habilidades ao morrer dentro da área"},
        {"Recovery", "Recuperação"},
        {"Extra health and stamina regained per second while standing inside. Zero leaves vanilla alone.", "Vida e vigor extras recuperados por segundo dentro da área. Zero mantém os valores normais do jogo."},
        {"Extra health per second", "Vida extra por segundo"}, {"Extra stamina per second", "Vigor extra por segundo"},
        {"Charge", "Carga"},
        {"<color=#00FF00>never expires</color>", "<color=#00FF00>nunca expira</color>"},
        {"<color=#FF0000>spent, recharge it to switch the ward back on</color>", "<color=#FF0000>esgotada; recarregue para reativar a Ward</color>"},
        {"server default", "padrão do servidor"}, {"this ward", "esta Ward"},
        {"<color={0}>{1}</color> of {2} days left ({3})", "<color={0}>{1}</color> de {2} dias restantes ({3})"},
        {"A ward runs on a charge that lasts a set number of days. Hold Left Alt and use the ward to top it back up with the charge item. A ward whose charge runs out switches itself off until somebody recharges it, and its bubble fades as the charge drops.", "A carga da Ward dura um número definido de dias. Segure Alt esquerdo e interaja com a Ward para recarregá-la. Quando a carga acaba, ela se desativa até ser recarregada, e sua bolha desaparece aos poucos conforme a carga diminui."},
        {"Never expires (no recharging needed)", "Nunca expira (não precisa recarregar)"},
        {"Days a full charge lasts", "Duração da carga completa em dias"},
        {"Notifications", "Notificações"},
        {"Connected wards can share one outside boundary instead of announcing every overlap. {0} is replaced with the ward owner's name.", "Wards conectadas podem compartilhar um limite externo, evitando avisos em cada sobreposição. {0} é substituído pelo nome do dono da Ward."},
        {"Tell people when they enter or leave", "Avisar jogadores ao entrar ou sair"},
        {"Notification boundary", "Limite das notificações"}, {"Outside edge of connected wards", "Borda externa das Wards conectadas"},
        {"Every ward boundary", "Limite de cada Ward"}, {"Entry message", "Mensagem de entrada"}, {"Exit message", "Mensagem de saída"},
        {"Access decides who the ward treats as permitted. Explicitly permitted players and the creator still keep access where applicable.", "O acesso define quem a Ward considera autorizado. O criador e os jogadores adicionados continuam autorizados quando aplicável."},
        {"Access mode", "Modo de acesso"}, {"Creature exclusion", "Exclusão de criaturas"}, {"Disabled", "Desativado"},
        {"Keep monsters out", "Manter monstros fora"}, {"Show the shield bubble", "Mostrar a bolha de proteção"},
        {"Remove water inside the bubble", "Remover água de dentro da bolha"}, {"Keep unpermitted players outside", "Manter jogadores não autorizados fora"},
        {"Keep creatures outside the shield", "Manter criaturas fora do escudo"}, {"Stop unpermitted players using portals", "Impedir jogadores não autorizados de usar portais"},
        {"Allow unpermitted players to pick up items", "Permitir que jogadores não autorizados peguem itens"},
        {"Unpermitted interactions", "Interações de jogadores não autorizados"},
        {"Turn an option on to let players without ward access use that object inside this ward.", "Ative uma opção para permitir que jogadores sem acesso usem esse objeto dentro da Ward."},
        {"Allow item stand use", "Permitir uso de suportes de itens"}, {"Allow portal interaction", "Permitir interação com portais"},
        {"Allow harvesting crops and pickables", "Permitir colheita e coleta"}, {"Allow using dropped items", "Permitir uso de itens no chão"},
        {"Allow opening doors", "Permitir abertura de portas"}, {"Allow opening containers", "Permitir abertura de baús e contêineres"},
        {"Allow controlling ships", "Permitir controle de barcos"}, {"Allow editing signs", "Permitir edição de placas"},
        {"Allow using crafting and cooking stations", "Permitir uso de estações de criação e cozinha"},
        {"Allow using smelters", "Permitir uso de fundições"}, {"Allow using beehives", "Permitir uso de colmeias"},
        {"Allow using cartography tables", "Permitir uso de mesas de mapas"},
        {"Allow using fireplaces, torches and candles", "Permitir uso de fogueiras, tochas e velas"},
        {"Allow loading and changing turrets", "Permitir abastecer e alterar torres"}, {"Allow arming traps", "Permitir armar armadilhas"},
        {"Living things", "Seres vivos"}, {"What the ward does for the people and animals inside it.", "O que a Ward faz pelas pessoas e animais dentro dela."},
        {"Arrows and thrown things bounce off the bubble", "Flechas e projéteis ricocheteiam na bolha"},
        {"Tames cannot be hurt inside", "Animais domesticados não recebem dano dentro da área"},
        {"Structures", "Estruturas"}, {"What the ward does for the buildings standing inside it.", "O que a Ward faz pelas construções dentro dela."},
        {"Prevent rain damage", "Impedir dano de chuva"}, {"Prevent heavy snow damage", "Impedir dano de neve pesada"},
        {"Prevent falling ash damage", "Impedir dano de cinzas"}, {"Prevent lava damage", "Impedir dano de lava"},
        {"Make listed structures indestructible", "Tornar as estruturas listadas indestrutíveis"},
        {"Which structures (prefab names)", "Estruturas protegidas (nomes dos prefabs)"},
        {"Damage taken by buildings (% cut)", "Redução de dano nas construções (%)"}, {"Raids", "Incursões"},
        {"Offline raid protection makes structures here immune while too few of the ward's players are online.", "A proteção contra incursões deixa as estruturas imunes quando há poucos jogadores da Ward online."},
        {"Protect this base while its owners are offline", "Proteger a base enquanto os donos estão offline"},
        {"Players who must be online to allow a raid", "Jogadores online necessários para permitir uma incursão"},
        {"Unlimited fuel", "Combustível ilimitado"},
        {"Fires, ovens and hot tubs inside this ward burn forever. Nothing to do with the ward's own charge.", "Fogueiras, fornos e banheiras dentro da Ward queimam para sempre. Isso não afeta a carga da Ward."},
        {"Fires never go out", "Fogueiras nunca apagam"}, {"Ovens never go out", "Fornos nunca apagam"},
        {"Hot tubs never go out", "Banheiras nunca apagam"}, {"Which fires count (prefab names)", "Fontes de fogo incluídas (nomes dos prefabs)"},
        {"Rules", "Regras"},
        {"Force a PvP or PvE state on players inside, and pick who that applies to. Leave both off to use whatever the player chose.", "Force PvP ou PvE para jogadores dentro da área e escolha a quem se aplica. Desative ambos para manter a escolha de cada jogador."},
        {"Force PvP on inside", "Forçar PvP dentro da área"}, {"Force PvE on inside", "Forçar PvE dentro da área"},
        {"Apply to players with access", "Aplicar a jogadores com acesso"}, {"Apply to players without access", "Aplicar a jogadores sem acesso"},
        {"Call to arms message", "Mensagem de chamada às armas"}, {"Creature damage", "Dano a criaturas"},
        {"The ward can hurt monsters standing inside it. Tames are never touched.", "A Ward pode ferir monstros dentro da área. Animais domesticados não são atingidos."},
        {"Damage type", "Tipo de dano"}, {"Damage dealt each tick", "Dano causado por intervalo"},
        {"Extra damage monsters take here (%)", "Dano extra recebido por monstros (%)"},
        {"Ward model", "Modelo da Ward"}, {"Looks only. None of this changes what the ward does.", "Apenas aparência. Essas opções não alteram o funcionamento da Ward."},
        {"Model", "Modelo"}, {"Play What Is Love", "Tocar What Is Love"}, {"Play the powered hum", "Reproduzir som de funcionamento"},
        {"Shield color", "Cor do escudo"}, {"Edit color", "Editar cor"},
        {"Who can use this ward", "Quem pode usar esta Ward"}, {"Permitted", "Permitidos"}, {"Remove", "Remover"},
        {"Add somebody", "Adicionar jogador"}, {"Online now", "Online agora"}, {"Add", "Adicionar"},
        {"Seen before", "Vistos anteriormente"}, {"Refresh list", "Atualizar lista"},
        {"Owner: ", "Dono: "}, {"Permitted: ", "Permitidos: "}, {"Permitted: nobody yet", "Permitidos: ninguém ainda"},
        {"Nobody is permitted", "Nenhum jogador autorizado"}, {"Nobody else online", "Nenhum outro jogador online"},
        {"Nobody on record", "Nenhum jogador registrado"}, {"Asking the server...", "Consultando o servidor..."},
        {"Online now is who is on the server this minute.", "Online agora mostra quem está no servidor neste momento."},
        {"Online now is who is on the server this minute. Seen before is everyone the server remembers, so you can permit a friend who is logged off.", "Online agora mostra quem está no servidor. Vistos anteriormente mostra jogadores lembrados pelo servidor, inclusive amigos desconectados."},
        {"{0}  ({1}d ago)", "{0}  (há {1} dias)"}, {"  (today)", "  (hoje)"},
        {"Choose one color for the entire shield. Changes preview immediately.", "Escolha uma cor para todo o escudo. As alterações aparecem imediatamente."},
        {"Red", "Vermelho"}, {"Green", "Verde"}, {"Blue", "Azul"}, {"Opacity", "Opacidade"},
        {"Cancel", "Cancelar"}, {"Save", "Salvar"}
    };

    private static readonly Dictionary<string, Dictionary<string, string>> Languages = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
    private static Type localizationType;
    private static PropertyInfo localizationInstance;
    private static MethodInfo selectedLanguage;

    private void Awake()
    {
        Languages["Portuguese_Brazilian"] = Textos;
        LoadLanguageFiles();
        localizationType = AccessTools.TypeByName("Localization");
        if (localizationType != null)
        {
            localizationInstance = AccessTools.Property(localizationType, "instance");
            selectedLanguage = AccessTools.Method(localizationType, "GetSelectedLanguage");
        }
        Type type = AccessTools.TypeByName("WardIsLove.Util.UI.WardControlWindow");
        if (type == null) { Logger.LogError("Janela da Ward não encontrada; confira a versão do WardIsLove."); return; }
        Harmony harmony = new Harmony("br.codex.wardislove.traducao");
        MethodInfo transpiler = AccessTools.Method(typeof(WardTraducao), "Traduzir");
        int count = 0;
        PatchType(type, harmony, transpiler, ref count);
        Type charge = AccessTools.TypeByName("WardIsLove.WardManagers.ChargeManager");
        MethodInfo describe = charge == null ? null : AccessTools.Method(charge, "Describe");
        if (describe != null) { try { harmony.Patch(describe, transpiler: new HarmonyMethod(transpiler)); count++; } catch (Exception ex) { Logger.LogWarning("Não foi possível traduzir o estado da carga: " + ex.Message); } }
        Logger.LogInfo("Tradução da interface ativada; métodos processados: " + count);
    }

    private static void PatchType(Type type, Harmony harmony, MethodInfo transpiler, ref int count)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
        foreach (MethodInfo method in type.GetMethods(flags))
        {
            if (method.IsAbstract || method.ContainsGenericParameters || method.GetMethodBody() == null) continue;
            string n = method.Name;
            bool allowed = n == "Build" || n == "BuildGeneral" || n == "BuildAccess" || n == "BuildProtection" || n == "BuildCombat" || n == "BuildAppearance" || n == "BuildPlayers" || n == "TabName" || n == "UpdatePreviewButton" || n == "RebuildContent" || n == "DescribeOfflinePlayer" || n == "EnsureColorEditor" || n == "AddSelectedOnlinePlayer" || n == "AddSelectedOfflinePlayer" || n == "RemoveSelectedPlayer" || n.Contains("<BuildPlayers>");
            if (!allowed) continue;
            try { harmony.Patch(method, transpiler: new HarmonyMethod(transpiler)); count++; }
            catch (Exception ex) { BepInEx.Logging.Logger.CreateLogSource("WardTraducao").LogWarning("Não foi possível traduzir " + method.Name + ": " + ex.Message); }
        }
        foreach (Type nested in type.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)) PatchType(nested, harmony, transpiler, ref count);
    }

    private void LoadLanguageFiles()
    {
        string directory = Path.Combine(Paths.ConfigPath, "WardIsLoveInterfaceTranslations");
        if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
        foreach (string file in Directory.GetFiles(directory, "*.json"))
        {
            try
            {
                string language = Path.GetFileNameWithoutExtension(file);
                var loaded = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(file, Encoding.UTF8));
                if (loaded == null) throw new InvalidDataException("O arquivo deve conter um objeto JSON com textos.");
                Dictionary<string, string> entries;
                if (!Languages.TryGetValue(language, out entries))
                    Languages[language] = entries = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var item in loaded)
                {
                    if (item.Value != null) entries[item.Key] = item.Value;
                }
                Logger.LogInfo("Arquivo de idioma carregado: " + language);
            }
            catch (Exception ex) { Logger.LogWarning("Não foi possível carregar " + Path.GetFileName(file) + ": " + ex.Message); }
        }
    }

    public static string Translate(string original)
    {
        try
        {
            if (localizationInstance == null || selectedLanguage == null) return original;
            object instance = localizationInstance.GetValue(null, null);
            if (instance == null) return original;
            string language = selectedLanguage.Invoke(instance, null) as string;
            Dictionary<string, string> entries;
            string result;
            return language != null && Languages.TryGetValue(language, out entries) &&
                   entries.TryGetValue(original, out result) ? result : original;
        }
        catch { return original; }
    }

    public static IEnumerable<CodeInstruction> Traduzir(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo translate = AccessTools.Method(typeof(WardTraducao), "Translate");
        foreach (CodeInstruction instruction in instructions)
        {
            yield return instruction;
            // Ignore internal object names, IDs and numeric format strings.
            string value = instruction.operand as string;
            if (instruction.opcode == OpCodes.Ldstr && value != null && value.Length > 0
                && value != "0" && value != "0.0" && !value.StartsWith("Picker_")
                && !value.StartsWith("Dropdown_") && !value.StartsWith("Slider_")
                && !value.StartsWith("Toggle_") && !value.StartsWith("Input_")
                && value.IndexOf("WILDropdown", StringComparison.Ordinal) < 0)
                yield return new CodeInstruction(OpCodes.Call, translate);
        }
    }
}

