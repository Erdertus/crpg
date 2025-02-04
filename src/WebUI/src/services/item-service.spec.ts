import { type PartialDeep } from 'type-fest';
import { mockGet } from 'vi-fetch';
import { response } from '@/__mocks__/crpg-client';
import {
  type ItemFlat,
  ItemType,
  type Item,
  WeaponClass,
  ItemUsage,
  ItemFlags,
  WeaponFlags,
  ItemFieldFormat,
  ItemFieldCompareRule,
  ItemSlot,
  ItemFamilyType,
  DamageType,
} from '@/models/item';
import { type EquippedItemsBySlot } from '@/models/character';
import { type AggregationConfig, AggregationView } from '@/models/item-search';
import { UserItem } from '@/models/user';
import { Culture } from '@/models/culture';

vi.mock('@/services/item-search-service/aggregations.ts', () => ({
  aggregationsConfig: {
    type: {
      view: AggregationView.Checkbox,
    },
    flags: {
      view: AggregationView.Checkbox,
      format: ItemFieldFormat.List,
    },
    price: {
      view: AggregationView.Range,
      format: ItemFieldFormat.Number,
    },
    thrustDamage: {
      view: AggregationView.Range,
      format: ItemFieldFormat.Damage,
    },
    swingDamage: {
      view: AggregationView.Range,
      format: ItemFieldFormat.Damage,
    },
    damage: {
      view: AggregationView.Range,
      format: ItemFieldFormat.Damage,
    },
    requirement: {
      view: AggregationView.Range,
      format: ItemFieldFormat.Requirement,
      compareRule: ItemFieldCompareRule.Less,
    },
  } as AggregationConfig,
}));

const { mockedNotify } = vi.hoisted(() => ({
  mockedNotify: vi.fn(),
}));
vi.mock('@/services/notification-service', async () => ({
  ...(await vi.importActual<typeof import('@/services/notification-service')>(
    '@/services/notification-service'
  )),
  notify: mockedNotify,
}));

import mockItems from '@/__mocks__/items.json';

import {
  getItems,
  getItemImage,
  getAvailableSlotsByItem,
  getLinkedSlots,
  hasWeaponClassesByItemType,
  getWeaponClassesByItemType,
  groupItemsByTypeAndWeaponClass,
  getCompareItemsResult,
  getRelativeEntries,
  humanizeBucket,
  getItemFieldAbsoluteDiffStr,
  type HumanBucket,
  IconBucketType,
  getItemGraceTimeEnd,
  isGraceTimeExpired,
  computeSalePrice,
  computeBrokenItemRepairCost,
  computeAverageRepairCostPerHour,
} from './item-service';

beforeEach(() => {
  vi.useFakeTimers();
  vi.setSystemTime('2022-11-27T21:00:00.0000000Z');
});

afterEach(() => {
  vi.useRealTimers();
});

it('getItems', async () => {
  mockGet('/items').willResolve(response<Item[]>(mockItems as Item[]));

  expect(await getItems()).toEqual(mockItems);
});

it('getItemImage', () => {
  expect(getItemImage('crpg_aserai_noble_sword_2_t5')).toEqual(
    '/items/crpg_aserai_noble_sword_2_t5.png'
  );
});

