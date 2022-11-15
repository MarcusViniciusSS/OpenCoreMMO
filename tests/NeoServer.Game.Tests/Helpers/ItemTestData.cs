﻿using System;
using System.Collections.Generic;
using System.Linq;
using NeoServer.Data.InMemory.DataStores;
using NeoServer.Game.Common.Contracts.DataStores;
using NeoServer.Game.Common.Contracts.Items;
using NeoServer.Game.Common.Contracts.Items.Types;
using NeoServer.Game.Common.Contracts.Items.Types.Body;
using NeoServer.Game.Common.Contracts.Items.Types.Containers;
using NeoServer.Game.Common.Contracts.Items.Types.Runes;
using NeoServer.Game.Common.Item;
using NeoServer.Game.Common.Location.Structs;
using NeoServer.Game.Common.Parsers;
using NeoServer.Game.Items;
using NeoServer.Game.Items.Bases;
using NeoServer.Game.Items.Factories;
using NeoServer.Game.Items.Items;
using NeoServer.Game.Items.Items.Attributes;
using NeoServer.Game.Items.Items.Containers;
using NeoServer.Game.Items.Items.Cumulatives;
using NeoServer.Game.Items.Items.UsableItems;
using NeoServer.Game.Items.Items.Weapons;

namespace NeoServer.Game.Tests.Helpers;

