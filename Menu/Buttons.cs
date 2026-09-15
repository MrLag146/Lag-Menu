using GorillaLocomotion;
using LagMenu.Mods;
using LagMenu.Mods.ConsoleAssets;
using LagMenu.Notifications;
using LagMenu.Patches;
using LagMenu.Utilities;
using Photon.Pun;
using UnityEngine;


namespace LagMenu.Menu
{
    public static class Buttons
    {
        public static ButtonInfo[][] buttons =
        {
            // Category 0 — Main
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Settings", isTogglable = false, method = () => { Main.category = 1; Main.page = 0; } },
                new ButtonInfo { buttonText = "Movement", isTogglable = false, method = () => { Main.category = 2; Main.page = 0; } },
                new ButtonInfo { buttonText = "Rig", isTogglable = false, method = () => { Main.category = 3; Main.page = 0; } },
                new ButtonInfo { buttonText = "Room", isTogglable = false, method = () => { Main.category = 4; Main.page = 0; } },
                new ButtonInfo { buttonText = "Advantage", isTogglable = false, method = () => { Main.category = 5; Main.page = 0; } },
                new ButtonInfo { buttonText = "Visual", isTogglable = false, method = () => { Main.category = 6; Main.page = 0; } },
                new ButtonInfo { buttonText = "Saftey", isTogglable = false, method = () => { Main.category = 7; Main.page = 0; } },
                new ButtonInfo { buttonText = "World", isTogglable = false, method = () => { Main.category = 8; Main.page = 0; } },
                new ButtonInfo { buttonText = "OP", isTogglable = false, method = () => { Main.category = 9; Main.page = 0; } },
                 new ButtonInfo { buttonText = "Virtual Stump", isTogglable = false, method = () => { Main.category = 20; Main.page = 0; } },
                 new ButtonInfo { buttonText = "Master", isTogglable = false, method = () => { Main.category = 17; Main.page = 0; } },
                new ButtonInfo { buttonText = "Block", isTogglable = false, method = () => { Main.category = 10; Main.page = 0; } },
                new ButtonInfo { buttonText = "Experimental", isTogglable = false, method = () => { Main.category = 12; Main.page = 0; } },
                new ButtonInfo { buttonText = "Admin", isTogglable = false, method = () => { if (Admin.IsAdmin()) { Main.category = 13; Main.page = 0; } }, shouldShow = () => Admin.IsAdmin() },
                new ButtonInfo { buttonText = "Soundboard", isTogglable = false, method = () => { Main.category = 14; Main.page = 0; } },
                  
                 new ButtonInfo { buttonText = "Networking", isTogglable = false, method = () => { Main.category = 21; Main.page = 0; } },
                 new ButtonInfo { buttonText = "Players", isTogglable = false, method = () => Main.OpenPlayers() },
                 new ButtonInfo { buttonText = "Beta", isTogglable = false, method = () => { Main.category = 22; Main.page = 0; }, shouldShow = () => Settings.betaUnlocked },
                 new ButtonInfo { buttonText = "Detected", isTogglable = false, method = () => { Main.category = 16; Main.page = 0; } },
                 new ButtonInfo { buttonText = "Credits", isTogglable = false, method = () => { Main.category = 19; Main.page = 0; } },




            },

            // Category 1 — Settings
         new ButtonInfo[]
{
    new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
        new ButtonInfo { buttonText = "Save Preference", isTogglable = false, method = () => ModPresets.SaveAll() },
    new ButtonInfo { buttonText = "Load Preference", isTogglable = false, method = () => ModPresets.LoadAll(), shouldShow = () => ModPresets.HasSave },
    new ButtonInfo { buttonText = "Fly Speed +", isTogglable = false, method = () => Settings.flyspeed += 1f },
    new ButtonInfo { buttonText = "Fly Speed -", isTogglable = false, method = () => Settings.flyspeed = Mathf.Max(1f, Settings.flyspeed - 1f) },
     new ButtonInfo { buttonText = "Change Gun Style", isTogglable = false, method = () => GunLib.CycleGunStyle()},
   //  new ButtonInfo { buttonText = "Barrel Fling Mode +", isTogglable = false, method = () => OverPowered.IncreaseBarrelFlingMode() },
// new ButtonInfo { buttonText = "Barrel Fling Mode -", isTogglable = false, method = () => OverPowered.barrelFlingMode = Mathf.Max(0, OverPowered.barrelFlingMode - 1) },
    new ButtonInfo { buttonText = "Button Sound: " + Settings.ActiveButtonSound, isTogglable = false, method = () => { Settings.ToggleButtonSound(); foreach (var b in buttons[1]) if (b.buttonText.StartsWith("Button Sound:")) b.buttonText = "Button Sound: " + Settings.ActiveButtonSound; } },
    new ButtonInfo { buttonText = "Theme: Default", isTogglable = false, method = () => { ThemeChanger.CycleNext(); foreach (var b in buttons[1]) { if (b.buttonText.StartsWith("Theme:")) { b.buttonText = "Theme: " + ThemeChanger.CurrentName; break; } } } },
    new ButtonInfo { buttonText = "Prev Theme", isTogglable = false, method = () => { ThemeChanger.CyclePrev(); foreach (var b in buttons[1]) { if (b.buttonText.StartsWith("Theme:")) { b.buttonText = "Theme: " + ThemeChanger.CurrentName; break; } } } },
    new ButtonInfo { buttonText = "Button Click Animations", isTogglable = true, enabled = Main.ButtonClickAnimations, method = () => Main.ButtonClickAnimations = !Main.ButtonClickAnimations },
    new ButtonInfo { buttonText = "Animate Menu", isTogglable = true, enabled = Main.useShrinkClose, enableMethod = () => Main.useShrinkClose = true, disableMethod = () => Main.useShrinkClose = false },
    new ButtonInfo { buttonText = "Animated Title", isTogglable = true, enabled = Main.animatedTitle, enableMethod = () => Main.animatedTitle = true, disableMethod = () => Main.animatedTitle = false },

    new ButtonInfo { buttonText = "Menu Size +", isTogglable = false, method = () => { Settings.menuScale += 0.1f; Main.RefreshMenuScale(); } },
    new ButtonInfo { buttonText = "Menu Size -", isTogglable = false, method = () => { Settings.menuScale = Mathf.Max(0.3f, Settings.menuScale - 0.1f); Main.RefreshMenuScale(); } },
    new ButtonInfo { buttonText = "Menu Distance +", isTogglable = false, method = () => Settings.menuDistance += 0.1f },
    new ButtonInfo { buttonText = "Menu Distance -", isTogglable = false, method = () => Settings.menuDistance = Mathf.Max(0.2f, Settings.menuDistance - 0.1f) },
    new ButtonInfo { buttonText = "Text Size +", isTogglable = false, method = () => { Settings.textScale += 0.1f; Main.RecreateMenu(); } },
    new ButtonInfo { buttonText = "Text Size -", isTogglable = false, method = () => { Settings.textScale = Mathf.Max(0.3f, Settings.textScale - 0.1f); Main.RecreateMenu(); } },
    new ButtonInfo { buttonText = "Mirror Text", isTogglable = true, enabled = Settings.mirrorText, enableMethod = () => { Settings.mirrorText = true; Main.RecreateMenu(); }, disableMethod = () => { Settings.mirrorText = false; Main.RecreateMenu(); } },
    new ButtonInfo { buttonText = "Show Home Button", isTogglable = true, enabled = Settings.showHome, enableMethod = () => { Settings.showHome = true; Main.RecreateMenu(); }, disableMethod = () => { Settings.showHome = false; Main.RecreateMenu(); } },
    new ButtonInfo { buttonText = "Show Disconnect Button", isTogglable = true, enabled = Settings.showDisconnect, enableMethod = () => { Settings.showDisconnect = true; Main.RecreateMenu(); }, disableMethod = () => { Settings.showDisconnect = false; Main.RecreateMenu(); } },

},

            