it.each<[PartialDeep<Item>, PartialDeep<EquippedItemsBySlot>, ItemSlot[]]>([
  [
    { type: ItemType.MountHarness, flags: [], armor: { familyType: ItemFamilyType.Horse } },
    {},
    [ItemSlot.MountHarness],
  ],
  [
    { type: ItemType.MountHarness, flags: [], armor: { familyType: ItemFamilyType.Horse } },
    { [ItemSlot.Mount]: { item: { mount: { familyType: ItemFamilyType.Horse } } } },
    [ItemSlot.MountHarness],
  ],
  [
    { type: ItemType.MountHarness, flags: [], armor: { familyType: ItemFamilyType.Camel } },
    {},
    [ItemSlot.MountHarness],
  ],
  [
    { type: ItemType.MountHarness, flags: [], armor: { familyType: ItemFamilyType.Camel } },
    { [ItemSlot.Mount]: { item: { mount: { familyType: ItemFamilyType.Horse } } } },
    [],
  ],
  [
    { type: ItemType.MountHarness, flags: [], armor: { familyType: ItemFamilyType.Horse } },
    { [ItemSlot.Mount]: { item: { mount: { familyType: ItemFamilyType.Camel } } } },
    [],
  ],
  [
    { type: ItemType.OneHandedWeapon, flags: [] },
    {},
    [ItemSlot.Weapon0, ItemSlot.Weapon1, ItemSlot.Weapon2, ItemSlot.Weapon3],
  ],
  [{ type: ItemType.Banner, flags: [] }, {}, [ItemSlot.WeaponExtra]],
  [{ type: ItemType.Polearm, flags: [ItemFlags.DropOnWeaponChange] }, {}, [ItemSlot.WeaponExtra]], // Pike

  // Large shields on horseback
  [
    { type: ItemType.Shield, flags: [], weapons: [{ class: WeaponClass.LargeShield }] },
    {},
    [ItemSlot.Weapon0, ItemSlot.Weapon1, ItemSlot.Weapon2, ItemSlot.Weapon3],
  ],
  [
    { type: ItemType.Shield, flags: [], weapons: [{ class: WeaponClass.LargeShield }] },
    { [ItemSlot.Mount]: { item: { mount: { familyType: ItemFamilyType.Horse } } } },
    [],
  ],
  [
    { type: ItemType.Mount, flags: [] },
    {
      [ItemSlot.Weapon2]: {
        item: { type: ItemType.Shield, weapons: [{ class: WeaponClass.LargeShield }] },
      },
    },
    [],
  ],
  [
    { type: ItemType.Shield, flags: [], weapons: [{ class: WeaponClass.SmallShield }] },
    { [ItemSlot.Mount]: { item: { mount: { familyType: ItemFamilyType.Horse } } } },
    [ItemSlot.Weapon0, ItemSlot.Weapon1, ItemSlot.Weapon2, ItemSlot.Weapon3],
  ],
  [
    { type: ItemType.Mount, flags: [] },
    {
      [ItemSlot.Weapon2]: {
        item: { type: ItemType.Shield, weapons: [{ class: WeaponClass.SmallShield }] },
      },
    },
    [ItemSlot.Mount],
  ],

  // EBA
  [{ type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA }, flags: [] }, {}, []],
  [
    { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA }, flags: [] },
    {
      [ItemSlot.Leg]: {
        item: { type: ItemType.LegArmor, armor: { familyType: ItemFamilyType.Undefined } },
      },
    },
    [],
  ],
  [
    { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA }, flags: [] },
    {
      [ItemSlot.Leg]: {
        item: { type: ItemType.LegArmor, armor: { familyType: ItemFamilyType.EBA } },
      },
    },
    [ItemSlot.Body],
  ],
  [
    { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA }, flags: [] },
    {
      [ItemSlot.Leg]: {
        item: { type: ItemType.LegArmor, armor: { familyType: ItemFamilyType.Undefined } },
      },
    },
    [],
  ],
  [
    { type: ItemType.LegArmor, armor: { familyType: ItemFamilyType.Undefined }, flags: [] },
    {
      [ItemSlot.Body]: {
        item: { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA } },
      },
    },
    [],
  ],
  [
    { type: ItemType.LegArmor, armor: { familyType: ItemFamilyType.EBA }, flags: [] },
    {
      [ItemSlot.Body]: {
        item: { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA } },
      },
    },
    [ItemSlot.Leg],
  ],
])('getAvailableSlotsByItem - item: %j, equipedItems: %j', (item, equipedItems, expectation) => {
  expect(getAvailableSlotsByItem(item as Item, equipedItems as EquippedItemsBySlot)).toEqual(
    expectation
  );
});

