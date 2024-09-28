# Nex Layout Changes

Always try to keep your database table columns up-to-date.

## CLI 1.4.0

#### `access`
* `Unk10` -> `AccessHoldParamId`

#### `areadefine`
* `Unk3` -> `PlaceNameId`

#### `command`
* `Unk3` -> `Eikon`
* `Unk4` -> `LeviathanAmmo`
* `Unk6` -> `UnkType6`
* `Unk7` -> `IconTexFileId`
* `Unk18` -> `SummonModeId`
* `Unk19` -> `UnkBitFlags19`
* `Unk20` -> `UnkActionId`
* `Unk23`: `int` -> `float`
* `Unk31` -> `SkillId`
* `PerformedSkillArray` -> `GroundActionIds`
* `Unk34` -> `AirBorneActionIds`
* `Unk37` -> `UnkType37`
* `Unk45` -> `AutoAttackExecConditionId`
* `Unk48`: `int` -> `float`
* `Unk50` -> `UnkType50`
* `Unk51` -> `UnkType51`
* `Unk58` -> `UnkType58`
* `Unk63`: `int` -> `short`

#### `droptable`

* `Unk3` -> `Exp`
* `Unk5` -> `ItemCount1`
* `Unk6` -> `ItemCount2`
* `Unk7` -> `ItemCount3`
* `Unk8` -> `ItemCount4`
* `Unk9` -> `ItemCount5`
* `Unk10` -> `ItemCount6`
* `Unk11` -> `RewardType1` 
* `Unk12` -> `ID1`
* `Unk13` -> `RewardType2` 
* `Unk14` -> `ID2`
* `Unk15` -> `RewardType3` 
* `Unk16` -> `ID3`
* `Unk17` -> `RewardType4` 
* `Unk18` -> `ID4`
* `Unk19` -> `RewardType5` 
* `Unk20` -> `ID5`
* `Unk21` -> `RewardType6` 
* `Unk22` -> `ID6`
* `Unk23` -> `Chance1`
* `Unk24` -> `Chance2`
* `Unk25` -> `Chance3`
* `Unk26` -> `Chance4`
* `Unk27` -> `Chance5`
* `Unk28` -> `Chance6`
* `Unk29` & `Unk30` removed (padding)

#### `fatalattack`
* `Unk9` -> `Rarity`
* `Unk10` -> `IconTexFileId`


#### `fatalattack`
* `Unk7`: `int` -> `float`

#### `gamemap`

* `Unk1` -> `DLCFlags`
* `Unk4` -> `LayoutNamedInstanceId`
* `Unk5` -> `MapDirectorId`
* `Unk6` -> `LevelSequenceId`
* `Unk8` -> `BgmType` & `BgmOrBgmSelectId`
* `Unk9` -> `BgmType2` & `BgmOrBgmSelectId2`
* `Unk10` -> `Unk10` & `PartySelectId`
* `Unk13` -> `SpecialAreaId`
* `Unk15` -> `CollisionMaterialColorId`
* `Unk16` -> `Unk16`/`Unk16_2`/`Unk16_3`/`Unk16_4`
* `Unk17` -> `PlaceNameId`
* `Unk18` -> `LayoutSettingId`
* `Unk19` -> `LayoutSettingSubId`
* `Unk26` -> `UserSituationId`

#### `orchestrion`

* `Unk3` -> `BgmModeId`
* `Unk4` -> `Name`
* `Unk5` -> `How`
* `Unk6` -> `HowFormat`
* `Unk7` -> `UnlockCondition` (int -> short)
* `Unk10` -> `SortOrder`
* `Unk12` -> `IsDefault`


#### `quest`
* `Unk3` -> `Name`
* `Unk4` -> `Desc`
* `Unk5` -> `CompletedDesc`
* `Unk7` -> `QuestType`
* `Unk18` -> `DropTableId`


#### `result`
* `Unk2` -> `ResultCompleteMessageId`
* `Unk4` -> `DropTableId`
* `Unk5` -> `DropTableId2`

#### `simpleeventchoice`
* `Unk6` -> `PzdSimpleQuestId`

#### `skill`
* `Unk8` -> `CommandId`

#### `specialarea`
* `Unk2` -> `UserConditionId`
* `Unk3` -> `MovementRestrictionBitSet`
* `Unk7` -> `EnvironmentVibrationId`
* `Unk8` -> `CharaSituationId`
* `Unk9` -> `CustomSpeedId`
* `Unk12` -> `NormalCameraModeIdMaybe`

#### `worldmapanchor`
* `Unk10` -> `GameMapId`

##### Contributors
* Nenkai
* darchiev (Discord)
* Veralion (Discord)

---

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
