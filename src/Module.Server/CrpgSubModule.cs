﻿using Crpg.Module.Common;
using Crpg.Module.Common.Models;
using Crpg.Module.Modes.Battle;
using Crpg.Module.Modes.Conquest;
using Crpg.Module.Modes.Dtv;
using Crpg.Module.Modes.Duel;
using Crpg.Module.Modes.Siege;
using Crpg.Module.Modes.TeamDeathmatch;
using Newtonsoft.Json;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Diamond;
using WindowsFirewallHelper.Addresses;

#if CRPG_SERVER
using System.Runtime.CompilerServices;
using TaleWorlds.PlayerServices;
using WindowsFirewallHelper;
using Crpg.Module.HarmonyPatches;
#else
using TaleWorlds.Engine.GauntletUI;
#endif

#if CRPG_EXPORT
using System.Runtime.CompilerServices;
using Crpg.Module.DataExport;
using TaleWorlds.Library;
using TaleWorlds.Localization;
#endif

namespace Crpg.Module;

internal class CrpgSubModule : MBSubModuleBase
{
#if CRPG_SERVER
    public static CrpgSubModule Instance = default!;
    public Dictionary<PlayerId, IAddress> WhitelistedIps = new();
    private IFirewallRule? _cachedFirewallRule;
    public int Port()
    {
        return TaleWorlds.MountAndBlade.Module.CurrentModule.StartupInfo.ServerPort;
    }

    public IFirewallRule? GetCachedFirewallRule()
    {
        return _cachedFirewallRule;
    }
#endif
    static CrpgSubModule()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
            Debug.Print(args.ExceptionObject.ToString(), color: Debug.DebugColor.Red);
    }

    private CrpgConstants _constants = default!;

    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();

        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

#if CRPG_SERVER
        CrpgSubModule.Instance = this;

        var firewallRule = Firewall.GetFirewallRule(Port(), _cachedFirewallRule);
        if (firewallRule == null)
        {
            Debug.Print("[Firewall] FirewallRule " + Firewall.GetFirewallRuleName(Port()) + " not found on your server. Creating...", 0, Debug.DebugColor.Red);
            _cachedFirewallRule = Firewall.CreateFirewallRule(Port());
        }
        else
        {
            _cachedFirewallRule = firewallRule;
        }
#endif
        _constants = LoadCrpgConstants();
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgBattleGameMode(_constants, isSkirmish: true));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgBattleGameMode(_constants, isSkirmish: false));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgConquestGameMode(_constants));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgSiegeGameMode(_constants));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgTeamDeathmatchGameMode(_constants));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgDuelGameMode(_constants));
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddMultiplayerGameMode(new CrpgDtvGameMode(_constants));

#if CRPG_SERVER
        CrpgServerConfiguration.Init();
        CrpgFeatureFlags.Init();
#endif

#if CRPG_EXPORT
        LoadMainMenu();

        /*
        TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(new InitialStateOption("Scale",
            new TextObject("Scale"), 4578, Scale, () => (false, null)));*/
#endif

        // Uncomment to start watching UI changes.
#if CRPG_CLIENT
        // UIResourceManager.UIResourceDepot.StartWatchingChangesInDepot();
#endif
    }

    protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
    {
        base.InitializeGameStarter(game, starterObject);
        InitializeGameModels(starterObject);
        CrpgSkills.Initialize(game);
        CrpgBannerEffects.Initialize(game);
        ManagedParameters.Instance.Initialize(ModuleHelper.GetXmlPath("Crpg", "managed_core_parameters"));
#if CRPG_CLIENT
        game.GameTextManager.LoadGameTexts();
#endif
    }

    protected override void OnApplicationTick(float delta)
    {
        base.OnApplicationTick(delta);
        // Uncomment to hot reload UI after changes.
#if CRPG_CLIENT
        // UIResourceManager.UIResourceDepot.CheckForChanges();
#endif
    }

    private CrpgConstants LoadCrpgConstants()
    {
        string path = ModuleHelper.GetModuleFullPath("cRPG") + "ModuleData/constants.json";
        return JsonConvert.DeserializeObject<CrpgConstants>(File.ReadAllText(path))!;
    }

    private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
    {
        Debug.Print(args.ExceptionObject.ToString(), color: Debug.DebugColor.Red);
    }

    private void InitializeGameModels(IGameStarter basicGameStarter)
    {
        basicGameStarter.AddModel(new CrpgAgentStatCalculateModel(_constants));
        basicGameStarter.AddModel(new CrpgItemValueModel(_constants));
        basicGameStarter.AddModel(new CrpgAgentApplyDamageModel(_constants));
        basicGameStarter.AddModel(new CrpgStrikeMagnitudeModel(_constants));
    }