it.each<[ItemSlot, PartialDeep<EquippedItemsBySlot>, ItemSlot[]]>([
  // EBA
  [
    ItemSlot.Leg,
    {
      [ItemSlot.Body]: {
        item: { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.EBA } },
      },
    },
    [ItemSlot.Body],
  ],
  [
    ItemSlot.Leg,
    {
      [ItemSlot.Body]: {
        item: { type: ItemType.BodyArmor, armor: { familyType: ItemFamilyType.Undefined } },
      },
    },
    [],
  ],
])('getLinkedSlots - slot: %s', (slot, equipedItems, expectation) => {
  expect(getLinkedSlots(slot, equipedItems as EquippedItemsBySlot)).toEqual(expectation);
});

it.each([
  [ItemType.Bow, false],
  [ItemType.Polearm, true],
])('hasWeaponClassesByItemType - itemType: %s', (itemType, expectation) => {
  expect(hasWeaponClassesByItemType(itemType)).toEqual(expectation);
});

it.each<[ItemType, WeaponClass[]]>([
  [
    ItemType.OneHandedWeapon,
    [WeaponClass.OneHandedSword, WeaponClass.OneHandedAxe, WeaponClass.Mace, WeaponClass.Dagger],
  ],
  [ItemType.Bow, []],
])('getWeaponClassesByItemType - itemType: %s', (itemType, expectation) => {
  expect(getWeaponClassesByItemType(itemType)).toEqual(expectation);
});

describe('humanizeBucket', () => {
  it.each<[keyof ItemFlat, any, HumanBucket]>([
    ['type', null, { label: '', icon: null, tooltip: null }],
    [
      'type',
      ItemType.OneHandedWeapon,
      {
        label: 'item.type.OneHandedWeapon',
        icon: { name: 'item-type-one-handed-weapon', type: IconBucketType.Svg },
        tooltip: null,
      },
    ],
    [
      'weaponClass',
      WeaponClass.OneHandedPolearm,
      {
        label: 'item.weaponClass.OneHandedPolearm',
        icon: { name: 'weapon-class-one-handed-polearm', type: IconBucketType.Svg },
        tooltip: null,
      },
    ],
    [
      'damageType',
      DamageType.Cut,
      {
        label: 'item.damageType.Cut.long',
        icon: { name: 'damage-type-cut', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.damageType.Cut.title',
          description: 'item.damageType.Cut.description',
        },
      },
    ],
    [
      'culture',
      Culture.Vlandia,
      {
        label: 'item.culture.Vlandia',
        icon: { name: 'culture-vlandia', type: IconBucketType.Asset },
        tooltip: {
          title: 'item.culture.Vlandia',
          description: null,
        },
      },
    ],
    [
      'mountArmorFamilyType',
      ItemFamilyType.Horse,
      {
        label: 'item.familyType.1.title',
        icon: { name: 'mount-type-horse', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.familyType.1.title',
          description: 'item.familyType.1.description',
        },
      },
    ],
    [
      'mountFamilyType',
      ItemFamilyType.Camel,
      {
        label: 'item.familyType.2.title',
        icon: { name: 'mount-type-camel', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.familyType.2.title',
          description: 'item.familyType.2.description',
        },
      },
    ],
    [
      'armorFamilyType',
      ItemFamilyType.EBA,
      {
        label: 'item.familyType.3.title',
        icon: null,
        tooltip: {
          title: 'item.familyType.3.title',
          description: 'item.familyType.3.description',
        },
      },
    ],
    [
      'flags',
      ItemFlags.DropOnWeaponChange,
      {
        label: 'item.flags.DropOnWeaponChange',
        icon: { name: 'item-flag-drop-on-change', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.flags.DropOnWeaponChange',
          description: null,
        },
      },
    ],
    [
      'flags',
      WeaponFlags.CanDismount,
      {
        label: 'item.weaponFlags.CanDismount',
        icon: { name: 'item-flag-can-dismount', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.weaponFlags.CanDismount',
          description: null,
        },
      },
    ],
    [
      'flags',
      ItemUsage.PolearmBracing,
      {
        label: 'item.usage.polearm_bracing.title',
        icon: { name: 'item-flag-brace', type: IconBucketType.Svg },
        tooltip: {
          title: 'item.usage.polearm_bracing.title',
          description: 'item.usage.polearm_bracing.description',
        },
      },
    ],
    [
      'requirement',
      18,
      {
        label: 'item.requirementFormat',
        icon: null,
        tooltip: null,
      },
    ],
    [
      'price',
      1234,
      {
        label: '1234',
        icon: null,
        tooltip: null,
      },
    ],
    [
      'handling',
      12,
      {
        label: '12',
        icon: null,
        tooltip: null,
      },
    ],
  ])('aggKey: %s, bucket: %s ', (aggKey, bucket, expectation) => {
    expect(humanizeBucket(aggKey, bucket)).toEqual(expectation);
  });

  it('damage', () => {
    const item: Partial<ItemFlat> = {
      swingDamageType: DamageType.Cut,
    };

    expect(humanizeBucket('swingDamage', 10, item as ItemFlat)).toEqual({
      label: 'item.damageTypeFormat',
      icon: null,
      tooltip: {
        title: 'item.damageType.Cut.title',
        description: 'item.damageType.Cut.description',
      },
    });
  });

  it('damage', () => {
    const item: Partial<ItemFlat> = {};

    expect(humanizeBucket('thrustDamage', 0, item as ItemFlat)).toEqual({
      label: '0',
      icon: null,
      tooltip: null,
    });
  });
});

