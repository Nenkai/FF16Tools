# Nex Layout Changes

Always try to keep your database table columns up-to-date.

## CLI 1.5.6, FF16Tools.Files 1.0.8

#### `aiactionselector`
* `Unk3` -> `ActionId`
* `Unk23` -> `Comment`

#### `attackparam`
* `Unk3` -> `AttackValue`
* `Unk75` -> `HitCount`

#### `characterskincategory`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `ReqUserSituationId`
* `Unk4` -> `BNpcBaseId`
* `Unk5` -> `DefaultIconId`
* `Unk6` -> `Skin2IconId`
* `Unk7` -> `VariantReqUserSituationId`
* `Unk8` -> `TimeSkipDefaultIconId`
* `Unk9` -> `TimeSkipSkin2IconId`
* `Unk10` -> `SkinUiId1`
* `Unk11` -> `SkinUiId2`

#### `characterskinid`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Enabled`

#### `characterskinmodelparam`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `ModelCoordinateId`
* `Unk4` -> `ModelCoordinateIdKey2`

#### `partymember`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `BNpcBaseIds`
* `Unk4` -> `ParamGrowId`
* `Unk5` -> `UnkCounts1`
* `Unk6` -> `BNpcBaseIds2`
* `Unk8` -> `UnkCounts2`
* `Unk9` -> `UnkUiRelated`
* `Unk10` -> `UnkSkillRefundPoints`
* `Unk11` through `Unk15` -> Split into 8 sbytes - `UnkMember1` through `UnkMember8`

#### `partyselect`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `PartyMemberId`
* `Unk3` -> `ReqUserSituationId`

#### `questchara`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`

#### `skill`
* `Unk24` -> `RefundPointsOnReset1`
* `RefundPointsOnReset` -> `RefundPointsOnReset2`

#### `weaponskincategory`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `EquipItemId`
* `Unk4` -> `IconTexId`

##### Contributors
* Nenkai
* Maybri (Discord)
* CyberSoul (Discord)
* xeeynamo (Discord)

---

## CLI 1.5.5, FF16Tools.Files 1.0.7