#if CRPG_EXPORT
    private void LoadMainMenu()
    {
        List<InitialStateOption> mainMenuOptions = new()
        {
            new InitialStateOption("ExportData",
            new TextObject("Export Data"), 4578, ExportData, () => (false, null)),
            new InitialStateOption("ComputeAutoStats",
            new TextObject("Compute AutoGenerated Stats"), 4578, ComputeAutoStats, () => (false, null)),
            new InitialStateOption("ExportImages",
            new TextObject("Export Thumbnails"), 4578, ExportImages, () => (false, null)),
            new InitialStateOption("SelectWhatToRefund",
            new TextObject("Change Refund"), 4578, ChangeRefund, () => (false, null)),
            new InitialStateOption("SelectWhatToRefund",
            new TextObject("Refund Selected"), 4578, Refund, () => (false, null)),
            new InitialStateOption("Scale",
            new TextObject("Scale"), 4578, Scale, () => (false, null)),
            new InitialStateOption("ScaleWeapon",
            new TextObject("ScaleWeapon"), 4578, ScaleWeapon, () => (false, null)),

        };
        foreach (var opt in mainMenuOptions)
        {
            TaleWorlds.MountAndBlade.Module.CurrentModule.AddInitialStateOption(opt);
        }
    }

    private void RefundCrossbow()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Crossbows."));
        Task.WhenAll(exporters.Select(e => e.RefundCrossbow("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Crossbows were refunded"));
        });
    }

    private void RefundArmor()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Armors."));
        Task.WhenAll(exporters.Select(e => e.RefundArmor("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Armors were refunded"));
        });
    }

    private void RefundWeapons()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Armors."));
        Task.WhenAll(exporters.Select(e => e.RefundWeapons("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Weapons were refunded"));
        });
    }

    private void RefundThrowing()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Throwing."));
        Task.WhenAll(exporters.Select(e => e.RefundThrowing("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Throwing were refunded"));
        });
    }

    private void RefundCav()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Cav."));
        Task.WhenAll(exporters.Select(e => e.RefundCav("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("cav has been refunded"));
        });
    }

    private void RefundBow()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Bows."));
        Task.WhenAll(exporters.Select(e => e.RefundBow("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Bow were Refunded"));
        });
    }

    private void RefundShield()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Refunding Bows."));
        Task.WhenAll(exporters.Select(e => e.RefundShield("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Shield were Refunded"));
        });
    }

    private void Scale()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Editing Class."));
        Task.WhenAll(exporters.Select(e => e.Scale("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Done."));
        });
    }

    private void ScaleWeapon()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Editing Class."));
        Task.WhenAll(exporters.Select(e => e.ScaleWeapon("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Done."));
        });
    }

    private void ComputeAutoStats()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Computing Auto Generated Stats."));
        Task.WhenAll(exporters.Select(e => e.ComputeAutoStats("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Done."));
        });
    }

    private void ExportData()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
            // new SettlementExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Exporting data."));
        Task.WhenAll(exporters.Select(e => e.Export("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Done."));
        });
    }

    private void ExportImages()
    {
        IDataExporter[] exporters =
        {
            new ItemExporter(),
        };

        InformationManager.DisplayMessage(new InformationMessage("Exporting Images."));
        Task.WhenAll(exporters.Select(e => e.ImageExport("lol"))).ContinueWith(t =>
        {
            InformationManager.DisplayMessage(t.IsFaulted
                ? new InformationMessage(t.Exception!.Message)
                : new InformationMessage("Done."));
        });
    }

    private void Refund()
    {
        switch (_toRefund)
        {
            case 0:
                RefundCrossbow();
                break;
            case 1:
                RefundThrowing();
                break;
            case 2:
                RefundBow();
                break;
            case 3:
                RefundShield();
                break;
            case 4:
                RefundArmor();
                break;
            case 5:
                RefundCav();
                break;
            case 6:
                RefundWeapons();
                break;
            default:
                throw new ArgumentException("Invalid argument for 'toRefund'");
        }
    }

    private void ChangeRefund()
    {
        _toRefund = (_toRefund + 1) % 7;
        string message = _toRefund switch
        {
            0 => "Refund Crossbows",
            1 => "Refund Throwings",
            2 => "Refund Bows",
            3 => "Refund Shields",
            4 => "Refund Armors",
            5 => "Refund Cav",
            6 => "Refund Weapons"
        }

        + " has been selected";
        InformationManager.DisplayMessage(new InformationMessage(message));
    }

    private int _toRefund = 0;

#endif
}