it('compareItemsResult', () => {
  const items = [
    {
      weight: 2.22,
      length: 105,
      handling: 82,
      flags: ['UseTeamColor'],
    },
    {
      weight: 2.07,
      length: 99,
      handling: 86,
      flags: [],
    },
  ] as ItemFlat[];

  const aggregationConfig = {
    length: {
      title: 'Length',
      compareRule: 'Bigger',
    },
    flags: {
      title: 'Flags',
    },
    handling: {
      title: 'Handling',
      compareRule: 'Bigger',
    },
    weight: {
      title: 'Thrust damage',
      compareRule: 'Less',
    },
  } as AggregationConfig;

  expect(getCompareItemsResult(items, aggregationConfig)).toEqual({
    length: 105,
    handling: 86,
    weight: 2.07,
  });
});

it('groupItemsByTypeAndWeaponClass', () => {
  const items = [
    {
      id: 'item1',
      type: ItemType.OneHandedWeapon,
      weaponClass: WeaponClass.OneHandedSword,
    },
    {
      id: 'item2',
      type: ItemType.OneHandedWeapon,
      weaponClass: WeaponClass.OneHandedSword,
    },
    {
      id: 'item3',
      type: ItemType.OneHandedWeapon,
      weaponClass: WeaponClass.OneHandedAxe,
    },
    // Shield Case
    {
      id: 'item4',
      type: ItemType.Shield,
      weaponClass: WeaponClass.SmallShield,
    },
    {
      id: 'item5',
      type: ItemType.Shield,
      weaponClass: WeaponClass.LargeShield,
    },
  ] as ItemFlat[];

  expect(groupItemsByTypeAndWeaponClass(items)).toEqual([
    {
      type: 'OneHandedWeapon',
      weaponClass: 'OneHandedSword',
      items: [
        {
          id: 'item1',
          type: 'OneHandedWeapon',
          weaponClass: 'OneHandedSword',
        },
        {
          id: 'item2',
          type: 'OneHandedWeapon',
          weaponClass: 'OneHandedSword',
        },
      ],
    },
    {
      type: 'OneHandedWeapon',
      weaponClass: 'OneHandedAxe',
      items: [
        {
          id: 'item3',
          type: 'OneHandedWeapon',
          weaponClass: 'OneHandedAxe',
        },
      ],
    },
    {
      type: 'Shield',
      weaponClass: 'SmallShield',
      items: [
        {
          id: 'item4',
          type: 'Shield',
          weaponClass: 'SmallShield',
        },
        {
          id: 'item5',
          type: 'Shield',
          weaponClass: 'LargeShield',
        },
      ],
    },
  ]);
});