            // Category 2 — Movement
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "WASD Fly", enableMethod = Movement.WASDFly, isTogglable = true, method = Movement.WASDFly, disableMethod =() => GTPlayer.Instance.GetControllerTransform(false).parent.rotation = Quaternion.Euler(0, 0, 0), },
                new ButtonInfo { buttonText = "Fly", isTogglable = true, method = () => Movement.Fly(Settings.flyspeed) },
                new ButtonInfo { buttonText = "MrLag Fly", isTogglable = true, method = () => Movement.LJFly(Settings.flyspeed) },
                new ButtonInfo { buttonText = "Plats", isTogglable = true, method = () => Movement.Platforms() },
                new ButtonInfo { buttonText = "BarkFly", isTogglable = true, method = () => Movement.BarkFly() },
                new ButtonInfo { buttonText = "Noclip", isTogglable = true, method = () => Movement.NoClip() },
                new ButtonInfo { buttonText = "Mr Lag Styled Noclip", isTogglable = true, method = () => Movement.NoClipRJ() },
                new ButtonInfo { buttonText = "PC Button Click", isTogglable = true, method = () => Movement.PCButtonClickGun() },
                new ButtonInfo { buttonText = "PC Walking", isTogglable = true, enableMethod = () => Movement.EnablePCWalking(), method = () => Movement.PCWalking(), disableMethod = () => Movement.DisablePCWalking() },
                new ButtonInfo { buttonText = "Long Arms", isTogglable = true, enableMethod = () => Movement.SteamLongArms(), disableMethod = () => Movement.NormalArms() },
                new ButtonInfo { buttonText = "Uncap Max Velocity", isTogglable = false, method = () => Movement.UncapMaxVelocity() },
                new ButtonInfo { buttonText = "Speed Boost", isTogglable = true, enableMethod = () => Movement.SpeedBoost(), disableMethod = () => Movement.OffSpeedBoost() },
                new ButtonInfo { buttonText = "Wall Walk", isTogglable = true, method = () => Movement.WallWalk() },
                new ButtonInfo { buttonText = "Spiderwalk", isTogglable = true, method = () => Movement.SpiderWalk(), disableMethod = () => Movement.SpiderWalkOff() },
                new ButtonInfo { buttonText = "Pull Mod", isTogglable = true, method = () => Movement.PullMod() },
                new ButtonInfo { buttonText = "Teleport Gun", isTogglable = true, method = () => Movement.TeleportGun() },
                new ButtonInfo { buttonText = "Tp To Stump", isTogglable = false, method = () => Movement.TpToStump() },
                new ButtonInfo { buttonText = "Hand Fly", isTogglable = true, method = () => Movement.HandFly() },
                new ButtonInfo { buttonText = "Body Fly", isTogglable = true, method = () => Movement.BodyFly() },
                new ButtonInfo { buttonText = "Slingshot Fly", isTogglable = true, method = () => Movement.SlingshotFly() },
                new ButtonInfo { buttonText = "Joystick Fly", isTogglable = true, method = () => Movement.JoystickFly() },
                new ButtonInfo { buttonText = "Noclip Fly", isTogglable = true, method = () => Movement.NoClipFly(), disableMethod = () => Movement.NoClipFlyOff() },
                new ButtonInfo { buttonText = "Iron Monkey", isTogglable = true, method = () => Movement.IronMonkey() },
                new ButtonInfo { buttonText = "Banana Car", isTogglable = true, method = () => Movement.BananaCar() },
                new ButtonInfo { buttonText = "Dash", isTogglable = true, method = () => Movement.DashMod() },
                new ButtonInfo { buttonText = "Strafe", isTogglable = true, method = () => Movement.Strafe() },
                new ButtonInfo { buttonText = "Bouncy Map", isTogglable = true, enableMethod = () => Movement.Bouncy(), disableMethod = () => Movement.BouncyOff() },
                new ButtonInfo { buttonText = "Air Swim", isTogglable = true, method = () => Movement.AirSwim(), disableMethod = () => Movement.AirSwimOff() },
                new ButtonInfo { buttonText = "Size Changer", isTogglable = true, method = () => Movement.SizeChanger(), disableMethod = () => Movement.DisableSizeChanger() },
                new ButtonInfo { buttonText = "Punch Mod", isTogglable = true, method = () => Movement.PunchMod() },
                new ButtonInfo { buttonText = "Grab Mod", isTogglable = true, method = () => Movement.GrabMod() },
            },

            // Category 3 — Rig
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                 new ButtonInfo { buttonText = "Ghost", isTogglable = true, method = () => RIg.GhostMonkey(), disableMethod = () => RIg.EnableRig() },
                 new ButtonInfo { buttonText = "Invis", isTogglable = true, method = () => RIg.InvisibleMonke(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Jumpscare Gun", isTogglable = true, method = () => RIg.JumpscareGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Sex Gun", isTogglable = true, method = () => RIg.SexGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Orbit Player Gun", isTogglable = true, method = () => RIg.OrbitPlayerGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Mirror Player Gun", isTogglable = true, method = () => RIg.MirrorGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Shadow Clone Gun", isTogglable = true, method = () => RIg.ShadowCloneGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Follow Player Gun", isTogglable = true, method = () => RIg.FollowGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Upside Down Rig", isTogglable = true, method = () => RIg.UpsideDownHead() },
                new ButtonInfo { buttonText = "Spin Rig X", isTogglable = true, method = () => RIg.VRRigLateUpdate_SpinX() },
                 new ButtonInfo { buttonText = "Spin Rig Y", isTogglable = true, method = () => RIg.VRRigLateUpdate_SpinY() },
                 new ButtonInfo { buttonText = "Spin Rig Z", isTogglable = true, method = () => RIg.VRRigLateUpdate_SpinZ() },
                new ButtonInfo { buttonText = "Michael Jackson", isTogglable = true, method = () => RIg.TiltForward(), disableMethod = () => RIg.UnTiltForward() },
                new ButtonInfo { buttonText = "Stare At Closest", isTogglable = true, method = () => RIg.StareAtClosestPlayer() },
                new ButtonInfo { buttonText = "PC Rig", isTogglable = true, method = () => RIg.PCRig(), enableMethod = () => RIg.OnEnable(), disableMethod = () => RIg.OnDisable() },

                new ButtonInfo { buttonText = "Rig Gun", isTogglable = true, method = () => RIg.RigGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Spaz Rig", isTogglable = true, method = () => RIg.SpazRig(), disableMethod = () => RIg.DisableSpazRig() },
                new ButtonInfo { buttonText = "Spaz Hands", isTogglable = true, method = () => RIg.SpazHands(), disableMethod = () => RIg.DisableSpazHands() },
                new ButtonInfo { buttonText = "Amputate Rig", isTogglable = true, method = () => RIg.AmputateRig(), disableMethod = () => RIg.DisableAmputateRig() },
                new ButtonInfo { buttonText = "Piggyback Gun", isTogglable = true, method = () => RIg.PiggybackGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Orbit All Players", isTogglable = true, method = () => RIg.OrbitAllPlayers(), disableMethod = () => RIg.DisableOrbitAllPlayers() },
                new ButtonInfo { buttonText = "Jumpscare All", isTogglable = true, method = () => RIg.JumpscareAll(), disableMethod = () => RIg.DisableJumpscareAll() },
                new ButtonInfo { buttonText = "Overstimulate Gun", isTogglable = true, method = () => RIg.OverstimulateGun(), disableMethod = () => RIg.DisableAnnoyAll() },
                new ButtonInfo { buttonText = "Overstimulate All", isTogglable = true, method = () => RIg.OverstimulateAll(), disableMethod = () => RIg.DisableOverstimulateAll() },
                new ButtonInfo { buttonText = "Annoy Gun", isTogglable = true, method = () => RIg.AnnoyGun(), disableMethod = () => RIg.EnableRig() },
                new ButtonInfo { buttonText = "Annoy All", isTogglable = true, method = () => RIg.AnnoyAll(), disableMethod = () => RIg.DisableAnnoyAll() }


            },

            // Category 4 — Room
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Disconnect", isTogglable = false, method = () => PhotonNetwork.Disconnect() },
                new ButtonInfo { buttonText = "Reconnect", isTogglable = false, method = () => Room.Reconnect() },
                  new ButtonInfo { buttonText = "Join LAG12", isTogglable = false, method = () => Room.JoinLag12() },
                  new ButtonInfo { buttonText = "Join Random", isTogglable = false, method = () => Room.JoinRandom() },
                  new ButtonInfo { buttonText = "Create LagMenu Lobby", isTogglable = false, method = () => OverPowered.CreateLobby("LagMenu", "Menu"), },
                  new ButtonInfo { buttonText = "Disable Network Triggers", isTogglable = true, enableMethod = () => Settings.DisableNetworkTriggers(true), disableMethod = () => Settings.DisableNetworkTriggers(false) },
            },

            // Category 5 — Advantage
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Instant Tag All", isTogglable = false, method = () => Advantage.InstantTagAll() },
                new ButtonInfo { buttonText = "Instant Tag Gun", isTogglable = true, method = () => Advantage.TagGun() },
                new ButtonInfo { buttonText = "Tag Self", isTogglable = true, method = () => Advantage.TagSelf() },


                new ButtonInfo { buttonText = "No Tag Freeze", isTogglable = true, method = () => Advantage.NoTagFreeze(), disableMethod = () => Advantage.EnableTagFreeze() },
                new ButtonInfo { buttonText = "No Tag On Join", isTogglable = true, method = () => Advantage.NoTagOnJoin() },
                new ButtonInfo { buttonText = "PB Kill Gun", isTogglable = true, method = () => Advantage.KillGun() },
                new ButtonInfo { buttonText = "PB Kill All", isTogglable = true, method = () => Advantage.KillAll() },
                new ButtonInfo { buttonText = "Always Guardian", isTogglable = true, method = () => Advantage.AlwaysGuardian() },
                 new ButtonInfo { buttonText = "Guardian Protecter", isTogglable = true, method = () => Advantage.GuardianProtector() },
                new ButtonInfo { buttonText = "Guardian Fling Gun", isTogglable = true, method = () => Advantage.GuardianFlingGun() },
                new ButtonInfo { buttonText = "Guardian Fling All", isTogglable = true, method = () => Advantage.GuardianFlingAll() },
                new ButtonInfo { buttonText = "Guardian Orbit All", isTogglable = true, method = () => Advantage.OrbitAll() },
                new ButtonInfo { buttonText = "Guardian Orbit Gun", isTogglable = true, method = () => Advantage.OrbitGun() },
                new ButtonInfo { buttonText = "Guardian Freeze Gun", isTogglable = true, method = () => Advantage.FreezeGunFling() },
                new ButtonInfo { buttonText = "Guardian Fly Gun", isTogglable = true, method = () => Advantage.GuardianFlyGun() }
                // new ButtonInfo { buttonText = "PaintBrawl Aimbot", isTogglable = true, method = () => Advantage.Aimbot(), disableMethod = () => Advantage.AimbotOff() },

            },
// Category 6 — Visual
new ButtonInfo[]
{
    new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
    new ButtonInfo { buttonText = "Tracers", isTogglable = true, method = () => Visuals.Tracers() },
    new ButtonInfo { buttonText = "Disappear Gun", isTogglable = true, method = () => Visuals.CSInvisGun() },
     new ButtonInfo { buttonText = "Shiny Rocks", isTogglable = true, method = () => Visuals.GiveUnlimitedShinyRocks() },
    new ButtonInfo { buttonText = "FPC", isTogglable = true, enableMethod = () => Visuals.EnableFPC(), disableMethod = () => Visuals.DisableFPC() },
    new ButtonInfo { buttonText = "Nametags", isTogglable = true, method = () => Visuals.Nametags(), disableMethod = () => Visuals.NametagsDisable() },
    new ButtonInfo { buttonText = "LagMenu Tags", isTogglable = true, enabled = true, method = () => Visuals.LagMenuUserTags(), disableMethod = () => Visuals.LagMenuUserTagsDisable() },
    new ButtonInfo { buttonText = "Grip Indicators", isTogglable = true, enabled = true, method = () => Visuals.GripESP(), disableMethod = () => Visuals.DisableGripESP() },
         new ButtonInfo { buttonText = "Morning", isTogglable = false, method =() => BetterDayNightManager.instance.SetTimeOfDay(1),},
         new ButtonInfo { buttonText = "Day", isTogglable = false, method =() => BetterDayNightManager.instance.SetTimeOfDay(3),},
        new ButtonInfo { buttonText = "Evening", isTogglable = false, method =() => BetterDayNightManager.instance.SetTimeOfDay(7)},
         new ButtonInfo { buttonText = "Night", isTogglable = false, method =() => BetterDayNightManager.instance.SetTimeOfDay(0)},
          new ButtonInfo { buttonText = "Fake Unban Self", isTogglable = false, method =() =>Visuals.FakeUnbanSelf() },
                new ButtonInfo { buttonText = "Chams", isTogglable = true, method = () => Visuals.Chams(), disableMethod = () => Visuals.ChamsOff() },
                new ButtonInfo { buttonText = "144 FPS", isTogglable = false, method = () => Settings.SetFPS(144) },
                new ButtonInfo { buttonText = "90 FPS", isTogglable = false, method = () => Settings.SetFPS(90) },
                new ButtonInfo { buttonText = "60 FPS", isTogglable = false, method = () => Settings.SetFPS(60) },
                new ButtonInfo { buttonText = "30 FPS", isTogglable = false, method = () => Settings.SetFPS(30) },
                new ButtonInfo { buttonText = "Unlock FPS", isTogglable = false, method = () => Settings.UnlockFPS() },
},

            // Category 7 — Safety
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Anti Report", isTogglable = true, method = () => Saftey.AntiReportDisconnect() },
                new ButtonInfo { buttonText = "Anti Notification", isTogglable = true, method = () => Saftey.AntiReportNotify() },
                new ButtonInfo { buttonText = "Anti RPC Kick", isTogglable = true, method = () => Saftey.AntiRPCKick() },
                new ButtonInfo { buttonText = "Name Spoof", isTogglable = false, method = () => Saftey.NameSpoof() },
                new ButtonInfo { buttonText = "Color Spoof", isTogglable = false, method = () => Saftey.ColorSpoof() },
                new ButtonInfo { buttonText = "Anti Report Murder(VStump)", isTogglable = true, method = () => Saftey.AntiReportKill() },
                new ButtonInfo { buttonText = "Anti AFK", isTogglable = true, enableMethod = () => Saftey.EnableAntiAFK(), disableMethod = () => Saftey.DisableAntiAFK() },
                new ButtonInfo { buttonText = "Warn On Steam User Join", isTogglable = true, method = () => Saftey.WarnOnSteamUserJoin(), disableMethod = () => Saftey.DisableWarnOnSteamUserJoin() },
            },
