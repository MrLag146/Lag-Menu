using System.Collections.Generic;
using ExitGames.Client.Photon;

namespace LagMenu.Utilities
{
    public class ModInfo
    {
        public string name;
        public bool legal;
    }

    public static class ModDatabase
    {
        public static readonly Dictionary<string, ModInfo> modDictionary = new Dictionary<string, ModInfo>
        {
            { "genesis", new ModInfo { name = "Genesis", legal = false } },
            { "HP_Left", new ModInfo { name = "Holdable Pad", legal = true } },
            { "GrateVersion", new ModInfo { name = "Grate", legal = true } },
            { "void", new ModInfo { name = "Void", legal = false } },
            { "BANANAOS", new ModInfo { name = "Banana OS", legal = true } },
            { "GC", new ModInfo { name = "GorillaCraft", legal = true } },
            { "CarName", new ModInfo { name = "GorillaVehicles", legal = true } },
            { "6p72ly3j85pau2g9mda6ib8px", new ModInfo { name = "ColossalCheatMenu V2", legal = false } },
            { "6XpyykmrCthKhFeUfkYGxv7xnXpoe2", new ModInfo { name = "ColossalCheatMenu V2", legal = false } },
            { "FPS-Nametags for Zlothy", new ModInfo { name = "Zlothy FPS Tags", legal = true } },
            { "cronos", new ModInfo { name = "Cronos", legal = false } },
            { "ORBIT", new ModInfo { name = "Orbit", legal = false } },
            { "Violet On Top", new ModInfo { name = "Violet", legal = false } },
            { "violetpaiduser", new ModInfo { name = "Violet Paid", legal = false } },
            { "violetfree", new ModInfo { name = "Violet Free", legal = false } },
            { "MP25", new ModInfo { name = "MonkePhone", legal = true } },
            { "monkephone", new ModInfo { name = "MonkePhone", legal = true } },
            { "GorillaWatch", new ModInfo { name = "GorillaWatch", legal = true } },
            { "InfoWatch", new ModInfo { name = "GorillaInfoWatch", legal = true } },
            { "BananaPhone", new ModInfo { name = "Banana Phone", legal = true } },
            { "Vivid", new ModInfo { name = "Vivid", legal = false } },
            { "RGBA", new ModInfo { name = "Custom Cosmetics", legal = true } },
            { "colour", new ModInfo { name = "Custom Cosmetics", legal = true } },
            { "cheese is gouda", new ModInfo { name = "WhoIsCheating", legal = true } },
            { "shirtversion", new ModInfo { name = "GorillaShirts", legal = true } },
            { "gpronouns", new ModInfo { name = "GorillaPronouns", legal = true } },
            { "gfaces", new ModInfo { name = "GorillaFaces", legal = true } },
            { "pmversion", new ModInfo { name = "PlayerModels", legal = true } },
            { "gtrials", new ModInfo { name = "GorillaTrial", legal = true } },
            { "msp", new ModInfo { name = "MonkeSmartphone", legal = true } },
            { "gorillastats", new ModInfo { name = "GorillaStats", legal = true } },
            { "using gorilladrift", new ModInfo { name = "GorillaDrift", legal = true } },
            { "monkehavocversion", new ModInfo { name = "MonkeHavoc", legal = true } },
            { "tictactoe", new ModInfo { name = "TicTacToe", legal = true } },
            { "ccolor", new ModInfo { name = "Index", legal = true } },
            { "imposter", new ModInfo { name = "GorillaAmongUs", legal = true } },
            { "spectapeversion", new ModInfo { name = "Spectape", legal = true } },
            { "cats", new ModInfo { name = "Cats", legal = false } },
            { "made by biotest05 :3", new ModInfo { name = "Dogs", legal = false } },
            { "fys cool magic mod", new ModInfo { name = "FYSMagicMod", legal = false } },
            { "chainedtogether", new ModInfo { name = "Chained Together", legal = true } },
            { "ChainedTogetherActive", new ModInfo { name = "Chained Together", legal = true } },
            { "goofywalkversion", new ModInfo { name = "Goofy Walk", legal = true } },
            { "void_menu_open", new ModInfo { name = "Void", legal = false } },
            { "obsidianmc", new ModInfo { name = "Obsidian.lol", legal = false } },
            { "dark", new ModInfo { name = "ShibaGT Dark", legal = false } },
            { "hidden", new ModInfo { name = "Hidden Menu", legal = false } },
            { "oblivionuser", new ModInfo { name = "Oblivion", legal = false } },
            { "hgrehngio889584739_hugb\n", new ModInfo { name = "Resurgence", legal = false } },
            { "hgrehngio889584739_hugb", new ModInfo { name = "Resurgence", legal = false } },
            { "eyerock reborn", new ModInfo { name = "Eyerock Reborn", legal = false } },
            { "asteroidlite", new ModInfo { name = "Asteroid Lite", legal = false } },
            { "elux", new ModInfo { name = "Elux", legal = false } },
            { "cokecosmetics", new ModInfo { name = "Coke Cosmetx", legal = false } },
            { "GFaces", new ModInfo { name = "G Faces", legal = false } },
            { "github.com/maroon-shadow/SimpleBoards", new ModInfo { name = "Simple Boards", legal = true } },
            {
                "github.com/ZlothY29IQ/GorillaMediaDisplay",
                new ModInfo { name = "Gorilla Media Display", legal = true }
            },
            { "github.com/ZlothY29IQ/TooMuchInfo", new ModInfo { name = "Too Much Info", legal = false } },
            { "github.com/ZlothY29IQ/RoomUtils-IW", new ModInfo { name = "Room Utils IW", legal = true } },
            { "github.com/ZlothY29IQ/MonkeClick", new ModInfo { name = "Monke Click", legal = true } },
            { "github.com/ZlothY29IQ/MonkeClick-CI", new ModInfo { name = "Monke Click CI", legal = true } },
            { "github.com/ZlothY29IQ/MonkeRealism", new ModInfo { name = "Monke Realism", legal = true } },
            { "github.com/ZlothY29IQ/Zloth-RecRoomRig", new ModInfo { name = "Zloth Rec Room Rig", legal = true } },
            { "MediaPad", new ModInfo { name = "Media Pad", legal = true } },
            { "GorillaCinema", new ModInfo { name = "Gorilla Cinema", legal = true } },
            { "CSVersion", new ModInfo { name = "Custom Skin", legal = true } },
            { "ShirtProperties", new ModInfo { name = "GorillaShirts Legacy", legal = true } },
            { "GS", new ModInfo { name = "GorillaShirts Legacy", legal = true } },
            { "Body Tracking", new ModInfo { name = "Body Track Old", legal = true } },
            { "Body Estimation", new ModInfo { name = "Han Body Est", legal = true } },
            { "Gorilla Track", new ModInfo { name = "Body Track", legal = true } },
            { "CustomMaterial", new ModInfo { name = "Custom Cosmetics", legal = true } },
            { "I like cheese", new ModInfo { name = "Rec Room Rig", legal = true } },
            { "silliness", new ModInfo { name = "Silliness", legal = false } },
            { "EmoteWheel", new ModInfo { name = "Fortnite Emote Wheel", legal = false } },
            { "untitled", new ModInfo { name = "Untitled", legal = false } },
            { "BoyDoILoveInformation Public", new ModInfo { name = "BoyDoILoveInformation", legal = true } },
            { "DTAOI", new ModInfo { name = "DTAOI", legal = false } },
            { "DTASLOI", new ModInfo { name = "DTASLOI", legal = false } },
            { "GorillaShop", new ModInfo { name = "GorillaShop", legal = false } },
            { "Fusioned", new ModInfo { name = "Fusioned", legal = false } },
            { "y u lookin in here weirdo", new ModInfo { name = "Malachi Menu Reborn", legal = false } },
            { "ØƦƁƖƬ", new ModInfo { name = "Orbit", legal = false } },
            { "Atlas", new ModInfo { name = "Atlas", legal = false } },
            { "kingbingus.oculusreportmenu", new ModInfo { name = "Report Menu", legal = true } },
            { "FYS Epic Face Tracking Shit Mod", new ModInfo { name = "Eye Tracking", legal = true } },
            { "AceUtilsPad", new ModInfo { name = "Ace Utils Pad", legal = true } },
            { "Silly.Net", new ModInfo { name = "Silly.Net", legal = false } },
            { "BarkSillyVersion", new ModInfo { name = "Silly Bark", legal = false } },
            { "BarkVersion", new ModInfo { name = "Bark", legal = true } },
            { "CFA", new ModInfo { name = "Console For All", legal = true } },
            { "Juul_V", new ModInfo { name = "Juul", legal = false } },
            { "Haze_V", new ModInfo { name = "Haze", legal = false } },
            { "Grate", new ModInfo { name = "Mist", legal = false } },
            { "Gemstone", new ModInfo { name = "Gemstone", legal = false } },
            { "Nebular Paid", new ModInfo { name = "Nebular", legal = false } },
            { "Deez's GorillaMedia", new ModInfo { name = "Deez GMedia", legal = true } },
            { "Hose Nametags", new ModInfo { name = "Hose Nametags", legal = true } },
            { "Track Track Track Sahur", new ModInfo { name = "Track Track", legal = false } },
            { "UsingWraith", new ModInfo { name = "Wraith", legal = false } },
            { "Chud menu", new ModInfo { name = "Chud Menu", legal = false } },
            { "LagMenu", new ModInfo { name = "Lag Menu", legal = false } }
        };

        public static List<ModInfo> GetDetectedMods(Hashtable customProperties)
        {
            List<ModInfo> detected = new List<ModInfo>();

            if (customProperties == null)
                return detected;

            foreach (object key in customProperties.Keys)
            {
                if (key is string propertyKey && modDictionary.TryGetValue(propertyKey, out ModInfo info))
                    detected.Add(info);
            }

            return detected;
        }
    }
}