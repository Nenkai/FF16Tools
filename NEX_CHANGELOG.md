# Nex Layout Changes

Always try to keep your database table columns up-to-date.

## CLI 1.3.7

#### `command`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `JPName`
* `Unk11` -> `Name`
* `Unk33` -> `PerformedSkillArray`
* `CooldownArray` -> `Cooldowns`


#### `equipitem`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `EquipCategory`
* `Unk5` -> `Defense`
* `WillStagger` -> `Stagger`
* `Unk8` -> `SalePrice`
* `Unk11` -> `EffectFlag`
* `Unk12` -> `Name`
* `Unk13` -> `Description`
* `Unk14` -> `Name1`
* `Unk15` -> `Name2`
* `Unk25` -> `EffectPotency`
* `Unk26` -> `EffectArray`
* `Unk27` -> `HP`

#### `item`
* `Unk15` -> `FileIconID`
* `Unk24` (`int`) split into 4 bools (Unk24 through 27)
* Old `Unk27` (`int`) split into 4 bools (Unk27 through 30)

#### `skill`
* `Unk2` -> `DLCFlags`
* `Unk9` -> `Name`
* `Unk10` -> `Description`
* `Unk11` -> `Description1`
* `Unk12` -> `UpgradeCost`

##### Contributors
* darchiev (Discord)

---

## CLI 1.3.5

* `command`
  * `Unk42` -> `CooldownArray`

* `debugjumpmarkertype`
   * Fixed missing row id flag
     
* `difficultylevel`
  * `Unk12` -> `BossDamage`
  * `Unk13` -> `EnemyDamage`
  * `Unk14` -> `BossStagger`
  * `Unk15` -> `EnemyFlinch`
  * `Unk16` -> `BossHp`
  * `Unk17` -> `EnemyHp`
 
* `equipitem`
  * `Unk4` -> `Attack`
  * `Unk6` -> `WillStagger`
 
* `razerevent`
  * Added layout file (table added in retail)
 
* `uifonttype`
  * Added `Unk6` (added in retail?)

* `uileaderboard`
  * Added `Unk6/7` (added in retail?)
 
### Contributors

* [gladias9](https://www.nexusmods.com/finalfantasy16/users/5780417)
* darchiev (Discord)
* Nenkai