// Category 8 — World
new ButtonInfo[]
{
    new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
    new ButtonInfo { buttonText = "Bug Gun", isTogglable = true, method = () => World.BugGun() },
    new ButtonInfo { buttonText = "Become Bug", isTogglable = true, method = () => World.BecomeBug() },
    new ButtonInfo { buttonText = "Board Gun", isTogglable = true, method = () => World.HoverboardGun() },
    new ButtonInfo { buttonText = "Shower Gun", isTogglable = true, method = () => World.ShowerGun() },
    new ButtonInfo { buttonText = "Spaz Rope Gun", isTogglable = true, method = () => World.RopeGun() },
    new ButtonInfo { buttonText = "Spaz All Ropes", isTogglable = true, method = () => World.SpazAllRopes() },
    new ButtonInfo { buttonText = "Spawn Hoverboard", isTogglable = false, method = () => World.SpawnHoverboard() },
    new ButtonInfo { buttonText = "Stump TP Spam", isTogglable = true, method = () => World.StumpTeleporterEffectSpam() },
    new ButtonInfo { buttonText = "Arcade TP Spam", isTogglable = true, method = () => World.ArcadeTeleporterEffectSpam() },
    new ButtonInfo { buttonText = "Buy Barrel", isTogglable = false, method = () => OverPowered.BuyBarrel() },
new ButtonInfo { buttonText = "Strip Cosmetics", isTogglable = false, method = () => GorillaTagger.Instance.StartCoroutine(OverPowered.q(0)) },
new ButtonInfo { buttonText = "Cosmetic Slideshow", isTogglable = false, method = () => GorillaTagger.Instance.StartCoroutine(OverPowered.q(1)) },
new ButtonInfo { buttonText = "Try All Cosmetics", isTogglable = false, method = () => GorillaTagger.Instance.StartCoroutine(OverPowered.q(2)) },
new ButtonInfo { buttonText = "Reset Cosmetics", isTogglable = false, method = () => GorillaTagger.Instance.StartCoroutine(OverPowered.q(-1)) },
new ButtonInfo { buttonText = "Stack Cosemtics", isTogglable = true, method = () => World.StackCosmetics() },
new ButtonInfo { buttonText = "Kick Modders", isTogglable = true, method = () => World.KickModders() },
new ButtonInfo { buttonText = "Unlock VIM", isTogglable = true, method = () => SubscriptionPatches.enabled = true, disableMethod = () => SubscriptionPatches.enabled = false, },
new ButtonInfo { buttonText = "ShopLift", isTogglable = true, method = () => World.CosmeticsSpoof(), disableMethod = () =>  World.UnCosmeticsSpoof(), },
new ButtonInfo { buttonText = "RGB Monke", isTogglable = true, method = () => World.RGBMonkey() },
new ButtonInfo { buttonText = "Disable Network Triggers", isTogglable = true, method = () => World.DisableNetworkTriggers(), disableMethod = () =>  World.EnableNetworkTriggers(), },
new ButtonInfo { buttonText = "Bracelet Spam", isTogglable = true, method = () => World.BraceletSpam(), disableMethod = () => World.RemoveBracelet() },
new ButtonInfo { buttonText = "Rainbow Bracelet", isTogglable = true, method = () => World.RainbowBracelet(), disableMethod = () => World.RemoveRainbowBracelet() },
new ButtonInfo { buttonText = "Hoverboard Screen Gun", isTogglable = true, method = () => World.HoverboardScreenGun() },
new ButtonInfo { buttonText = "Hoverboard Screen All", isTogglable = false, method = () => World.HoverboardScreenAll() },
new ButtonInfo { buttonText = "Become Hoverboard", isTogglable = true, method = () => World.BecomeHoverboard(), disableMethod = () => World.DisableBecomeHoverboard() },
new ButtonInfo { buttonText = "Animated Name", isTogglable = true, method = () => World.AnimatedName(), disableMethod = () => World.DisableAnimatedName() },
new ButtonInfo { buttonText = "Copy Identity Gun", isTogglable = true, method = () => World.CopyIdentityGun() },
new ButtonInfo { buttonText = "Name Cycle", isTogglable = true, method = () => World.RandomNameCycle() },
new ButtonInfo { buttonText = "Flash Nametag", isTogglable = true, method = () => World.FlashNameTag(), disableMethod = () => World.DisableFlashNameTag() },
new ButtonInfo { buttonText = "Laggy Mic", isTogglable = true, enableMethod = () => World.LaggyMic(true), disableMethod = () => World.LaggyMic(false) },
new ButtonInfo { buttonText = "Glitchy Mic", isTogglable = true, enableMethod = () => World.GlitchyMic(true), disableMethod = () => World.GlitchyMic(false) },
new ButtonInfo { buttonText = "Echo Mic", isTogglable = true, enableMethod = () => World.EchoMic(true), disableMethod = () => World.EchoMic(false) },
new ButtonInfo { buttonText = "Mute Mic", isTogglable = true, enableMethod = () => World.MuteMic(true), disableMethod = () => World.MuteMic(false) },
},

            // Category 9 — OP
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
               new ButtonInfo { buttonText = "Deafen All", isTogglable = true, method = () => OverPowered.DeafenAll() },
                new ButtonInfo { buttonText = "Deafen Gun", isTogglable = true, method = () => OverPowered.DeafenGun() },
                new ButtonInfo { buttonText = "Lag Gun", isTogglable = true, method = () => OverPowered.LagGunMethod2() },
                new ButtonInfo { buttonText = "Lag Gun(Meth 2)", isTogglable = true, method = () => OverPowered.LagGunMethod2() },
                new ButtonInfo { buttonText = "Unlock All Gadgets", isTogglable = true, method = () => OverPowered.SIUnlockAll() },
                new ButtonInfo { buttonText = "Unlock All Resources", isTogglable = true, method = () => OverPowered.GiveAllResources() },
                new ButtonInfo { buttonText = "Complete Quests", isTogglable = true, method = () => OverPowered.CompleteAllQuests() },
                new ButtonInfo { buttonText = "Always Own Terms", isTogglable = true, method = () => OverPowered.AlwaysOwnTerminals() },
                new ButtonInfo { buttonText = "No Terminal Timeout", isTogglable = true, method = () => OverPowered.DisableTerminalTimeout() },
                new ButtonInfo { buttonText = "DestroyAll", isTogglable = false, method = () => OverPowered.DestroyAll() }, 
                // new ButtonInfo { buttonText = "Barrel Fling Gun", isTogglable = true, method = () => OverPowered.BarrelFlingGun() },
                //   new ButtonInfo { buttonText = "Give Barrel Fly Gun", isTogglable = true, method = () => OverPowered.GiveFlyWithBarrel() },
                //  new ButtonInfo { buttonText = "Barrel Fling All", isTogglable = true, method = () => OverPowered.BarrelFlingAll() },
                //   new ButtonInfo { buttonText = "Barrel Fling Anti Report", isTogglable = true, method = () => OverPowered.BarrelFlingAntiReport() },
                //  new ButtonInfo { buttonText = "Barrel Punch Mod", isTogglable = true, method = () => OverPowered.BarrelPunchMod() },
              
                new ButtonInfo { buttonText = "Always Grab Ownership", isTogglable = true, enableMethod = () => OverPowered.AlwaysGrabOwnership = true, disableMethod = () => OverPowered.AlwaysGrabOwnership = false },
               
            },

            // Category 10 — Block
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Block Gun", isTogglable = true, method = () => BlockMods.BlockGun() },
                new ButtonInfo { buttonText = "Sphere Gun", isTogglable = true, method = () => BlockMods.SphereGun() },
                new ButtonInfo { buttonText = "Destroy Block", isTogglable = true, method = () => BlockMods.DestroyBlockGun() },
                new ButtonInfo { buttonText = "Crash Gun", isTogglable = true, method = () => BlockMods.CrashGun() },
                new ButtonInfo { buttonText = "Lag Gun", isTogglable = true, method = () => BlockMods.LagGun() },
                new ButtonInfo { buttonText = "Fling Gun", isTogglable = true, method = () => BlockMods.FlingGun() },
                new ButtonInfo { buttonText = "Block Spam", isTogglable = true, method = () => BlockMods.BlockSpam() },
            },

            // Category 11 — Place Holder
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                
            },

            // Category 12 — Experimental
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Object Info Gun", isTogglable = true, method = () => Experimental.CopyObjectInfoGun() },
                new ButtonInfo { buttonText = "Spam Props", isTogglable = false, method = () => Experimental.CustomPropExpander() },
                new ButtonInfo { buttonText = "Copy Self ID", isTogglable = false, method = () => Experimental.CopySelfID() },
            },

            // Category 13 — Admin
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
new ButtonInfo { buttonText = "<color=#4B0082>C</color><color=#3A3A3A>o</color><color=#8B0000>n</color><color=#4B0082>s</color><color=#3A3A3A>o</color><color=#8B0000>l</color><color=#4B0082>e</color><color=#3A3A3A> </color><color=#8B0000>A</color><color=#4B0082>s</color><color=#3A3A3A>s</color><color=#8B0000>e</color><color=#4B0082>t</color><color=#3A3A3A>s</color>", isTogglable = false, method = () => { Main.category = 15; Main.page = 0; } },
                new ButtonInfo { buttonText = "Punch Power +", isTogglable = true, method = () => Admin.CycleNextPunch() },
                new ButtonInfo { buttonText = "Punch Power -", isTogglable = true, method = () => Admin.CyclePrevPunch() },
                new ButtonInfo { buttonText = "Teleport Gun", isTogglable = true, method = () => Admin.TeleportAllGun() },
                 new ButtonInfo { buttonText = "Get Menu Users", isTogglable = false, method = () => Admin.GetMenuUsers() },
                  new ButtonInfo { buttonText = "Menu User Nametags", isTogglable = true, method = () => Admin.AdminMenuUserTags() },
                new ButtonInfo { buttonText = "Join Gun", isTogglable = true, method = () => Admin.JoinGun() },
                new ButtonInfo { buttonText = "Lag Spike All", isTogglable = false, method = () => Admin.AdminLagSpikeAll() },
                new ButtonInfo { buttonText = "Lag Spike Gun", isTogglable = true, method = () => Admin.AdminLagSpikeGun() },
                new ButtonInfo { buttonText = "Lag Gun", isTogglable = true, method = () => Admin.AdminLagGun() },
                new ButtonInfo { buttonText = "Punch Mod", isTogglable = true, method = () => Admin.AdminPunchMod() },
                 new ButtonInfo { buttonText = "Random Object Gun", isTogglable = true, method = () => Admin.AdminRandomObjectGun() },
                new ButtonInfo { buttonText = "All Orbit Self", isTogglable = true, method = () => Admin.OrbitAllUsing() },
                 new ButtonInfo { buttonText = "Fling Gun", isTogglable = true, method = () => Admin.AdminFlingGun() },
                 new ButtonInfo { buttonText = "Upside Down Head Gun", isTogglable = true, method = () => Admin.ForceUpsideDownHeadGun() },
                  new ButtonInfo { buttonText = "Admin Fake Cosmetics", isTogglable = true, method =() => NetworksCosmetx.Start(), enableMethod =() => { NetworkSystem.Instance.OnPlayerJoined += Admin.OnPlayerJoinSpoof; Admin.AdminSpoofCosmetics(true); }, disableMethod =() => { NetworkSystem.Instance.OnPlayerJoined -= Admin.OnPlayerJoinSpoof; Admin.oldCosmetics = null; }, },
                  new ButtonInfo { buttonText = "Admin Bring All", isTogglable = true, method = Admin.BringAllUsing },
                    new ButtonInfo { buttonText = "Admin Strangle", isTogglable = true, method = Admin.AdminStrangle },

            },

            // Category 14 — Soundboard
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Stop All", isTogglable = false, method = () => Soundboard.HaltAudio() },
                new ButtonInfo { buttonText = "Hear Self", isTogglable = true, enabled = Soundboard.HearSelf, enableMethod = () => Soundboard.HearSelf = true, disableMethod = () => Soundboard.HearSelf = false },
                new ButtonInfo { buttonText = "Loop Audio", isTogglable = true, enabled = Soundboard.LoopAudio, enableMethod = () => Soundboard.LoopAudio = true, disableMethod = () => Soundboard.LoopAudio = false },
                new ButtonInfo { buttonText = "Open Folder", isTogglable = false, method = () => Soundboard.RevealSoundFolder() },
                new ButtonInfo { buttonText = "Reload Sounds", isTogglable = false, method = () => { Soundboard.PopulateSounds(); Main.RecreateMenu(); } },
                new ButtonInfo { buttonText = "Loading...", isTogglable = false, method = () => { } },
            },

            // Category 15 — Admin Assets
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
               new ButtonInfo { buttonText = "Travis", isTogglable = true, enableMethod = () => AdminAssets.SpawnTravis(), disableMethod = () => AdminAssets.DestroyTravis() },
               new ButtonInfo { buttonText = "Coin Flip", isTogglable = true, enableMethod = () => CoinFlip.Update(), disableMethod = () => CoinFlip.OnDisable() },
                 new ButtonInfo { buttonText = "Skeleton", isTogglable = true, enableMethod = () => AdminAssets.SpawnSkele(), disableMethod = () => AdminAssets.DestroySkele() },
                new ButtonInfo { buttonText = "Pistol(Kick)", isTogglable = true, enableMethod = () => AdminAssets.SpawnPistol(), method = () => AdminAssets.UpdatePistol(), disableMethod = () => AdminAssets.DestroyPistol() },
                   new ButtonInfo { buttonText = "BoomBox", isTogglable = true, enableMethod = () => AdminAssets.BoomBoxEnable(), method = () => AdminAssets.BoomBoxUpdate(), disableMethod = () => AdminAssets.BoomBoxDisable() },
                 new ButtonInfo { buttonText = "Pistol(Fling)", isTogglable = true, enableMethod = () => AdminAssets.SpawnPistol(), method = () => AdminAssets.UpdatePistolVel(), disableMethod = () => AdminAssets.DestroyPistol() },
             
               new ButtonInfo { buttonText = "Burger Gun", isTogglable = true, method = () => AdminAssets.BurgerGun(), disableMethod = () => AdminAssets.BurgerGun_OnDisable() },
               new ButtonInfo { buttonText = "Rat Gun", isTogglable = true, method = () => AdminAssets.RatGun(), disableMethod = () => AdminAssets.RatGun_OnDisable() },
                new ButtonInfo { buttonText = "Jail Gun", isTogglable = true, method = () => AdminAssets.JailGun(), disableMethod = () => AdminAssets.JailGun_OnDisable() },
                new ButtonInfo { buttonText = "Mini Travis", isTogglable = true, enableMethod = () => AdminAssets.SpawnMiniTravis(), disableMethod = () => AdminAssets.DestroyMiniTravis() },
                new ButtonInfo { buttonText = "Flash Effects", isTogglable = true, enableMethod = () => AdminAssets.SpawnFlashEffects(), disableMethod = () => AdminAssets.DestroyFlashEffects() },
                new ButtonInfo { buttonText = "Flash Trail <", isTogglable = false, method = () => { AdminAssets.flashTrailIndex = (AdminAssets.flashTrailIndex - 1 + AdminAssets.EffectNames.Length) % AdminAssets.EffectNames.Length; } },
                new ButtonInfo { buttonText = "Flash Trail >", isTogglable = false, method = () => { AdminAssets.flashTrailIndex = (AdminAssets.flashTrailIndex + 1) % AdminAssets.EffectNames.Length; } },
                new ButtonInfo { buttonText = "Concert", isTogglable = true, enableMethod = () => AdminAssets.SpawnConcert(), disableMethod = () => AdminAssets.DestroyConcert() },
                new ButtonInfo { buttonText = "Concert <", isTogglable = false, method = () => { AdminAssets.concertVideoIndex = (AdminAssets.concertVideoIndex - 1 + AdminAssets.ConcertVideoNames.Length) % AdminAssets.ConcertVideoNames.Length; } },
                new ButtonInfo { buttonText = "Concert >", isTogglable = false, method = () => { AdminAssets.concertVideoIndex = (AdminAssets.concertVideoIndex + 1) % AdminAssets.ConcertVideoNames.Length; } },
                new ButtonInfo { buttonText = "RB Sword", isTogglable = true, enableMethod = () => AdminAssets.SpawnRbSword(), method = () => AdminAssets.UpdateRBSword(), disableMethod = () => AdminAssets.DestroyRbSword() },
                new ButtonInfo { buttonText = "Ender Scythe", isTogglable = true, enableMethod = () => AdminAssets.SpawnEnderScythe(), method = () => AdminAssets.UpdateEnderScythe(), disableMethod = () => AdminAssets.DestroyEnderScythe() },
                new ButtonInfo { buttonText = "Jukebox", isTogglable = true, enableMethod = () => AdminAssets.SpawnJukebox(), disableMethod = () => AdminAssets.DestroyJukebox() },
                new ButtonInfo { buttonText = "Pizza Man", isTogglable = true, enableMethod = () => AdminAssets.SpawnPizzaMan(), disableMethod = () => AdminAssets.DestroyPizzaMan() },
                new ButtonInfo { buttonText = "Pizza Man Hand", isTogglable = true, enableMethod = () => AdminAssets.SpawnPizzaManHand(), disableMethod = () => AdminAssets.DestroyPizzaMan() },
                new ButtonInfo { buttonText = "Donation Nuke", isTogglable = true, enableMethod = () => AdminAssets.SpawnDonationNuke(), disableMethod = () => AdminAssets.DestroyDonationNuke(), },
                 new ButtonInfo { buttonText = "Hammer", isTogglable = true, enableMethod = () => AdminAssets.SpawnBanHammer(), method = () => AdminAssets.UpdateBanHammer(), disableMethod = () => AdminAssets.DestroyBanHammer() },
                new ButtonInfo { buttonText = "TV", isTogglable = true, enableMethod = () => AdminAssets.TV(), disableMethod = () => AdminAssets.DisableTV() },
               new ButtonInfo { buttonText = "Video: " + VideoPlayerType.CurrentName, isTogglable = false, method = () => { VideoPlayerType.CycleNext(); VideoPlayerType.RefreshVideoButton(); } },