Too many. **Refer to [this commit](https://github.com/Nenkai/FF16Tools/commit/c4eba6337d5afae7e168c033272bbfacf5663a5a)**

## CLI 1.5.4, FF16Tools.Files 1.0.6

#### `abyssgaterating`
* `Unk1` -> `DLCFlags`
* `Unk4` -> `SRankScoreReqActionMode`
* `Unk5` -> `ARankScoreReqActionMode`
* `Unk6` -> `BRankScoreReqActionMode`
* `Unk11` -> `SRankScoreReqFFMode`
* `Unk12` -> `ARankScoreReqFFMode`
* `Unk13` -> `BRankScoreReqFFMode`
* `Unk21` - int -> byte

#### `bnpcbase`
* `Unk29` -> `SpeakerId`

#### `directorcondition`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `ConditionType`
* `Unk3` -> `TableValuesToCheck`
* `Unk4` -> `Comment`

#### `directormodule`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `TableValueCheckUnion`
* `Unk4` -> `Comment`

#### `enpcbase`
* `Unk8` -> `SpeakerId`

#### `fieldmapobelisk`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk4` -> `WorldMapAnchorId`

#### `layoutnamedinstance`
* `Unk1` -> `DLCFlags`
* `Unk4` -> `Comment`
* `Unk5` -> `GameMapId`
* `Unk6` -> `X`
* `Unk7` -> `Y`
* `Unk8` -> `Z`

#### `mapdirector`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `MapDirectorContentIds`
* `Unk3` -> `BattleBlockDirectorIds`
* `Unk4` -> `BattleDirectorIds`
* `Unk5` -> `ReqUserSituationIds1`
* `Unk6` -> `TutorialDirectorIds`
* `Unk7` -> `ReqUserSituationIds2`
* `Unk8` -> `SMobDirectorIds`
* `Unk9` -> `Comment`
* `Unk10` -> `FieldEventDirectorIds`
* `Unk11` -> `BehaviorEventDirectorIds`
* `Unk12` -> `MissionBattleDirectorIdToReqDlcFlag`
* `Unk13` -> `UnkType13`
* `Unk14` -> `SideQuestBattleDirectors`
* `Unk15` -> `FixedPaletteDirectorIdToReqDlcFlag`
* `Unk16` -> `AbyssGateDirectorIdToReqDlcFlag`

#### `mapdirectorcontent`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `MapDirectorModuleIds`
* `Unk4` -> `UnkDirectorConditionId1`
* `Unk5` -> `UnkDirectorConditionId2`
* `Unk6` -> `UnkDirectorConditionId3`
* `Unk7` -> `UnkDirectorConditionArray`
* `Unk8` -> `Comment`

#### `mapdirectorflag` (unused)
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`

#### `mapdirectormodule`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `DirectorModuleId`

#### `obelisk`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `LayoutNamedInstanceId`
* `Unk3` -> `PlaceNameId`

#### `smobdirector`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `BattleTagId`
* `Unk4` -> `ResultId`
* `UserSituationId` -> `ReqUserSituationId`
* `Unk13` -> `QuestId`
* `Unk14` -> `MarkName`
* `Unk15` -> `MarkTitle`
* `Unk21` -> `SortOrder`
* `Unk22` -> `IconTexFileId`
* `Unk25` -> `MainImageTexFileId`
* `Unk31` - int -> byte

#### `trainingenemy`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `ReqUserSituationId`
* `Unk4` -> `BNpcBaseId`
* `Unk5` -> `NameUiId`
* `Unk6` -> `DescriptionUiId`

#### `uiaddon`
* `Unk1` -> `DLCFlags`
* `Unk2` -> int -> short
* `Unk3` -> `UiFilePath`
* `Unk4` -> `UiShopHowToId`
* `Unk5` -> `IsPreloaded`
* `Unk6` -> `AlwaysVisibleMaybe`
* `Unk11` -> `AllowCharacterInput`

##### Contributors
* Nenkai
* Jj (Discord)
* Rukkard (Discord)
* CyberSoul (Discord)

## CLI 1.5.3, FF16Tools.Files 1.0.5

> [!NOTE]
> Some color related tables had their columns changed to `hex_uint` for better viewing & editing of ARGB color values.

#### `abyssgate`
* `Unk8` -> `AbyssGateRatingId`

#### `action`
* `Unk_0x00` -> `DLCFlags`
* `Unk_0x04` -> `CharaTimelineId`
* `Unk_0x08` -> `CharaTimelineId2`

#### `aiactionselector`
* `Unk1` -> `DLCFlags`
* `Unk13` -> `LayoutNamedInstanceId1Maybe`
* `Unk14` -> `LayoutNamedInstanceId2Maybe`

#### `aicounter`
* `Unk1` -> `DLCFlags`

#### `attackparam`
* `Unk4` -> `UnkType4`
* `Unk15` -> `UnkType15`
* `Unk31` -> `UnkType31`
* `Unk33` -> int -> float
* `Unk38` -> `GuardReactionId`
* `Unk44` -> `UnkType44`
* `Unk47` -> `Unk47TitleRelatedToUnk5`
* `Unk49` -> `BuffIdMaybe`
* `Unk50` -> int -> float
* `Unk51` -> `UnkType51`
* `Unk59` through `Unk70` all changes to bytes
* `Unk73` changes to bytes
* `Unk76` changes to bytes

#### `battleai`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `AiActionSelectorId1`
* `Unk4` -> `AiActionSelectorId2`

#### `bnpcbase`
* `Unk38` -> `ColorCoordinateId`

#### `caption`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `IdType`
* `Unk3` -> `Id`

#### `charatimelinevariation`
* `Unk1` -> `DLCFlags`
* `Unk5` -> `MoveParamId`

#### `collisionmaterialcolor`
* `Unk2` -> int[] -> uint[]

#### `colorcoordinate`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `EyeColorId`
* `Unk3` -> `SkinColorId`
* `Unk4` -> `HairColorId`

#### `destructorparampreset`
* `Unk2` -> `DestructorParamIds`

#### `difficultylevel`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `WeaponEquipItemId`
* `Unk4` -> `WeaponEquipItemId2`
* `Unk5` -> `WeaponEquipItemId3`
* `Unk7` -> `GearEquipItemIds`
* `Unk9` -> `Unk9EquipItemId`
* `Unk10` -> `Unk10EquipItemId`

#### `edgeemission`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Color`, int -> hex_uint

#### `enpcbase`
* `Unk9` -> `ColorCoordinateId`

#### `equipitem`
* `EffectArray` -> `TypeAndAttackParamId`
* `Unk31` -> `WeaponSkinCategoryId`

#### `eyecolor`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `Color`, int -> hex_uint

#### `footground`
* `Unk4` -> `MastsParamId`

#### `haircolor`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `UnkColor3`, int -> hex_uint
* `Unk4` -> `UnkColor4`, int -> hex_uint

#### `item`
* `Unk1` -> `DLCFlags`

#### `jingle`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `SoundFilePath`

#### `howto`
* `Unk1` -> `DLCFlags`
* `Unk4` -> `Description`
* `Unk5` -> `Title`
* `Unk10` -> `AssetIdType1`
* `Unk11` -> `AssetId1`
* `Unk12` -> `AssetIdType2`
* `Unk13` -> `AssetId2`

#### `model`
* `Unk20` -> `HitSeIdMaybe`
* `Unk32` -> `DeadParamid`

#### `modelcoordinate`
* `Unk1` -> `DLCFlags`
* `Unk13` -> `EdgeEmissionId`

#### `moviedata`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `MovieFilePath`

#### `quest`
* `Unk1` -> `DLCFlags`

#### `ragdollparam`
* `Unk31` -> `RagdollParamSetIds1`
* `Unk32` -> `RagdollParamSetIds2`

#### `recipe`
* `Unk1` -> `DLCFlags`
* `Unk18` -> `UnkUserSituationId1`
* `Unk19` -> `UnkUserSituationId1`

#### `skincolor`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `Color`, int -> hex_uint

#### `shopbase`
* `Unk1` -> `DLCFlags`

#### `shopbase`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `ShopTalkSequenceIds`
* `Unk4` -> `LayoutNamedInstanceId`

#### `simpleevent`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk4` -> `ShopBaseId`

#### `smithshopbase`
* `Unk1` -> `DLCFlags`

#### `speakerset`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `UserSituationId1`
* `Unk4` -> `UserSituationId2`

#### `stageshopbase`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `Comment`
* `Unk3` -> `ItemShopBaseId`
* `Unk4` -> `SmithShopBaseId`
* `Unk5` -> `ShopTalkId1`
* `Unk6` -> `ShopTalkId2`
* `Unk7` -> `ShopTalkId3`
* `Unk8` -> `ShopTalkId4`
* `Unk9` -> `ShopTalkId5`
* `Unk10` -> `ShopTalkId6`
* `Unk11` -> `ShopTalkId7`
* `Unk12` -> `ShopTalkId8`
* `Unk13` -> `ShopTalkId9`
* `Unk14` -> `ShopTalkId10`
* `Unk15` -> `ShopTalkId11`
* `Unk16` -> `ShopTalkId12`
* `Unk17` -> `ShopTalkId13`
* `Unk18` -> `ShopTalkId14`
* `Unk19` -> `ShopTalkId15`

#### `skill`
* `Unk1` -> `DLCFlags`
* `DLCFlags` -> `Comment`
* `Unk15` -> `MovieDataId`

#### `uicolor`
* `Unk1` -> `DLCFlags`
* `Unk3` -> `Color`, int -> hex_uint

#### `useitem`
* `Unk1` -> `DLCFlags`
* `AttackParamId` -> `AttackParamId1`
* `Unk3` -> `AttackParamId2`
* `Unk4` -> `AttackParamId3`
* `Unk5` -> `AttackParamId4`
* `Unk6` -> `AttackParamId5`

#### `uifieldmapicon`
* `Unk2` -> `UnkType2`
* `Unk7` -> `ReqUserSituationId`
* `Unk8` -> `IdType`
* `Unk9` -> `Id`
* `Unk10` -> `ShopBaseId`
* `Unk12` -> `UnkUserSituationId`

#### `uisummonaction`
* `Unk7` -> `MovieDataId`
* `Unk8` -> `Name`
* `Unk9` -> `Description`

#### `window`
* `Unk1` -> `DLCFlags`

##### Contributors
* Nenkai
* TrisChandler (discord)

## CLI 1.5.2, FF16Tools.Files 1.0.4

#### `action`
* `Unk_0x30` -> `ActionMoveTypeId`
* `Unk_0x84`: int -> float
* `Unk_0xA8` -> `UnkSkillId`

#### `command`
* `UnkType58` -> `SkillBgOrIconColor`

#### `eid`
* `Unk3` -> `ModelFileUnkComponentName`

#### `fixedpalette`
* `Unk4` -> `SummonModeId`

#### `phoenixshiftmove`
* `Unk2` -> `GroundExitActionId`
* `Unk3` -> `AirExitActionId`
* `Unk4` -> `GroundExitComboId`
* `Unk5` -> `AirExitComboId`
* `Unk7` -> `ShiftAttackDistance`
* `Unk14` -> `GroundDashDistance`
* `Unk15` -> `AirDashDistance`

#### `skill`
* `Unk3` -> `SkillCategoryId`

#### `summonmode`
* `Unk2` -> `CommandId`
* `Unk15` -> `SummonPartsId`
* `Unk22` -> `UnkCommandId1`
* `Unk23` -> `UnkCommandId2`
* `Unk24` -> `UnkCommandId3`
* `Unk25` -> `UnkCommandId4`
* `Unk26` -> `UnkCommandId5`
* `Unk27` -> `UnkCommandId6`
* `Unk32` -> `RazerEventId`

#### `weaponbase`
* `Unk1` -> `DLCFlags`
* `Unk4` -> `WeaponAttachTypeId`
* `Unk6` -> `WeaponCollisionId`

#### `weaponattachtype`
* `Unk1` -> `DLCFlags`
* `Unk1` -> `EidId1`
* `Unk2` -> `EidId2`
* `Unk3` -> `EidId3`
* `Unk4` -> `EidId4`
* `Unk5` -> `EidId5`
* `Unk6` -> `EidId6`
* `Unk7` -> `EidId7`

#### `weaponcollision`
* `Unk1` -> `DLCFlags`
* `Unk2` -> `UnkType1`
* `Unk3` -> `UnkTable1_1`
* `Unk4` -> `UnkTable1_2`
* `Unk5` -> `UnkType2`
* `Unk6` -> `UnkTable2_1`
* `Unk7` -> `UnkTable2_2`

##### Contributors
* Nenkai
* Jj (Discord)
* Mrwill1019 (Discord)

---

## CLI 1.5.1, FF16Tools.Files 1.0.3
 
 #### `animalbase`
* `Unk2` -> `ModelId`
* `Unk3` -> `ModelIdKey2`

 #### `bnpcbase`
* `Unk4` -> `ModelId`
* `Unk5` -> `ModelIdKey2`

 #### `equipitem`
* `EquipCategory` -> `Category`
* `Rarity` -> `Rarity_UIArrayIdRelated`
* `Unk22` & `Unk23`: Removed (implicit padding)
* `Unk30` -> `SortOrder2`
* `Unk40` -> `SortOrder2`

#### `enpcbase`
`Unk3` -> `ModelId`
`Unk4` -> `ModelIdKey2`
`Unk21` -> `ModelCoordinateId`
`Unk22` -> `ModelCoordinateIdKey2`

#### `gamemap`
* `Unk7` -> `ReqUserSituationId`

#### `item`
* `Unk2` -> `Category`
* `Unk3` -> `Name`
* `Unk4` -> `Description`
* `Unk12` -> `ItemCap`
* `Unk13` -> `GilCost`
* `Unk14` -> `UIArrayIdRelated`
* `FileIconID` -> `IconTexFileId`
* `Unk16` -> `UseItemId`
* `Unk18-21` -> `SortOrder1` (int)
* `Unk29` -> `SortOrder2`

#### `layoutsettings`
* `Unk2` -> `GameMapSettingsId`
* `Unk11` -> `MapSettingsId`

#### `loadingimage`
* `Unk2` -> `ImageFileId`
* `Unk3` -> `ReqUserSituationId`

#### `model`
* `Unk2` -> `ModelCoordinateId`

#### `modelcoordinate`
* `Unk2` -> `FaceId`
* `Unk3` -> `SkeletonParamId`
* `Unk4` -> `BodyId`
* `Unk5` -> `HeadId`

#### `nullactorbase`
* `Unk2` -> `ModelId`
* `Unk3` -> `ModelIdKey2`

#### `placement`
* `Unk2` -> `HasUserSituationCondition`
* `Unk4` -> `ReqUserSituationId`

#### `propbase`
* `Unk2` -> `ModelId`
* `Unk3` -> `ModelIdKey2`

#### `questcharalayout`
* `Unk12` -> `PlacementId`

#### `smobdirector`

* `Unk5` -> `UserSituationId`
* `Unk9` -> `LayoutNamedInstanceId`
* `Unk11` -> `UnkLayoutNamedInstanceIds`
* `Unk16` -> `Name`
* `Unk17` -> `Description`
* `Unk18` -> `Location`
* `Unk26` -> `UnkLayoutNamedInstanceId2`

#### `useitem`

* `Unk2` -> `AttackParamId`

#### `usersituation`

* `Unk2` -> `UnkType`
* `Unk3` -> `UnkIdType`
* `Unk4` -> `Id1`

#### `worldmapanchor`

* `Unk13` -> `LoadingImageId`
* `Unk16` -> `GameMapSubId`

#### `weaponbase`
* `Unk2` -> `ModelId`
* `Unk3` -> `ModelIdKey2`
* `Unk5` -> `MotionWeaponTypeId`

* ##### Contributors
* Nenkai
* Veralion (Discord)
* Mrwill1019 (Discord)

----

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