it('getRelativeEntries', () => {
  const item = {
    weight: 2.22,
    length: 105,
    handling: 82,
    flags: ['UseTeamColor'],
  } as ItemFlat;

  const aggregationConfig = {
    length: {
      title: 'Length',
      compareRule: 'Bigger',
    },
    flags: {
      title: 'Flags',
    },
    handling: {
      title: 'Handling',
      compareRule: 'Bigger',
    },
    weight: {
      title: 'Thrust damage',
      compareRule: 'Less',
    },
  } as AggregationConfig;

  expect(getRelativeEntries(item, aggregationConfig)).toEqual({
    length: 105,
    handling: 82,
    weight: 2.22,
  });
});

it.each<[ItemFieldCompareRule, number, number, string]>([
  [ItemFieldCompareRule.Bigger, 0, 0, ''],
  [ItemFieldCompareRule.Bigger, -1, 0, '-1'],
  [ItemFieldCompareRule.Bigger, -1, -1, ''],

  [ItemFieldCompareRule.Bigger, 1, 1, ''],
  [ItemFieldCompareRule.Bigger, 2, 1, ''],
  [ItemFieldCompareRule.Bigger, 1, 2, '-1'],

  [ItemFieldCompareRule.Less, 0, 0, ''],
  [ItemFieldCompareRule.Less, 0, -1, '+1'],
  [ItemFieldCompareRule.Less, -1, -1, ''],

  [ItemFieldCompareRule.Less, 1, 1, ''],
  [ItemFieldCompareRule.Less, 1, 2, ''],
  [ItemFieldCompareRule.Less, 2, 1, '+1'],
])(
  'getItemFieldAbsoluteDiffStr - compareRule: %s, value: %s, bestValue: %s',
  (compareRule, value, bestValue, expectation) => {
    expect(getItemFieldAbsoluteDiffStr(compareRule, value, bestValue)).toEqual(expectation);
  }
);

it('getItemGraceTimeEnd', () => {
  const userItem: PartialDeep<UserItem> = {
    createdAt: new Date('2022-11-27T20:00:00.0000000Z'),
  };

  expect(getItemGraceTimeEnd(userItem as UserItem)).toEqual(
    new Date('2022-11-27T21:00:00.0000000Z')
  );
});

it.each<[Date, boolean]>([
  [new Date('2022-11-27T20:00:00.0000000Z'), true],
  [new Date('2022-11-27T20:59:00.0000000Z'), true],
  [new Date('2022-11-27T21:00:00.0000000Z'), false],
  [new Date('2022-11-27T21:01:00.0000000Z'), false],
])('isGraceTimeExpired - date: %s,', (itemGraceTimeEnd, expectation) => {
  expect(isGraceTimeExpired(itemGraceTimeEnd)).toEqual(expectation);
});

it.each<[PartialDeep<UserItem>, { price: number; graceTimeEnd: Date | null }]>([
  [
    {
      createdAt: new Date('2022-11-27T20:00:00.0000000Z'),
      item: {
        price: 1100,
      },
    },
    {
      graceTimeEnd: new Date('2022-11-27T21:00:00.000Z'),
      price: 1100,
    },
  ],
  [
    {
      createdAt: new Date('2022-11-27T19:00:00.0000000Z'),
      item: {
        price: 1100,
      },
    },
    {
      graceTimeEnd: null,
      price: 550,
    },
  ],
])('computeSalePrice - userItem: %s,', (userItem, expectation) => {
  expect(computeSalePrice(userItem as UserItem)).toEqual(expectation);
});

it.each<[number, number]>([
  [1, 0],
  [10, 0],
  [100, 1],
  [1000, 19],
  [10000, 192],
  [100000, 1919],
])('computeBrokenItemRepairCost - price: %s,', (price, expectation) => {
  expect(computeBrokenItemRepairCost(price)).toEqual(expectation);
});

it.each<[number, number]>([
  [1, 0],
  [10, 1],
  [100, 11],
  [1000, 115],
  [10000, 1152],
  [100000, 11519],
])('computeAverageRepairCostPerHour - price: %s,', (price, expectation) => {
  expect(computeAverageRepairCostPerHour(price)).toEqual(expectation);
});