new ButtonInfo { buttonText = "Prev Video", isTogglable = false, method = () => { VideoPlayerType.CyclePrev(); VideoPlayerType.RefreshVideoButton(); } },
            },

            
            // Category 16 — Detected
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                // new ButtonInfo { buttonText = "Set Master ", isTogglable = false, method = () => Detected.SetMaster() },
                //  new ButtonInfo { buttonText = "Crash Gun ", isTogglable = true, method = () => Detected.CrashGun() },
                new ButtonInfo { buttonText = "UnGhost Gun ", isTogglable = true, method = () => Detected.UnghostGun    () },
                  new ButtonInfo { buttonText = "UnGhost All ", isTogglable = true, method = () => Detected.UnghostGun    () },
                new ButtonInfo { buttonText = "Ghost All ", isTogglable = false, method = () => Detected.GhostAll() },
                 new ButtonInfo { buttonText = "Ghost Gun ", isTogglable = true, method = () => Detected.GhostGun() },
                    new ButtonInfo { buttonText = "Lag All", isTogglable = true, method = () => Detected.LagAll() },
                    new ButtonInfo { buttonText = "Lag Gun", isTogglable = true, method = () => Detected.LagGun() },
 new ButtonInfo { buttonText = "Change Gamemode | Casual ", isTogglable = false, method = () => Detected.ChangeMode("Casual") },
 new ButtonInfo { buttonText = "Change Gamemode | Infection ", isTogglable = false, method = () => Detected.ChangeMode("Infection") },
 new ButtonInfo { buttonText = "Change Gamemode | HuntDown ", isTogglable = false, method = () => Detected.ChangeMode("HuntDown") },
 new ButtonInfo { buttonText = "Change Gamemode | Paintbrawl ", isTogglable = false, method = () => Detected.ChangeMode("Paintbrawl") },
 new ButtonInfo { buttonText = "Change Gamemode | Ambush ", isTogglable = false, method = () => Detected.ChangeMode("Ambush") },