public class ItemTestData
{
    public static Container CreateContainer(byte capacity = 6, IEnumerable<IItem> children = null)
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, capacity);

        return new Container(itemType, new Location(100, 100, 7), children);
    }

    public static IPickupableContainer CreatePickupableContainer(byte capacity = 6, IEnumerable<IItem> children = null,
        bool backpack = false)
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, capacity);
        itemType.Attributes.SetAttribute(ItemAttribute.Weight, 20);

        if (backpack)
            itemType.Attributes.SetAttribute(ItemAttribute.BodyPosition, "backpack");

        return new PickupableContainer(itemType, new Location(100, 100, 7), children);
    }

    public static PickupableContainer CreateBackpack()
    {
        var itemType = new ItemType();
        itemType.Attributes.SetAttribute(ItemAttribute.Capacity, 20);
        itemType.Attributes.SetAttribute(ItemAttribute.Weight, 20);

        itemType.Attributes.SetAttribute(ItemAttribute.BodyPosition, "backpack");

        return new PickupableContainer(itemType, new Location(100, 100, 7), null);
    }

    public static ICumulative CreateCumulativeItem(ushort id, byte amount, string slot = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");
        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
        type.Flags.Add(ItemFlag.Stackable);
        type.Attributes.SetAttribute(ItemAttribute.Weight, 1);

        return new Cumulative(type, new Location(100, 100, 7), amount);
    }

    public static Item CreateRegularItem(ushort id)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");

        return new Item(type, new Location(100, 100, 7));
    }

    public static IItem CreateMoveableItem(ushort id)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);

        type.SetName("item");

        return new MeleeWeapon(type, new Location(100, 100, 7));
    }

    public static IPickupable CreatePot(ushort id,
        (ItemAttribute, IConvertible)[] attributes = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("pot");
        type.Attributes.SetAttribute(ItemAttribute.Weight, 10);

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new HealingItem(type, new Location(100, 100, 7),
            attributes.ToDictionary(x => x.Item1, x => x.Item2));
    }

    public static IEquipment CreateMagicWeapon(ushort id, bool twoHanded = false, byte charges = 0,
        (ItemAttribute, IConvertible)[] attributes = null, Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("magic weapon");
        type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, twoHanded ? "two-handed" : "weapon");

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new MagicWeapon(type, new Location(100, 100, 7))
        {
            Chargeable = charges > 0
                ? new Chargeable(charges, type.Attributes.GetAttribute<bool>(ItemAttribute.ShowCharges))
                : null,
            ItemTypeFinder = itemTypeFinder
        };
    }

    public static IPickupable CreateWeaponItem(ushort id, string weaponType = "sword", bool twoHanded = false,
        byte charges = 0,
        (ItemAttribute, IConvertible)[] attributes = null, Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");
        type.Attributes.SetAttribute(ItemAttribute.WeaponType, weaponType);
        type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, twoHanded ? "two-handed" : "weapon");

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new MeleeWeapon(type, new Location(100, 100, 7))
        {
            Chargeable = charges > 0
                ? new Chargeable(charges, type.Attributes.GetAttribute<bool>(ItemAttribute.ShowCharges))
                : null,
            ItemTypeFinder = itemTypeFinder
        };
    }

    public static IPickupable CreateDistanceWeapon(ushort id, bool twoHanded = false, byte charges = 0,
        (ItemAttribute, IConvertible)[] attributes = null, Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");
        type.Attributes.SetAttribute(ItemAttribute.WeaponType, "distance");
        type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, twoHanded ? "two-handed" : "weapon");

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new DistanceWeapon(type, new Location(100, 100, 7))
        {
            Chargeable = charges > 0
                ? new Chargeable(charges, type.Attributes.GetAttribute<bool>(ItemAttribute.ShowCharges))
                : null,
            ItemTypeFinder = itemTypeFinder
        };
    }

    public static IEquipment CreateThrowableDistanceItem(ushort id, byte amount = 1, bool twoHanded = false,
        (ItemAttribute, IConvertible)[] attributes = null, Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");

        type.Attributes.SetAttribute(ItemAttribute.WeaponType, "distance");
        type.Attributes.SetAttribute(ItemAttribute.Weight, 40);

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new ThrowableDistanceWeapon(type, new Location(100, 100, 7), amount)
        {
            Chargeable = null,
            ItemTypeFinder = itemTypeFinder
        };
    }

    public static IDefenseEquipment CreateDefenseEquipmentItem(ushort id, string slot = "", ushort charges = 10,
        (ItemAttribute, IConvertible)[] attributes = null, Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
        type.Attributes.SetAttribute(ItemAttribute.Charges, charges);
        type.SetName("item");

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new BodyDefenseEquipment(type, new Location(100, 100, 7))
        {
            Chargeable = charges > 0
                ? new Chargeable(charges, type.Attributes.GetAttribute<bool>(ItemAttribute.ShowCharges))
                : null,
            ItemTypeFinder = itemTypeFinder
        };
    }

    public static IPickupable CreateBodyEquipmentItem(ushort id, string slot, string weaponType = "")
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, slot);
        type.Attributes.SetAttribute(ItemAttribute.WeaponType, weaponType);
        type.Attributes.SetAttribute(ItemAttribute.Weight, 40);
        type.SetName("item");

        return new BodyDefenseEquipment(type, new Location(100, 100, 7));
    }

    public static IPickupable CreateAmmo(ushort id, byte amount, (ItemAttribute, IConvertible)[] attributes = null,
        Func<ushort, IItemType> itemTypeFinder = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");
        type.Attributes.SetAttribute(ItemAttribute.WeaponType, "ammunition");
        type.Attributes.SetAttribute(ItemAttribute.BodyPosition, "ammo");
        type.Attributes.SetAttribute(ItemAttribute.Weight, 1);
        type.Flags.Add(ItemFlag.Stackable);

        attributes ??= Array.Empty<(ItemAttribute, IConvertible)>();
        foreach (var (attributeType, value) in attributes) type.Attributes.SetAttribute(attributeType, value);

        return new Ammo(type, new Location(100, 100, 7), amount)
        {
            Chargeable = null,
            ItemTypeFinder = itemTypeFinder
        };
    }
    
    public static Food CreateFood(ushort id)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("meat");
        type.Attributes.SetAttribute(ItemAttribute.Type, "food");
        type.Attributes.SetAttribute(ItemAttribute.Duration, 30);
        type.Attributes.SetAttribute(ItemAttribute.Sentence, "Munch.");
        type.Flags.Add(ItemFlag.Stackable);

        return new Food(type, new Location(100, 100, 7), new Dictionary<ItemAttribute, IConvertible>());
    }

    public static IPickupable CreateCoin(ushort id, byte amount, uint multiplier)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("coin");
        type.Attributes.SetAttribute(ItemAttribute.Type, "coin");
        type.Attributes.SetAttribute(ItemAttribute.Worth, multiplier);
        type.Attributes.SetAttribute(ItemAttribute.Weight, 1);
        type.Flags.Add(ItemFlag.Stackable);

        return new Coin(type, new Location(100, 100, 7), amount);
    }

    public static IAttackRune CreateAttackRune(ushort id, DamageType damageType = DamageType.Energy,
        byte amount = 100,
        bool needTarget = true, ushort min = 100, ushort max = 100, IAreaEffectStore areaEffectStore = null)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("hmm");
        type.Flags.Add(ItemFlag.Stackable);
        type.Attributes.SetAttribute(ItemAttribute.Damage, DamageTypeParser.Parse(damageType));
        type.Attributes.SetAttribute(ItemAttribute.Type, "rune");
        type.Attributes.SetAttribute(ItemAttribute.NeedTarget, needTarget);
        type.Attributes.SetCustomAttribute("x", new[] { min.ToString(), max.ToString() });
        type.Attributes.SetCustomAttribute("y", new[] { min.ToString(), max.ToString() });

        var attributes = new Dictionary<ItemAttribute, IConvertible>
        {
            [ItemAttribute.Count] = amount
        };

        areaEffectStore ??= new AreaEffectStore();
        var factory = new RuneFactory(areaEffectStore);
        return (IAttackRune)factory.Create(type, new Location(100, 100, 7), attributes);
    }

    public static IItem CreateTopItem(ushort id, byte topOrder)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");

        if (topOrder == 1)
            type.SetFlag(ItemFlag.AlwaysOnTop);
        else
            type.SetFlag(ItemFlag.Bottom);

        return new Item(type, new Location(100, 100, 7));
    }

    public static IItem CreateUnpassableItem(ushort id)
    {
        var type = new ItemType();
        type.SetClientId(id);
        type.SetId(id);
        type.SetName("item");

        type.SetFlag(ItemFlag.Unpassable);

        return new Item(type, new Location(100, 100, 7));
    }

    public static IItemTypeStore GetItemTypeStore(params IItemType[] itemTypes)
    {
        var itemTypeStore = new ItemTypeStore();
        foreach (var itemType in itemTypes) itemTypeStore.Add(itemType.ClientId, itemType);

        return itemTypeStore;
    }

    public static IItemTypeStore AddItemTypeStore(IItemTypeStore itemTypeStore, params IItemType[] itemTypes)
    {
        foreach (var itemType in itemTypes) itemTypeStore.Add(itemType.ClientId, itemType);

        return itemTypeStore;
    }
}