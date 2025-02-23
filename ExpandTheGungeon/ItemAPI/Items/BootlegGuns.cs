﻿using Gungeon;
using System.Collections.Generic;
using ExpandTheGungeon.ExpandComponents;
using ExpandTheGungeon.ExpandPrefab;

namespace ExpandTheGungeon.ItemAPI {

    public class BootlegGuns : Gun {

        public static int BootlegPistolID;
        public static int BootlegMachinePistolID;
        public static int BootlegShotgunID;

        public static Gun BootlegPistol;
        public static Gun BootlegMachinePistol;
        public static Gun BootlegShotgun;
        
        public static void Init() {
            Gun pistol = ETGMod.Databases.Items.NewGun("Bootleg Pistol", "bootleg_pistol");
            Game.Items.Rename("outdated_gun_mods:bootleg_pistol", "ex:bootleg_pistol");
            pistol.SetShortDescription("Of questionable quality...");
            pistol.SetLongDescription("It's a counterfeit gun.\n\nDue to low quality standards, this weapon may be prone to exploding under certain circumstances...");
            GunExt.SetupSprite(pistol, null, "bootleg_pistol_idle_001", 18);            
            pistol.AddProjectileModuleFrom("Magnum", true, false);
            pistol.DefaultModule.ammoCost = 1;
            pistol.PreventOutlines = true;
            pistol.Volley = (PickupObjectDatabase.GetById(38) as Gun).Volley;
            pistol.singleModule = (PickupObjectDatabase.GetById(38) as Gun).singleModule;
            pistol.RawSourceVolley = (PickupObjectDatabase.GetById(38) as Gun).RawSourceVolley;
            pistol.alternateVolley = (PickupObjectDatabase.GetById(38) as Gun).alternateVolley;
            pistol.reloadTime = 1;
            pistol.gunClass = GunClass.PISTOL;
            pistol.ammo = 140;
            pistol.SetBaseMaxAmmo(140);
            pistol.quality = ItemQuality.D;
            pistol.UsesCustomCost = true;
            pistol.CustomCost = 10;
            pistol.encounterTrackable.EncounterGuid = "baad9dd6d005458daf02933f6a1ba926";            
            pistol.gameObject.AddComponent<ExpandRemoveGunOnAmmoDepletion>();
            pistol.gameObject.AddComponent<ExpandMaybeLoseAmmoOnDamage>();
            ETGMod.Databases.Items.Add(pistol);

            BootlegPistolID = pistol.PickupObjectId;


            Gun machinepistol = ETGMod.Databases.Items.NewGun("Bootleg Machine Pistol", "bootleg_machinepistol");
            Game.Items.Rename("outdated_gun_mods:bootleg_machine_pistol", "ex:bootleg_machine_pistol");
            machinepistol.SetShortDescription("Of questionable quality...");
            machinepistol.SetLongDescription("It's a counterfeit machine gun.\n\nDue to low quality standards, this weapon may be prone to exploding under certain circumstances...");
            GunExt.SetupSprite(machinepistol, null, "bootleg_machinepistol_idle_001", 30);
            machinepistol.AddProjectileModuleFrom("Magnum", true, false);
            machinepistol.Volley = (PickupObjectDatabase.GetById(43) as Gun).Volley;
            machinepistol.singleModule = (PickupObjectDatabase.GetById(43) as Gun).singleModule;
            machinepistol.RawSourceVolley = (PickupObjectDatabase.GetById(43) as Gun).RawSourceVolley;
            machinepistol.alternateVolley = (PickupObjectDatabase.GetById(43) as Gun).alternateVolley;
            machinepistol.PreventOutlines = true;
            machinepistol.reloadTime = 1.2f;
            machinepistol.gunClass = GunClass.FULLAUTO;
            machinepistol.ammo = 600;
            machinepistol.SetBaseMaxAmmo(600);
            machinepistol.quality = ItemQuality.D;
            machinepistol.gunSwitchGroup = "Uzi";
            machinepistol.UsesCustomCost = true;
            machinepistol.CustomCost = 15;
            machinepistol.encounterTrackable.EncounterGuid = "e56adda5081347e5b9e0cf2556689b0e";
            machinepistol.gameObject.AddComponent<ExpandRemoveGunOnAmmoDepletion>();
            machinepistol.gameObject.AddComponent<ExpandMaybeLoseAmmoOnDamage>();

            ETGMod.Databases.Items.Add(machinepistol);

            BootlegMachinePistolID = machinepistol.PickupObjectId;


            Gun shotgun = ETGMod.Databases.Items.NewGun("Bootleg Shotgun", "bootleg_shotgun");
            Game.Items.Rename("outdated_gun_mods:bootleg_shotgun", "ex:bootleg_shotgun");
            shotgun.SetShortDescription("Of questionable quality...");
            shotgun.SetLongDescription("It's a counterfeit shotgun.\n\nDue to low quality standards, this weapon may be prone to exploding under certain circumstances...");
            GunExt.SetupSprite(shotgun, null, "bootleg_shotgun_idle_001", 18);
            shotgun.AddProjectileModuleFrom("AK-47", true, false);
            shotgun.Volley = (PickupObjectDatabase.GetById(51) as Gun).Volley;
            shotgun.singleModule = (PickupObjectDatabase.GetById(51) as Gun).singleModule;
            shotgun.RawSourceVolley = (PickupObjectDatabase.GetById(51) as Gun).RawSourceVolley;
            shotgun.alternateVolley = (PickupObjectDatabase.GetById(51) as Gun).alternateVolley;
            shotgun.PreventOutlines = true;
            shotgun.reloadTime = 1.8f;
            shotgun.gunClass = GunClass.SHOTGUN;
            shotgun.ammo = 150;
            shotgun.SetBaseMaxAmmo(150);
            shotgun.quality = ItemQuality.D;
            shotgun.gunSwitchGroup = "Shotgun";
            shotgun.UsesCustomCost = true;
            shotgun.CustomCost = 18;
            shotgun.encounterTrackable.EncounterGuid = "fa0575b4cf0140ddb6b0ed6d962bff47";
            shotgun.gameObject.AddComponent<ExpandRemoveGunOnAmmoDepletion>();
            shotgun.gameObject.AddComponent<ExpandMaybeLoseAmmoOnDamage>();

            ETGMod.Databases.Items.Add(shotgun);

            BootlegShotgunID = shotgun.PickupObjectId;


            BootlegPistol = pistol;
            BootlegMachinePistol = machinepistol;
            BootlegShotgun = shotgun;
        }

        public static void PostInit() {
            if (BootlegPistol && BootlegPistol.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>()) {
                BootlegPistol.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>().TransfmorgifyTargetGUIDs = new List<string>() {
                    ExpandCustomEnemyDatabase.BootlegBulletManGUID
                };
            }
            if (BootlegMachinePistol && BootlegMachinePistol.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>()) {
                BootlegMachinePistol.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>().TransfmorgifyTargetGUIDs = new List<string>() {
                    ExpandCustomEnemyDatabase.BootlegBulletManBandanaGUID
                };
            }
            if (BootlegShotgun && BootlegShotgun.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>()) {
                BootlegShotgun.gameObject.GetComponent<ExpandMaybeLoseAmmoOnDamage>().IsBootlegShotgun = true;
            }
        }
    }
}