new ButtonInfo { buttonText = "Change Gamemode | Ghost ", isTogglable = false, method = () => Detected.ChangeMode("Ghost") },
new ButtonInfo { buttonText = "Change Gamemode | Guardian ", isTogglable = false, method = () => Detected.ChangeMode("Guardian") },
new ButtonInfo { buttonText = "Change Gamemode | FreezeTag ", isTogglable = false, method = () => Detected.ChangeMode("FreezeTag") },
 new ButtonInfo { buttonText = "Change Gamemode | Custom ", isTogglable = false, method = () => Detected.ChangeMode("Custom") },
 new ButtonInfo { buttonText = "Change Gamemode | PropHunt ", isTogglable = false, method = () => Detected.ChangeMode("PropHunt") },
new ButtonInfo { buttonText = "Change Gamemode | SuperInfect ", isTogglable = false, method = () => Detected.ChangeMode("SuperInfect") },
  new ButtonInfo { buttonText = "Change Gamemode | SuperCasual ", isTogglable = false, method = () => Detected.ChangeMode("SuperCasual") },
new ButtonInfo { buttonText = "Change Gamemode | Spaz ", isTogglable = true, method = () => Detected.ModeSpaz() },

            },

            // Category 17 — Master
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },

                new ButtonInfo { buttonText = "Vibrate Self", isTogglable = true, method = () => Master.VibrateSelf() },
                new ButtonInfo { buttonText = "Vibrate All", isTogglable = true, method = () => Master.VibrateAll() },
                new ButtonInfo { buttonText = "Vibrate Gun", isTogglable = true, method = () => Master.VibrateGun() },
                new ButtonInfo { buttonText = "Slow Self", isTogglable = true, method = () => Master.SlowSelf() },
                new ButtonInfo { buttonText = "Slow All", isTogglable = true, method = () => Master.SlowAll() },
                new ButtonInfo { buttonText = "Slow Gun", isTogglable = true, method = () => Master.SlowGun() },
                 new ButtonInfo { buttonText = "Rock To Lava(M)", isTogglable = false, method = () => OverPowered.TagToInfection() },
                new ButtonInfo { buttonText = "Lava To Rock(M)", isTogglable = false, method = () => OverPowered.InfectionToTag() },
                new ButtonInfo { buttonText = "Target Spam(M)", isTogglable = true, method = () => Advantage.TargetHitSpam() },
                new ButtonInfo { buttonText = "PB Insta Kill Gun(M)", isTogglable = true, method = () => Master.PaintbrawlKillGun() },
                new ButtonInfo { buttonText = "PB Insta Kill All(M)", isTogglable = false, method = () => Master.PaintbrawlKillAll() },
                 new ButtonInfo { buttonText = "Guardian All(M)", isTogglable = false, method = () => Master.GuardianAll() },
                new ButtonInfo { buttonText = "Guardian Self(M)", isTogglable = false, method = () => Master.GuardianSelf() },
                 new ButtonInfo { buttonText = "Material All(M)", isTogglable = true, method = () => Master.MatAll() },
                 new ButtonInfo { buttonText = "Material Gun(M)", isTogglable = true, method = () => Master.MatGun() },
                   new ButtonInfo { buttonText = "UnTag Self", isTogglable = false, method = () => Master.UntagSelf() },
                new ButtonInfo { buttonText = "UnTag All", isTogglable = false, method = () => Master.UntagAll() },
                new ButtonInfo { buttonText = "UnTag Gun", isTogglable = true, method = () => Master.UntagGun() },
                  new ButtonInfo { buttonText = "Critter Gun(M)", isTogglable = true, method = () => Master.CritterGun() },
                  new ButtonInfo { buttonText = "Grey Screen all", isTogglable = true, method = () => OverPowered.GreyScreenAll(), disableMethod = () => OverPowered.DisableGreyscreen() },
                //  new ButtonInfo { buttonText = "Lava Rising", isTogglable = false, method = () => Master.ChangeLavaState(InfectionLavaController.RisingLavaState.Rising) },
                //  new ButtonInfo { buttonText = "Lava Full", isTogglable = false, method = () => Master.ChangeLavaState(InfectionLavaController.RisingLavaState.Full) },
                //  new ButtonInfo { buttonText = "Lava Draining", isTogglable = false, method = () => Master.ChangeLavaState(InfectionLavaController.RisingLavaState.Draining) },
                 // new ButtonInfo { buttonText = "Lava Drained", isTogglable = false, method = () => Master.ChangeLavaState(InfectionLavaController.RisingLavaState.Drained) },
                 //  new ButtonInfo { buttonText = "Lava Erupting", isTogglable = false, method = () => Master.ChangeLavaState(InfectionLavaController.RisingLavaState.Erupting) },
                   // new ButtonInfo { buttonText = "Spaz Lava", isTogglable = true, method = () => Master.SpazLava() },
                   // new ButtonInfo { buttonText = "Lava Swim Gun", isTogglable = true, method = () => Master.LavaSwimGun() },
            },

            // Category 18 - Sandbox
              new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },

            },

              // Category 19 - Credits
              new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; }, },
                new ButtonInfo { buttonText = "Mr Lag", isTogglable = false, method = () => Credits.MrLag() },
                new ButtonInfo { buttonText = "Pika/Femboy Client(Made the menu design for me)", isTogglable = false, method = () => Credits.PikaorFemboyClient() },
                new ButtonInfo { buttonText = "Hamburbur(Some Patches, Console and a few Mods)", isTogglable = false, method = () => Credits.HamBurBur() },
                new ButtonInfo { buttonText = "Seralyth(Console System and a few patches)", isTogglable = false, method = () => Credits.Seralyth() },
                new ButtonInfo { buttonText = "Untitled(Inspiration for some mods", isTogglable = false, method = () => Credits.Untitled() },
                new ButtonInfo { buttonText = "Juul(Inspiration for Gunlib And Lag method)", isTogglable = false, method = () => Credits.Juul() },

                },

            // Category 20 - Virtual Stump
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
              //   new ButtonInfo { buttonText = "VStump Crash Gun", isTogglable = true, method = () => VStump.VStumpCrashGun() },
             //   new ButtonInfo { buttonText = "VStump Crash All", isTogglable = false, method = () => VStump.VStumpCrashAll() },
                  new ButtonInfo { buttonText = "Lightning Strike Gun", isTogglable = true, method = () => VStump.LightningStrikeGun() },
                        new ButtonInfo { buttonText = "Spawn Lucy Gun", isTogglable = true, method = () => VStump.SpawnLucyGun() },
                         new ButtonInfo { buttonText = "Spawn Lucy All", isTogglable = true, method = () => VStump.SpawnLucyAll() },
       new ButtonInfo { buttonText = "Gorilla Mystery Murder Kill Gun", isTogglable = true, method = () => VStump.MurderKillGun() },
new ButtonInfo { buttonText = "Gorilla Mystery Sheriff Kill Gun", isTogglable = true, method = () => VStump.SheriffKillGun() },
new ButtonInfo { buttonText = "Gorilla Mystery Kill All", isTogglable = false, method = () => VStump.MysteryKillAll() },
new ButtonInfo { buttonText = "Gorilla Mystery Kill Sheriff", isTogglable = false, method = () => VStump.KillSheriff() },
new ButtonInfo { buttonText = "Gorilla Mystery Kill Murderer", isTogglable = false, method = () => VStump.KillMurderer() },
new ButtonInfo { buttonText = "Gorilla Mystery Kill All Innocents", isTogglable = false, method = () => VStump.KillAllInnocents() },
new ButtonInfo { buttonText = "Gorilla Mystery Murderer ESP", isTogglable = true, method = () => VStump.UpdateMurdererEsp(), disableMethod = () => VStump.DisableMurdererEsp() },
new ButtonInfo { buttonText = "Gorilla Mystery Sheriff ESP",  isTogglable = true, method = () => VStump.UpdateSheriffEsp(),  disableMethod = () => VStump.DisableSheriffEsp()  },
new ButtonInfo { buttonText = "Gorilla Mystery Innocent ESP", isTogglable = true, method = () => VStump.UpdateInnocentEsp(), disableMethod = () => VStump.DisableInnocentEsp() },
new ButtonInfo { buttonText = "Gorilla Mystery Toggle Knife",       isTogglable = false, method = () => VStump.ToggleMysteryKnife() },
new ButtonInfo { buttonText = "Gorilla Mystery Toggle Pistol",      isTogglable = false, method = () => VStump.ToggleMysteryPistol() },
new ButtonInfo { buttonText = "Gorilla Mystery Weapon Right Hand",  isTogglable = false, method = () => VStump.MysteryWeaponRightHand() },
new ButtonInfo { buttonText = "Gorilla Mystery Weapon Left Hand",   isTogglable = false, method = () => VStump.MysteryWeaponLeftHand() },
new ButtonInfo { buttonText = "Gorilla Mystery Become Sheriff",     isTogglable = false, method = () => VStump.BecomeSheriff() },
new ButtonInfo { buttonText = "Gorilla Mystery Become Murderer",    isTogglable = false, method = () => VStump.BecomeMurderer() },
new ButtonInfo { buttonText = "Gorilla Mystery Start Map 1",        isTogglable = false, method = () => VStump.StartMysteryMap(1) },
new ButtonInfo { buttonText = "Gorilla Mystery Start Map 2",        isTogglable = false, method = () => VStump.StartMysteryMap(2) },
new ButtonInfo { buttonText = "Gorilla Mystery Start Map 3",        isTogglable = false, method = () => VStump.StartMysteryMap(3) },
new ButtonInfo { buttonText = "Gorilla Mystery Murderer Wins",      isTogglable = false, method = () => VStump.ForceMurdererWin() },
new ButtonInfo { buttonText = "Gorilla Mystery Innocents Win",      isTogglable = false, method = () => VStump.ForceInnocentsWin() },
new ButtonInfo { buttonText = "Gorilla Mystery Spam Rounds",        isTogglable = true,  method = () => VStump.SpamMysteryRounds() },
new ButtonInfo { buttonText = "Meccha Kill Gun",      isTogglable = true,  method = () => VStump.MecchaKillGun() },
new ButtonInfo { buttonText = "Meccha Killshot Gun",  isTogglable = true,  method = () => VStump.MecchaKillShotGun() },
new ButtonInfo { buttonText = "Meccha Respawn Gun",   isTogglable = true,  method = () => VStump.MecchaRespawnGun() },
new ButtonInfo { buttonText = "Meccha Whistle Gun",   isTogglable = true,  method = () => VStump.MecchaWhistleGun() },
new ButtonInfo { buttonText = "Meccha Splatter Gun",  isTogglable = true,  method = () => VStump.MecchaSplatterGun() },
new ButtonInfo { buttonText = "Meccha Explosion Gun", isTogglable = true,  method = () => VStump.MecchaExplosionGun() },
new ButtonInfo { buttonText = "Meccha Kill All",      isTogglable = false, method = () => VStump.MecchaKillAll() },
new ButtonInfo { buttonText = "Meccha Kill Hiders",   isTogglable = false, method = () => VStump.MecchaKillHiders() },
new ButtonInfo { buttonText = "Meccha Kill Seekers",  isTogglable = false, method = () => VStump.MecchaKillSeekers() },
new ButtonInfo { buttonText = "Meccha Respawn All",   isTogglable = false, method = () => VStump.MecchaRespawnAll() },
new ButtonInfo { buttonText = "Meccha Always Alive",  isTogglable = true,  method = () => VStump.MecchaAlwaysAlive() },
new ButtonInfo { buttonText = "Meccha Whistle All",   isTogglable = false, method = () => VStump.MecchaWhistleAll() },
new ButtonInfo { buttonText = "Meccha Whistle Spam",  isTogglable = true,  method = () => VStump.MecchaWhistleSpam() },

            },


            // Category 21 - Networking
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },
                new ButtonInfo { buttonText = "Fuck Console Admins", isTogglable = true, method = () => Networking.EnableConsoleSpoof(), disableMethod = () => Networking.DisableConsoleSpoof() },
                 new ButtonInfo { buttonText = "Fuck GShirts", isTogglable = true, method = () => Networking.FuckGShirts(), disableMethod = () => Networking.DisableFuckGShirts() },
            },

            // Category 22 - Beta
            new ButtonInfo[]
            {
                new ButtonInfo { buttonText = "Back", isTogglable = false, method = () => { Main.category = 0; Main.page = 0; } },

            },
        };
    
    }
}